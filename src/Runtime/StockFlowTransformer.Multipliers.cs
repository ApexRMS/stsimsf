// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Diagnostics;

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
    }
}
