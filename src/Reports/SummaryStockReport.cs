// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

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
                "stsimsf_OutputStock.ScenarioID, ";
           
            if (!isCSV)
            {
                Query += "core_Scenario.Name AS ScenarioName, ";
            }

            Query += string.Format(CultureInfo.InvariantCulture,
                "stsimsf_OutputStock.Iteration, " +
                "stsimsf_OutputStock.Timestep, " +
                "ST1.Name AS Stratum, " +
                "ST2.Name AS SecondaryStratum, " +
                "ST3.Name AS TertiaryStratum, " +
                "SC1.Name AS StateClass, " +
                "stsimsf_StockGroup.Name as StockGroup, " +
                "stsimsf_OutputStock.Amount " +
                "FROM stsimsf_OutputStock " +
                "INNER JOIN core_Scenario ON core_Scenario.ScenarioID = stsimsf_OutputStock.ScenarioID " +
                "INNER JOIN stsim_Stratum AS ST1 ON ST1.StratumID = stsimsf_OutputStock.StratumID " +
                "LEFT JOIN stsim_SecondaryStratum AS ST2 ON ST2.SecondaryStratumID = stsimsf_OutputStock.SecondaryStratumID " +
                "LEFT JOIN stsim_TertiaryStratum AS ST3 ON ST3.TertiaryStratumID = stsimsf_OutputStock.TertiaryStratumID " +
                "INNER JOIN stsim_StateClass AS SC1 ON SC1.StateClassID = stsimsf_OutputStock.StateClassID " +
                "INNER JOIN stsimsf_StockGroup ON stsimsf_StockGroup.StockGroupID = stsimsf_OutputStock.StockGroupID " +
                "WHERE stsimsf_OutputStock.ScenarioID IN ({0}) " +
                "ORDER BY " +
                "stsimsf_OutputStock.ScenarioID, ", 
                ScenFilter);

            if (!isCSV)
            {
                Query += "core_Scenario.Name, ";
            }

            Query += string.Format(CultureInfo.InvariantCulture, 
                "stsimsf_OutputStock.Iteration, " + 
                "stsimsf_OutputStock.Timestep, " + 
                "ST1.Name, " + 
                "ST2.Name, " + 
                "ST3.Name, " + 
                "SC1.Name, " + 
                "stsimsf_StockGroup.Name", 
                ScenFilter);

            return Query;
		}
	}
}
