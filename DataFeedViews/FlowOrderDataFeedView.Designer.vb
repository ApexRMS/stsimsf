<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FlowOrderDataFeedView
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
        Me.LabelFlowOrder = New System.Windows.Forms.Label()
        Me.CheckBoxApplyBeforeTransitions = New System.Windows.Forms.CheckBox()
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously = New System.Windows.Forms.CheckBox()
        Me.PanelOptions = New System.Windows.Forms.Panel()
        Me.PanelFlowOrder = New System.Windows.Forms.Panel()
        Me.PanelOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelFlowOrder
        '
        Me.LabelFlowOrder.AutoSize = True
        Me.LabelFlowOrder.Location = New System.Drawing.Point(0, 57)
        Me.LabelFlowOrder.Name = "LabelFlowOrder"
        Me.LabelFlowOrder.Size = New System.Drawing.Size(61, 13)
        Me.LabelFlowOrder.TabIndex = 2
        Me.LabelFlowOrder.Text = "Flow Order:"
        '
        'CheckBoxApplyBeforeTransitions
        '
        Me.CheckBoxApplyBeforeTransitions.AutoSize = True
        Me.CheckBoxApplyBeforeTransitions.Location = New System.Drawing.Point(2, 6)
        Me.CheckBoxApplyBeforeTransitions.Name = "CheckBoxApplyBeforeTransitions"
        Me.CheckBoxApplyBeforeTransitions.Size = New System.Drawing.Size(135, 17)
        Me.CheckBoxApplyBeforeTransitions.TabIndex = 0
        Me.CheckBoxApplyBeforeTransitions.Text = "Apply before transitions"
        Me.CheckBoxApplyBeforeTransitions.UseVisualStyleBackColor = True
        '
        'CheckBoxApplyEquallyRankedFlowsSimultaneously
        '
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.AutoSize = True
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.Location = New System.Drawing.Point(2, 27)
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.Name = "CheckBoxApplyEquallyRankedFlowsSimultaneously"
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.Size = New System.Drawing.Size(222, 17)
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.TabIndex = 1
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.Text = "Apply equally ranked flows simultaneously"
        Me.CheckBoxApplyEquallyRankedFlowsSimultaneously.UseVisualStyleBackColor = True
        '
        'PanelOptions
        '
        Me.PanelOptions.Controls.Add(Me.CheckBoxApplyBeforeTransitions)
        Me.PanelOptions.Controls.Add(Me.CheckBoxApplyEquallyRankedFlowsSimultaneously)
        Me.PanelOptions.Controls.Add(Me.LabelFlowOrder)
        Me.PanelOptions.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelOptions.Location = New System.Drawing.Point(0, 0)
        Me.PanelOptions.Name = "PanelOptions"
        Me.PanelOptions.Size = New System.Drawing.Size(569, 77)
        Me.PanelOptions.TabIndex = 12
        '
        'PanelFlowOrder
        '
        Me.PanelFlowOrder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelFlowOrder.Location = New System.Drawing.Point(0, 77)
        Me.PanelFlowOrder.Name = "PanelFlowOrder"
        Me.PanelFlowOrder.Size = New System.Drawing.Size(569, 247)
        Me.PanelFlowOrder.TabIndex = 0
        '
        'FlowOrderDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.PanelFlowOrder)
        Me.Controls.Add(Me.PanelOptions)
        Me.Name = "FlowOrderDataFeedView"
        Me.Size = New System.Drawing.Size(569, 324)
        Me.PanelOptions.ResumeLayout(False)
        Me.PanelOptions.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LabelFlowOrder As System.Windows.Forms.Label
    Friend WithEvents CheckBoxApplyBeforeTransitions As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxApplyEquallyRankedFlowsSimultaneously As System.Windows.Forms.CheckBox
    Friend WithEvents PanelOptions As System.Windows.Forms.Panel
    Friend WithEvents PanelFlowOrder As System.Windows.Forms.Panel

End Class
