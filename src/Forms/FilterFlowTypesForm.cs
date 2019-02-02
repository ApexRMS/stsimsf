// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;

namespace SyncroSim.STSimStockFlow
{
	internal partial class FilterFlowTypesForm
	{
		public FilterFlowTypesForm()
		{
			InitializeComponent();
		}

		private void ButtonOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Validate();
			this.Close();
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Validate();
			this.Close();
		}
	}
}