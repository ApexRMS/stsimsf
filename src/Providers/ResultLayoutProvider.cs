// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
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
                    SyncroSimLayoutItem StkGroup = new SyncroSimLayoutItem("stsimsf_StockGroups", "Stocks Groups", true);
                    StkGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_StockRasterMap", "Iteration", false));
                    StkGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_AvgStockRasterMap", "Average", false));
                    MapsGroup.Items.Add(StkGroup);

                    SyncroSimLayoutItem FloGroup = new SyncroSimLayoutItem("stsimsf_FlowGroups", "Flow Groups", true);
                    FloGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_FlowRasterMap", "Iteration", false));
                    FloGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_LateralFlowRasterMap", "Iteration - Lateral", false));
                    FloGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_AvgFlowRasterMap", "Average", false));
                    FloGroup.Items.Add(new SyncroSimLayoutItem("stsimsf_AvgLateralFlowRasterMap", "Average - Lateral", false));

                    MapsGroup.Items.Add(FloGroup);
				}
			}        
		}
	}
}
