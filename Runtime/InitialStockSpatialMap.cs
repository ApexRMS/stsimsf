// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Common;
using System.Diagnostics;

namespace SyncroSim.STSimStockFlow
{
	internal class InitialStockSpatialMap
	{
		private bool m_HasItems;
		private SortedKeyMap1<InitialStockSpatialCollection> m_Map = new SortedKeyMap1<InitialStockSpatialCollection>(SearchMode.ExactPrev);

		public InitialStockSpatialMap(InitialStockSpatialCollection icd)
		{
			foreach (InitialStockSpatial t in icd)
			{
				this.AddISS(t);
			}
		}

		private void AddISS(InitialStockSpatial iss)
		{
			InitialStockSpatialCollection l = this.m_Map.GetItemExact(iss.Iteration);

			if (l == null)
			{
				l = new InitialStockSpatialCollection();
				this.m_Map.AddItem(iss.Iteration, l);
			}

			l.Add(iss);
			this.m_HasItems = true;
		}

		public InitialStockSpatialCollection GetItem(int? iteration)
		{
			if (!this.m_HasItems)
			{
				return null;
			}

			InitialStockSpatialCollection l = this.m_Map.GetItem(iteration);

			if (l == null)
			{
				return null;
			}

			Debug.Assert(l.Count > 0);
			return l;
		}
	}
}