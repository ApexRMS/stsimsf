<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InitialStocksDataFeedView
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
        Me.SplitContainerMain = New System.Windows.Forms.SplitContainer()
        Me.PanelNonSpatial = New System.Windows.Forms.Panel()
        Me.PanelLabelTop = New System.Windows.Forms.Panel()
        Me.LabelNonSpatial = New System.Windows.Forms.Label()
        Me.PanelSpatial = New System.Windows.Forms.Panel()
        Me.PanelLabelBottom = New System.Windows.Forms.Panel()
        Me.LabelSpatial = New System.Windows.Forms.Label()
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerMain.Panel1.SuspendLayout()
        Me.SplitContainerMain.Panel2.SuspendLayout()
        Me.SplitContainerMain.SuspendLayout()
        Me.PanelLabelTop.SuspendLayout()
        Me.PanelLabelBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainerMain
        '
        Me.SplitContainerMain.BackColor = System.Drawing.Color.Gainsboro
        Me.SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerMain.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerMain.Name = "SplitContainerMain"
        Me.SplitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerMain.Panel1
        '
        Me.SplitContainerMain.Panel1.BackColor = System.Drawing.Color.White
        Me.SplitContainerMain.Panel1.Controls.Add(Me.PanelNonSpatial)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.PanelLabelTop)
        Me.SplitContainerMain.Panel1.Padding = New System.Windows.Forms.Padding(0, 0, 0, 4)
        '
        'SplitContainerMain.Panel2
        '
        Me.SplitContainerMain.Panel2.Controls.Add(Me.PanelSpatial)
        Me.SplitContainerMain.Panel2.Controls.Add(Me.PanelLabelBottom)
        Me.SplitContainerMain.Size = New System.Drawing.Size(604, 416)
        Me.SplitContainerMain.SplitterDistance = 207
        Me.SplitContainerMain.TabIndex = 11
        '
        'PanelNonSpatial
        '
        Me.PanelNonSpatial.BackColor = System.Drawing.SystemColors.Control
        Me.PanelNonSpatial.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelNonSpatial.Location = New System.Drawing.Point(0, 30)
        Me.PanelNonSpatial.Name = "PanelNonSpatial"
        Me.PanelNonSpatial.Size = New System.Drawing.Size(604, 173)
        Me.PanelNonSpatial.TabIndex = 1
        '
        'PanelLabelTop
        '
        Me.PanelLabelTop.BackColor = System.Drawing.Color.White
        Me.PanelLabelTop.Controls.Add(Me.LabelNonSpatial)
        Me.PanelLabelTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelLabelTop.Location = New System.Drawing.Point(0, 0)
        Me.PanelLabelTop.Name = "PanelLabelTop"
        Me.PanelLabelTop.Size = New System.Drawing.Size(604, 30)
        Me.PanelLabelTop.TabIndex = 0
        '
        'LabelNonSpatial
        '
        Me.LabelNonSpatial.AutoSize = True
        Me.LabelNonSpatial.Location = New System.Drawing.Point(0, 8)
        Me.LabelNonSpatial.Name = "LabelNonSpatial"
        Me.LabelNonSpatial.Size = New System.Drawing.Size(128, 13)
        Me.LabelNonSpatial.TabIndex = 0
        Me.LabelNonSpatial.Text = "Initial stocks - non-spatial:"
        '
        'PanelSpatial
        '
        Me.PanelSpatial.BackColor = System.Drawing.SystemColors.Control
        Me.PanelSpatial.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelSpatial.Location = New System.Drawing.Point(0, 30)
        Me.PanelSpatial.Name = "PanelSpatial"
        Me.PanelSpatial.Size = New System.Drawing.Size(604, 175)
        Me.PanelSpatial.TabIndex = 1
        '
        'PanelLabelBottom
        '
        Me.PanelLabelBottom.BackColor = System.Drawing.Color.White
        Me.PanelLabelBottom.Controls.Add(Me.LabelSpatial)
        Me.PanelLabelBottom.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelLabelBottom.Location = New System.Drawing.Point(0, 0)
        Me.PanelLabelBottom.Name = "PanelLabelBottom"
        Me.PanelLabelBottom.Size = New System.Drawing.Size(604, 30)
        Me.PanelLabelBottom.TabIndex = 0
        '
        'LabelSpatial
        '
        Me.LabelSpatial.AutoSize = True
        Me.LabelSpatial.Location = New System.Drawing.Point(0, 8)
        Me.LabelSpatial.Name = "LabelSpatial"
        Me.LabelSpatial.Size = New System.Drawing.Size(107, 13)
        Me.LabelSpatial.TabIndex = 0
        Me.LabelSpatial.Text = "Initial stocks - spatial:"
        '
        'InitialStocksDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainerMain)
        Me.Name = "InitialStocksDataFeedView"
        Me.Size = New System.Drawing.Size(604, 416)
        Me.SplitContainerMain.Panel1.ResumeLayout(False)
        Me.SplitContainerMain.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerMain.ResumeLayout(False)
        Me.PanelLabelTop.ResumeLayout(False)
        Me.PanelLabelTop.PerformLayout()
        Me.PanelLabelBottom.ResumeLayout(False)
        Me.PanelLabelBottom.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainerMain As System.Windows.Forms.SplitContainer
    Friend WithEvents PanelNonSpatial As System.Windows.Forms.Panel
    Friend WithEvents PanelLabelTop As System.Windows.Forms.Panel
    Friend WithEvents LabelNonSpatial As System.Windows.Forms.Label
    Friend WithEvents PanelSpatial As System.Windows.Forms.Panel
    Friend WithEvents PanelLabelBottom As System.Windows.Forms.Panel
    Friend WithEvents LabelSpatial As System.Windows.Forms.Label

End Class
