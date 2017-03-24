<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StockTypeQuickView
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
        Me.PanelMain = New SyncroSim.Core.Forms.BasePanel()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.BorderColor = System.Drawing.Color.Gray
        Me.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelMain.Location = New System.Drawing.Point(4, 4)
        Me.PanelMain.Name = "PanelMain"
        Me.PanelMain.PaintBottomBorder = True
        Me.PanelMain.PaintLeftBorder = True
        Me.PanelMain.PaintRightBorder = True
        Me.PanelMain.PaintTopBorder = True
        Me.PanelMain.ShowBorder = True
        Me.PanelMain.Size = New System.Drawing.Size(466, 203)
        Me.PanelMain.TabIndex = 0
        '
        'StockTypeQuickView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PanelMain)
        Me.Name = "StockTypeQuickView"
        Me.Padding = New System.Windows.Forms.Padding(4)
        Me.Size = New System.Drawing.Size(474, 211)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelMain As SyncroSim.Core.Forms.BasePanel

End Class
