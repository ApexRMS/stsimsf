// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

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
			DataSheet dstermSTSim = this.Project.GetDataSheet(Constants.DATASHEET_STSIM_TERMINOLOGY);
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
			c.Add(new ExportColumn("FlowGroup", "Flow Group"));
			c.Add(new ExportColumn("EndStratum", "End " + PrimaryStratumLabel));
			c.Add(new ExportColumn("EndSecondaryStratum", "End " + SecondaryStratumLabel));
			c.Add(new ExportColumn("EndTertiaryStratum", "End " + TertiaryStratumLabel));
			c.Add(new ExportColumn("EndStateClass", "End State Class"));
			c.Add(new ExportColumn("EndMinAge", "End Min Age"));

			c.Add(new ExportColumn("Amount", TotalValue));

			c["Amount"].DecimalPlaces = 2;
			c["Amount"].Alignment = ColumnAlignment.Right;

			return c;
		}

		private string CreateReportQuery(bool isCSV)
		{
			string ScenFilter = this.CreateActiveResultScenarioFilter();

            string Query =
                "SELECT " +
                "stsimsf_OutputFlow.ScenarioID, ";

            if (!isCSV)
            {
                Query += "core_Scenario.Name AS ScenarioName, ";
            }

            Query += string.Format(CultureInfo.InvariantCulture,
                "stsimsf_OutputFlow.Iteration,  " +
                "stsimsf_OutputFlow.Timestep,  " +
                "ST1.Name AS FromStratum, " +
                "SS1.Name AS FromSecondaryStratum, " +
                "TS1.Name AS FromTertiaryStratum, " +
                "SC1.Name AS FromStateClass, " +
                "STK1.Name AS FromStock, " +
                "stsim_TransitionType.Name AS TransitionType, " +
                "ST2.Name AS ToStratum, " +
                "SC2.Name AS ToStateClass, " +
                "STK2.Name AS ToStock, " +
                "stsimsf_FlowGroup.Name as FlowGroup, " +
                "ST3.Name AS EndStratum, " +
                "SS2.Name AS EndSecondaryStratum, " +
                "TS2.Name AS EndTertiaryStratum, " +
                "SC3.Name AS EndStateClass, " +
                "stsimsf_OutputFlow.EndMinAge, " +
                "stsimsf_OutputFlow.Amount " +
                "FROM stsimsf_OutputFlow " +
                "INNER JOIN core_Scenario ON core_Scenario.ScenarioID = stsimsf_OutputFlow.ScenarioID " +
                "INNER JOIN stsim_Stratum AS ST1 ON ST1.StratumID = stsimsf_OutputFlow.FromStratumID " +
                "INNER JOIN stsim_Stratum AS ST2 ON ST2.StratumID = stsimsf_OutputFlow.ToStratumID " +
                "LEFT JOIN stsim_Stratum AS ST3 ON ST3.StratumID = stsimsf_OutputFlow.EndStratumID " +
                "LEFT JOIN stsim_SecondaryStratum AS SS1 ON SS1.SecondaryStratumID = stsimsf_OutputFlow.FromSecondaryStratumID " +
                "LEFT JOIN stsim_SecondaryStratum AS SS2 ON SS2.SecondaryStratumID = stsimsf_OutputFlow.EndSecondaryStratumID " +
                "LEFT JOIN stsim_TertiaryStratum AS TS1 ON TS1.TertiaryStratumID = stsimsf_OutputFlow.FromTertiaryStratumID " +
                "LEFT JOIN stsim_TertiaryStratum AS TS2 ON TS2.TertiaryStratumID = stsimsf_OutputFlow.EndTertiaryStratumID " +
                "INNER JOIN stsim_StateClass AS SC1 ON SC1.StateClassID = stsimsf_OutputFlow.FromStateClassID " +
                "INNER JOIN stsim_StateClass AS SC2 ON SC2.StateClassID = stsimsf_OutputFlow.ToStateClassID " +
                "LEFT JOIN stsim_StateClass AS SC3 ON SC3.StateClassID = stsimsf_OutputFlow.EndStateClassID " +
                "INNER JOIN stsimsf_StockType AS STK1 ON STK1.StockTypeID = stsimsf_OutputFlow.FromStockTypeID " +
                "INNER JOIN stsimsf_StockType AS STK2 ON STK2.StockTypeID = stsimsf_OutputFlow.ToStockTypeID " +
                "INNER JOIN stsimsf_FlowGroup ON stsimsf_FlowGroup.FlowGroupID = stsimsf_OutputFlow.FlowGroupID " +
                "LEFT JOIN stsim_TransitionType ON stsim_TransitionType.TransitionTypeID = stsimsf_OutputFlow.TransitionTypeID " +
                "WHERE stsimsf_OutputFlow.ScenarioID IN ({0})  " +
                "ORDER BY " +
                "stsimsf_OutputFlow.ScenarioID, ", ScenFilter);

            if (!isCSV)
            {
                Query += "core_Scenario.Name, ";
            }
             
            Query +=       
                "stsimsf_OutputFlow.Iteration, " + 
                "stsimsf_OutputFlow.Timestep, " + 
                "ST1.Name, " + 
                "SS1.Name, " + 
                "TS1.Name, " + 
                "SC1.Name, " + 
                "STK1.Name, " + 
                "stsim_TransitionType.Name, " + 
                "ST2.Name, " + 
                "SC2.Name, " +
                "STK2.Name, " + 
                "stsimsf_FlowGroup.Name, " + 
                "ST3.Name, " +
                "SS2.Name, " +
                "TS2.Name, " +
                "SC3.Name, " +
                "stsimsf_OutputFlow.EndMinAge";

            return Query;
		}
	}
}
