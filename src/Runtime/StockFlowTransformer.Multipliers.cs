// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Diagnostics;
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
            int cellId,
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
            double v = raster.DblCells[cellId];

            if ((v < 0.0) || (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, double.Epsilon)))
            {
                return 1.0;
            }
            else
            {
                return v;
            }
        }

        private double GetFlowLateralMultiplier(
            int cellId,
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
            double v = raster.DblCells[cellId];

            if ((v < 0.0) || (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, double.Epsilon)))
            {
                return 1.0;
            }
            else
            {
                return v;
            }
        }
    }
}
