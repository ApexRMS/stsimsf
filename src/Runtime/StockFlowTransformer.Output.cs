// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.STSim;
using SyncroSim.Common;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		private DataTable m_OutputStockTable;
		private bool m_CreateSummaryStockOutput;
		private int m_SummaryStockOutputTimesteps;
		private DataTable m_OutputFlowTable;
		private bool m_CreateSummaryFlowOutput;
		private int m_SummaryFlowOutputTimesteps;
		private bool m_CreateSpatialStockOutput;
		private int m_SpatialStockOutputTimesteps;
		private bool m_CreateSpatialFlowOutput;
		private int m_SpatialFlowOutputTimesteps;
        private bool m_CreateLateralFlowOutput;
        private int m_LateralFlowOutputTimesteps;
		private bool m_CreateAvgSpatialStockOutput;
		private int m_AvgSpatialStockOutputTimesteps;
		private bool m_CreateAvgSpatialFlowOutput;
		private int m_AvgSpatialFlowOutputTimesteps;

		private Dictionary<int, SpatialOutputFlowRecord> m_SpatialOutputFlowDict;
		private Dictionary<int, SpatialOutputFlowRecord> m_LateralOutputFlowDict;
		private OutputFlowCollection m_SummaryOutputFlowRecords = new OutputFlowCollection();
		private OutputStockCollection m_SummaryOutputStockRecords = new OutputStockCollection();

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

                foreach (int StockTypeId in StockAmounts.Keys)
                {
                    StockType t = this.m_StockTypes[StockTypeId];
                    double amount = StockAmounts[StockTypeId];

                    foreach (StockGroupLinkage l in t.StockGroupLinkages)
                    {
                        FiveIntegerLookupKey k = new FiveIntegerLookupKey(
                            c.StratumId, GetSecondaryStratumIdKey(c),
                            GetTertiaryStratumIdKey(c), c.StateClassId, l.StockGroup.Id);

                        if (this.m_SummaryOutputStockRecords.Contains(k))
                        {
                            OutputStock r = this.m_SummaryOutputStockRecords[k];
                            r.Amount += (amount * l.Value);
                        }
                        else
                        {
                            OutputStock r = new OutputStock(
                                c.StratumId, GetSecondaryStratumIdValue(c),
                                GetTertiaryStratumIdValue(c), c.StateClassId, l.StockGroup.Id, amount * l.Value);

                            this.m_SummaryOutputStockRecords.Add(r);
                        }
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
				DataRow dr = this.m_OutputStockTable.NewRow();

				dr[Constants.ITERATION_COLUMN_NAME] = iteration;
				dr[Constants.TIMESTEP_COLUMN_NAME] = timestep;
				dr[Constants.STRATUM_ID_COLUMN_NAME] = r.StratumId;
				dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.SecondaryStratumId);
				dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TertiaryStratumId);
				dr[Constants.STATECLASS_ID_COLUMN_NAME] = r.StateClassId;
                dr[Constants.STOCK_GROUP_ID_COLUMN_NAME] = r.StockGroupId;
                dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount;

				this.m_OutputStockTable.Rows.Add(dr);
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
                FlowType t = this.m_FlowTypes[flowPathway.FlowTypeId];

                foreach (FlowGroupLinkage l in t.FlowGroupLinkages)
                {
                    FifteenIntegerLookupKey k = new FifteenIntegerLookupKey(
                        cell.StratumId,
                        GetSecondaryStratumIdKey(cell),
                        GetTertiaryStratumIdKey(cell),
                        cell.StateClassId,
                        flowPathway.FromStockTypeId,
                        LookupKeyUtilities.GetOutputCollectionKey(TransitionTypeId),
                        StratumIdDest,
                        StateClassIdDest,
                        flowPathway.ToStockTypeId,
                        l.FlowGroup.Id, 
                        LookupKeyUtilities.GetOutputCollectionKey(flowPathway.TransferToStratumId),
                        LookupKeyUtilities.GetOutputCollectionKey(flowPathway.TransferToSecondaryStratumId),
                        LookupKeyUtilities.GetOutputCollectionKey(flowPathway.TransferToTertiaryStratumId),
                        LookupKeyUtilities.GetOutputCollectionKey(flowPathway.TransferToStateClassId),
                        LookupKeyUtilities.GetOutputCollectionKey(flowPathway.TransferToMinimumAge));

                    if (this.m_SummaryOutputFlowRecords.Contains(k))
                    {
                        OutputFlow r = this.m_SummaryOutputFlowRecords[k];
                        r.Amount += (flowAmount * l.Value);
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
                            l.FlowGroup.Id,
                            flowPathway.TransferToStratumId,
                            flowPathway.TransferToSecondaryStratumId,
                            flowPathway.TransferToTertiaryStratumId,
                            flowPathway.TransferToStateClassId,
                            flowPathway.TransferToMinimumAge,
                            flowAmount * l.Value);

                        this.m_SummaryOutputFlowRecords.Add(r);
                    }
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
                dr[Constants.FLOW_GROUP_ID_COLUMN_NAME] = r.FlowGroupId;
				dr[Constants.END_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransferToStratumId);
				dr[Constants.END_SECONDARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransferToSecondaryStratumId);
				dr[Constants.END_TERTIARY_STRATUM_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransferToTertiaryStratumId);
				dr[Constants.END_STATECLASS_ID_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransferToStateClassId);
				dr[Constants.END_MIN_AGE_COLUMN_NAME] = DataTableUtilities.GetNullableDatabaseValue(r.TransferToMinimumAge);

                dr[Constants.AMOUNT_COLUMN_NAME] = r.Amount;

				this.m_OutputFlowTable.Rows.Add(dr);
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
                bool AtLeastOne = false;
                StochasticTimeRaster rastOutput = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                {
                    if (GetSpatialOutputFlowDictionary().ContainsKey(l.FlowType.Id))
                    {
                        SpatialOutputFlowRecord rec = GetSpatialOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            StochasticTimeRaster rastFlowType = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);
                            double[] arr = rastFlowType.DblCells;

                            foreach (Cell c in this.m_STSimTransformer.Cells)
                            {
                                arr[c.CellId] = rec.Data[c.CollectionIndex];
                            }

                            rastFlowType.ScaleDblCells(l.Value);
                            rastOutput.AddDblCells(rastFlowType);

                            AtLeastOne = true;
                        }
                    }
                }

                if (AtLeastOne)
                {
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
        }

        /// <summary>
        /// Processes the current lateral flow group spatial data
        /// </summary>
        /// <remarks></remarks>
        private void ProcessLateralFlowGroupSpatialData(int iteration, int timestep)
        {
            Debug.Assert(this.m_IsSpatial);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                bool AtLeastOne = false;
                StochasticTimeRaster rastOutput = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

                foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                {
                    if (GetLateralOutputFlowDictionary().ContainsKey(l.FlowType.Id))
                    {
                        SpatialOutputFlowRecord rec = GetLateralOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            StochasticTimeRaster rastFlowType = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);
                            double[] arr = rastFlowType.DblCells;

                            foreach (Cell c in this.m_STSimTransformer.Cells)
                            {
                                arr[c.CellId] = rec.Data[c.CollectionIndex];
                            }

                            rastFlowType.ScaleDblCells(l.Value);
                            rastOutput.AddDblCells(rastFlowType);

                            AtLeastOne = true;
                        }
                    }
                }

                if (AtLeastOne)
                {
                    Spatial.WriteRasterData(
                        rastOutput,
                        this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_LATERAL_FLOW_GROUP),
                        iteration,
                        timestep,
                        g.Id,
                        Constants.SPATIAL_MAP_LATERAL_FLOW_GROUP_VARIABLE_PREFIX,
                        Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
                }
            }
        }

        /// <summary>
        /// Adds to the spatial flow summary result collection
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
				if (GetSpatialOutputFlowDictionary().ContainsKey(flowTypeId))
				{
                    SpatialOutputFlowRecord rec = GetSpatialOutputFlowDictionary()[flowTypeId];
					double amt = rec.Data[cell.CollectionIndex];

					if (amt.Equals(Spatial.DefaultNoDataValue))
					{
						amt = 0;
					}

					amt += (flowAmount / this.m_STSimTransformer.AmountPerCell);

					rec.Data[cell.CollectionIndex] = amt;
                    rec.HasOutputData = true;
				}
				else
				{
					Debug.Assert(false, "I think we expected to find a m_SpatialOutputFlow object for the flowType " + flowTypeId.ToString("0000", CultureInfo.InvariantCulture));
				}
			}
		}

        /// <summary>
        /// Adds to the lateral flow summary result collection
        /// </summary>
        /// <param name="timestep"></param>
        /// <param name="cell"></param>
        /// <param name="flowTypeId"></param>
        /// <param name="flowAmount"></param>
        /// <remarks></remarks>
        private void OnLateralFlowOutput(int timestep, Cell cell, int flowTypeId, double flowAmount)
        {
            if (this.m_STSimTransformer.IsOutputTimestep(
                timestep,
                this.m_LateralFlowOutputTimesteps,
                this.m_CreateLateralFlowOutput))
            {
                if (GetLateralOutputFlowDictionary().ContainsKey(flowTypeId))
                {
                    SpatialOutputFlowRecord rec = GetLateralOutputFlowDictionary()[flowTypeId];
                    double amt = rec.Data[cell.CollectionIndex];

                    if (amt.Equals(Spatial.DefaultNoDataValue))
                    {
                        amt = 0;
                    }

                    amt += (flowAmount / this.m_STSimTransformer.AmountPerCell);

                    rec.Data[cell.CollectionIndex] = amt;
                    rec.HasOutputData = true;
                }
                else
                {
                    Debug.Assert(false, "I think we expected to find a m_LateralOutputFlow object for the flowType " + flowTypeId.ToString("0000", CultureInfo.InvariantCulture));
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