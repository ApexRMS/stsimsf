// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class StockFlowType
	{
		private int m_Id;

		public StockFlowType(int id)
		{
			this.m_Id = id;
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}
	}
}