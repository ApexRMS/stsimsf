// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using SyncroSim.Core;
using System.Globalization;
using System.Reflection;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal partial class OutputOptionsDataFeedView
	{
		public OutputOptionsDataFeedView()
		{
			InitializeComponent();
		}

		private const string DEFAULT_TIMESTEP_VALUE = "1";

		public override void LoadDataFeed(DataFeed dataFeed)
		{
			base.LoadDataFeed(dataFeed);

			this.SetCheckBoxBinding(this.CheckBoxSummaryST, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME);
			this.SetTextBoxBinding(this.TextBoxSummarySTTimesteps, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME);
			this.SetCheckBoxBinding(this.CheckBoxSummaryFL, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME);
			this.SetTextBoxBinding(this.TextBoxSummaryFLTimesteps, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME);
			this.SetCheckBoxBinding(this.CheckBoxSpatialST, Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME);
			this.SetTextBoxBinding(this.TextBoxSpatialSTTimesteps, Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME);
			this.SetCheckBoxBinding(this.CheckBoxSpatialFL, Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME);
			this.SetTextBoxBinding(this.TextBoxSpatialFLTimesteps, Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME);

			this.MonitorDataSheet("STSim_Terminology", this.OnTerminologyChanged, true);
			this.AddStandardCommands();
		}

		public override void EnableView(bool enable)
		{
			base.EnableView(enable);

			if (enable)
			{
				this.EnableControls();
			}
		}

		protected override void OnRowsAdded(object sender, DataSheetRowEventArgs e)
		{
			base.OnRowsAdded(sender, e);

			if (this.ShouldEnableView())
			{
				this.EnableControls();
			}
		}

		protected override void OnRowsDeleted(object sender, DataSheetRowEventArgs e)
		{
			base.OnRowsDeleted(sender, e);

			if (this.ShouldEnableView())
			{
				this.EnableControls();
			}
		}

		private void OnTerminologyChanged(DataSheetMonitorEventArgs e)
		{
			string t = Convert.ToString(
                e.GetValue("TimestepUnits", "timestep"), CultureInfo.InvariantCulture).
                ToLower(CultureInfo.InvariantCulture);

			this.LabelSummarySTTimesteps.Text = t;
			this.LabelSummaryFLTimesteps.Text = t;
			this.LabelSpatialSTTimesteps.Text = t;
			this.LabelSpatialFLTimesteps.Text = t;
		}

		protected override void OnBoundCheckBoxChanged(System.Windows.Forms.CheckBox checkBox, string columnName)
		{
			base.OnBoundCheckBoxChanged(checkBox, columnName);

			if (checkBox == this.CheckBoxSummaryST && this.CheckBoxSummaryST.Checked & string.IsNullOrEmpty(this.TextBoxSummarySTTimesteps.Text))
			{
				this.SetTextBoxData(this.TextBoxSummarySTTimesteps, DEFAULT_TIMESTEP_VALUE);
			}
			else if (checkBox == this.CheckBoxSummaryFL && this.CheckBoxSummaryFL.Checked & string.IsNullOrEmpty(this.TextBoxSummaryFLTimesteps.Text))
			{
				this.SetTextBoxData(this.TextBoxSummaryFLTimesteps, DEFAULT_TIMESTEP_VALUE);
			}
			else if (checkBox == this.CheckBoxSpatialST && this.CheckBoxSpatialST.Checked & string.IsNullOrEmpty(this.TextBoxSpatialSTTimesteps.Text))
			{
				this.SetTextBoxData(this.TextBoxSpatialSTTimesteps, DEFAULT_TIMESTEP_VALUE);
			}
			else if (checkBox == this.CheckBoxSpatialFL && this.CheckBoxSpatialFL.Checked & string.IsNullOrEmpty(this.TextBoxSpatialFLTimesteps.Text))
			{
				this.SetTextBoxData(this.TextBoxSpatialFLTimesteps, DEFAULT_TIMESTEP_VALUE);
			}

			this.EnableControls();
		}

		private void EnableControls()
		{
			//Text Boxes
			this.TextBoxSummarySTTimesteps.Enabled = this.CheckBoxSummaryST.Checked;
			this.TextBoxSummaryFLTimesteps.Enabled = this.CheckBoxSummaryFL.Checked;
			this.TextBoxSpatialSTTimesteps.Enabled = this.CheckBoxSpatialST.Checked;
			this.TextBoxSpatialFLTimesteps.Enabled = this.CheckBoxSpatialFL.Checked;

			//Timesteps labels
			this.LabelSummarySTTimesteps.Enabled = this.CheckBoxSummaryST.Checked;
			this.LabelSummaryFLTimesteps.Enabled = this.CheckBoxSummaryFL.Checked;
			this.LabelSpatialSTTimesteps.Enabled = this.CheckBoxSpatialST.Checked;
			this.LabelSpatialFLTimesteps.Enabled = this.CheckBoxSpatialFL.Checked;
		}
	}
}