// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class FlowTypeLinkage
    {
        private FlowType m_FlowType;
        private float m_Value;

        public FlowTypeLinkage(FlowType flowType, float value)
        {
            this.m_FlowType = flowType;
            this.m_Value = value;
        }

        internal FlowType FlowType
        {
            get
            {
                return m_FlowType;
            }
        }

        public float Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}
