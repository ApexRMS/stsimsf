// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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