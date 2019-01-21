// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
using SyncroSim.Core.Forms;
using System.Reflection;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class ResultLayoutProvider : LayoutProvider
	{
		protected override void ModifyLayout(SyncroSimLayout layout)
		{
			SyncroSimLayoutItem ExportGroup = layout.Items.GetItem("Export");

			if (ExportGroup != null)
			{
				SyncroSimLayoutItem ReportsGroup = ExportGroup.Items.GetItem("Reports");
				SyncroSimLayoutItem MapsGroup = ExportGroup.Items.GetItem("Maps");

				if (ReportsGroup != null)
				{
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:summary-stock-report", "Stocks", false));
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:summary-flow-report", "Flows", false));
				}

				if (MapsGroup != null)
				{
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:stock-raster-map", "Stock Types", false));
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:flow-raster-map", "Flow Types", false));
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:stock-group-raster-map", "Stock Groups", false));
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:flow-group-raster-map", "Flow Groups", false));
				}
			}        
		}
	}
}