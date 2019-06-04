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
            this.GroupBoxSpatial.SuspendLayout();
            this.GroupBoxSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelSpatialFLTimesteps
            // 
            this.LabelSpatialFLTimesteps.AutoSize = true;
            this.LabelSpatialFLTimesteps.Location = new System.Drawing.Point(206, 46);
            this.LabelSpatialFLTimesteps.Name = "LabelSpatialFLTimesteps";
            this.LabelSpatialFLTimesteps.Size = new System.Drawing.Size(51, 13);
            this.LabelSpatialFLTimesteps.TabIndex = 5;
            this.LabelSpatialFLTimesteps.Text = "timesteps";
            // 
            // LabelSpatialSTTimesteps
            // 
            this.LabelSpatialSTTimesteps.AutoSize = true;
            this.LabelSpatialSTTimesteps.Location = new System.Drawing.Point(206, 25);
            this.LabelSpatialSTTimesteps.Name = "LabelSpatialSTTimesteps";
            this.LabelSpatialSTTimesteps.Size = new System.Drawing.Size(51, 13);
            this.LabelSpatialSTTimesteps.TabIndex = 2;
            this.LabelSpatialSTTimesteps.Text = "timesteps";
            // 
            // TextBoxSpatialFLTimesteps
            // 
            this.TextBoxSpatialFLTimesteps.Location = new System.Drawing.Point(144, 44);
            this.TextBoxSpatialFLTimesteps.Name = "TextBoxSpatialFLTimesteps";
            this.TextBoxSpatialFLTimesteps.Size = new System.Drawing.Size(56, 20);
            this.TextBoxSpatialFLTimesteps.TabIndex = 3;
            this.TextBoxSpatialFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TextBoxSpatialSTTimesteps
            // 
            this.TextBoxSpatialSTTimesteps.Location = new System.Drawing.Point(144, 21);
            this.TextBoxSpatialSTTimesteps.Name = "TextBoxSpatialSTTimesteps";
            this.TextBoxSpatialSTTimesteps.Size = new System.Drawing.Size(56, 20);
            this.TextBoxSpatialSTTimesteps.TabIndex = 1;
            this.TextBoxSpatialSTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxSpatialFL
            // 
            this.CheckBoxSpatialFL.AutoSize = true;
            this.CheckBoxSpatialFL.Location = new System.Drawing.Point(15, 47);
            this.CheckBoxSpatialFL.Name = "CheckBoxSpatialFL";
            this.CheckBoxSpatialFL.Size = new System.Drawing.Size(82, 17);
            this.CheckBoxSpatialFL.TabIndex = 2;
            this.CheckBoxSpatialFL.Text = "Flows every";
            this.CheckBoxSpatialFL.UseVisualStyleBackColor = true;
            // 
            // CheckBoxSpatialST
            // 
            this.CheckBoxSpatialST.AutoSize = true;
            this.CheckBoxSpatialST.Location = new System.Drawing.Point(15, 24);
            this.CheckBoxSpatialST.Name = "CheckBoxSpatialST";
            this.CheckBoxSpatialST.Size = new System.Drawing.Size(88, 17);
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
            this.GroupBoxSpatial.Location = new System.Drawing.Point(3, 99);
            this.GroupBoxSpatial.Name = "GroupBoxSpatial";
            this.GroupBoxSpatial.Size = new System.Drawing.Size(278, 100);
            this.GroupBoxSpatial.TabIndex = 1;
            this.GroupBoxSpatial.TabStop = false;
            this.GroupBoxSpatial.Text = "Spatial output";
            // 
            // LabelLateralFLTimesteps
            // 
            this.LabelLateralFLTimesteps.AutoSize = true;
            this.LabelLateralFLTimesteps.Location = new System.Drawing.Point(206, 69);
            this.LabelLateralFLTimesteps.Name = "LabelLateralFLTimesteps";
            this.LabelLateralFLTimesteps.Size = new System.Drawing.Size(51, 13);
            this.LabelLateralFLTimesteps.TabIndex = 5;
            this.LabelLateralFLTimesteps.Text = "timesteps";
            // 
            // TextBoxLateralFLTimesteps
            // 
            this.TextBoxLateralFLTimesteps.Location = new System.Drawing.Point(144, 67);
            this.TextBoxLateralFLTimesteps.Name = "TextBoxLateralFLTimesteps";
            this.TextBoxLateralFLTimesteps.Size = new System.Drawing.Size(56, 20);
            this.TextBoxLateralFLTimesteps.TabIndex = 5;
            this.TextBoxLateralFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxLateralFL
            // 
            this.CheckBoxLateralFL.AutoSize = true;
            this.CheckBoxLateralFL.Location = new System.Drawing.Point(15, 70);
            this.CheckBoxLateralFL.Name = "CheckBoxLateralFL";
            this.CheckBoxLateralFL.Size = new System.Drawing.Size(117, 17);
            this.CheckBoxLateralFL.TabIndex = 4;
            this.CheckBoxLateralFL.Text = "Lateral Flows every";
            this.CheckBoxLateralFL.UseVisualStyleBackColor = true;
            // 
            // LabelSummaryFLTimesteps
            // 
            this.LabelSummaryFLTimesteps.AutoSize = true;
            this.LabelSummaryFLTimesteps.Location = new System.Drawing.Point(206, 47);
            this.LabelSummaryFLTimesteps.Name = "LabelSummaryFLTimesteps";
            this.LabelSummaryFLTimesteps.Size = new System.Drawing.Size(51, 13);
            this.LabelSummaryFLTimesteps.TabIndex = 5;
            this.LabelSummaryFLTimesteps.Text = "timesteps";
            // 
            // LabelSummarySTTimesteps
            // 
            this.LabelSummarySTTimesteps.AutoSize = true;
            this.LabelSummarySTTimesteps.Location = new System.Drawing.Point(206, 26);
            this.LabelSummarySTTimesteps.Name = "LabelSummarySTTimesteps";
            this.LabelSummarySTTimesteps.Size = new System.Drawing.Size(51, 13);
            this.LabelSummarySTTimesteps.TabIndex = 2;
            this.LabelSummarySTTimesteps.Text = "timesteps";
            // 
            // TextBoxSummaryFLTimesteps
            // 
            this.TextBoxSummaryFLTimesteps.Location = new System.Drawing.Point(144, 45);
            this.TextBoxSummaryFLTimesteps.Name = "TextBoxSummaryFLTimesteps";
            this.TextBoxSummaryFLTimesteps.Size = new System.Drawing.Size(56, 20);
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
            this.GroupBoxSummary.Location = new System.Drawing.Point(3, 11);
            this.GroupBoxSummary.Name = "GroupBoxSummary";
            this.GroupBoxSummary.Size = new System.Drawing.Size(278, 77);
            this.GroupBoxSummary.TabIndex = 0;
            this.GroupBoxSummary.TabStop = false;
            this.GroupBoxSummary.Text = "Summary output";
            // 
            // TextBoxSummarySTTimesteps
            // 
            this.TextBoxSummarySTTimesteps.Location = new System.Drawing.Point(144, 22);
            this.TextBoxSummarySTTimesteps.Name = "TextBoxSummarySTTimesteps";
            this.TextBoxSummarySTTimesteps.Size = new System.Drawing.Size(56, 20);
            this.TextBoxSummarySTTimesteps.TabIndex = 1;
            this.TextBoxSummarySTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CheckBoxSummaryFL
            // 
            this.CheckBoxSummaryFL.AutoSize = true;
            this.CheckBoxSummaryFL.Location = new System.Drawing.Point(15, 47);
            this.CheckBoxSummaryFL.Name = "CheckBoxSummaryFL";
            this.CheckBoxSummaryFL.Size = new System.Drawing.Size(82, 17);
            this.CheckBoxSummaryFL.TabIndex = 2;
            this.CheckBoxSummaryFL.Text = "Flows every";
            this.CheckBoxSummaryFL.UseVisualStyleBackColor = true;
            // 
            // CheckBoxSummaryST
            // 
            this.CheckBoxSummaryST.AutoSize = true;
            this.CheckBoxSummaryST.Location = new System.Drawing.Point(15, 24);
            this.CheckBoxSummaryST.Name = "CheckBoxSummaryST";
            this.CheckBoxSummaryST.Size = new System.Drawing.Size(88, 17);
            this.CheckBoxSummaryST.TabIndex = 0;
            this.CheckBoxSummaryST.Text = "Stocks every";
            this.CheckBoxSummaryST.UseVisualStyleBackColor = true;
            // 
            // OutputOptionsDataFeedView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBoxSpatial);
            this.Controls.Add(this.GroupBoxSummary);
            this.Name = "OutputOptionsDataFeedView";
            this.Size = new System.Drawing.Size(305, 216);
            this.GroupBoxSpatial.ResumeLayout(false);
            this.GroupBoxSpatial.PerformLayout();
            this.GroupBoxSummary.ResumeLayout(false);
            this.GroupBoxSummary.PerformLayout();
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
    }
}