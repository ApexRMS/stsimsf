// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

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