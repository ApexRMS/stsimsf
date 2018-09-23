// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Diagnostics;
using System.Windows.Forms;
using SyncroSim.Common.Forms;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowPathwayTabStripItem : TabStripItem
	{
		private Control m_Control;

		public FlowPathwayTabStripItem(string text) : base(text)
		{
		}

		public Control Control
		{
			get
			{
				return this.m_Control;
			}
			set
			{
				Debug.Assert(this.m_Control == null);
				this.m_Control = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.m_Control != null)
			{
				this.m_Control.Dispose();
				this.m_Control = null;
			}

			base.Dispose(disposing);
		}
	}
}