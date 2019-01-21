// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Linq;
using SyncroSim.Core;
using SyncroSim.Core.Forms;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
    internal partial class SFConsole
    {
        private void HandleCreateReportArgument()
        {
            if (this.Help)
            {
                PrintCreateReportHelp();
            }
            else
            {
                this.CreateReport();
            }
        }

        private void CreateReport()
        {
            string n = this.GetReportName();
            string f = this.GetOutputFileName();
            Library l = this.OpenLibrary();
            IEnumerable<int> sids = this.GetMultiDatabaseIdArguments("sids");

            ValidateScenarios(sids, l);

            Project p = this.ConfigureActiveProject(sids, l);

            if (n == Constants.STOCK_REPORT_NAME)
            {
                SummaryStockReport t = (SummaryStockReport)this.Session.CreateTransformer("stsim-stockflow:summary-stock-report", p.Library, p, null);
                t.InternalExport(f, ExportType.CSVFile, false);
            }
            else if (n == Constants.FLOW_REPORT_NAME)
            {
                SummaryFlowReport t = (SummaryFlowReport)this.Session.CreateTransformer("stsim-stockflow:summary-flow-report", p.Library, p, null);
                t.InternalExport(f, ExportType.CSVFile, false);
            }
        }

        private string GetReportName()
        {
            string n = this.GetRequiredArgument("name");

            if (n != Constants.STOCK_REPORT_NAME && n != Constants.FLOW_REPORT_NAME)
            {
                ExceptionUtils.ThrowArgumentException("The report name is not valid.");
            }

            return n;
        }

        private static void ValidateScenarios(IEnumerable<int> sids, Library l)
        {
            Dictionary<int, bool> pids = new Dictionary<int, bool>();

            foreach (int id in sids)
            {
                if (!l.Scenarios.Contains(id))
                {
                    ExceptionUtils.ThrowArgumentException("The scenario does not exist: {0}", id);
                }

                Scenario s = l.Scenarios[id];

                if (!s.IsResult)
                {
                    ExceptionUtils.ThrowArgumentException("The scenario is not a result scenario: {0}", id);
                }

                if (!pids.ContainsKey(s.Project.Id))
                {
                    pids.Add(s.Project.Id, true);
                }

                if (pids.Count > 1)
                {
                    ExceptionUtils.ThrowArgumentException("The scenarios must belong to the same project: {0}", id);
                }
            }
        }

        private Project ConfigureActiveProject(IEnumerable<int> sids, Library l)
        {
            Project p = l.Scenarios[sids.First()].Project;
            this.Session.SetActiveProject(p);

            foreach (int id in sids)
            {
                Scenario s = l.Scenarios[id];
                s.IsActive = true;
                p.Results.Add(s);
            }

            return p;
        }

        private static void PrintCreateReportHelp()
        {       
            System.Console.WriteLine("Creates a Stocks and Flows Report");
            System.Console.WriteLine("USAGE: --create-report [Arguments]");
            System.Console.WriteLine();
            System.Console.WriteLine("  --lib={name}           The library file name");
            System.Console.WriteLine("  --sids={ids}           The scenario IDs separated by commas");
            System.Console.WriteLine("  --name={name}          The name of the report to create");
            System.Console.WriteLine("  --file={name}          The file name for the report");
            System.Console.WriteLine();
            System.Console.WriteLine("Examples:");
            System.Console.WriteLine("  --create-report --lib=test.ssim --sids=\"1,2,3\" --name=stocks --file=stocks.csv");
            System.Console.WriteLine("  --create-report --lib=\"my lib.ssim\" --sids=1 --name=flows --file=\"my data.csv\"");
        }
    }
}