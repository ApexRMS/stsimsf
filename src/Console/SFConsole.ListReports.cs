// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal partial class SFConsole
	{
		private void HandleListReportsArgument()
		{
			if (this.Help)
			{
				System.Console.WriteLine("Lists available Stocks and Flows reports");
			}
			else
			{
				System.Console.WriteLine("Available reports:");
				System.Console.WriteLine();
				System.Console.WriteLine(Constants.STOCK_REPORT_NAME);
				System.Console.WriteLine(Constants.FLOW_REPORT_NAME);
			}
		}
	}
}