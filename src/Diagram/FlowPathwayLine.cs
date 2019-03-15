// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Drawing;
using SyncroSim.Common.Forms;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowPathwayLine : ConnectorLine
	{
		private FlowPathway m_Pathway;

		public FlowPathwayLine(Color lineColor, FlowPathway pathway) : base(lineColor)
		{
			this.m_Pathway = pathway;
		}

		public FlowPathway Pathway
		{
			get
			{
				return this.m_Pathway;
			}
		}
	}
}