// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class SpatialOutputFlowRecord
    {
        public int FlowTypeId;
        public double[] Data;
        public bool HasOutputData;
    }
}
