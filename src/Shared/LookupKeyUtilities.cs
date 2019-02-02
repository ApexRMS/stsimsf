// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using SyncroSim.Core;
using System.Globalization;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	internal static class LookupKeyUtilities
	{
		public static int GetOutputCollectionKey(int? stratumId)
		{
			if (stratumId.HasValue)
			{
				return stratumId.Value;
			}
			else
			{
				return Constants.OUTPUT_COLLECTION_WILDCARD_KEY;
			}
		}

		public static Dictionary<int, bool> CreateRecordLookup(DataSheet ds, string colName)
		{
			Dictionary<int, bool> d = new Dictionary<int, bool>();
			DataTable dt = ds.GetData();

			foreach (DataRow dr in dt.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					d.Add(Convert.ToInt32(dr[colName], CultureInfo.InvariantCulture), true);
				}
			}

			return d;
		}
	}
}