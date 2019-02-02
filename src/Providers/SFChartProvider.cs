// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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
		private const string DENSITY_GROUP_NAME = "stockflow_density_group";

		public override DataTable GetData(DataStore store, ChartDescriptor descriptor, DataSheet dataSheet)
		{
			if (descriptor.DatasheetName == "SF_OutputStock" || descriptor.DatasheetName == "SF_OutputFlow")
			{
				string[] v = descriptor.VariableName.Split('-');
				string VarName = v[0];

				if (VarName == "stocktype" || 
                    VarName == "stocktypedensity" || 
                    VarName == "flowtype" || 
                    VarName == "flowtypedensity" || 
                    VarName == "stockgroup" || 
                    VarName == "stockgroupdensity" || 
                    VarName == "flowgroup" || 
                    VarName == "flowgroupdensity")
				{
					return CreateRawChartData(
                        dataSheet, descriptor, store, VarName, 
                        int.Parse(v[1], CultureInfo.InvariantCulture));
				}
			}

			return null;
		}

		public override void RefreshCriteria(SyncroSimLayout layout, Project project)
		{
			//Stock Types
			SyncroSimLayoutItem StockTypesGroup = new SyncroSimLayoutItem("StockTypes", "Stock Types", true);

			StockTypesGroup.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
			StockTypesGroup.Properties.Add(new MetaDataProperty("filter", "StratumID|SecondaryStratumID|TertiaryStratumID|StateClassID"));

			AddStockTypeChartVariables(project, StockTypesGroup.Items);

			if (StockTypesGroup.Items.Count > 0)
			{
				layout.Items.Add(StockTypesGroup);
			}

			//Stock Groups
			SyncroSimLayoutItem StockGroupsGroup = new SyncroSimLayoutItem("StockGroups", "Stock Groups", true);

			StockGroupsGroup.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
			StockGroupsGroup.Properties.Add(new MetaDataProperty("filter", "StratumID|SecondaryStratumID|TertiaryStratumID|StateClassID"));

			AddStockGroupChartVariables(project, StockGroupsGroup.Items);

			if (StockGroupsGroup.Items.Count > 0)
			{
				layout.Items.Add(StockGroupsGroup);
			}

			//Flow Types
			SyncroSimLayoutItem FlowTypesGroup = new SyncroSimLayoutItem("FlowTypes", "Flow Types", true);

			FlowTypesGroup.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
			FlowTypesGroup.Properties.Add(new MetaDataProperty("filter", "FromStratumID|FromSecondaryStratumID|FromTertiaryStratumID|FromStateClassID|FromStockTypeID|TransitionTypeID|ToStratumID|ToStateClassID|ToStockTypeID"));

			AddFlowTypeChartVariables(project, FlowTypesGroup.Items);

			if (FlowTypesGroup.Items.Count > 0)
			{
				layout.Items.Add(FlowTypesGroup);
			}

			//Flow Groups
			SyncroSimLayoutItem FlowGroupsGroup = new SyncroSimLayoutItem("FlowGroups", "Flow Groups", true);

			FlowGroupsGroup.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
			FlowGroupsGroup.Properties.Add(new MetaDataProperty("filter", "FromStratumID|FromSecondaryStratumID|FromTertiaryStratumID|FromStateClassID|FromStockTypeID|TransitionTypeID|ToStratumID|ToStateClassID|ToStockTypeID"));

			AddFlowGroupChartVariables(project, FlowGroupsGroup.Items);

			if (FlowGroupsGroup.Items.Count > 0)
			{
				layout.Items.Add(FlowGroupsGroup);
			}
		}

		private static void AddStockTypeChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_NAME);
			DataTable dt = ds.GetData();
			DataView dv = new DataView(dt, null, ds.ValidationTable.DisplayMember, DataViewRowState.CurrentRows);
			SyncroSimLayoutItem DensityGroup = new SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", true);

			foreach (DataRowView drv in dv)
			{
				int id = Convert.ToInt32(drv.Row[ds.ValidationTable.ValueMember], CultureInfo.InvariantCulture);
				string DisplayName = Convert.ToString(drv.Row[ds.ValidationTable.DisplayMember], CultureInfo.InvariantCulture);

				//Normal
				string VarNameNormal = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stocktype", id);
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem(VarNameNormal, DisplayName, false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemNormal.Properties.Add(new MetaDataProperty("customTitle", "Stock Type " + DisplayName));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemNormal);

				//Density
				string VarNameDensity = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stocktypedensity", id);
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem(VarNameDensity, DisplayName, false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemDensity.Properties.Add(new MetaDataProperty("customTitle", "(Density): Stock Type " + DisplayName));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				DensityGroup.Items.Add(ItemDensity);
			}

			if (DensityGroup.Items.Count > 0)
			{
				items.Add(DensityGroup);
			}
		}

		private static void AddStockGroupChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_STOCK_GROUP_NAME);
			DataTable dt = ds.GetData();
			DataView dv = new DataView(dt, null, ds.ValidationTable.DisplayMember, DataViewRowState.CurrentRows);
			SyncroSimLayoutItem DensityGroup = new SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", true);

			foreach (DataRowView drv in dv)
			{
				int id = Convert.ToInt32(drv.Row[ds.ValidationTable.ValueMember], CultureInfo.InvariantCulture);
				string DisplayName = Convert.ToString(drv.Row[ds.ValidationTable.DisplayMember], CultureInfo.InvariantCulture);

				//Normal
				string VarNameNormal = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stockgroup", id);
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem(VarNameNormal, DisplayName, false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemNormal.Properties.Add(new MetaDataProperty("customTitle", "Stock Group " + DisplayName));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemNormal);

				//Density
				string VarNameDensity = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stockgroupdensity", id);
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem(VarNameDensity, DisplayName, false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputStock"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemDensity.Properties.Add(new MetaDataProperty("customTitle", "(Density): Stock Group " + DisplayName));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				DensityGroup.Items.Add(ItemDensity);
			}

			if (DensityGroup.Items.Count > 0)
			{
				items.Add(DensityGroup);
			}
		}

		private static void AddFlowTypeChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_FLOW_TYPE_NAME);
			DataTable dt = ds.GetData();
			DataView dv = new DataView(dt, null, ds.ValidationTable.DisplayMember, DataViewRowState.CurrentRows);
			SyncroSimLayoutItem DensityGroup = new SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", true);

			foreach (DataRowView drv in dv)
			{
				int id = Convert.ToInt32(drv.Row[ds.ValidationTable.ValueMember], CultureInfo.InvariantCulture);
				string DisplayName = Convert.ToString(drv.Row[ds.ValidationTable.DisplayMember], CultureInfo.InvariantCulture);

				//Normal
				string VarNameNormal = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowtype", id);
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem(VarNameNormal, DisplayName, false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemNormal.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemNormal.Properties.Add(new MetaDataProperty("customTitle", "Flow Type " + DisplayName));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemNormal);

				//Density
				string VarNameDensity = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowtypedensity", id);
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem(VarNameDensity, DisplayName, false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemDensity.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemDensity.Properties.Add(new MetaDataProperty("customTitle", "(Density): Flow Type " + DisplayName));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				DensityGroup.Items.Add(ItemDensity);
			}

			if (DensityGroup.Items.Count > 0)
			{
				items.Add(DensityGroup);
			}
		}

		private static void AddFlowGroupChartVariables(Project project, SyncroSimLayoutItemCollection items)
		{
			DataSheet ds = project.GetDataSheet(Constants.DATASHEET_FLOW_GROUP_NAME);
			DataTable dt = ds.GetData();
			DataView dv = new DataView(dt, null, ds.ValidationTable.DisplayMember, DataViewRowState.CurrentRows);
			SyncroSimLayoutItem DensityGroup = new SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", true);

			foreach (DataRowView drv in dv)
			{
				int id = Convert.ToInt32(drv.Row[ds.ValidationTable.ValueMember], CultureInfo.InvariantCulture);
				string DisplayName = Convert.ToString(drv.Row[ds.ValidationTable.DisplayMember], CultureInfo.InvariantCulture);

				//Normal
				string VarNameNormal = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowgroup", id);
				SyncroSimLayoutItem ItemNormal = new SyncroSimLayoutItem(VarNameNormal, DisplayName, false);

				ItemNormal.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
				ItemNormal.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemNormal.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemNormal.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemNormal.Properties.Add(new MetaDataProperty("customTitle", "Flow Group " + DisplayName));
				ItemNormal.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				items.Add(ItemNormal);

				//Density
				string VarNameDensity = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowgroupdensity", id);
				SyncroSimLayoutItem ItemDensity = new SyncroSimLayoutItem(VarNameDensity, DisplayName, false);

				ItemDensity.Properties.Add(new MetaDataProperty("dataSheet", "SF_OutputFlow"));
				ItemDensity.Properties.Add(new MetaDataProperty("column", "Amount"));
				ItemDensity.Properties.Add(new MetaDataProperty("skipTimestepZero", "True"));
				ItemDensity.Properties.Add(new MetaDataProperty("prefixFolderName", "False"));
				ItemDensity.Properties.Add(new MetaDataProperty("customTitle", "(Density): Flow Group " + DisplayName));
				ItemDensity.Properties.Add(new MetaDataProperty("defaultValue", "0.0"));

				DensityGroup.Items.Add(ItemDensity);
			}

			if (DensityGroup.Items.Count > 0)
			{
				items.Add(DensityGroup);
			}
		}

		private static DataTable CreateRawChartData(
            DataSheet dataSheet, 
            ChartDescriptor descriptor, 
            DataStore store, 
            string variableName, 
            int variableId)
		{
			string query = null;

			if (variableName == "stocktype" || 
                variableName == "stocktypedensity" || 
                variableName == "flowtype" || 
                variableName == "flowtypedensity")
			{
				query = CreateRawChartDataQueryForType(dataSheet, descriptor, variableName, variableId);
			}
			else
			{
				Debug.Assert(
                    variableName == "stockgroup" || 
                    variableName == "stockgroupdensity" || 
                    variableName == "flowgroup" || 
                    variableName == "flowgroupdensity");

				query = CreateRawChartDataQueryForGroup(dataSheet, descriptor, variableName, variableId);
			}

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

		private static string CreateRawChartDataQueryForType(DataSheet dataSheet, ChartDescriptor descriptor, string variableName, int variableId)
		{
			Debug.Assert(
                variableName == "stocktype" || variableName == "stocktypedensity" || 
                variableName == "flowtype" || variableName == "flowtypedensity");

            string TypeColumnName = null;

			if (variableName == "stocktype" || variableName == "stocktypedensity")
			{
				TypeColumnName = Constants.STOCK_TYPE_ID_COLUMN_NAME;
			}
			else
			{
				TypeColumnName = Constants.FLOW_TYPE_ID_COLUMN_NAME;
			}

			string ScenarioClause = string.Format(CultureInfo.InvariantCulture, 
                "([{0}]={1})", 
                Constants.SCENARIO_ID_COLUMN_NAME, dataSheet.Scenario.Id);

			string WhereClause = string.Format(CultureInfo.InvariantCulture,
                "{0} AND ([{1}]={2})", 
                ScenarioClause, TypeColumnName, variableId);

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
                "SELECT Iteration, Timestep, SUM(Amount) AS SumOfAmount FROM {0} WHERE ({1}) GROUP BY Iteration, Timestep", 
                dataSheet.Name, WhereClause);

			return query;
		}

		private static string CreateRawChartDataQueryForGroup(DataSheet dataSheet, ChartDescriptor descriptor, string variableName, int variableId)
		{
			Debug.Assert(variableName == "stockgroup" || variableName == "stockgroupdensity" || 
                variableName == "flowgroup" || variableName == "flowgroupdensity");

			string GroupColumnName = null;
			string JoinColumnName = null;
			string TypeGroupTableName = null;

			if (variableName == "stockgroup" || variableName == "stockgroupdensity")
			{
				GroupColumnName = Constants.STOCK_GROUP_ID_COLUMN_NAME;
				JoinColumnName = Constants.STOCK_TYPE_ID_COLUMN_NAME;
				TypeGroupTableName = Constants.DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME;
			}
			else
			{
				GroupColumnName = Constants.FLOW_GROUP_ID_COLUMN_NAME;
				JoinColumnName = Constants.FLOW_TYPE_ID_COLUMN_NAME;
				TypeGroupTableName = Constants.DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME;
			}

			string ScenarioClause = string.Format(CultureInfo.InvariantCulture, 
                "({0}.{1}={2})", 
                dataSheet.Name, Constants.SCENARIO_ID_COLUMN_NAME, dataSheet.Scenario.Id);

			string WhereClause = string.Format(CultureInfo.InvariantCulture, 
                "{0} AND ({1}.{2}={3})", 
                ScenarioClause, TypeGroupTableName, GroupColumnName, variableId);

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
                "SELECT Iteration, Timestep, Sum({0}.Amount * CASE WHEN {1}.Value IS NULL THEN 1.0 ELSE {2}.Value END) AS SumOfAmount " + "FROM {3} INNER JOIN {4} ON {5}.{6} = {7}.{8} AND {9}.ScenarioID = {10}.ScenarioID " + "WHERE ({11}) GROUP BY Iteration, Timestep", 
                dataSheet.Name, TypeGroupTableName, TypeGroupTableName, dataSheet.Name, TypeGroupTableName, dataSheet.Name, JoinColumnName, TypeGroupTableName, JoinColumnName, dataSheet.Name, TypeGroupTableName, WhereClause);

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
                "SELECT Iteration, Timestep, SUM(Amount) AS SumOfAmount FROM STSim_OutputStratum WHERE ({0}) GROUP BY Iteration, Timestep", 
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
