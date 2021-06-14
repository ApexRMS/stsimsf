// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class FlowGroup : StockFlowType
	{
        private FlowTypeLinkageCollection m_FlowTypeLinkages = new FlowTypeLinkageCollection();

        public FlowGroup(int id, string name) : base(id, name)
        {
        }

        internal FlowTypeLinkageCollection FlowTypeLinkages
        {
            get
            {
                return this.m_FlowTypeLinkages;
            }
        }
	}
}