﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class FlowOrder
	{
		private int? m_iteration;
		private int? m_timestep;
		private int m_flowTypeId;
		private double m_Order = Constants.DEFAULT_FLOW_ORDER;

		public FlowOrder(int? iteration, int? timestep, int flowTypeId, double? order)
		{
			this.m_iteration = iteration;
			this.m_timestep = timestep;
			this.m_flowTypeId = flowTypeId;

			if (order.HasValue)
			{
				this.m_Order = order.Value;
			}
		}

		public int? Iteration
		{
			get
			{
				return m_iteration;
			}
		}

		public int? Timestep
		{
			get
			{
				return m_timestep;
			}
		}

		public int FlowTypeId
		{
			get
			{
				return m_flowTypeId;
			}
		}

		public double Order
		{
			get
			{
				return m_Order;
			}
		}
	}
}