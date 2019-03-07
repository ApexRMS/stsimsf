// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Reflection;
using System.Globalization;
using SyncroSim.Core;
using SyncroSim.Core.Forms;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class SummaryFlowReport : ExportTransformer
	{
		protected override void Export(string location, ExportType exportType)
		{
			this.InternalExport(location, exportType, true);
		}

		internal void InternalExport(string location, ExportType exportType, bool showMessage)
		{
			ExportColumnCollection columns = this.CreateColumnCollection();

			if (exportType == ExportType.ExcelFile)
			{
				this.ExcelExport(location, columns, this.CreateReportQuery(false), "Flows");
			}
			else
			{
				columns.Remove("ScenarioName");
				this.CSVExport(location, columns, this.CreateReportQuery(true));

				if (showMessage)
				{
					FormsUtilities.InformationMessageBox("Data saved to '{0}'.", location);
				}
			}
		}

		private ExportColumnCollection CreateColumnCollection()
		{
			ExportColumnCollection c = new ExportColumnCollection();
			DataSheet dsterm = this.Project.GetDataSheet(Constants.DATASHEET_TERMINOLOGY_NAME);
			DataSheet dstermSTSim = this.Project.GetDataSheet("STSim_Terminology");
			string FlowUnits = TerminologyUtilities.GetTerminology(dsterm, Constants.STOCK_UNITS_COLUMN_NAME);
			string TimestepLabel = TerminologyUtilities.GetTimestepUnits(this.Project);
			string PrimaryStratumLabel = null;
			string SecondaryStratumLabel = null;
			string TertiaryStratumLabel = null;

			TerminologyUtilities.GetStratumLabelStrings(dstermSTSim, ref PrimaryStratumLabel, ref SecondaryStratumLabel, ref TertiaryStratumLabel);
			string TotalValue = string.Format(CultureInfo.InvariantCulture, "Total Value ({0})", FlowUnits);

			c.Add(new ExportColumn("ScenarioID", "Scenario ID"));
			c.Add(new ExportColumn("ScenarioName", "Scenario"));
			c.Add(new ExportColumn("Iteration", "Iteration"));
			c.Add(new ExportColumn("Timestep", TimestepLabel));
			c.Add(new ExportColumn("FromStratum", "From " + PrimaryStratumLabel));
			c.Add(new ExportColumn("FromSecondaryStratum", "From " + SecondaryStratumLabel));
			c.Add(new ExportColumn("FromTertiaryStratum", "From " + TertiaryStratumLabel));
			c.Add(new ExportColumn("FromStateClass", "From State Class"));
			c.Add(new ExportColumn("FromStock", "From Stock"));
			c.Add(new ExportColumn("TransitionType", "TransitionType"));
			c.Add(new ExportColumn("ToStratum", "To " + PrimaryStratumLabel));
			c.Add(new ExportColumn("ToStateClass", "To State Class"));
			c.Add(new ExportColumn("ToStock", "To Stock"));
			c.Add(new ExportColumn("FlowGroup", "Flow Type/Group"));
			c.Add(new ExportColumn("Amount", TotalValue));

			c["Amount"].DecimalPlaces = 2;
			c["Amount"].Alignment = Core.ColumnAlignment.Right;

			return c;
		}

		private string CreateReportQuery(bool isCSV)
		{
			string ScenFilter = this.CreateActiveResultScenarioFilter();

			if (isCSV)
			{
				return string.Format(CultureInfo.InvariantCulture, 
                    "SELECT " + "SF_OutputFlow.ScenarioID, " + 
                    "SF_OutputFlow.Iteration,  " + 
                    "SF_OutputFlow.Timestep,  " + 
                    "ST1.Name AS FromStratum, " + 
                    "SS1.Name AS FromSecondaryStratum, " + 
                    "TS1.Name AS FromTertiaryStratum, " + 
                    "SC1.Name AS FromStateClass, " + 
                    "STK1.Name AS FromStock, " + 
                    "STSim_TransitionType.Name AS TransitionType, " + 
                    "ST2.Name AS ToStratum, " + 
                    "SC2.Name AS ToStateClass, " + 
                    "STK2.Name AS ToStock, " + 
                    "SF_FlowGroup.Name as FlowGroup, " + 
                    "SF_OutputFlow.Amount " + 
                    "FROM SF_OutputFlow " + 
                    "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputFlow.FromStratumID " + 
                    "INNER JOIN STSim_Stratum AS ST2 ON ST2.StratumID = SF_OutputFlow.ToStratumID " + 
                    "LEFT JOIN STSim_SecondaryStratum AS SS1 ON SS1.SecondaryStratumID = SF_OutputFlow.FromSecondaryStratumID " + 
                    "LEFT JOIN STSim_TertiaryStratum AS TS1 ON TS1.TertiaryStratumID = SF_OutputFlow.FromTertiaryStratumID " + 
                    "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputFlow.FromStateClassID " + 
                    "INNER JOIN STSim_StateClass AS SC2 ON SC2.StateClassID = SF_OutputFlow.ToStateClassID " + 
                    "INNER JOIN SF_StockType AS STK1 ON STK1.StockTypeID = SF_OutputFlow.FromStockTypeID " + 
                    "INNER JOIN SF_StockType AS STK2 ON STK2.StockTypeID = SF_OutputFlow.ToStockTypeID " + 
                    "INNER JOIN SF_FlowGroup ON SF_FlowGroup.FlowGroupID = SF_OutputFlow.FlowGroupID " + 
                    "LEFT JOIN STSim_TransitionType ON STSim_TransitionType.TransitionTypeID = SF_OutputFlow.TransitionTypeID " + 
                    "WHERE SF_OutputFlow.ScenarioID IN ({0})  " + 
                    "ORDER BY " + 
                    "SF_OutputFlow.ScenarioID, " + 
                    "SF_OutputFlow.Iteration, " + 
                    "SF_OutputFlow.Timestep, " + 
                    "ST1.Name, " + "SS1.Name, " + "TS1.Name, " + "SC1.Name, " + "STK1.Name, " + 
                    "STSim_TransitionType.Name, " + "ST2.Name, " + "SC2.Name, " + "STK2.Name, " + "SF_FlowGroup.Name", 
                    ScenFilter);
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture, 
                    "SELECT " + "SF_OutputFlow.ScenarioID, " + "SSim_Scenario.Name AS ScenarioName, " + 
                    "SF_OutputFlow.Iteration,  " + 
                    "SF_OutputFlow.Timestep,  " + 
                    "ST1.Name AS FromStratum, " + 
                    "SS1.Name AS FromSecondaryStratum, " + 
                    "TS1.Name AS FromTertiaryStratum, " + 
                    "SC1.Name AS FromStateClass, " + 
                    "STK1.Name AS FromStock, " + 
                    "STSim_TransitionType.Name AS TransitionType, " + 
                    "ST2.Name AS ToStratum, " + 
                    "SC2.Name AS ToStateClass, " + "STK2.Name AS ToStock, " + 
                    "SF_FlowGroup.Name as FlowGroup, " +
                    "SF_OutputFlow.Amount " + 
                    "FROM SF_OutputFlow " + 
                    "INNER JOIN SSim_Scenario ON SSim_Scenario.ScenarioID = SF_OutputFlow.ScenarioID " + 
                    "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputFlow.FromStratumID " + 
                    "INNER JOIN STSim_Stratum AS ST2 ON ST2.StratumID = SF_OutputFlow.ToStratumID " + 
                    "LEFT JOIN STSim_SecondaryStratum AS SS1 ON SS1.SecondaryStratumID = SF_OutputFlow.FromSecondaryStratumID " + 
                    "LEFT JOIN STSim_TertiaryStratum AS TS1 ON TS1.TertiaryStratumID = SF_OutputFlow.FromTertiaryStratumID " + 
                    "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputFlow.FromStateClassID " + 
                    "INNER JOIN STSim_StateClass AS SC2 ON SC2.StateClassID = SF_OutputFlow.ToStateClassID " + 
                    "INNER JOIN SF_StockType AS STK1 ON STK1.StockTypeID = SF_OutputFlow.FromStockTypeID " + 
                    "INNER JOIN SF_StockType AS STK2 ON STK2.StockTypeID = SF_OutputFlow.ToStockTypeID " + 
                    "INNER JOIN SF_FlowGroup ON SF_FlowGroup.FlowGroupID = SF_OutputFlow.FlowGroupID " + 
                    "LEFT JOIN STSim_TransitionType ON STSim_TransitionType.TransitionTypeID = SF_OutputFlow.TransitionTypeID " + 
                    "WHERE SF_OutputFlow.ScenarioID IN ({0})  " + 
                    "ORDER BY " + 
                    "SF_OutputFlow.ScenarioID, " + 
                    "SSim_Scenario.Name, " + 
                    "SF_OutputFlow.Iteration, " + 
                    "SF_OutputFlow.Timestep, " + 
                    "ST1.Name, " + "SS1.Name, " + "TS1.Name, " + "SC1.Name, " + "STK1.Name, " + "STSim_TransitionType.Name, " + 
                    "ST2.Name, " + "SC2.Name, " + "STK2.Name, " + "SF_FlowGroup.Name", 
                    ScenFilter);
			}
		}
	}
}