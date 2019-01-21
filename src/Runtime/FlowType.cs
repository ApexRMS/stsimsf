// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class FlowType : StockFlowType
	{
		private FlowGroupCollection m_FlowGroups = new FlowGroupCollection();
		private double m_Order = Constants.DEFAULT_FLOW_ORDER;

		public FlowType(int id) : base(id)
		{
		}

		public FlowGroupCollection FlowGroups
		{
			get
			{
				return this.m_FlowGroups;
			}
		}

		public double Order
		{
			get
			{
				return m_Order;
			}
			set
			{
				m_Order = value;
			}
		}
	}
}