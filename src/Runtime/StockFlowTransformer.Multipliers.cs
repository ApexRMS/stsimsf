// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using SyncroSim.Core;
using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
    partial class StockFlowTransformer
    {
        private FlowMultiplierType GetFlowMultiplierType(int? id)
        {
            foreach (FlowMultiplierType t in this.m_FlowMultiplierTypes)
            {
                if (Nullable.Compare(t.FlowMultiplierTypeId, id) == 0)
                {
                    return t;
                }
            }

            Debug.Assert(false);
            return null;
        }

        private double GetFlowSpatialMultiplier(
            Cell cell,
            FlowSpatialMultiplierMap map,
            int flowGroupId,
            int iteration,
            int timestep)
        {
            Debug.Assert(this.m_IsSpatial);
            Debug.Assert(this.m_FlowSpatialMultipliers.Count > 0);

            FlowSpatialMultiplier m = map.GetFlowSpatialMultiplier(flowGroupId, iteration, timestep);

            if (m == null)
            {
                return 1.0;
            }

            if (!this.m_FlowSpatialMultiplierRasters.ContainsKey(m.FileName))
            {
                return 1.0;
            }

            StochasticTimeRaster raster = this.m_FlowSpatialMultiplierRasters[m.FileName];
            double v = raster.DblCells[cell.CollectionIndex];

            if (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, double.Epsilon))
            {
                return 1.0;
            }
            else
            {
                return v;
            }
        }

        private double GetFlowLateralMultiplier(
            Cell cell,
            FlowLateralMultiplierMap map,
            int flowGroupId,
            int iteration,
            int timestep)
        {
            Debug.Assert(this.m_IsSpatial);
            Debug.Assert(this.m_FlowLateralMultipliers.Count > 0);

            FlowLateralMultiplier m = map.GetFlowLateralMultiplier(flowGroupId, iteration, timestep);

            if (m == null)
            {
                return 1.0;
            }

            if (!this.m_FlowLateralMultiplierRasters.ContainsKey(m.FileName))
            {
                return 1.0;
            }

            StochasticTimeRaster raster = this.m_FlowLateralMultiplierRasters[m.FileName];
            double v = raster.DblCells[cell.CollectionIndex];

            if ((v < 0.0) || (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, double.Epsilon)))
            {
                return 1.0;
            }
            else
            {
                return v;
            }
        }

        private double GetFlowLateralMultiplier(int flowTypeId, Cell cell, int iteration, int timestep)
        {
            double Multiplier = 1.0;
            FlowType ft = this.m_FlowTypes[flowTypeId];

            foreach (FlowMultiplierType mt in this.m_FlowMultiplierTypes)
            {
                foreach (FlowGroupLinkage fgl in ft.FlowGroupLinkages)
                {
                    if (this.m_IsSpatial && mt.FlowLateralMultiplierMap != null)
                    {
                        Multiplier *= this.GetFlowLateralMultiplier(
                            cell,
                            mt.FlowLateralMultiplierMap,
                            fgl.FlowGroup.Id,
                            iteration,
                            timestep);
                    }
                }
            }

            Debug.Assert(Multiplier >= 0.0);
            return Multiplier;
        }

        private void ValidateFlowSpatialMultipliers()
        {
            Debug.Assert(this.m_IsSpatial);
            DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME);

            for (int i = this.m_FlowSpatialMultipliers.Count - 1; i >= 0; i--)
            {
                FlowSpatialMultiplier r = this.m_FlowSpatialMultipliers[i];

                if (!this.m_FlowSpatialMultiplierRasters.ContainsKey(r.FileName))
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_PROCESS_WARNING, r.FileName);
                    RecordStatus(StatusType.Warning, msg);

                    continue;
                }

                string cmpMsg = "";
                var cmpRes = this.STSimTransformer.InputRasters.CompareMetadata(this.m_FlowSpatialMultiplierRasters[r.FileName], ref cmpMsg);
                string FullFilename = Spatial.GetSpatialInputFileName(ds, r.FileName, false);

                if (cmpRes == CompareMetadataResult.RowColumnMismatch)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_ROW_COLUMN_MISMATCH, FullFilename);
                    ExceptionUtils.ThrowArgumentException(msg);
                }
                else
                {
                    if (cmpRes == CompareMetadataResult.UnimportantDifferences)
                    {
                        string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_INFO, FullFilename, cmpMsg);
                        RecordStatus(StatusType.Information, msg);
                    }
                }
            }
        }

        private void ValidateFlowLateralMultipliers()
        {
            Debug.Assert(this.m_IsSpatial);
            DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_LATERAL_MULTIPLIER_NAME);

            for (int i = this.m_FlowLateralMultipliers.Count - 1; i >= 0; i--)
            {
                FlowLateralMultiplier r = this.m_FlowLateralMultipliers[i];

                if (!this.m_FlowLateralMultiplierRasters.ContainsKey(r.FileName))
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_PROCESS_WARNING, r.FileName);
                    RecordStatus(StatusType.Warning, msg);

                    continue;
                }

                string cmpMsg = "";
                var cmpRes = this.STSimTransformer.InputRasters.CompareMetadata(this.m_FlowLateralMultiplierRasters[r.FileName], ref cmpMsg);
                string FullFilename = Spatial.GetSpatialInputFileName(ds, r.FileName, false);

                if (cmpRes == CompareMetadataResult.RowColumnMismatch)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_ROW_COLUMN_MISMATCH, FullFilename);
                    ExceptionUtils.ThrowArgumentException(msg);
                }
                else
                {
                    if (cmpRes == CompareMetadataResult.UnimportantDifferences)
                    {
                        string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_INFO, FullFilename, cmpMsg);
                        RecordStatus(StatusType.Information, msg);
                    }
                }
            }
        }
    }
}
