// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Common;
using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
	internal class OutputStockCollection : KeyedCollection<FiveIntegerLookupKey, OutputStock>
	{
		public OutputStockCollection() : base(new FiveIntegerLookupKeyEqualityComparer())
		{
		}
		protected override FiveIntegerLookupKey GetKeyForItem(OutputStock item)
		{
			return new FiveIntegerLookupKey(
                item.StratumId,
                LookupKeyUtilities.GetOutputCollectionKey(item.SecondaryStratumId),
                LookupKeyUtilities.GetOutputCollectionKey(item.TertiaryStratumId), 
                item.StateClassId, 
                item.StockTypeId);
		}
	}
}