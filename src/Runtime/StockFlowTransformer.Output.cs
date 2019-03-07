// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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
        /// Processes the current stock summary data
        /// </summary>
        /// <remarks></remarks>
        private void ProcessStockSummaryData(int iteration, int timestep)
		{
			foreach (OutputStock r in this.m_SummaryOutputStockRecords)
			{
                StockType t = this.m_StockTypes[r.StockTypeId];
                
                foreach (StockGroupLinkage l in t.StockGroupLinkages)
                {
				    DataRow dr = this.m_OutputStockTable.NewRow();

				    dr[Constants.ITERATION_COLUMN_NAME] = iteration;
				    dr[Constants.TIMESTEP_COLUMN_NAME] = timestep;
				    dr[Constants.STRATUM_ID_COLUMN_NAME] = r.StratumId;
				    dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.SecondaryStratumId);
				    dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TertiaryStratumId);
				    dr[Constants.STATECLASS_ID_COLUMN_NAME] = r.StateClassId;
                    dr[Constants.STOCK_GROUP_ID_COLUMN_NAME] = l.StockGroup.Id;
				    dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount * l.Value;

				    this.m_OutputStockTable.Rows.Add(dr);
                }
			}

			this.m_SummaryOutputStockRecords.Clear();
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

            if (this.m_STSimTransformer.IsOutputTimestep(
                timestep, 
                this.m_SummaryFlowOutputTimesteps, 
                this.m_CreateSummaryFlowOutput))
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
        /// Processes the current flow summary data
        /// </summary>
        /// <remarks></remarks>
        private void ProcessFlowSummaryData(int iteration, int timestep)
		{
			foreach (OutputFlow r in this.m_SummaryOutputFlowRecords)
			{
                FlowType t = this.m_FlowTypes[r.FlowTypeId];

                foreach (FlowGroupLinkage l in t.FlowGroupLinkages)
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
				    dr[Constants.FLOW_GROUP_ID_COLUMN_NAME] = l.FlowGroup.Id;
				    dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount * l.Value;

				    this.m_OutputFlowTable.Rows.Add(dr);
                }
			}

			this.m_SummaryOutputFlowRecords.Clear();
		}

		/// <summary>
		/// Processes the Spatial Stock Group data.
		/// </summary>
		/// <remarks></remarks>
		private void ProcessStockGroupSpatialData(int iteration, int timestep)
		{
			Debug.Assert(this.m_IsSpatial);

            foreach (StockGroup g in this.m_StockGroups)
            {
                Debug.Assert(g.StockTypeLinkages.Count > 0);
                StochasticTimeRaster rastOutput = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                foreach (StockTypeLinkage l in g.StockTypeLinkages)
                {
                    StochasticTimeRaster rastStockType = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                    GetStockValues(l.StockType.Id, rastStockType);
                    rastStockType.ScaleDblCells(l.Value);
                    rastOutput.AddDblCells(rastStockType);
                }

                Spatial.WriteRasterData(
                    rastOutput,
                    this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_STOCK_GROUP),
                    iteration,
                    timestep,
                    g.Id,
                    Constants.SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX,
                    Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
            }
		}

		/// <summary>
		/// Processes the current flow group spatial data
		/// </summary>
		/// <remarks></remarks>
		private void ProcessFlowGroupSpatialData(int iteration, int timestep)
		{
            Debug.Assert(this.m_IsSpatial);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                Debug.Assert(g.FlowTypeLinkages.Count > 0);
                StochasticTimeRaster rastOutput = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                {
                    StochasticTimeRaster rastFlowType = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                    rastFlowType.DblCells = (double[])(GetOutputFlowDictionary()[l.FlowType.Id].Clone());
                    rastFlowType.ScaleDblCells(l.Value);
                    rastOutput.AddDblCells(rastFlowType);
                }

                Spatial.WriteRasterData(
                    rastOutput,
                    this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_SPATIAL_FLOW_GROUP),
                    iteration,
                    timestep,
                    g.Id,
                    Constants.SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX,
                    Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
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
                timestep, 
                this.m_SpatialFlowOutputTimesteps, 
                this.m_CreateSpatialFlowOutput) 
                && this.m_IsSpatial)
			{
				if (GetOutputFlowDictionary().ContainsKey(flowTypeId))
				{
					double amt = GetOutputFlowDictionary()[flowTypeId][cell.CellId];

					if (amt.Equals(Spatial.DefaultNoDataValue))
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