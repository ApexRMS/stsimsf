'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Globalization
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports SyncroSim.Common.Forms
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class FlowPathwayDataFeedView

    Private m_IsEnabled As Boolean
    Private m_TooltipZoomOut As New ToolTip
    Private m_TooltipZoomIn As New ToolTip
    Private m_DiagramTab As New FlowPathwayTabStripItem("Diagram")
    Private m_StockTab As New FlowPathwayTabStripItem("Stocks")
    Private m_FlowTab As New FlowPathwayTabStripItem("Flows")
    Private m_FilterCriteria As New DiagramFilterCriteria()

    Protected Overrides Sub InitializeView()

        MyBase.InitializeView()

        Me.InitializeToolTips()
        Me.InitializeCommands()

        Me.TabStripMain.Items.Add(Me.m_DiagramTab)
        Me.TabStripMain.Items.Add(Me.m_StockTab)
        Me.TabStripMain.Items.Add(Me.m_FlowTab)

        Me.Padding = New Padding(1)
        Me.SplitContainerTabStrip.SplitterWidth = 8

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)

        If (disposing And Not Me.IsDisposed) Then

            Me.m_TooltipZoomOut.Dispose()
            Me.m_TooltipZoomIn.Dispose()

            Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()

            If (d IsNot Nothing) Then

                RemoveHandler d.ZoomChanged, AddressOf OnDiagramZoomChanged
                RemoveHandler d.MouseDoubleClick, AddressOf OnDiagramMouseDoubleClick

            End If

            If components IsNot Nothing Then
                components.Dispose()
            End If

            Me.m_DiagramTab.Dispose()
            Me.m_StockTab.Dispose()
            Me.m_FlowTab.Dispose()

        End If

        MyBase.Dispose(disposing)

    End Sub

    Public Overrides Sub LoadDataFeed(dataFeed As DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Me.TabStripMain.SelectItem(0)
        Me.SyncronizeFilterCriteria()

    End Sub

    Public Overrides Sub EnableView(enable As Boolean)

        Me.m_IsEnabled = enable

        Dim d As FlowPathwayDiagram = CType(Me.m_DiagramTab.Control, FlowPathwayDiagram)

        If (d IsNot Nothing) Then

            d.IsReadOnly = (Not Me.m_IsEnabled)
            d.ConfigureShapeReadOnlySetting()

        End If

    End Sub

    Private Function GetFlowDiagram() As FlowPathwayDiagram
        Return CType(Me.m_DiagramTab.Control, FlowPathwayDiagram)
    End Function

    Private Sub InitializeCommands()

        Dim CmdOpen As New Command("stock_flow_open_stock_types", "Open", Nothing, AddressOf OnExecuteOpenCommand, AddressOf OnUpdateOpenCommand)
        CmdOpen.IsBold = True
        Me.Commands.Add(CmdOpen)

        Me.Commands.Add(Command.CreateSeparatorCommand())
        Me.Commands.Add(New Command("ssim_delete", "Delete", My.Resources.Delete16x16, AddressOf OnExecuteDeleteCommand, AddressOf OnUpdateDeleteCommand))
        Me.Commands.Add(Command.CreateSeparatorCommand())
        Me.Commands.Add(New Command("ssim_select_all", "Select All", AddressOf OnExecuteSelectAllCommand, AddressOf OnUpdateSelectAllCommand))
        Me.Commands.Add(Command.CreateSeparatorCommand())
        Me.Commands.Add(New Command("stock_flow_show_grid", "Show Grid", AddressOf OnExecuteShowGridCommand, AddressOf OnUpdateShowGridCommand))
        Me.Commands.Add(New Command("stock_flow_show_tooltips", "Show Tooltips", AddressOf OnExecuteShowTooltipsCommand, AddressOf OnUpdateShowTooltipsCommand))
        Me.Commands.Add(Command.CreateSeparatorCommand())
        Me.Commands.Add(New Command("stock_flow_add_stock", "Add Stock...", AddressOf OnExecuteAddStockCommand, AddressOf OnUpdateAddStockCommand))
        Me.Commands.Add(Command.CreateSeparatorCommand())
        Me.Commands.Add(New Command("stock_flow_filter_flow_types", "Filter Flow Types...", My.Resources.Filter16x16, AddressOf OnExecuteFilterFlowsCommand, AddressOf OnUpdateFilterFlowsCommand))

    End Sub

    Protected Overrides Sub OnRowsAdded(sender As Object, e As DataSheetRowEventArgs)

        MyBase.OnRowsAdded(sender, e)

        Me.SyncronizeFilterCriteria(sender)
        Me.GetFlowDiagram.RefreshDiagram()

    End Sub

    Protected Overrides Sub OnRowsDeleted(sender As Object, e As DataSheetRowEventArgs)

        MyBase.OnRowsDeleted(sender, e)

        Me.SyncronizeFilterCriteria(sender)
        Me.GetFlowDiagram.RefreshDiagram()

    End Sub

    Protected Overrides Sub OnRowsModified(sender As Object, e As DataSheetRowEventArgs)

        MyBase.OnRowsModified(sender, e)
        Me.GetFlowDiagram.RefreshDiagram()

    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)

        MyBase.OnResize(e)
        Me.ResetScrollbars()

    End Sub

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)

        MyBase.OnPaint(e)

        Dim p As Pen = Pens.Silver

        e.Graphics.DrawLine(p, 0, 0, Me.Bounds.Width - 1, 0)
        e.Graphics.DrawLine(p, Me.Bounds.Width - 1, 0, Me.Bounds.Width - 1, Me.Bounds.Height - 1)
        e.Graphics.DrawLine(p, Me.Bounds.Width - 1, Me.Bounds.Height - 1, 0, Me.Bounds.Height - 1)
        e.Graphics.DrawLine(p, 0, Me.Bounds.Height - 1, 0, 0)

    End Sub

    Private Sub OnPaintSplitContainer(
        ByVal sender As System.Object,
        ByVal e As System.Windows.Forms.PaintEventArgs) Handles SplitContainerTabStrip.Paint

        Dim rc As New Rectangle(
            Me.SplitContainerTabStrip.SplitterRectangle.Left,
            Me.SplitContainerTabStrip.SplitterRectangle.Top,
            Me.SplitContainerTabStrip.SplitterRectangle.Width - 1,
            Me.SplitContainerTabStrip.SplitterRectangle.Height - 1)

        Dim brs As New System.Drawing.Drawing2D.LinearGradientBrush(
            rc, Color.SteelBlue, Color.White, Drawing2D.LinearGradientMode.Horizontal)

        e.Graphics.FillRectangle(brs, rc)
        e.Graphics.DrawRectangle(Pens.SteelBlue, rc)
        rc.Inflate(-1, -1)
        e.Graphics.DrawRectangle(Pens.White, rc)

        brs.Dispose()

    End Sub

    Private Sub OnSelectedTabItemChanging(
        ByVal sender As System.Object,
        ByVal e As SelectedTabStripItemChangingEventArgs) Handles TabStripMain.SelectedItemChanging

        If ((Me.TabStripMain.SelectedItem Is Nothing) Or
            (Me.PanelControlHost.Controls.Count = 0)) Then

            Return

        End If

        If (Me.TabStripMain.SelectedItem IsNot Me.m_DiagramTab) Then

            Dim v As SyncroSimView = CType(Me.PanelControlHost.Controls(0), SyncroSimView)

            If (Not v.Validate()) Then
                e.Cancel = True
            End If

        End If

    End Sub

    Private Sub OnSelectedTabItemChanged(
        ByVal sender As System.Object,
        ByVal e As SelectedTabStripItemChangedEventArgs) Handles TabStripMain.SelectedItemChanged

        Using h As New HourGlass

            If (Me.TabStripMain.SelectedItem Is Me.m_DiagramTab) Then
                Me.SwitchToDiagramView()
            Else
                Me.SwitchToGridView()
            End If

        End Using

    End Sub

    Private Sub OnVerticalScroll(
        ByVal sender As System.Object,
        ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ScrollBarVertical.Scroll

        If (e.NewValue = e.OldValue) Then
            Return
        End If

        If (Me.TabStripMain.SelectedItem Is Me.m_DiagramTab) Then

            Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()

            d.VerticalScrollValue = e.NewValue
            d.Invalidate()

        End If

    End Sub

    Private Sub OnHorizontalScroll(
        ByVal sender As System.Object,
        ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ScrollBarHorizontal.Scroll

        If (e.NewValue = e.OldValue) Then
            Return
        End If

        If (Me.TabStripMain.SelectedItem Is Me.m_DiagramTab) Then

            Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()

            d.HorizontalScrollValue = e.NewValue
            d.Invalidate()

        End If

    End Sub

    Private Sub OnDiagramMouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs)

        If (Me.GetFlowDiagram().CanOpenStockTypes()) Then
            Me.OpenSelectedStockTypes()
        End If

    End Sub

    Private Sub OnDiagramZoomChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.ResetScrollbars()
    End Sub

    Private Sub ZoomIn(sender As System.Object, e As System.EventArgs) Handles ButtonZoomIn.Click
        Me.GetFlowDiagram.ZoomIn()
    End Sub

    Private Sub ZoomOut(sender As System.Object, e As System.EventArgs) Handles ButtonZoomOut.Click
        Me.GetFlowDiagram.ZoomOut()
    End Sub

    Private Sub OnExecuteOpenCommand(ByVal cmd As Command)
        Me.OpenSelectedStockTypes()
    End Sub

    Private Sub OnUpdateOpenCommand(ByVal cmd As Command)
        cmd.IsEnabled = Me.GetFlowDiagram.CanOpenStockTypes()
    End Sub

    Private Sub OnExecuteDeleteCommand(ByVal cmd As Command)

        If (FormsUtilities.ApplicationMessageBox(
            "Are you sure you want to delete the selected stock types and associated flows?",
            MessageBoxButtons.YesNo) <> DialogResult.Yes) Then

            Return

        End If

        Using h As New HourGlass
            Me.GetFlowDiagram.DeleteSelectedStockTypeShapes()
        End Using

    End Sub

    Private Sub OnUpdateDeleteCommand(ByVal cmd As Command)
        cmd.IsEnabled = Me.GetFlowDiagram.CanCutStockTypes()
    End Sub

    Private Sub OnExecuteSelectAllCommand(ByVal cmd As Command)
        Me.GetFlowDiagram.SelectAllShapes()
    End Sub

    Private Sub OnUpdateSelectAllCommand(ByVal cmd As Command)
        cmd.IsEnabled = (Me.m_IsEnabled And Me.GetFlowDiagram.Shapes.Count > 0)
    End Sub

    Private Sub OnExecuteShowGridCommand(ByVal cmd As Command)

        Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()
        d.ShowGrid = (Not d.ShowGrid)

    End Sub

    Private Sub OnUpdateShowGridCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = (Me.GetFlowDiagram.ShowGrid)

    End Sub

    Private Sub OnExecuteShowTooltipsCommand(ByVal cmd As Command)

        Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()
        d.ShowToolTips = (Not d.ShowToolTips)

    End Sub

    Private Sub OnUpdateShowTooltipsCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = (Me.GetFlowDiagram.ShowToolTips)

    End Sub

    Private Sub OnExecuteAddStockCommand(ByVal cmd As Command)

        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim dv As New DataView(ds.GetData(), Nothing, ds.DisplayMember, DataViewRowState.CurrentRows)

        If (dv.Count = 0) Then

            FormsUtilities.InformationMessageBox("No stock types have been defined.")
            Return

        End If

        Dim d As FlowPathwayDiagram = GetFlowDiagram()
        Dim f As New ChooseStockTypeForm()

        f.Initialize(d, Me.Project)

        If (f.ShowDialog(Me) = DialogResult.OK) Then

            Using h As New HourGlass
                d.AddStockTypeShape(f.StockTypeId)
            End Using

        End If

    End Sub

    Private Sub OnUpdateAddStockCommand(ByVal cmd As Command)
        cmd.IsEnabled = Me.m_IsEnabled
    End Sub

    Private Sub OnExecuteFilterFlowsCommand(ByVal cmd As Command)

        Dim dlg As New FilterFlowTypesForm()
        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_FLOW_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        dlg.CheckBoxPanelMain.Initialize()
        dlg.CheckBoxPanelMain.BeginAddItems()

        For Each dr As DataRow In dt.Rows

            If (dr.RowState <> DataRowState.Deleted) Then

                Dim Id As Integer = CInt(dr(ds.ValueMember))
                Dim Name As String = CStr(dr(ds.DisplayMember))
                Dim IsSelected As Boolean = Me.m_FilterCriteria.FlowTypes(Id)

                dlg.CheckBoxPanelMain.AddItem(IsSelected, Id, Name)

            End If

        Next

        dlg.CheckBoxPanelMain.EndAddItems()
        dlg.CheckBoxPanelMain.TitleBarText = "Flow Types"

        If (dlg.ShowDialog(Me) <> DialogResult.OK) Then
            Return
        End If

        For Each dr As DataRow In dlg.CheckBoxPanelMain.DataSource.Rows
            Me.m_FilterCriteria.FlowTypes(CInt(dr("ItemID"))) = CBool(dr("IsSelected"))
        Next

        Me.GetFlowDiagram().FilterFlowPathways(Me.m_FilterCriteria)

    End Sub

    Private Sub OnUpdateFilterFlowsCommand(ByVal cmd As Command)
        cmd.IsEnabled = (Me.GetFlowDiagram().Shapes.Count > 0)
    End Sub

    Private Sub InitializeToolTips()

        Me.m_TooltipZoomOut.SetToolTip(Me.ButtonZoomOut, "Zoom Out")
        Me.m_TooltipZoomIn.SetToolTip(Me.ButtonZoomIn, "Zoom In")

    End Sub

    Private Sub ResetScrollbars()

        If (Me.TabStripMain.SelectedItem Is Nothing) Then
            Return
        End If

        If (Me.TabStripMain.SelectedItem IsNot Me.m_DiagramTab) Then
            Return
        End If

        If (Me.m_DiagramTab.Control Is Nothing) Then
            Return
        End If

        Me.ResetHorizontalScrollbar()
        Me.ResetVerticalScrollbar()

    End Sub

    Private Sub ResetHorizontalScrollbar()

        Dim d As BoxArrowDiagram = Me.GetFlowDiagram

        Dim zoom As Single = d.Zoom
        Dim diagwid As Single = d.WorkspaceRectangle.Width * zoom
        Dim clientwid As Single = d.ClientSize.Width
        Dim Extra As Integer = CInt(30 * zoom)

        If (clientwid >= diagwid) Then

            Me.ScrollBarHorizontal.Enabled = False
            Me.ScrollBarHorizontal.Value = 0
            d.HorizontalScrollValue = 0

        Else

            Me.ScrollBarHorizontal.Enabled = True

            Me.ScrollBarHorizontal.Minimum = 0
            Me.ScrollBarHorizontal.Maximum = CInt(diagwid - clientwid + Extra)
            Me.ScrollBarHorizontal.SmallChange = CInt(d.CellSize * zoom)
            Me.ScrollBarHorizontal.LargeChange = CInt(d.CellSize * zoom)

            If (Me.ScrollBarHorizontal.SmallChange = 0) Then
                Me.ScrollBarHorizontal.SmallChange = 1
            End If

            If (Me.ScrollBarHorizontal.LargeChange = 0) Then
                Me.ScrollBarHorizontal.LargeChange = 1
            End If

            Me.ScrollBarHorizontal.Maximum += CInt(Me.ScrollBarHorizontal.LargeChange)

            If (d.HorizontalScrollValue <= ScrollBarHorizontal.Maximum) Then
                d.HorizontalScrollValue = d.HorizontalScrollValue
            End If

        End If

    End Sub

    Private Sub ResetVerticalScrollbar()

        Dim d As BoxArrowDiagram = Me.GetFlowDiagram()
        Dim zoom As Single = d.Zoom
        Dim diaghgt As Single = d.WorkspaceRectangle.Height * zoom
        Dim clienthgt As Single = d.ClientSize.Height
        Dim Extra As Integer = CInt(30 * zoom)

        If (clienthgt >= diaghgt) Then

            Me.ScrollBarVertical.Enabled = False
            Me.ScrollBarVertical.Value = 0
            d.VerticalScrollValue = 0

        Else

            Me.ScrollBarVertical.Enabled = True

            Me.ScrollBarVertical.Minimum = 0
            Me.ScrollBarVertical.Maximum = CInt(diaghgt - clienthgt + Extra)
            Me.ScrollBarVertical.SmallChange = CInt(d.CellSize * zoom)
            Me.ScrollBarVertical.LargeChange = CInt(d.CellSize * zoom)

            If (Me.ScrollBarVertical.SmallChange = 0) Then
                Me.ScrollBarVertical.SmallChange = 1
            End If

            If (Me.ScrollBarVertical.LargeChange = 0) Then
                Me.ScrollBarVertical.LargeChange = 1
            End If

            Me.ScrollBarVertical.Maximum += CInt(Me.ScrollBarVertical.LargeChange)

            If (d.VerticalScrollValue <= ScrollBarVertical.Maximum) Then
                d.VerticalScrollValue = d.VerticalScrollValue
            End If

        End If

    End Sub

    Private Sub OpenSelectedStockTypes()

        Dim lst As New List(Of Integer)
        Dim title As String = Me.CreateQuickViewTitle()
        Dim tag As String = Me.CreateQuickViewTag()
        Dim sess As WinFormSession = CType(Me.Project.Library.Session, WinFormSession)

        If (sess.Application.GetView(tag) IsNot Nothing) Then
            sess.Application.ActivateView(tag)
        Else

            Using h As New HourGlass

                Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()

                For Each s As StockTypeShape In d.SelectedShapes
                    lst.Add(s.StockTypeId)
                Next

                Dim v As StockTypeQuickView = CType(Me.Session.CreateDataFeedView(
                    GetType(StockTypeQuickView), Me.Library, Me.Project, Me.Scenario, Nothing), StockTypeQuickView)

                v.LoadStockTypes(Me.DataFeed, lst)
                sess.Application.HostView(v, title, tag)

            End Using

        End If

    End Sub

    Private Function CreateQuickViewTitle() As String

        Dim sb As New StringBuilder()
        Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()
        Dim ds As DataSheet = Me.DataFeed.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)

        sb.AppendFormat(CultureInfo.CurrentCulture, "{0} - ", Me.DataFeed.Scenario.DisplayName)

        For Each s As StockTypeShape In d.SelectedShapes

            sb.AppendFormat(CultureInfo.CurrentCulture, "{0},",
                CStr(DataTableUtilities.GetTableValue(ds.GetData(), ds.ValueMember, s.StockTypeId, ds.DisplayMember)))

        Next

        Return sb.ToString().Trim(CChar(","))

    End Function

    Private Function CreateQuickViewTag() As String

        Dim sb As New StringBuilder()
        Dim lst As New List(Of Integer)
        Dim d As FlowPathwayDiagram = Me.GetFlowDiagram()
        Dim ds As DataSheet = Me.DataFeed.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)

        For Each s As StockTypeShape In d.SelectedShapes
            lst.Add(s.StockTypeId)
        Next

        lst.Sort()

        For Each i As Integer In lst

            For Each s As StockTypeShape In d.SelectedShapes

                If (s.StockTypeId = i) Then

                    Dim DisplayValue As String = CStr(DataTableUtilities.GetTableValue(
                        ds.GetData(), ds.ValueMember, s.StockTypeId, ds.DisplayMember))

                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0}-{1}:", s.StockTypeId, DisplayValue)

                End If

            Next

        Next

        Dim k1 As String = sb.ToString().Trim(CChar(":"))

        Return String.Format(CultureInfo.InvariantCulture,
            "{0}:{1}", Me.Project.Library.Connection.ConnectionString, k1)

    End Function

    Private Sub SyncronizeFilterCriteria(ByVal sender As Object)

        Dim ds As DataSheet = CType(sender, DataSheet)

        If (ds.Name = DATASHEET_FLOW_TYPE_NAME Or
            ds.Name = DATASHEET_FLOW_PATHWAY_NAME) Then

            Me.SyncronizeFilterCriteria()

        End If

    End Sub

    Private Sub SyncronizeFilterCriteria()

        Dim cr As New DiagramFilterCriteria()
        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_FLOW_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        For Each dr As DataRow In dt.Rows

            If (dr.RowState <> DataRowState.Deleted) Then
                cr.FlowTypes.Add(CInt(dr(ds.ValueMember)), True)
            End If

        Next

        For Each tg As Integer In Me.m_FilterCriteria.FlowTypes.Keys

            If (cr.FlowTypes.ContainsKey(tg)) Then
                cr.FlowTypes(tg) = Me.m_FilterCriteria.FlowTypes(tg)
            End If

        Next

        Me.m_FilterCriteria = cr

    End Sub

    Private Sub SwitchToDiagramView()

        Dim d As FlowPathwayDiagram = Nothing

        If (Me.m_DiagramTab.Control Is Nothing) Then

            d = New FlowPathwayDiagram()

            d.Load(Me.DataFeed)
            d.IsReadOnly = (Not Me.m_IsEnabled)
            d.ConfigureShapeReadOnlySetting()

            AddHandler d.ZoomChanged, AddressOf Me.OnDiagramZoomChanged
            AddHandler d.MouseDoubleClick, AddressOf Me.OnDiagramMouseDoubleClick

            Me.m_DiagramTab.Control = d

        Else
            d = Me.GetFlowDiagram()
        End If

        Me.ButtonZoomIn.Enabled = True
        Me.ButtonZoomOut.Enabled = True
        Me.ScrollBarVertical.Visible = True
        Me.ScrollBarHorizontal.Enabled = True
        Me.ScrollBarVertical.Value = d.VerticalScrollValue
        Me.ScrollBarHorizontal.Value = d.HorizontalScrollValue
        Me.PanelControlHost.Width = Me.PanelBottomControls.Width - Me.ScrollBarVertical.Width - 2

        Me.ResetScrollbars()
        Me.SetCurrentControl()

    End Sub

    Private Sub SwitchToGridView()

        If (Me.TabStripMain.SelectedItem Is Me.m_StockTab) Then

            If (Me.m_StockTab.Control Is Nothing) Then

                Dim v As MultiRowDataFeedView =
                    Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)

                v.ShowBorder = False
                v.LoadDataFeed(DataFeed, DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME)
                v.EnableView(Me.m_IsEnabled)

                Me.m_StockTab.Control = v

            End If

        Else

            If (Me.m_FlowTab.Control Is Nothing) Then

                Dim v As MultiRowDataFeedView =
                    Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)

                v.ShowBorder = False
                v.LoadDataFeed(DataFeed, DATASHEET_FLOW_PATHWAY_NAME)
                v.EnableView(Me.m_IsEnabled)

                Me.m_FlowTab.Control = v

            End If

        End If

        Me.ButtonZoomIn.Enabled = False
        Me.ButtonZoomOut.Enabled = False
        Me.PanelControlHost.Width = Me.PanelBottomControls.Width
        Me.ScrollBarVertical.Visible = False
        Me.ScrollBarHorizontal.Enabled = False

        Me.SetCurrentControl()

    End Sub

    Private Sub SetCurrentControl()

        Dim c As Control = CType(Me.TabStripMain.SelectedItem, FlowPathwayTabStripItem).Control

        Me.PanelControlHost.Controls.Clear()
        Me.PanelControlHost.Controls.Add(c)

        c.Dock = DockStyle.Fill
        c.Parent = Me.PanelControlHost

    End Sub

End Class
