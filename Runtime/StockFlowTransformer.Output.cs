//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

using System;
using System.Data;
using System.Diagnostics;
using SyncroSim.Core;
using SyncroSim.STSim;
using SyncroSim.Common;
using SyncroSim.StochasticTime;
using System.Globalization;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		private DataTable m_OutputStockTable;
		private bool m_CreateSummaryStockOutput;
		private int m_SummaryStockOutputTimesteps;
		private OutputStockCollection m_SummaryOutputStockRecords = new OutputStockCollection();
		private DataTable m_OutputFlowTable;
		private bool m_CreateSummaryFlowOutput;
		private int m_SummaryFlowOutputTimesteps;
		private OutputFlowCollection m_SummaryOutputFlowRecords = new OutputFlowCollection();
		private Dictionary<int, double[]> m_SpatialOutputFlowDict;
		private bool m_CreateSpatialStockOutput;
		private int m_SpatialStockOutputTimesteps;
		private bool m_CreateSpatialFlowOutput;
		private int m_SpatialFlowOutputTimesteps;

		/// <summary>
		/// Gets the output flow dictionary
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// We must lazy-load this dictionary because this transformer runs before ST-Sim's
		/// and so the cell data is not there yet.
		/// </remarks>
		private Dictionary<int, double[]> GetOutputFlowDictionary()
		{
			if (this.m_SpatialOutputFlowDict == null)
			{
				this.m_SpatialOutputFlowDict = new Dictionary<int, double[]>();

				foreach (FlowType ft in this.m_FlowTypes.Values)
				{
					double[] flowVals = null;
					flowVals = new double[this.STSimTransformer.InputRasters.NumberCells];
					this.m_SpatialOutputFlowDict.Add(ft.Id, flowVals);
				}
			}

			return this.m_SpatialOutputFlowDict;
		}

		/// <summary>
		/// Initializes the output data tables
		/// </summary>
		/// <remarks></remarks>
		private void InitializeOutputDataTables()
		{
			Debug.Assert(this.m_OutputStockTable == null);
			Debug.Assert(this.m_OutputFlowTable == null);

			this.m_OutputStockTable = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_STOCK_NAME).GetData();
			this.m_OutputFlowTable = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_FLOW_NAME).GetData();

			Debug.Assert(this.m_OutputStockTable.Rows.Count == 0);
			Debug.Assert(this.m_OutputFlowTable.Rows.Count == 0);
		}

		/// <summary>
		/// Processes the current stock summary data
		/// </summary>
		/// <remarks></remarks>
		private void ProcessStockSummaryData(int iteration, int timestep)
		{
			foreach (OutputStock r in this.m_SummaryOutputStockRecords)
			{
				DataRow dr = this.m_OutputStockTable.NewRow();

				dr[Constants.ITERATION_COLUMN_NAME] = iteration;
				dr[Constants.TIMESTEP_COLUMN_NAME] = timestep;
				dr[Constants.STRATUM_ID_COLUMN_NAME] = r.StratumId;
				dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.SecondaryStratumId);
				dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TertiaryStratumId);
				dr[Constants.STATECLASS_ID_COLUMN_NAME] = r.StateClassId;
				dr[Constants.STOCK_TYPE_ID_COLUMN_NAME] = r.StockTypeId;
				dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount;

				this.m_OutputStockTable.Rows.Add(dr);
			}

			this.m_SummaryOutputStockRecords.Clear();
		}

		/// <summary>
		/// Processes the current flow summary data
		/// </summary>
		/// <remarks></remarks>
		private void ProcessFlowSummaryData(int iteration, int timestep)
		{
			foreach (OutputFlow r in this.m_SummaryOutputFlowRecords)
			{
				DataRow dr = this.m_OutputFlowTable.NewRow();

				dr[Constants.ITERATION_COLUMN_NAME] = iteration;
				dr[Constants.TIMESTEP_COLUMN_NAME] = timestep;
				dr[Constants.FROM_STRATUM_ID_COLUMN_NAME] = r.FromStratumId;
				dr[Constants.FROM_SECONDARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.FromSecondaryStratumId);
				dr[Constants.FROM_TERTIARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.FromTertiaryStratumId);
				dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME] = r.FromStateClassId;
				dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME] = r.FromStockTypeId;
				dr[Constants.TRANSITION_TYPE_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransitionTypeId);
				dr[Constants.TO_STRATUM_ID_COLUMN_NAME] = r.ToStratumId;
				dr[Constants.TO_STATECLASS_ID_COLUMN_NAME] = r.ToStateClassId;
				dr[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME] = r.ToStockTypeId;
				dr[Constants.FLOW_TYPE_ID_COLUMN_NAME] = r.FlowTypeId;
				dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount;

				this.m_OutputFlowTable.Rows.Add(dr);
			}

			this.m_SummaryOutputFlowRecords.Clear();
		}

		/// <summary>
		/// Processes the Spatial Stock data. Create a raster file as a snapshot of the current Stock values.
		/// </summary>
		/// <remarks></remarks>
		private void ProcessStockSpatialData(int iteration, int timestep)
		{
			Debug.Assert(this.m_IsSpatial);

			foreach (StockType s in this.m_StockTypes)
			{
				StochasticTimeRaster rastOutput = new StochasticTimeRaster();
				// Fetch the raster metadata from the InpRasters object
				this.STSimTransformer.InputRasters.GetMetadata(rastOutput);
				rastOutput.InitDblCells();

				GetStockValues(s.Id, rastOutput);

				RasterFiles.SaveOutputRaster(
                    rastOutput, 
                    this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_STOCK_TYPE), 
                    RasterDataType.DTDouble, 
                    iteration, 
                    timestep, 
                    Constants.SPATIAL_MAP_STOCK_TYPE_VARIABLE_PREFIX, 
                    s.Id, 
                    Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
			}
		}

		/// <summary>
		/// Get Stock Values for the specified Stock Type ID, placing then into the DblCells() in the specified raster.
		/// </summary>
		/// <param name="stockTypeId">The Stock Type ID that we want values for</param>
		/// <param name="rastStockType">An object of type ApexRaster, where we will write the Stock Type values. The raster should be initialized with metadata and appropriate array sizing.</param>
		/// <remarks></remarks>
		private void GetStockValues(int stockTypeId, StochasticTimeRaster rastStockType)
		{
			double AmountPerCell = this.m_STSimTransformer.AmountPerCell;

			foreach (Cell c in this.STSimTransformer.Cells)
			{
				Dictionary<int, double> StockAmounts = GetStockAmountDictionary(c);

				if (StockAmounts.Count > 0)
				{
					rastStockType.DblCells[c.CellId] = (StockAmounts[stockTypeId] / AmountPerCell);
				}
				else
				{
					//I wouldnt expect to get here because of Stratum/StateClass test above
					Debug.Assert(false);
				}
			}
		}

		/// <summary>
		/// Processes the Spatial Stock Group data. Create a raster file as a snapshot of the Stock Group values.
		/// </summary>
		/// <remarks></remarks>
		private void ProcessStockGroupSpatialData(int iteration, int timestep)
		{
			using (DataStore store = this.Library.CreateDataStore())
			{
				Debug.Assert(this.m_IsSpatial);

				// Loop thru the Stock Groups
				DataSheet dsGrp = this.Project.GetDataSheet(Constants.DATASHEET_STOCK_GROUP_NAME);
				foreach (DataRow dr in dsGrp.GetData().Rows)
				{
					var sgId = Convert.ToInt32(dr[dsGrp.ValidationTable.ValueMember]);
					var sgName = dr[dsGrp.ValidationTable.DisplayMember];

					StochasticTimeRaster rastOutput = new StochasticTimeRaster();
					// Fetch the raster metadata from the InpRasters object
					this.STSimTransformer.InputRasters.GetMetadata(rastOutput);
					rastOutput.InitDblCells();

					// Fetch the Stock Group Multipler table, and add the Stock Type values together, applying the multiplier
					string query = string.Format(CultureInfo.InvariantCulture, "select * from {0} where ScenarioId = {1} and {2}={3}", Constants.DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME, ResultScenario.Id, Constants.STOCK_GROUP_ID_COLUMN_NAME, sgId);
					DataTable dtGrpTypes = store.CreateDataTableFromQuery(query, "GrpTypeMultData");
					if (dtGrpTypes.Rows.Count > 0)
					{
						foreach (DataRow drGrpType in dtGrpTypes.Rows)
						{
							// Find the Stock Types and associated multiplier that apply. Interpret and empty value (Null) as 1.0
							double amt = 0;
							if (Convert.IsDBNull(drGrpType["Value"]))
							{
								amt = 1.0;
							}
							else
							{
								amt = Convert.ToDouble(drGrpType["Value"]);
							}
							int stockTypeId = Convert.ToInt32(drGrpType[Constants.STOCK_TYPE_ID_COLUMN_NAME]);
							Debug.Print(string.Format(CultureInfo.InvariantCulture, "Group Name {0}, Group ID:{3}, Type:{1}, Amount:{2}", sgName, stockTypeId, amt, sgId));
							StochasticTimeRaster rastStockType = new StochasticTimeRaster();
							// Fetch the raster metadata from the InpRasters object
							this.STSimTransformer.InputRasters.GetMetadata(rastStockType);
							rastStockType.InitDblCells();
							GetStockValues(stockTypeId, rastStockType);
							rastStockType.ScaleDbl(amt);
							rastOutput.AddDbl(rastStockType);
						}

						RasterFiles.SaveOutputRaster(
                            rastOutput, 
                            this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_STOCK_GROUP),
                            RasterDataType.DTDouble, 
                            iteration, 
                            timestep, 
                            Constants.SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX, 
                            sgId, 
                            Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
					}
				}
			}
		}

		/// <summary>
		/// Processes the current flow spatial data
		/// </summary>
		/// <remarks></remarks>
		private void ProcessFlowSpatialData(int iteration, int timestep)
		{
			Debug.Assert(this.m_IsSpatial);

			StochasticTimeRaster rastOutput = new StochasticTimeRaster();
			// Fetch the raster metadata from the InpRasters object
			this.STSimTransformer.InputRasters.GetMetadata(rastOutput);

			foreach (FlowType flowType in this.m_FlowTypes.Values)
			{
				rastOutput.DblCells = GetOutputFlowDictionary()[flowType.Id];

				RasterFiles.SaveOutputRaster(
                    rastOutput, 
                    this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_FLOW_TYPE), 
                    RasterDataType.DTDouble, 
                    iteration, 
                    timestep, 
                    Constants.SPATIAL_MAP_FLOW_TYPE_VARIABLE_PREFIX, 
                    flowType.Id, 
                    Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
			}
		}

		/// <summary>
		/// Processes the current flow group spatial data
		/// </summary>
		/// <remarks></remarks>
		private void ProcessFlowGroupSpatialData(int iteration, int timestep)
		{
			using (DataStore store = this.Library.CreateDataStore())
			{
				Debug.Assert(this.m_IsSpatial);

				// Loop thru the Flow Groups
				DataSheet dsGrp = this.Project.GetDataSheet(Constants.DATASHEET_FLOW_GROUP_NAME);

				foreach (DataRow dr in dsGrp.GetData().Rows)
				{
					var fgId = Convert.ToInt32(dr[dsGrp.ValidationTable.ValueMember]);
					var fgName = dr[dsGrp.ValidationTable.DisplayMember];

					StochasticTimeRaster rastOutput = new StochasticTimeRaster();
					// Fetch the raster metadata from the InpRasters object
					this.STSimTransformer.InputRasters.GetMetadata(rastOutput);
					rastOutput.InitDblCells();

					// Loop through the Flow Group Multipler table, and add the Flow Type values together, applying the multiplier
					string query = string.Format(CultureInfo.InvariantCulture, "select * from {0} where scenarioId = {1} and {2}={3}", Constants.DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME, ResultScenario.Id, Constants.FLOW_GROUP_ID_COLUMN_NAME, fgId);
					DataTable dtGrpTypes = store.CreateDataTableFromQuery(query, "GrpTypeMultData");
					if (dtGrpTypes.Rows.Count > 0)
					{
						foreach (DataRow drGrpType in dtGrpTypes.Rows)
						{
							// Find the Flow Types and associated multiplier that apply. Interpret and empty value (Null) as 1.0
							double amt = 0;
							if (Convert.IsDBNull(drGrpType["Value"]))
							{
								amt = 1.0;
							}
							else
							{
								amt = Convert.ToDouble(drGrpType["Value"]);
							}

							int flowTypeId = Convert.ToInt32(drGrpType[Constants.FLOW_TYPE_ID_COLUMN_NAME]);
							Debug.Print(string.Format(CultureInfo.InvariantCulture, "Group Name {0}, Group ID:{3}, Type:{1}, Amount:{2}", fgName, flowTypeId, amt, fgId));
							StochasticTimeRaster rastFlowType = new StochasticTimeRaster();
							// Fetch the raster metadata from the InpRasters object
							this.STSimTransformer.InputRasters.GetMetadata(rastFlowType);
							rastFlowType.InitDblCells();

							rastFlowType.DblCells = (double[])(GetOutputFlowDictionary()[flowTypeId].Clone());
							rastFlowType.ScaleDbl(amt);
							rastOutput.AddDbl(rastFlowType);
						}

						RasterFiles.SaveOutputRaster(
                            rastOutput, 
                            this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_FLOW_GROUP), 
                            RasterDataType.DTDouble, 
                            iteration, 
                            timestep, 
                            Constants.SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX, 
                            fgId, 
                            Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
					}
				}
			}
		}

		/// <summary>
		/// Adds to the stock summary result collection
		/// </summary>
		/// <remarks></remarks>
		private void OnSummaryStockOutput()
		{
			foreach (Cell c in this.STSimTransformer.Cells)
			{
				Dictionary<int, double> StockAmounts = GetStockAmountDictionary(c);

				foreach (int id in StockAmounts.Keys)
				{
					double amount = StockAmounts[id];

					FiveIntegerLookupKey k = new FiveIntegerLookupKey(
                        c.StratumId, GetSecondaryStratumIdKey(c), 
                        GetTertiaryStratumIdKey(c), c.StateClassId, id);

					if (this.m_SummaryOutputStockRecords.Contains(k))
					{
						OutputStock r = this.m_SummaryOutputStockRecords[k];
						r.Amount += amount;
					}
					else
					{
						OutputStock r = new OutputStock(
                            c.StratumId, GetSecondaryStratumIdValue(c), 
                            GetTertiaryStratumIdValue(c), c.StateClassId, id, amount);

						this.m_SummaryOutputStockRecords.Add(r);
					}
				}
			}        
		}

		/// <summary>
		/// Adds to the flow summary result collection
		/// </summary>
		/// <param name="timestep"></param>
		/// <param name="cell"></param>
		/// <param name="stockTypeId"></param>
		/// <param name="flowTypeId"></param>
		/// <param name="flowAmount"></param>
		/// <param name="transitionPathway"></param>
		/// <param name="flowPathway"></param>
		/// <remarks></remarks>
		private void OnSummaryFlowOutput(
            int timestep, 
            Cell cell,
            DeterministicTransition deterministicPathway, 
            Transition probabilisticPathway, 
            FlowPathway flowPathway, 
            double flowAmount)
		{
			int? TransitionTypeId = null;
			int StratumIdDest = cell.StratumId;
			int StateClassIdDest = cell.StateClassId;

			if (probabilisticPathway != null)
			{
				TransitionTypeId = probabilisticPathway.TransitionTypeId;

				if (probabilisticPathway.StratumIdDestination.HasValue)
				{
					StratumIdDest = probabilisticPathway.StratumIdDestination.Value;
				}

				if (probabilisticPathway.StateClassIdDestination.HasValue)
				{
					StateClassIdDest = probabilisticPathway.StateClassIdDestination.Value;
				}
			}
			else
			{
				if (deterministicPathway != null)
				{
					if (deterministicPathway.StratumIdDestination.HasValue)
					{
						StratumIdDest = deterministicPathway.StratumIdDestination.Value;
					}

					if (deterministicPathway.StateClassIdDestination.HasValue)
					{
						StateClassIdDest = deterministicPathway.StateClassIdDestination.Value;
					}
				}
			}

			if (this.m_STSimTransformer.IsOutputTimestep(timestep, this.m_SummaryFlowOutputTimesteps, this.m_CreateSummaryFlowOutput))
			{
				TenIntegerLookupKey k = new TenIntegerLookupKey(
                    cell.StratumId,
                    GetSecondaryStratumIdKey(cell), 
                    GetTertiaryStratumIdKey(cell), 
                    cell.StateClassId, 
                    flowPathway.FromStockTypeId, 
                    LookupKeyUtilities.GetOutputCollectionKey(TransitionTypeId), 
                    StratumIdDest, 
                    StateClassIdDest, 
                    flowPathway.ToStockTypeId, 
                    flowPathway.FlowTypeId);

				if (this.m_SummaryOutputFlowRecords.Contains(k))
				{
					OutputFlow r = this.m_SummaryOutputFlowRecords[k];
					r.Amount += flowAmount;
				}
				else
				{
					OutputFlow r = new OutputFlow(
                        cell.StratumId, 
                        GetSecondaryStratumIdValue(cell), 
                        GetTertiaryStratumIdValue(cell), 
                        cell.StateClassId, 
                        flowPathway.FromStockTypeId,
                        TransitionTypeId, 
                        StratumIdDest, 
                        StateClassIdDest, 
                        flowPathway.ToStockTypeId, 
                        flowPathway.FlowTypeId, 
                        flowAmount);

					this.m_SummaryOutputFlowRecords.Add(r);
				}
			}
		}

		/// <summary>
		/// Adds to the flow summary result collection
		/// </summary>
		/// <param name="timestep"></param>
		/// <param name="cell"></param>
		/// <param name="flowTypeId"></param>
		/// <param name="flowAmount"></param>
		/// <remarks></remarks>
		private void OnSpatialFlowOutput(int timestep, Cell cell, int flowTypeId, double flowAmount)
		{
			if (this.m_STSimTransformer.IsOutputTimestep(
                timestep, this.m_SpatialFlowOutputTimesteps, this.m_CreateSpatialFlowOutput) && this.m_IsSpatial)
			{
				if (GetOutputFlowDictionary().ContainsKey(flowTypeId))
				{
					double amt = GetOutputFlowDictionary()[flowTypeId][cell.CellId];

					if (amt.Equals(StochasticTimeRaster.DefaultNoDataValue))
					{
						amt = 0;
					}

					amt += (flowAmount / this.m_STSimTransformer.AmountPerCell);
					GetOutputFlowDictionary()[flowTypeId][cell.CellId] = amt;
				}
				else
				{
					Debug.Assert(false, "I think we expected to find a m_SpatialOutputFlow object for the flowType " + flowTypeId.ToString("0000", CultureInfo.InvariantCulture));
				}
			}
		}

		internal int GetSecondaryStratumIdKey(int? value)
		{
			if (this.m_SummaryOmitSecondaryStrata)
			{
				return Constants.OUTPUT_COLLECTION_WILDCARD_KEY;
			}
			else
			{
				return LookupKeyUtilities.GetOutputCollectionKey(value);
			}
		}

		private int? GetSecondaryStratumIdValue(int? value)
		{
			if (this.m_SummaryOmitSecondaryStrata)
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		internal int GetTertiaryStratumIdKey(int? value)
		{
			if (this.m_SummaryOmitTertiaryStrata)
			{
				return Constants.OUTPUT_COLLECTION_WILDCARD_KEY;
			}
			else
			{
				return LookupKeyUtilities.GetOutputCollectionKey(value);
			}
		}

		private int? GetTertiaryStratumIdValue(int? value)
		{
			if (this.m_SummaryOmitTertiaryStrata)
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		private int GetSecondaryStratumIdKey(Cell simulationCell)
		{
			return GetSecondaryStratumIdKey(simulationCell.SecondaryStratumId);
		}

		private int? GetSecondaryStratumIdValue(Cell simulationCell)
		{
			return GetSecondaryStratumIdValue(simulationCell.SecondaryStratumId);
		}

		private int GetTertiaryStratumIdKey(Cell simulationCell)
		{
			return GetTertiaryStratumIdKey(simulationCell.TertiaryStratumId);
		}

		private int? GetTertiaryStratumIdValue(Cell simulationCell)
		{
			return GetTertiaryStratumIdValue(simulationCell.TertiaryStratumId);
		}
	}
}