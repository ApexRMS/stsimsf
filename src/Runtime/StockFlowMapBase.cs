// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using SyncroSim.Core;
using System.Globalization;

namespace SyncroSim.STSimStockFlow
{
	internal abstract class StockFlowMapBase
	{
		private Scenario m_Scenario;
		private string m_PrimaryStratumLabel;
		private string m_SecondaryStratumLabel;
		private string m_TertiaryStratumLabel;
		private bool m_HasItems;

		protected StockFlowMapBase(Scenario scenario)
		{
            this.m_Scenario = scenario;

			TerminologyUtilities.GetStratumLabelStrings(
                scenario.Project.GetDataSheet("STSim_Terminology"), 
                ref this.m_PrimaryStratumLabel, 
                ref this.m_SecondaryStratumLabel, 
                ref this.m_TertiaryStratumLabel);
		}

		protected string PrimaryStratumLabel
		{
			get
			{
				return this.m_PrimaryStratumLabel;
			}
		}

		protected string SecondaryStratumLabel
		{
			get
			{
				return this.m_SecondaryStratumLabel;
			}
		}

		protected string TertiaryStratumLabel
		{
			get
			{
				return this.m_TertiaryStratumLabel;
			}
		}

		protected void SetHasItems()
		{
			this.m_HasItems = true;
		}

		public bool HasItems
		{
			get
			{
				return this.m_HasItems;
			}
		}

		protected static void ThrowDuplicateItemException()
		{
			throw new StockFlowMapDuplicateItemException("An item with the same keys has already been added.");
		}

		protected static string FormatValue(int? value)
		{
			if (!value.HasValue)
			{
				return "NULL";
			}
			else
			{
				return Convert.ToString(value, CultureInfo.InvariantCulture);
			}
		}

		protected string GetStratumName(int? id)
		{
			return this.GetProjectItemName("STSim_Stratum", id);
		}

		protected string GetSecondaryStratumName(int? id)
		{
			return this.GetProjectItemName("STSim_SecondaryStratum", id);
		}

		protected string GetTertiaryStratumName(int? id)
		{
			return this.GetProjectItemName("STSim_TertiaryStratum", id);
		}

		protected string GetStateClassName(int? id)
		{
			return this.GetProjectItemName("STSim_StateClass", id);
		}

		protected string GetStockTypeName(int? id)
		{
			return this.GetProjectItemName(Constants.DATASHEET_STOCK_TYPE_NAME, id);
		}

		protected string GetFlowGroupName(int? id)
		{
			return this.GetProjectItemName(Constants.DATASHEET_FLOW_GROUP_NAME, id);
		}

		protected string GetProjectItemName(string dataSheetName, int? id)
		{
			if (!id.HasValue)
			{
				return "NULL";
			}
			else
			{
				DataSheet ds = this.m_Scenario.Project.GetDataSheet(dataSheetName);
				return ds.ValidationTable.GetDisplayName(id.Value);
			}
		}
	}
}