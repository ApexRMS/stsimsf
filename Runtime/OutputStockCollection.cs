//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

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