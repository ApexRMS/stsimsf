// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using SyncroSim.Core;

namespace SyncroSim.STSimStockFlow
{
	internal static class TerminologyUtilities
	{
		public static string GetTerminology(DataSheet terminologySheet, string columnName)
		{

			DataRow dr = terminologySheet.GetDataRow();

			if (dr == null || dr[columnName] == DBNull.Value)
			{
				return "Units";
			}
			else
			{
				return Convert.ToString(dr[columnName]);
			}
		}

		public static string GetTimestepUnits(Project project)
		{
			DataRow dr = project.GetDataSheet("STSim_Terminology").GetDataRow();

			if (dr == null || dr["TimestepUnits"] == DBNull.Value)
			{
				return "Timestep";
			}
			else
			{
				return Convert.ToString(dr["TimestepUnits"]);
			}
		}

		public static void GetStratumLabelStrings(DataSheet terminologyDataSheet, ref string primaryStratumLabel, ref string secondaryStratumLabel, ref string tertiaryStratumLabel)
		{
			DataRow dr = terminologyDataSheet.GetDataRow();

			primaryStratumLabel = "Stratum";
			secondaryStratumLabel = "Secondary Stratum";
			tertiaryStratumLabel = "Tertiary Stratum";

			if (dr != null)
			{
				if (dr["PrimaryStratumLabel"] != DBNull.Value)
				{
					primaryStratumLabel = Convert.ToString(dr["PrimaryStratumLabel"]);
				}

				if (dr["SecondaryStratumLabel"] != DBNull.Value)
				{
					secondaryStratumLabel = Convert.ToString(dr["SecondaryStratumLabel"]);
				}

				if (dr["TertiaryStratumLabel"] != DBNull.Value)
				{
					tertiaryStratumLabel = Convert.ToString(dr["TertiaryStratumLabel"]);
				}
			}
		}
	}
}