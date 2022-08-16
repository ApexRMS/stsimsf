// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class StockTypeLinkage
    {
        private StockType m_StockType;
        private float m_Value;

        public StockTypeLinkage(StockType stockType, float value)
        {
            this.m_StockType = stockType;
            this.m_Value = value;
        }

        internal StockType StockType
        {
            get
            {
                return m_StockType;
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
