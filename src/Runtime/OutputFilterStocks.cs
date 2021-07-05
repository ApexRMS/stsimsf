// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class OutputFilterStocks : OutputFilterBase
    {
        public OutputFilterStocks(
            int id,
            bool outputSummary,
            bool outputSpatial,
            bool outputAvgSpatial) : base(id, outputSummary, outputSpatial, outputAvgSpatial)
        {
        }
    }
}
