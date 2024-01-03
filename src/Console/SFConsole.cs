// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Linq;
using System.Reflection;
using SyncroSim.Core;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal partial class SFConsole : SyncroSimConsole
	{
		public SFConsole(Session session, string[] args) : base(session, args)
		{
		}

		protected override void Execute()
		{
			if (this.GetArguments().Count() == 1)
			{
				System.Console.WriteLine("Use the --help switch to see available options.");
				return;
			}

			if (this.IsSwitchArgument("list-reports"))
			{
				this.HandleListReportsArgument();
			}
			else if (this.IsSwitchArgument("create-report"))
			{
				this.HandleCreateReportArgument();
			}
			else
			{
				if (this.Help)
				{
					PrintConsoleHelp();
				}
			}
		}

		private static void PrintConsoleHelp()
		{
			System.Console.WriteLine("Stocks and Flows Console [Arguments]");
			System.Console.WriteLine();
			System.Console.WriteLine("  --list-reports     Lists available Stocks and Flows reports");
			System.Console.WriteLine("  --create-report    Creates a Stocks and Flows report");
			System.Console.WriteLine("  --help             Prints help for an argument");
		}
	}
}