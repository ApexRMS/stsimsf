<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FlowPathwayDataFeedView
    Inherits SyncroSim.Core.Forms.DataFeedView

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PanelBottomControls = New System.Windows.Forms.Panel()
        Me.PanelZoomControls = New System.Windows.Forms.Panel()
        Me.ButtonZoomIn = New System.Windows.Forms.Button()
        Me.ButtonZoomOut = New System.Windows.Forms.Button()
        Me.SplitContainerTabStrip = New System.Windows.Forms.SplitContainer()
        Me.TabStripMain = New SyncroSim.Common.Forms.TabStrip()
        Me.ScrollBarHorizontal = New System.Windows.Forms.HScrollBar()
        Me.ScrollBarVertical = New System.Windows.Forms.VScrollBar()
        Me.PanelControlHost = New System.Windows.Forms.Panel()
        Me.PanelBottomControls.SuspendLayout()
        Me.PanelZoomControls.SuspendLayout()
        CType(Me.SplitContainerTabStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerTabStrip.Panel1.SuspendLayout()
        Me.SplitContainerTabStrip.Panel2.SuspendLayout()
        Me.SplitContainerTabStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottomControls
        '
        Me.PanelBottomControls.BackColor = System.Drawing.SystemColors.Control
        Me.PanelBottomControls.Controls.Add(Me.PanelZoomControls)
        Me.PanelBottomControls.Controls.Add(Me.SplitContainerTabStrip)
        Me.PanelBottomControls.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottomControls.Location = New System.Drawing.Point(0, 420)
        Me.PanelBottomControls.Name = "PanelBottomControls"
        Me.PanelBottomControls.Size = New System.Drawing.Size(883, 20)
        Me.PanelBottomControls.TabIndex = 1
        '
        'PanelZoomControls
        '
        Me.PanelZoomControls.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelZoomControls.Controls.Add(Me.ButtonZoomIn)
        Me.PanelZoomControls.Controls.Add(Me.ButtonZoomOut)
        Me.PanelZoomControls.Location = New System.Drawing.Point(823, 0)
        Me.PanelZoomControls.Name = "PanelZoomControls"
        Me.PanelZoomControls.Size = New System.Drawing.Size(43, 20)
        Me.PanelZoomControls.TabIndex = 14
        '
        'ButtonZoomIn
        '
        Me.ButtonZoomIn.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ButtonZoomIn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.ButtonZoomIn.FlatAppearance.BorderSize = 0
        Me.ButtonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonZoomIn.Image = Global.SyncroSim.STSimStockFlow.My.Resources.Resources.Plus16x16
        Me.ButtonZoomIn.Location = New System.Drawing.Point(20, 0)
        Me.ButtonZoomIn.Name = "ButtonZoomIn"
        Me.ButtonZoomIn.Size = New System.Drawing.Size(20, 20)
        Me.ButtonZoomIn.TabIndex = 1
        Me.ButtonZoomIn.UseVisualStyleBackColor = False
        '
        'ButtonZoomOut
        '
        Me.ButtonZoomOut.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ButtonZoomOut.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.ButtonZoomOut.FlatAppearance.BorderSize = 0
        Me.ButtonZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonZoomOut.Image = Global.SyncroSim.STSimStockFlow.My.Resources.Resources.Minus16x16
        Me.ButtonZoomOut.Location = New System.Drawing.Point(0, 0)
        Me.ButtonZoomOut.Name = "ButtonZoomOut"
        Me.ButtonZoomOut.Size = New System.Drawing.Size(20, 20)
        Me.ButtonZoomOut.TabIndex = 0
        Me.ButtonZoomOut.UseVisualStyleBackColor = False
        '
        'SplitContainerTabStrip
        '
        Me.SplitContainerTabStrip.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainerTabStrip.BackColor = System.Drawing.Color.LightSteelBlue
        Me.SplitContainerTabStrip.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerTabStrip.Name = "SplitContainerTabStrip"
        '
        'SplitContainerTabStrip.Panel1
        '
        Me.SplitContainerTabStrip.Panel1.Controls.Add(Me.TabStripMain)
        '
        'SplitContainerTabStrip.Panel2
        '
        Me.SplitContainerTabStrip.Panel2.Controls.Add(Me.ScrollBarHorizontal)
        Me.SplitContainerTabStrip.Size = New System.Drawing.Size(822, 20)
        Me.SplitContainerTabStrip.SplitterDistance = 612
        Me.SplitContainerTabStrip.SplitterWidth = 8
        Me.SplitContainerTabStrip.TabIndex = 5
        '
        'TabStripMain
        '
        Me.TabStripMain.BackColor = System.Drawing.Color.White
        Me.TabStripMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabStripMain.Location = New System.Drawing.Point(0, 0)
        Me.TabStripMain.Name = "TabStripMain"
        Me.TabStripMain.Size = New System.Drawing.Size(612, 20)
        Me.TabStripMain.TabIndex = 0
        Me.TabStripMain.TabStop = False
        Me.TabStripMain.Text = "TabStripMain"
        '
        'ScrollBarHorizontal
        '
        Me.ScrollBarHorizontal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ScrollBarHorizontal.Location = New System.Drawing.Point(0, 0)
        Me.ScrollBarHorizontal.Name = "ScrollBarHorizontal"
        Me.ScrollBarHorizontal.Size = New System.Drawing.Size(202, 20)
        Me.ScrollBarHorizontal.TabIndex = 0
        Me.ScrollBarHorizontal.TabStop = True
        '
        'ScrollBarVertical
        '
        Me.ScrollBarVertical.Dock = System.Windows.Forms.DockStyle.Right
        Me.ScrollBarVertical.Location = New System.Drawing.Point(863, 0)
        Me.ScrollBarVertical.Name = "ScrollBarVertical"
        Me.ScrollBarVertical.Size = New System.Drawing.Size(20, 420)
        Me.ScrollBarVertical.TabIndex = 1
        Me.ScrollBarVertical.TabStop = True
        '
        'PanelControlHost
        '
        Me.PanelControlHost.BackColor = System.Drawing.Color.White
        Me.PanelControlHost.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelControlHost.Location = New System.Drawing.Point(0, 0)
        Me.PanelControlHost.Name = "PanelControlHost"
        Me.PanelControlHost.Size = New System.Drawing.Size(863, 420)
        Me.PanelControlHost.TabIndex = 0
        Me.PanelControlHost.TabStop = True
        '
        'FlowPathwayDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PanelControlHost)
        Me.Controls.Add(Me.ScrollBarVertical)
        Me.Controls.Add(Me.PanelBottomControls)
        Me.Name = "FlowPathwayDataFeedView"
        Me.Size = New System.Drawing.Size(883, 440)
        Me.PanelBottomControls.ResumeLayout(False)
        Me.PanelZoomControls.ResumeLayout(False)
        Me.SplitContainerTabStrip.Panel1.ResumeLayout(False)
        Me.SplitContainerTabStrip.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerTabStrip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerTabStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelBottomControls As System.Windows.Forms.Panel
    Friend WithEvents PanelZoomControls As System.Windows.Forms.Panel
    Friend WithEvents ButtonZoomIn As System.Windows.Forms.Button
    Friend WithEvents ButtonZoomOut As System.Windows.Forms.Button
    Friend WithEvents SplitContainerTabStrip As System.Windows.Forms.SplitContainer
    Friend WithEvents TabStripMain As SyncroSim.Common.Forms.TabStrip
    Friend WithEvents ScrollBarHorizontal As System.Windows.Forms.HScrollBar
    Friend WithEvents ScrollBarVertical As System.Windows.Forms.VScrollBar
    Friend WithEvents PanelControlHost As System.Windows.Forms.Panel

End Class
