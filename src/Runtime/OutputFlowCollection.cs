// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Common;
using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
	internal class OutputFlowCollection : KeyedCollection<FifteenIntegerLookupKey, OutputFlow>
	{
		public OutputFlowCollection() : base(new FifteenIntegerLookupKeyEqualityComparer())
		{
		}

		protected override FifteenIntegerLookupKey GetKeyForItem(OutputFlow item)
		{
			return new FifteenIntegerLookupKey(
                item.FromStratumId, 
                LookupKeyUtilities.GetOutputCollectionKey(item.FromSecondaryStratumId), 
                LookupKeyUtilities.GetOutputCollectionKey(item.FromTertiaryStratumId), 
                item.FromStateClassId,
                LookupKeyUtilities.GetOutputCollectionKey(item.FromStockTypeId), 
                LookupKeyUtilities.GetOutputCollectionKey(item.TransitionTypeId), 
                item.ToStratumId, 
                item.ToStateClassId,
                LookupKeyUtilities.GetOutputCollectionKey(item.ToStockTypeId), 
                item.FlowGroupId,
                LookupKeyUtilities.GetOutputCollectionKey(item.TransferToStratumId),
                LookupKeyUtilities.GetOutputCollectionKey(item.TransferToSecondaryStratumId),
                LookupKeyUtilities.GetOutputCollectionKey(item.TransferToTertiaryStratumId),
                LookupKeyUtilities.GetOutputCollectionKey(item.TransferToStateClassId),
                LookupKeyUtilities.GetOutputCollectionKey(item.TransferToMinimumAge));
		}
	}
}
