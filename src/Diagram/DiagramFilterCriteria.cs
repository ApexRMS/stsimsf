// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	internal class DiagramFilterCriteria
	{
		private Dictionary<int, bool> m_FlowTypes = new Dictionary<int, bool>();

		public Dictionary<int, bool> FlowTypes
		{
			get
			{
				return this.m_FlowTypes;
			}
		}
	}
}