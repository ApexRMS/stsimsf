// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowGroupCollection : KeyedCollection<int, FlowGroup>
	{
		protected override int GetKeyForItem(FlowGroup item)
		{
			return item.Id;
		}
	}
}