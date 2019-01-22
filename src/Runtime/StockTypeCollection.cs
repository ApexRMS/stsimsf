﻿// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
	internal class StockTypeCollection : KeyedCollection<int, StockType>
	{
		protected override int GetKeyForItem(StockType item)
		{
			return item.Id;
		}
	}
}