// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;
using SyncroSim.StochasticTime.Forms;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class SFChartProvider : ChartProvider
	{
        private const string TOTAL_GROUP_NAME = "stockflow_total_group";
		private const string DENSITY_GROUP_NAME = "stockflow_density_group";

		public override DataTable GetData(DataStore store, ChartDescriptor descriptor, DataSheet dataSheet)
		{
			if (descriptor.DatasheetName == "stsim_stockflow__OutputStock" || descriptor.DatasheetName == "stsim_stockflow__OutputFlow")
			{
				string[] v = descriptor.VariableName.Split('-');
				string VarName = v[0];

				if (
                    VarName == "stockgroup" || 
                    VarName == "stockgroupdensity" || 
                    VarName == "flowgroup" || 
                    VarName == "flowgroupdensity")
				{
					return CreateRawChartData(dataSheet, descriptor, store, VarName);
				}
			}

			return null;
		}

		public override void RefreshCriteria(SyncroSimLayout layout, Project project)
		{
			//Stock Groups
			SyncroSimLayoutItem StockGroupsGroup = new SyncroSimLayoutItem("StockGroups", "Stocks", true);

			StockGroupsGroup.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputStock"));
			StockGroupsGroup.Properties.Add(new MetaDataProperty("filter", "StratumID|SecondaryStratumID|TertiaryStratumID|StateClassID|StockGroupID"));

			AddStockGroupChartVariables(project, StockGroupsGroup.Items);

			if (StockGroupsGroup.Items.Count > 0)
			{
				layout.Items.Add(StockGroupsGroup);
			}

			//Flow Groups
			SyncroSimLayoutItem FlowGroupsGroup = new SyncroSimLayoutItem("FlowGroups", "Flows", true);

			FlowGroupsGroup.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputFlow"));
			FlowGroupsGroup.Properties.Add(new MetaDataProperty("filter", "FromStratumID|FromSecondaryStratumID|FromTertiaryStratumID|FromStateClassID|FromStockTypeID|TransitionTypeID|ToStratumID|ToStateClassID|ToStockTypeID|FlowGroupID|EndStratumID|EndSecondaryStratumID|EndTertiaryStratumID|EndStateClassID"));

			AddFlowGroupChartVariables(project, FlowGroupsGroup.Items);

			if (FlowGroupsGroup.Items.Count > 0)
			{
				layout.Items.Add(FlowGroupsGroup);
			}
		}

		private static void AddStockGroupChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_STOCK_GROUP_NAME);

            if (ds.HasData())
            {
				//Normal
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem("stockgroup", "Total", false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputStock"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemNormal);

				//Density
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem("stockgroupdensity", "Density", false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputStock"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemDensity);
            }
		}

		private static void AddFlowGroupChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_FLOW_GROUP_NAME);

            if (ds.HasData())
            {
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem("flowgroup", "Total", false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputFlow"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

                items.Add(ItemNormal);

				//Density
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem("flowgroupdensity", "Density", false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "stsim_stockflow__OutputFlow"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

                items.Add(ItemDensity);
            }
        }

		private static DataTable CreateRawChartData(
            DataSheet dataSheet, 
            ChartDescriptor descriptor, 
            DataStore store, 
            string variableName)
		{
            Debug.Assert(
                variableName == "stockgroup" ||
                variableName == "stockgroupdensity" ||
                variableName == "flowgroup" ||
                variableName == "flowgroupdensity");

            string query = CreateRawChartDataQueryForGroup(dataSheet, descriptor, variableName);		
            DataTable dt = StochasticTime.ChartCache.GetCachedData(dataSheet.Scenario, query, null);

            if (dt == null)
            {
                dt = store.CreateDataTableFromQuery(query, "RawData");
                StochasticTime.ChartCache.SetCachedData(dataSheet.Scenario, query, dt, null);
            }
                              
			if (variableName.EndsWith("density", StringComparison.Ordinal))
			{
				Dictionary<string, double> dict = CreateAmountDictionary(dataSheet.Scenario, descriptor, variableName, store);

				if (dict.Count > 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						int it = Convert.ToInt32(dr["Iteration"], CultureInfo.InvariantCulture);
						int ts = Convert.ToInt32(dr["Timestep"], CultureInfo.InvariantCulture);

						string k = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", it, ts);
						dr["SumOfAmount"] = Convert.ToDouble(dr["SumOfAmount"], CultureInfo.InvariantCulture) / dict[k];
					}
				}
			}

			return dt;
		}

		private static string CreateRawChartDataQueryForGroup(
            DataSheet dataSheet, 
            ChartDescriptor descriptor,
            string variableName)
		{
            Debug.Assert(dataSheet.Scenario.Id > 0);

			Debug.Assert(
                variableName == "stockgroup" || variableName == "stockgroupdensity" || 
                variableName == "flowgroup" || variableName == "flowgroupdensity");

            string ScenarioClause = string.Format(CultureInfo.InvariantCulture,
                "([{0}]={1})",
                Constants.SCENARIO_ID_COLUMN_NAME, dataSheet.Scenario.Id);

            string SumStatement = string.Format(CultureInfo.InvariantCulture,
                "SUM([{0}]) AS {1}",
                descriptor.ColumnName, Constants.SUM_OF_AMOUNT_COLUMN_NAME);

            string WhereClause = ScenarioClause;

            if (!string.IsNullOrEmpty(descriptor.DisaggregateFilter))
            {
                WhereClause = string.Format(CultureInfo.InvariantCulture,
                    "{0} AND ({1})",
                    WhereClause, descriptor.DisaggregateFilter);
            }

            if (!string.IsNullOrEmpty(descriptor.IncludeDataFilter))
            {
                WhereClause = string.Format(CultureInfo.InvariantCulture,
                    "{0} AND ({1})",
                    WhereClause, descriptor.IncludeDataFilter);
            }

            string query = string.Format(CultureInfo.InvariantCulture,
                "SELECT {0},{1},{2} FROM {3} WHERE {4} GROUP BY [{5}],[{6}]",
                Constants.ITERATION_COLUMN_NAME,
                Constants.TIMESTEP_COLUMN_NAME,
                SumStatement,
                descriptor.DatasheetName,
                WhereClause,
                Constants.ITERATION_COLUMN_NAME,
                Constants.TIMESTEP_COLUMN_NAME);

            return query;
        }

		public static Dictionary<string, double> CreateAmountDictionary(
            Scenario scenario, 
            ChartDescriptor descriptor, 
            string variableName, 
            DataStore store)
		{
			Dictionary<string, double> dict = new Dictionary<string, double>();
			string query = CreateAmountQuery(scenario, descriptor, variableName);
            DataTable dt = StochasticTime.ChartCache.GetCachedData(scenario, query, null);

            if (dt == null)
            {
                dt = store.CreateDataTableFromQuery(query, "AmountData");
                StochasticTime.ChartCache.SetCachedData(scenario, query, dt, null);
            }
                             
			foreach (DataRow dr in dt.Rows)
			{
				int it = Convert.ToInt32(dr["Iteration"], CultureInfo.InvariantCulture);
				int ts = Convert.ToInt32(dr["Timestep"], CultureInfo.InvariantCulture);
				string k = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", it, ts);

				dict.Add(k, Convert.ToDouble(dr["SumOfAmount"], CultureInfo.InvariantCulture));
			}

			return dict;
		}

		private static string CreateAmountQuery(Scenario scenario, ChartDescriptor descriptor, string variableName)
		{
			Debug.Assert(variableName.EndsWith("density", StringComparison.Ordinal));

			string ScenarioClause = string.Format(CultureInfo.InvariantCulture, 
                "([{0}]={1})", 
                Constants.SCENARIO_ID_COLUMN_NAME, scenario.Id);

			string WhereClause = ScenarioClause;
			string Disagg = RemoveUnwantedColumnReferences(descriptor.DisaggregateFilter, variableName);
			string IncData = RemoveUnwantedColumnReferences(descriptor.IncludeDataFilter, variableName);

			if (!string.IsNullOrEmpty(Disagg))
			{
				WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, Disagg);
			}

			if (!string.IsNullOrEmpty(IncData))
			{
				WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, IncData);
			}

			string query = string.Format(CultureInfo.InvariantCulture, 
                "SELECT Iteration, Timestep, SUM(Amount) AS SumOfAmount FROM stsim__OutputStratum WHERE ({0}) GROUP BY Iteration, Timestep", 
                WhereClause);

			return query;
		}

		private static string RemoveUnwantedColumnReferences(string filter, string variableName)
		{
			if (filter == null)
			{
				return null;
			}

			string[] AndSplit = filter.Split(new[] {" AND "}, StringSplitOptions.None);
			StringBuilder sb = new StringBuilder();

			if (variableName.StartsWith("flow", StringComparison.Ordinal))
			{
				foreach (string s in AndSplit)
				{
				    string sCopy = s;

					if (sCopy.Contains("FromStratumID"))
					{
						sCopy = sCopy.Replace("FromStratumID", "StratumID");
						sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", sCopy);
					}
					else if (sCopy.Contains("FromSecondaryStratumID"))
					{
						sCopy = sCopy.Replace("FromSecondaryStratumID", "SecondaryStratumID");
						sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", sCopy);
					}
					else if (sCopy.Contains("FromTertiaryStratumID"))
					{
						sCopy = sCopy.Replace("FromTertiaryStratumID", "TertiaryStratumID");
						sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", sCopy);
					}
				}
			}
			else
			{
				foreach (string s in AndSplit)
				{
					if (s.Contains("StratumID") || s.Contains("SecondaryStratumID") | s.Contains("TertiaryStratumID"))
					{
						sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", s);
					}
				}
			}

			string final = sb.ToString();

			if (final.Count() > 0)
			{
				Debug.Assert(final.Count() >= 5);
				return final.Substring(0, final.Length - 5);
			}
			else
			{
				return null;
			}
		}
	}
}
