// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

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
			SyncroSimLayoutItem ExportGroup = layout.Items.GetItem("stsim_Export");

			if (ExportGroup != null)
			{
				SyncroSimLayoutItem ReportsGroup = ExportGroup.Items.GetItem("stsim_Reports");
				SyncroSimLayoutItem MapsGroup = ExportGroup.Items.GetItem("stsim_Maps");

				if (ReportsGroup != null)
				{
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_SummaryStockReport", "Stock Type/Group", false));
					ReportsGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_SummaryFlowReport", "Flow Type/Group", false));
				}

				if (MapsGroup != null)
				{
                    SyncroSimLayoutItem SFGroup = 
                        new SyncroSimLayoutItem("stsimsf_StockFlowMaps", "Stocks and Flows", true);

                    SFGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_StockRasterMap", "Stock Type/Group", false));
                    SFGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_FlowRasterMap", "Flow Type/Group", false));
                    SFGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_LateralFlowRasterMap", "Lateral Flow Type/Group", false));
                    SFGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_AvgStockRasterMap", "Average Stock Type/Group", false));
                    SFGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_AvgFlowRasterMap", "Average Flow Type/Group", false));

                    MapsGroup.Items.Add(SFGroup);
				}
			}        
		}
	}
}