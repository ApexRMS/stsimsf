// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
    class StockTypeLinkage
    {
        private StockType m_StockType;
        private double m_Value;

        public StockTypeLinkage(StockType stockType, double value)
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

        public double Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}
