﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class FlowGroupLinkage
    {
        private FlowGroup m_FlowGroup;
        private double m_Value;

        public FlowGroupLinkage(FlowGroup flowGroup, double value)
        {
            this.m_FlowGroup = flowGroup;
            this.m_Value = value;
        }

        internal FlowGroup FlowGroup
        {
            get
            {
                return m_FlowGroup;
            }
        }

        public double Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}
