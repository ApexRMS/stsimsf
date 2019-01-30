// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
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