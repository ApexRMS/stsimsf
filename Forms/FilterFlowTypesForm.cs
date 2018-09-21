//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

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