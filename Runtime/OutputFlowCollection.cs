// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Common;
using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
	internal class OutputFlowCollection : KeyedCollection<TenIntegerLookupKey, OutputFlow>
	{
		public OutputFlowCollection() : base(new TenIntegerLookupKeyEqualityComparer())
		{
		}

		protected override TenIntegerLookupKey GetKeyForItem(OutputFlow item)
		{
			return new TenIntegerLookupKey(
                item.FromStratumId, 
                LookupKeyUtilities.GetOutputCollectionKey(item.FromSecondaryStratumId), 
                LookupKeyUtilities.GetOutputCollectionKey(item.FromTertiaryStratumId), 
                item.FromStateClassId, 
                item.FromStockTypeId, 
                LookupKeyUtilities.GetOutputCollectionKey(item.TransitionTypeId), 
                item.ToStratumId, 
                item.ToStateClassId, 
                item.ToStockTypeId, 
                item.FlowTypeId);
		}
	}
}