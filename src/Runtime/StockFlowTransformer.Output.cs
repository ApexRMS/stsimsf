// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Linq;
using System.Diagnostics;
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
        private bool m_AvgSpatialStockOutputAcrossTimesteps;
		private bool m_CreateAvgSpatialFlowOutput;
		private int m_AvgSpatialFlowOutputTimesteps;
        private bool m_AvgSpatialFlowOutputAcrossTimesteps;
		private bool m_CreateAvgSpatialLateralFlowOutput;
		private int m_AvgSpatialLateralFlowOutputTimesteps;
        private bool m_AvgSpatialLateralFlowOutputAcrossTimesteps;

		private Dictionary<int, SpatialOutputFlowRecord> m_SpatialOutputFlowDict;
		private Dictionary<int, SpatialOutputFlowRecord> m_LateralOutputFlowDict;
		private OutputFlowCollection m_SummaryOutputFlowRecords = new OutputFlowCollection();
		private OutputStockCollection m_SummaryOutputStockRecords = new OutputStockCollection();

		private void InitializeOutputDataTables()
		{
			Debug.Assert(this.m_OutputStockTable == null);
			Debug.Assert(this.m_OutputFlowTable == null);

			this.m_OutputStockTable = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_STOCK_NAME).GetData();
			this.m_OutputFlowTable = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_FLOW_NAME).GetData();

			Debug.Assert(this.m_OutputStockTable.Rows.Count == 0);
			Debug.Assert(this.m_OutputFlowTable.Rows.Count == 0);
		}

        private void RecordSummaryStockOutputData()
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

        private void RecordSummaryFlowOutputData(
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

        private void RecordSpatialFlowOutputData(int timestep, Cell cell, int flowTypeId, double flowAmount)
        {
            if (!this.m_IsSpatial)
            {
                return;
            }

            bool IsNormalOutputTimestep = this.m_STSimTransformer.IsOutputTimestep(
                timestep,
                this.m_SpatialFlowOutputTimesteps,
                this.m_CreateSpatialFlowOutput);

            bool IsAverageOutputTimestep = this.m_STSimTransformer.IsOutputTimestep(
                timestep,
                this.m_AvgSpatialFlowOutputTimesteps,
                this.m_CreateAvgSpatialFlowOutput);

            if (!IsNormalOutputTimestep && !IsAverageOutputTimestep)
            {
                return;
            }

            Debug.Assert(GetSpatialOutputFlowDictionary().ContainsKey(flowTypeId));

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
        }

        private void RecordSpatialLateralFlowOutputData(int timestep, Cell cell, int flowTypeId, double flowAmount)
        {
            bool IsLFOutputTimestep = this.m_STSimTransformer.IsOutputTimestep(
                timestep,
                this.m_LateralFlowOutputTimesteps,
                this.m_CreateLateralFlowOutput);

            bool IsLFAverageOutputTimestep = this.m_STSimTransformer.IsOutputTimestep(
                timestep,
                this.m_AvgSpatialLateralFlowOutputTimesteps,
                this.m_CreateAvgSpatialLateralFlowOutput);

            if (!IsLFOutputTimestep && !IsLFAverageOutputTimestep)
            {
                return;
            }

            Debug.Assert(GetLateralOutputFlowDictionary().ContainsKey(flowTypeId));

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
        }

        private void WriteTabularSummaryStockOutput(int iteration, int timestep)
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

        private void WriteTabularSummaryFlowOutputData(int iteration, int timestep)
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

		private void WriteStockGroupRasters(int iteration, int timestep)
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

		private void WriteFlowGroupRasters(int iteration, int timestep)
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

        private void WriteLateralFlowRasters(int iteration, int timestep)
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
                            StochasticTimeRaster rastFlowType = 
                                this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);

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

        private void WriteAverageStockRasters()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialStockOutput);

            foreach (int id in this.m_AvgStockMap.Keys)
            {
                Dictionary<int, double[]> dict = this.m_AvgStockMap[id];

                foreach (int timestep in dict.Keys)
                {
                    double[] values = dict[timestep];
                    var distArray = values.Distinct();

                    if (distArray.Count() == 1)
                    {
                        var el0 = distArray.ElementAt(0);

                        if (el0.Equals(Spatial.DefaultNoDataValue))
                        {
                            continue;
                        }
                    }

                    StochasticTimeRaster rast = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);
                    double[] arr = rast.DblCells;

                    foreach (Cell c in this.STSimTransformer.Cells)
                    {
                        arr[c.CellId] = values[c.CollectionIndex];
                    }

                    Spatial.WriteRasterData(
                        rast,
                        this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_AVG_SPATIAL_STOCK_GROUP),
                        0,
                        timestep,
                        id,
                        Constants.SPATIAL_MAP_AVG_STOCK_GROUP_VARIABLE_PREFIX,
                        Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
                }
            }
        }

        private void WriteAverageFlowRasters()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialFlowOutput);

            foreach (int id in this.m_AvgFlowMap.Keys)
            {
                Dictionary<int, double[]> dict = this.m_AvgFlowMap[id];

                foreach (int timestep in dict.Keys)
                {
                    if (timestep == this.STSimTransformer.TimestepZero)
                    {
                        continue;
                    }

                    double[] values = dict[timestep];
                    var distArray = values.Distinct();

                    if (distArray.Count() == 1)
                    {
                        var el0 = distArray.ElementAt(0);

                        if (el0.Equals(Spatial.DefaultNoDataValue))
                        {
                            continue;
                        }
                    }

                    StochasticTimeRaster rast = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);
                    double[] arr = rast.DblCells;

                    foreach (Cell c in this.STSimTransformer.Cells)
                    {
                        arr[c.CellId] = values[c.CollectionIndex];
                    }

                    Spatial.WriteRasterData(
                        rast,
                        this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_AVG_SPATIAL_FLOW_GROUP),
                        0,
                        timestep,
                        id,
                        Constants.SPATIAL_MAP_AVG_FLOW_GROUP_VARIABLE_PREFIX,
                        Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
                }
            }
        }

        private void WriteAverageLateralFlowRasters()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialLateralFlowOutput);

            foreach (int id in this.m_AvgLateralFlowMap.Keys)
            {
                Dictionary<int, double[]> dict = this.m_AvgLateralFlowMap[id];

                foreach (int timestep in dict.Keys)
                {
                    if (timestep == this.STSimTransformer.TimestepZero)
                    {
                        continue;
                    }

                    double[] values = dict[timestep];
                    var distArray = values.Distinct();

                    if (distArray.Count() == 1)
                    {
                        var el0 = distArray.ElementAt(0);

                        if (el0.Equals(Spatial.DefaultNoDataValue))
                        {
                            continue;
                        }
                    }

                    StochasticTimeRaster rast = this.STSimTransformer.InputRasters.CreateOutputRaster(RasterDataType.DTDouble);
                    double[] arr = rast.DblCells;

                    foreach (Cell c in this.STSimTransformer.Cells)
                    {
                        arr[c.CellId] = values[c.CollectionIndex];
                    }

                    Spatial.WriteRasterData(
                        rast,
                        this.ResultScenario.GetDataSheet(Constants.DATASHEET_OUTPUT_AVG_SPATIAL_LATERAL_FLOW_GROUP),
                        0,
                        timestep,
                        id,
                        Constants.SPATIAL_MAP_AVG_LATERAL_FLOW_GROUP_VARIABLE_PREFIX,
                        Constants.DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN);
                }
            }
        }

        private void RecordAverageStockValues(int timestep)
        {
            if (!this.STSimTransformer.IsSpatial)
            {
                return;
            }

            if (!this.m_CreateAvgSpatialStockOutput)
            {
                return;
            }

            if (this.m_AvgSpatialStockOutputAcrossTimesteps)
            {
                this.RecordAverageStockValuesAcrossTimesteps(timestep);
            }
            else
            {
                this.RecordAverageStockValuesNormalMethod(timestep);
            }
        }

        private void RecordAverageStockValuesTimestepZero()
        {
            if (!this.STSimTransformer.IsSpatial)
            {
                return;
            }

            if (!this.m_CreateAvgSpatialStockOutput)
            {
                return;
            }

            this.RecordAverageStockValuesNormalMethod(this.STSimTransformer.TimestepZero);
        }

        private void RecordAverageStockValuesNormalMethod(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialStockOutput);

#if DEBUG
            if (timestep != this.STSimTransformer.TimestepZero) { Debug.Assert(!this.m_AvgSpatialStockOutputAcrossTimesteps); }
#endif

            foreach (StockGroup g in this.m_StockGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgStockMap[g.Id];
                double[] Values = dict[timestep];

                foreach (Cell c in this.STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;
                    Dictionary<int, double> StockAmounts = GetStockAmountDictionary(c);

                    foreach (StockTypeLinkage l in g.StockTypeLinkages)
                    {
                       Amount += StockAmounts[l.StockType.Id];
                    }

                    Values[i] += Amount / this.m_TotalIterations;
                }               
            }
        }

        private void RecordAverageStockValuesAcrossTimesteps(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialStockOutput);
            Debug.Assert(this.m_AvgSpatialStockOutputAcrossTimesteps);

            int timestepKey = this.GetTimestepKeyForCumulativeAverage(timestep, this.m_AvgSpatialStockOutputTimesteps);

            foreach (StockGroup g in this.m_StockGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgStockMap[g.Id];
                double[] Values = dict[timestepKey];

                foreach (Cell c in this.STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;
                    Dictionary<int, double> StockAmounts = GetStockAmountDictionary(c);

                    foreach (StockTypeLinkage l in g.StockTypeLinkages)
                    {
                        Amount += StockAmounts[l.StockType.Id];
                    }

                    if ((timestepKey == this.STSimTransformer.MaximumTimestep) && (((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialStockOutputTimesteps) != 0))
                    {
                        Values[i] += Amount / (double)((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialStockOutputTimesteps * this.m_TotalIterations);
                    }
                    else
                    {
                        Values[i] += Amount / (double)(this.m_AvgSpatialStockOutputTimesteps * this.m_TotalIterations);
                    }
                }
            }
        }

        private void RecordAverageFlowValues(int timestep)
        {
            if (!this.STSimTransformer.IsSpatial)
            {
                return;
            }

            if (!this.m_CreateAvgSpatialFlowOutput)
            {
                return;
            }

            if (this.m_AvgSpatialFlowOutputAcrossTimesteps)
            {
                this.RecordAverageFlowValuesAcrossTimesteps(timestep);
            }
            else
            {
                this.RecordAverageFlowValuesNormalMethod(timestep);
            }
        }

        private void RecordAverageFlowValuesNormalMethod(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialFlowOutput);
            Debug.Assert(!this.m_AvgSpatialFlowOutputAcrossTimesteps);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgFlowMap[g.Id];
                double[] Values = dict[timestep];

                foreach (Cell c in this.m_STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;

                    foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                    {
                        SpatialOutputFlowRecord rec = GetSpatialOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            Amount += rec.Data[i];
                        }
                    }

                    Values[i] += Amount / this.m_TotalIterations;
                }
            }
        }

        private void RecordAverageFlowValuesAcrossTimesteps(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialFlowOutput);
            Debug.Assert(this.m_AvgSpatialFlowOutputAcrossTimesteps);

            int timestepKey = this.GetTimestepKeyForCumulativeAverage(timestep, this.m_AvgSpatialFlowOutputTimesteps);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgFlowMap[g.Id];
                double[] Values = dict[timestepKey];

                foreach (Cell c in this.m_STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;

                    foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                    {
                        SpatialOutputFlowRecord rec = GetSpatialOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            Amount += rec.Data[i];
                        }
                    }

                    if ((timestepKey == this.STSimTransformer.MaximumTimestep) && (((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialFlowOutputTimesteps) != 0))
                    {
                        Values[i] += Amount / (double)((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialFlowOutputTimesteps * this.m_TotalIterations);
                    }
                    else
                    {
                        Values[i] += Amount / (double)(this.m_AvgSpatialFlowOutputTimesteps * this.m_TotalIterations);
                    }
                }
            }
        }

        private void RecordAverageLateralFlowValues(int timestep)
        {
            if (!this.STSimTransformer.IsSpatial)
            {
                return;
            }

            if (!this.m_CreateAvgSpatialLateralFlowOutput)
            {
                return;
            }

            if (this.m_AvgSpatialLateralFlowOutputAcrossTimesteps)
            {
                this.RecordAverageLateralFlowValuesAcrossTimesteps(timestep);
            }
            else
            {
                this.RecordAverageLateralFlowValuesNormalMethod(timestep);
            }
        }

        private void RecordAverageLateralFlowValuesNormalMethod(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialLateralFlowOutput);
            Debug.Assert(!this.m_AvgSpatialLateralFlowOutputAcrossTimesteps);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgLateralFlowMap[g.Id];
                double[] Values = dict[timestep];

                foreach (Cell c in this.m_STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;

                    foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                    {
                        SpatialOutputFlowRecord rec = GetLateralOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            Amount += rec.Data[i];
                        }
                    }

                    Values[i] += Amount / this.m_TotalIterations;
                }
            }
        }

        private void RecordAverageLateralFlowValuesAcrossTimesteps(int timestep)
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.m_CreateAvgSpatialLateralFlowOutput);
            Debug.Assert(this.m_AvgSpatialLateralFlowOutputAcrossTimesteps);

            int timestepKey = this.GetTimestepKeyForCumulativeAverage(timestep, this.m_AvgSpatialLateralFlowOutputTimesteps);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = this.m_AvgLateralFlowMap[g.Id];
                double[] Values = dict[timestepKey];

                foreach (Cell c in this.m_STSimTransformer.Cells)
                {
                    double Amount = 0;
                    int i = c.CollectionIndex;

                    foreach (FlowTypeLinkage l in g.FlowTypeLinkages)
                    {
                        SpatialOutputFlowRecord rec = GetLateralOutputFlowDictionary()[l.FlowType.Id];

                        if (rec.HasOutputData)
                        {
                            Amount += rec.Data[i];
                        }
                    }

                    if ((timestepKey == this.STSimTransformer.MaximumTimestep) && (((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialLateralFlowOutputTimesteps) != 0))
                    {
                        Values[i] += Amount / (double)((timestepKey - this.STSimTransformer.TimestepZero) % this.m_AvgSpatialLateralFlowOutputTimesteps * this.m_TotalIterations);
                    }
                    else
                    {
                        Values[i] += Amount / (double)(this.m_AvgSpatialLateralFlowOutputTimesteps * this.m_TotalIterations);
                    }
                }
            }
        }

        private int GetTimestepKeyForCumulativeAverage(int timestep, int frequency)
        {
            int timestepKey = 0;

            if (timestep == this.STSimTransformer.MaximumTimestep)
            {
                timestepKey = this.STSimTransformer.MaximumTimestep;
            }
            else
            {
                //We're looking for the the timestep which is the first one that is >= to the current timestep

                timestepKey = Convert.ToInt32(Math.Ceiling(
                    Convert.ToDouble(timestep - this.STSimTransformer.TimestepZero) / frequency) * frequency) +
                        this.STSimTransformer.TimestepZero;

                if (timestepKey > this.STSimTransformer.MaximumTimestep)
                {
                    timestepKey = this.STSimTransformer.MaximumTimestep;
                }
            }

            return timestepKey;
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