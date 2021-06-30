// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class OutputFilterBase
    {
        private int m_Id;
        private bool m_OutputSummary;
        private bool m_OutputSpatial;
        private bool m_OutputAvgSpatial;

        public OutputFilterBase(int id, bool outputSummary, bool outputSpatial, bool outputAvgSpatial)
        {
            this.Id = id;
            this.OutputSummary = outputSummary;
            this.OutputSpatial = outputSpatial;
            this.m_OutputAvgSpatial = outputAvgSpatial;
        }

        public int Id { get => m_Id; set => m_Id = value; }
        public bool OutputSummary { get => m_OutputSummary; set => m_OutputSummary = value; }
        public bool OutputSpatial { get => m_OutputSpatial; set => m_OutputSpatial = value; }
        public bool OutputAvgSpatial { get => m_OutputAvgSpatial; set => m_OutputAvgSpatial = value; }
    }
}
