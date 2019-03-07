// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:summary-stock-report", "Stock Type/Group", false));
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:summary-flow-report", "Flow Type/Group", false));
				}

				if (MapsGroup != null)
				{
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:stock-raster-map", "Stock Type/Group", false));
					MapsGroup.Items.Add(new SyncroSimLayoutItem("stsim-stockflow:flow-raster-map", "Flow Type/Group", false));
				}
			}        
		}
	}
}