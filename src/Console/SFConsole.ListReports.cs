// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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