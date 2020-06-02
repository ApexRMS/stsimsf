namespace SyncroSim.STSimStockFlow
{
	internal partial class OutputOptionsDataFeedView : SyncroSim.Core.Forms.DataFeedView
	{
		//UserControl overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components = null;

		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.LabelSpatialFLTimesteps = new System.Windows.Forms.Label();
            this.LabelSpatialSTTimesteps = new System.Windows.Forms.Label();
            this.TextBoxSpatialFLTimesteps = new System.Windows.Forms.TextBox();
            this.TextBoxSpatialSTTimesteps = new System.Windows.Forms.TextBox();
            this.CheckBoxSpatialFL = new System.Windows.Forms.CheckBox();
            this.CheckBoxSpatialST = new System.Windows.Forms.CheckBox();
            this.GroupBoxSpatial = new System.Windows.Forms.GroupBox();
            this.LabelLateralFLTimesteps = new System.Windows.Forms.Label();
            this.TextBoxLateralFLTimesteps = new System.Windows.Forms.TextBox();
            this.CheckBoxLateralFL = new System.Windows.Forms.CheckBox();
            this.LabelSummaryFLTimesteps = new System.Windows.Forms.Label();
            this.LabelSummarySTTimesteps = new System.Windows.Forms.Label();
            this.TextBoxSummaryFLTimesteps = new System.Windows.Forms.TextBox();
            this.GroupBoxSummary = new System.Windows.Forms.GroupBox();
            this.TextBoxSummarySTTimesteps = new System.Windows.Forms.TextBox();
            this.CheckBoxSummaryFL = new System.Windows.Forms.CheckBox();
            this.CheckBoxSummaryST = new System.Windows.Forms.CheckBox();
            this.GroupBoxAvgSpatial = new System.Windows.Forms.GroupBox();
            this.LabelAvgSpatialFLTimesteps = new System.Windows.Forms.Label();
            this.LabelAvgSpatialSTTimesteps = new System.Windows.Forms.Label();
            this.TextBoxAvgSpatialFLTimesteps = new System.Windows.Forms.TextBox();
            this.TextBoxAvgSpatialSTTimesteps = new System.Windows.Forms.TextBox();
            this.CheckBoxAvgSpatialFL = new System.Windows.Forms.CheckBox();
            this.CheckBoxAvgSpatialST = new System.Windows.Forms.CheckBox();
            this.GroupBoxSpatial.SuspendLayout();
            this.GroupBoxSummary.SuspendLayout();
            this.GroupBoxAvgSpatial.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelSpatialFLTimesteps
            // 
            this.LabelSpatialFLTimesteps.AutoSize = true;
            this.LabelSpatialFLTimesteps.Location = new System.Drawing.Point(275, 57);
            this.LabelSpatialFLTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelSpatialFLTimesteps.Name = "LabelSpatialFLTimesteps";
            this.LabelSpatialFLTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelSpatialFLTimesteps.TabIndex = 5;
            this.LabelSpatialFLTimesteps.Text = "timesteps";
            // 
            // LabelSpatialSTTimesteps
            // 
            this.LabelSpatialSTTimesteps.AutoSize = true;
            this.LabelSpatialSTTimesteps.Location = new System.Drawing.Point(275, 31);
            this.LabelSpatialSTTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelSpatialSTTimesteps.Name = "LabelSpatialSTTimesteps";
            this.LabelSpatialSTTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelSpatialSTTimesteps.TabIndex = 2;
            this.LabelSpatialSTTimesteps.Text = "timesteps";
            // 
            // TextBoxSpatialFLTimesteps
            // 
            this.TextBoxSpatialFLTimesteps.Location = new System.Drawing.Point(192, 54);
            this.TextBoxSpatialFLTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSpatialFLTimesteps.Name = "TextBoxSpatialFLTimesteps";
            this.TextBoxSpatialFLTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxSpatialFLTimesteps.TabIndex = 3;
            this.TextBoxSpatialFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TextBoxSpatialSTTimesteps
            // 
            this.TextBoxSpatialSTTimesteps.Location = new System.Drawing.Point(192, 26);
            this.TextBoxSpatialSTTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSpatialSTTimesteps.Name = "TextBoxSpatialSTTimesteps";
            this.TextBoxSpatialSTTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxSpatialSTTimesteps.TabIndex = 1;
            this.TextBoxSpatialSTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxSpatialFL
            // 
            this.CheckBoxSpatialFL.AutoSize = true;
            this.CheckBoxSpatialFL.Location = new System.Drawing.Point(20, 58);
            this.CheckBoxSpatialFL.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxSpatialFL.Name = "CheckBoxSpatialFL";
            this.CheckBoxSpatialFL.Size = new System.Drawing.Size(104, 21);
            this.CheckBoxSpatialFL.TabIndex = 2;
            this.CheckBoxSpatialFL.Text = "Flows every";
            this.CheckBoxSpatialFL.UseVisualStyleBackColor = true;
            // 
            // CheckBoxSpatialST
            // 
            this.CheckBoxSpatialST.AutoSize = true;
            this.CheckBoxSpatialST.Location = new System.Drawing.Point(20, 30);
            this.CheckBoxSpatialST.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxSpatialST.Name = "CheckBoxSpatialST";
            this.CheckBoxSpatialST.Size = new System.Drawing.Size(111, 21);
            this.CheckBoxSpatialST.TabIndex = 0;
            this.CheckBoxSpatialST.Text = "Stocks every";
            this.CheckBoxSpatialST.UseVisualStyleBackColor = true;
            // 
            // GroupBoxSpatial
            // 
            this.GroupBoxSpatial.Controls.Add(this.LabelLateralFLTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.LabelSpatialFLTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.LabelSpatialSTTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.TextBoxLateralFLTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.TextBoxSpatialFLTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.TextBoxSpatialSTTimesteps);
            this.GroupBoxSpatial.Controls.Add(this.CheckBoxLateralFL);
            this.GroupBoxSpatial.Controls.Add(this.CheckBoxSpatialFL);
            this.GroupBoxSpatial.Controls.Add(this.CheckBoxSpatialST);
            this.GroupBoxSpatial.Location = new System.Drawing.Point(4, 122);
            this.GroupBoxSpatial.Margin = new System.Windows.Forms.Padding(4);
            this.GroupBoxSpatial.Name = "GroupBoxSpatial";
            this.GroupBoxSpatial.Padding = new System.Windows.Forms.Padding(4);
            this.GroupBoxSpatial.Size = new System.Drawing.Size(371, 123);
            this.GroupBoxSpatial.TabIndex = 1;
            this.GroupBoxSpatial.TabStop = false;
            this.GroupBoxSpatial.Text = "Spatial output";
            // 
            // LabelLateralFLTimesteps
            // 
            this.LabelLateralFLTimesteps.AutoSize = true;
            this.LabelLateralFLTimesteps.Location = new System.Drawing.Point(275, 85);
            this.LabelLateralFLTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelLateralFLTimesteps.Name = "LabelLateralFLTimesteps";
            this.LabelLateralFLTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelLateralFLTimesteps.TabIndex = 5;
            this.LabelLateralFLTimesteps.Text = "timesteps";
            // 
            // TextBoxLateralFLTimesteps
            // 
            this.TextBoxLateralFLTimesteps.Location = new System.Drawing.Point(192, 82);
            this.TextBoxLateralFLTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxLateralFLTimesteps.Name = "TextBoxLateralFLTimesteps";
            this.TextBoxLateralFLTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxLateralFLTimesteps.TabIndex = 5;
            this.TextBoxLateralFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxLateralFL
            // 
            this.CheckBoxLateralFL.AutoSize = true;
            this.CheckBoxLateralFL.Location = new System.Drawing.Point(20, 86);
            this.CheckBoxLateralFL.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxLateralFL.Name = "CheckBoxLateralFL";
            this.CheckBoxLateralFL.Size = new System.Drawing.Size(152, 21);
            this.CheckBoxLateralFL.TabIndex = 4;
            this.CheckBoxLateralFL.Text = "Lateral Flows every";
            this.CheckBoxLateralFL.UseVisualStyleBackColor = true;
            // 
            // LabelSummaryFLTimesteps
            // 
            this.LabelSummaryFLTimesteps.AutoSize = true;
            this.LabelSummaryFLTimesteps.Location = new System.Drawing.Point(275, 58);
            this.LabelSummaryFLTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelSummaryFLTimesteps.Name = "LabelSummaryFLTimesteps";
            this.LabelSummaryFLTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelSummaryFLTimesteps.TabIndex = 5;
            this.LabelSummaryFLTimesteps.Text = "timesteps";
            // 
            // LabelSummarySTTimesteps
            // 
            this.LabelSummarySTTimesteps.AutoSize = true;
            this.LabelSummarySTTimesteps.Location = new System.Drawing.Point(275, 32);
            this.LabelSummarySTTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelSummarySTTimesteps.Name = "LabelSummarySTTimesteps";
            this.LabelSummarySTTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelSummarySTTimesteps.TabIndex = 2;
            this.LabelSummarySTTimesteps.Text = "timesteps";
            // 
            // TextBoxSummaryFLTimesteps
            // 
            this.TextBoxSummaryFLTimesteps.Location = new System.Drawing.Point(192, 55);
            this.TextBoxSummaryFLTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSummaryFLTimesteps.Name = "TextBoxSummaryFLTimesteps";
            this.TextBoxSummaryFLTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxSummaryFLTimesteps.TabIndex = 3;
            this.TextBoxSummaryFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GroupBoxSummary
            // 
            this.GroupBoxSummary.Controls.Add(this.LabelSummaryFLTimesteps);
            this.GroupBoxSummary.Controls.Add(this.LabelSummarySTTimesteps);
            this.GroupBoxSummary.Controls.Add(this.TextBoxSummaryFLTimesteps);
            this.GroupBoxSummary.Controls.Add(this.TextBoxSummarySTTimesteps);
            this.GroupBoxSummary.Controls.Add(this.CheckBoxSummaryFL);
            this.GroupBoxSummary.Controls.Add(this.CheckBoxSummaryST);
            this.GroupBoxSummary.Location = new System.Drawing.Point(4, 14);
            this.GroupBoxSummary.Margin = new System.Windows.Forms.Padding(4);
            this.GroupBoxSummary.Name = "GroupBoxSummary";
            this.GroupBoxSummary.Padding = new System.Windows.Forms.Padding(4);
            this.GroupBoxSummary.Size = new System.Drawing.Size(371, 95);
            this.GroupBoxSummary.TabIndex = 0;
            this.GroupBoxSummary.TabStop = false;
            this.GroupBoxSummary.Text = "Summary output";
            // 
            // TextBoxSummarySTTimesteps
            // 
            this.TextBoxSummarySTTimesteps.Location = new System.Drawing.Point(192, 27);
            this.TextBoxSummarySTTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSummarySTTimesteps.Name = "TextBoxSummarySTTimesteps";
            this.TextBoxSummarySTTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxSummarySTTimesteps.TabIndex = 1;
            this.TextBoxSummarySTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxSummaryFL
            // 
            this.CheckBoxSummaryFL.AutoSize = true;
            this.CheckBoxSummaryFL.Location = new System.Drawing.Point(20, 58);
            this.CheckBoxSummaryFL.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxSummaryFL.Name = "CheckBoxSummaryFL";
            this.CheckBoxSummaryFL.Size = new System.Drawing.Size(104, 21);
            this.CheckBoxSummaryFL.TabIndex = 2;
            this.CheckBoxSummaryFL.Text = "Flows every";
            this.CheckBoxSummaryFL.UseVisualStyleBackColor = true;
            // 
            // CheckBoxSummaryST
            // 
            this.CheckBoxSummaryST.AutoSize = true;
            this.CheckBoxSummaryST.Location = new System.Drawing.Point(20, 30);
            this.CheckBoxSummaryST.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxSummaryST.Name = "CheckBoxSummaryST";
            this.CheckBoxSummaryST.Size = new System.Drawing.Size(111, 21);
            this.CheckBoxSummaryST.TabIndex = 0;
            this.CheckBoxSummaryST.Text = "Stocks every";
            this.CheckBoxSummaryST.UseVisualStyleBackColor = true;
            // 
            // GroupBoxAvgSpatial
            // 
            this.GroupBoxAvgSpatial.Controls.Add(this.LabelAvgSpatialFLTimesteps);
            this.GroupBoxAvgSpatial.Controls.Add(this.LabelAvgSpatialSTTimesteps);
            this.GroupBoxAvgSpatial.Controls.Add(this.TextBoxAvgSpatialFLTimesteps);
            this.GroupBoxAvgSpatial.Controls.Add(this.TextBoxAvgSpatialSTTimesteps);
            this.GroupBoxAvgSpatial.Controls.Add(this.CheckBoxAvgSpatialFL);
            this.GroupBoxAvgSpatial.Controls.Add(this.CheckBoxAvgSpatialST);
            this.GroupBoxAvgSpatial.Location = new System.Drawing.Point(4, 257);
            this.GroupBoxAvgSpatial.Name = "GroupBoxAvgSpatial";
            this.GroupBoxAvgSpatial.Size = new System.Drawing.Size(371, 94);
            this.GroupBoxAvgSpatial.TabIndex = 2;
            this.GroupBoxAvgSpatial.TabStop = false;
            this.GroupBoxAvgSpatial.Text = "Average spatial output";
            // 
            // LabelAvgSpatialFLTimesteps
            // 
            this.LabelAvgSpatialFLTimesteps.AutoSize = true;
            this.LabelAvgSpatialFLTimesteps.Location = new System.Drawing.Point(279, 57);
            this.LabelAvgSpatialFLTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelAvgSpatialFLTimesteps.Name = "LabelAvgSpatialFLTimesteps";
            this.LabelAvgSpatialFLTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelAvgSpatialFLTimesteps.TabIndex = 11;
            this.LabelAvgSpatialFLTimesteps.Text = "timesteps";
            // 
            // LabelAvgSpatialSTTimesteps
            // 
            this.LabelAvgSpatialSTTimesteps.AutoSize = true;
            this.LabelAvgSpatialSTTimesteps.Location = new System.Drawing.Point(279, 31);
            this.LabelAvgSpatialSTTimesteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelAvgSpatialSTTimesteps.Name = "LabelAvgSpatialSTTimesteps";
            this.LabelAvgSpatialSTTimesteps.Size = new System.Drawing.Size(68, 17);
            this.LabelAvgSpatialSTTimesteps.TabIndex = 8;
            this.LabelAvgSpatialSTTimesteps.Text = "timesteps";
            // 
            // TextBoxAvgSpatialFLTimesteps
            // 
            this.TextBoxAvgSpatialFLTimesteps.Location = new System.Drawing.Point(192, 54);
            this.TextBoxAvgSpatialFLTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxAvgSpatialFLTimesteps.Name = "TextBoxAvgSpatialFLTimesteps";
            this.TextBoxAvgSpatialFLTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxAvgSpatialFLTimesteps.TabIndex = 10;
            this.TextBoxAvgSpatialFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TextBoxAvgSpatialSTTimesteps
            // 
            this.TextBoxAvgSpatialSTTimesteps.Location = new System.Drawing.Point(192, 26);
            this.TextBoxAvgSpatialSTTimesteps.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxAvgSpatialSTTimesteps.Name = "TextBoxAvgSpatialSTTimesteps";
            this.TextBoxAvgSpatialSTTimesteps.Size = new System.Drawing.Size(73, 22);
            this.TextBoxAvgSpatialSTTimesteps.TabIndex = 7;
            this.TextBoxAvgSpatialSTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxAvgSpatialFL
            // 
            this.CheckBoxAvgSpatialFL.AutoSize = true;
            this.CheckBoxAvgSpatialFL.Location = new System.Drawing.Point(20, 58);
            this.CheckBoxAvgSpatialFL.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxAvgSpatialFL.Name = "CheckBoxAvgSpatialFL";
            this.CheckBoxAvgSpatialFL.Size = new System.Drawing.Size(104, 21);
            this.CheckBoxAvgSpatialFL.TabIndex = 9;
            this.CheckBoxAvgSpatialFL.Text = "Flows every";
            this.CheckBoxAvgSpatialFL.UseVisualStyleBackColor = true;
            // 
            // CheckBoxAvgSpatialST
            // 
            this.CheckBoxAvgSpatialST.AutoSize = true;
            this.CheckBoxAvgSpatialST.Location = new System.Drawing.Point(20, 30);
            this.CheckBoxAvgSpatialST.Margin = new System.Windows.Forms.Padding(4);
            this.CheckBoxAvgSpatialST.Name = "CheckBoxAvgSpatialST";
            this.CheckBoxAvgSpatialST.Size = new System.Drawing.Size(111, 21);
            this.CheckBoxAvgSpatialST.TabIndex = 6;
            this.CheckBoxAvgSpatialST.Text = "Stocks every";
            this.CheckBoxAvgSpatialST.UseVisualStyleBackColor = true;
            // 
            // OutputOptionsDataFeedView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBoxAvgSpatial);
            this.Controls.Add(this.GroupBoxSpatial);
            this.Controls.Add(this.GroupBoxSummary);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OutputOptionsDataFeedView";
            this.Size = new System.Drawing.Size(396, 368);
            this.GroupBoxSpatial.ResumeLayout(false);
            this.GroupBoxSpatial.PerformLayout();
            this.GroupBoxSummary.ResumeLayout(false);
            this.GroupBoxSummary.PerformLayout();
            this.GroupBoxAvgSpatial.ResumeLayout(false);
            this.GroupBoxAvgSpatial.PerformLayout();
            this.ResumeLayout(false);

		}
		internal System.Windows.Forms.Label LabelSpatialFLTimesteps;
		internal System.Windows.Forms.Label LabelSpatialSTTimesteps;
		internal System.Windows.Forms.TextBox TextBoxSpatialFLTimesteps;
		internal System.Windows.Forms.TextBox TextBoxSpatialSTTimesteps;
		internal System.Windows.Forms.CheckBox CheckBoxSpatialFL;
		internal System.Windows.Forms.CheckBox CheckBoxSpatialST;
		internal System.Windows.Forms.GroupBox GroupBoxSpatial;
		internal System.Windows.Forms.Label LabelSummaryFLTimesteps;
		internal System.Windows.Forms.Label LabelSummarySTTimesteps;
		internal System.Windows.Forms.TextBox TextBoxSummaryFLTimesteps;
		internal System.Windows.Forms.GroupBox GroupBoxSummary;
		internal System.Windows.Forms.TextBox TextBoxSummarySTTimesteps;
		internal System.Windows.Forms.CheckBox CheckBoxSummaryFL;
		internal System.Windows.Forms.CheckBox CheckBoxSummaryST;
        internal System.Windows.Forms.Label LabelLateralFLTimesteps;
        internal System.Windows.Forms.TextBox TextBoxLateralFLTimesteps;
        internal System.Windows.Forms.CheckBox CheckBoxLateralFL;
        private System.Windows.Forms.GroupBox GroupBoxAvgSpatial;
        internal System.Windows.Forms.Label LabelAvgSpatialFLTimesteps;
        internal System.Windows.Forms.Label LabelAvgSpatialSTTimesteps;
        internal System.Windows.Forms.TextBox TextBoxAvgSpatialFLTimesteps;
        internal System.Windows.Forms.TextBox TextBoxAvgSpatialSTTimesteps;
        internal System.Windows.Forms.CheckBox CheckBoxAvgSpatialFL;
        internal System.Windows.Forms.CheckBox CheckBoxAvgSpatialST;
    }
}