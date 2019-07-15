// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Reflection;
using System.Globalization;
using SyncroSim.Core;
using SyncroSim.Core.Forms;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class SummaryStockReport : ExportTransformer
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
				this.ExcelExport(location, columns, this.CreateReportQuery(false), "Stocks");
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
			string StockUnits = TerminologyUtilities.GetTerminology(dsterm, Constants.STOCK_UNITS_COLUMN_NAME);
			string TimestepLabel = TerminologyUtilities.GetTimestepUnits(this.Project);
			string PrimaryStratumLabel = null;
			string SecondaryStratumLabel = null;
			string TertiaryStratumLabel = null;

			TerminologyUtilities.GetStratumLabelStrings(dstermSTSim, ref PrimaryStratumLabel, ref SecondaryStratumLabel, ref TertiaryStratumLabel);
			string TotalValue = string.Format(CultureInfo.InvariantCulture, "Total Value ({0})", StockUnits);

			c.Add(new ExportColumn("ScenarioID", "Scenario ID"));
			c.Add(new ExportColumn("ScenarioName", "Scenario"));
			c.Add(new ExportColumn("Iteration", "Iteration"));
			c.Add(new ExportColumn("Timestep", TimestepLabel));
			c.Add(new ExportColumn("Stratum", PrimaryStratumLabel));
			c.Add(new ExportColumn("SecondaryStratum", SecondaryStratumLabel));
			c.Add(new ExportColumn("TertiaryStratum", TertiaryStratumLabel));
			c.Add(new ExportColumn("StateClass", "State Class"));
			c.Add(new ExportColumn("StockGroup", "Stock Group"));
			c.Add(new ExportColumn("Amount", TotalValue));

			c["Amount"].DecimalPlaces = 2;
			c["Amount"].Alignment = Core.ColumnAlignment.Right;

			return c;
		}

		private string CreateReportQuery(bool isCSV)
		{
			string ScenFilter = this.CreateActiveResultScenarioFilter();

            string Query =
                "SELECT " +
                "stsim_stockflow__OutputStock.ScenarioID, ";
           
            if (!isCSV)
            {
                Query += "system__Scenario.Name AS ScenarioName, ";
            }

            Query += string.Format(CultureInfo.InvariantCulture,
                "stsim_stockflow__OutputStock.Iteration, " +
                "stsim_stockflow__OutputStock.Timestep, " +
                "ST1.Name AS Stratum, " +
                "ST2.Name AS SecondaryStratum, " +
                "ST3.Name AS TertiaryStratum, " +
                "SC1.Name AS StateClass, " +
                "stsim_stockflow__StockGroup.Name as StockGroup, " +
                "stsim_stockflow__OutputStock.Amount " +
                "FROM stsim_stockflow__OutputStock " +
                "INNER JOIN system__Scenario ON system__Scenario.ScenarioID = stsim_stockflow__OutputStock.ScenarioID " +
                "INNER JOIN stsim__Stratum AS ST1 ON ST1.StratumID = stsim_stockflow__OutputStock.StratumID " +
                "LEFT JOIN stsim__SecondaryStratum AS ST2 ON ST2.SecondaryStratumID = stsim_stockflow__OutputStock.SecondaryStratumID " +
                "LEFT JOIN stsim__TertiaryStratum AS ST3 ON ST3.TertiaryStratumID = stsim_stockflow__OutputStock.TertiaryStratumID " +
                "INNER JOIN stsim__StateClass AS SC1 ON SC1.StateClassID = stsim_stockflow__OutputStock.StateClassID " +
                "INNER JOIN stsim_stockflow__StockGroup ON stsim_stockflow__StockGroup.StockGroupID = stsim_stockflow__OutputStock.StockGroupID " +
                "WHERE stsim_stockflow__OutputStock.ScenarioID IN ({0}) " +
                "ORDER BY " +
                "stsim_stockflow__OutputStock.ScenarioID, ", 
                ScenFilter);

            if (!isCSV)
            {
                Query += "system__Scenario.Name, ";
            }

            Query += string.Format(CultureInfo.InvariantCulture, 
                "stsim_stockflow__OutputStock.Iteration, " + 
                "stsim_stockflow__OutputStock.Timestep, " + 
                "ST1.Name, " + 
                "ST2.Name, " + 
                "ST3.Name, " + 
                "SC1.Name, " + 
                "stsim_stockflow__StockGroup.Name", 
                ScenFilter);

            return Query;
		}
	}
}
