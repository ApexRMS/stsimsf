<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OutputOptionsDataFeedView
    Inherits SyncroSim.Core.Forms.DataFeedView

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LabelSpatialFLTimesteps = New System.Windows.Forms.Label()
        Me.LabelSpatialSTTimesteps = New System.Windows.Forms.Label()
        Me.TextBoxSpatialFLTimesteps = New System.Windows.Forms.TextBox()
        Me.TextBoxSpatialSTTimesteps = New System.Windows.Forms.TextBox()
        Me.CheckBoxSpatialFL = New System.Windows.Forms.CheckBox()
        Me.CheckBoxSpatialST = New System.Windows.Forms.CheckBox()
        Me.GroupBoxSpatial = New System.Windows.Forms.GroupBox()
        Me.LabelSummaryFLTimesteps = New System.Windows.Forms.Label()
        Me.LabelSummarySTTimesteps = New System.Windows.Forms.Label()
        Me.TextBoxSummaryFLTimesteps = New System.Windows.Forms.TextBox()
        Me.GroupBoxSummary = New System.Windows.Forms.GroupBox()
        Me.TextBoxSummarySTTimesteps = New System.Windows.Forms.TextBox()
        Me.CheckBoxSummaryFL = New System.Windows.Forms.CheckBox()
        Me.CheckBoxSummaryST = New System.Windows.Forms.CheckBox()
        Me.GroupBoxSpatial.SuspendLayout()
        Me.GroupBoxSummary.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelSpatialFLTimesteps
        '
        Me.LabelSpatialFLTimesteps.AutoSize = True
        Me.LabelSpatialFLTimesteps.Location = New System.Drawing.Point(185, 46)
        Me.LabelSpatialFLTimesteps.Name = "LabelSpatialFLTimesteps"
        Me.LabelSpatialFLTimesteps.Size = New System.Drawing.Size(51, 13)
        Me.LabelSpatialFLTimesteps.TabIndex = 5
        Me.LabelSpatialFLTimesteps.Text = "timesteps"
        '
        'LabelSpatialSTTimesteps
        '
        Me.LabelSpatialSTTimesteps.AutoSize = True
        Me.LabelSpatialSTTimesteps.Location = New System.Drawing.Point(185, 25)
        Me.LabelSpatialSTTimesteps.Name = "LabelSpatialSTTimesteps"
        Me.LabelSpatialSTTimesteps.Size = New System.Drawing.Size(51, 13)
        Me.LabelSpatialSTTimesteps.TabIndex = 2
        Me.LabelSpatialSTTimesteps.Text = "timesteps"
        '
        'TextBoxSpatialFLTimesteps
        '
        Me.TextBoxSpatialFLTimesteps.Location = New System.Drawing.Point(123, 44)
        Me.TextBoxSpatialFLTimesteps.Name = "TextBoxSpatialFLTimesteps"
        Me.TextBoxSpatialFLTimesteps.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxSpatialFLTimesteps.TabIndex = 4
        Me.TextBoxSpatialFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBoxSpatialSTTimesteps
        '
        Me.TextBoxSpatialSTTimesteps.Location = New System.Drawing.Point(123, 21)
        Me.TextBoxSpatialSTTimesteps.Name = "TextBoxSpatialSTTimesteps"
        Me.TextBoxSpatialSTTimesteps.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxSpatialSTTimesteps.TabIndex = 1
        Me.TextBoxSpatialSTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CheckBoxSpatialFL
        '
        Me.CheckBoxSpatialFL.AutoSize = True
        Me.CheckBoxSpatialFL.Location = New System.Drawing.Point(15, 47)
        Me.CheckBoxSpatialFL.Name = "CheckBoxSpatialFL"
        Me.CheckBoxSpatialFL.Size = New System.Drawing.Size(82, 17)
        Me.CheckBoxSpatialFL.TabIndex = 3
        Me.CheckBoxSpatialFL.Text = "Flows every"
        Me.CheckBoxSpatialFL.UseVisualStyleBackColor = True
        '
        'CheckBoxSpatialST
        '
        Me.CheckBoxSpatialST.AutoSize = True
        Me.CheckBoxSpatialST.Location = New System.Drawing.Point(15, 24)
        Me.CheckBoxSpatialST.Name = "CheckBoxSpatialST"
        Me.CheckBoxSpatialST.Size = New System.Drawing.Size(88, 17)
        Me.CheckBoxSpatialST.TabIndex = 0
        Me.CheckBoxSpatialST.Text = "Stocks every"
        Me.CheckBoxSpatialST.UseVisualStyleBackColor = True
        '
        'GroupBoxSpatial
        '
        Me.GroupBoxSpatial.Controls.Add(Me.LabelSpatialFLTimesteps)
        Me.GroupBoxSpatial.Controls.Add(Me.LabelSpatialSTTimesteps)
        Me.GroupBoxSpatial.Controls.Add(Me.TextBoxSpatialFLTimesteps)
        Me.GroupBoxSpatial.Controls.Add(Me.TextBoxSpatialSTTimesteps)
        Me.GroupBoxSpatial.Controls.Add(Me.CheckBoxSpatialFL)
        Me.GroupBoxSpatial.Controls.Add(Me.CheckBoxSpatialST)
        Me.GroupBoxSpatial.Location = New System.Drawing.Point(3, 91)
        Me.GroupBoxSpatial.Name = "GroupBoxSpatial"
        Me.GroupBoxSpatial.Size = New System.Drawing.Size(255, 77)
        Me.GroupBoxSpatial.TabIndex = 4
        Me.GroupBoxSpatial.TabStop = False
        Me.GroupBoxSpatial.Text = "Spatial output"
        '
        'LabelSummaryFLTimesteps
        '
        Me.LabelSummaryFLTimesteps.AutoSize = True
        Me.LabelSummaryFLTimesteps.Location = New System.Drawing.Point(185, 47)
        Me.LabelSummaryFLTimesteps.Name = "LabelSummaryFLTimesteps"
        Me.LabelSummaryFLTimesteps.Size = New System.Drawing.Size(51, 13)
        Me.LabelSummaryFLTimesteps.TabIndex = 5
        Me.LabelSummaryFLTimesteps.Text = "timesteps"
        '
        'LabelSummarySTTimesteps
        '
        Me.LabelSummarySTTimesteps.AutoSize = True
        Me.LabelSummarySTTimesteps.Location = New System.Drawing.Point(185, 26)
        Me.LabelSummarySTTimesteps.Name = "LabelSummarySTTimesteps"
        Me.LabelSummarySTTimesteps.Size = New System.Drawing.Size(51, 13)
        Me.LabelSummarySTTimesteps.TabIndex = 2
        Me.LabelSummarySTTimesteps.Text = "timesteps"
        '
        'TextBoxSummaryFLTimesteps
        '
        Me.TextBoxSummaryFLTimesteps.Location = New System.Drawing.Point(123, 45)
        Me.TextBoxSummaryFLTimesteps.Name = "TextBoxSummaryFLTimesteps"
        Me.TextBoxSummaryFLTimesteps.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxSummaryFLTimesteps.TabIndex = 4
        Me.TextBoxSummaryFLTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GroupBoxSummary
        '
        Me.GroupBoxSummary.Controls.Add(Me.LabelSummaryFLTimesteps)
        Me.GroupBoxSummary.Controls.Add(Me.LabelSummarySTTimesteps)
        Me.GroupBoxSummary.Controls.Add(Me.TextBoxSummaryFLTimesteps)
        Me.GroupBoxSummary.Controls.Add(Me.TextBoxSummarySTTimesteps)
        Me.GroupBoxSummary.Controls.Add(Me.CheckBoxSummaryFL)
        Me.GroupBoxSummary.Controls.Add(Me.CheckBoxSummaryST)
        Me.GroupBoxSummary.Location = New System.Drawing.Point(3, 3)
        Me.GroupBoxSummary.Name = "GroupBoxSummary"
        Me.GroupBoxSummary.Size = New System.Drawing.Size(255, 77)
        Me.GroupBoxSummary.TabIndex = 3
        Me.GroupBoxSummary.TabStop = False
        Me.GroupBoxSummary.Text = "Summary output"
        '
        'TextBoxSummarySTTimesteps
        '
        Me.TextBoxSummarySTTimesteps.Location = New System.Drawing.Point(123, 22)
        Me.TextBoxSummarySTTimesteps.Name = "TextBoxSummarySTTimesteps"
        Me.TextBoxSummarySTTimesteps.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxSummarySTTimesteps.TabIndex = 1
        Me.TextBoxSummarySTTimesteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CheckBoxSummaryFL
        '
        Me.CheckBoxSummaryFL.AutoSize = True
        Me.CheckBoxSummaryFL.Location = New System.Drawing.Point(15, 47)
        Me.CheckBoxSummaryFL.Name = "CheckBoxSummaryFL"
        Me.CheckBoxSummaryFL.Size = New System.Drawing.Size(82, 17)
        Me.CheckBoxSummaryFL.TabIndex = 3
        Me.CheckBoxSummaryFL.Text = "Flows every"
        Me.CheckBoxSummaryFL.UseVisualStyleBackColor = True
        '
        'CheckBoxSummaryST
        '
        Me.CheckBoxSummaryST.AutoSize = True
        Me.CheckBoxSummaryST.Location = New System.Drawing.Point(15, 24)
        Me.CheckBoxSummaryST.Name = "CheckBoxSummaryST"
        Me.CheckBoxSummaryST.Size = New System.Drawing.Size(88, 17)
        Me.CheckBoxSummaryST.TabIndex = 0
        Me.CheckBoxSummaryST.Text = "Stocks every"
        Me.CheckBoxSummaryST.UseVisualStyleBackColor = True
        '
        'OutputOptionsDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBoxSpatial)
        Me.Controls.Add(Me.GroupBoxSummary)
        Me.Name = "OutputOptionsDataFeedView"
        Me.Size = New System.Drawing.Size(263, 170)
        Me.GroupBoxSpatial.ResumeLayout(False)
        Me.GroupBoxSpatial.PerformLayout()
        Me.GroupBoxSummary.ResumeLayout(False)
        Me.GroupBoxSummary.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LabelSpatialFLTimesteps As System.Windows.Forms.Label
    Friend WithEvents LabelSpatialSTTimesteps As System.Windows.Forms.Label
    Friend WithEvents TextBoxSpatialFLTimesteps As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxSpatialSTTimesteps As System.Windows.Forms.TextBox
    Friend WithEvents CheckBoxSpatialFL As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxSpatialST As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxSpatial As System.Windows.Forms.GroupBox
    Friend WithEvents LabelSummaryFLTimesteps As System.Windows.Forms.Label
    Friend WithEvents LabelSummarySTTimesteps As System.Windows.Forms.Label
    Friend WithEvents TextBoxSummaryFLTimesteps As System.Windows.Forms.TextBox
    Friend WithEvents GroupBoxSummary As System.Windows.Forms.GroupBox
    Friend WithEvents TextBoxSummarySTTimesteps As System.Windows.Forms.TextBox
    Friend WithEvents CheckBoxSummaryFL As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxSummaryST As System.Windows.Forms.CheckBox

End Class
