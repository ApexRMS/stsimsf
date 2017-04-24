'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Globalization
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class StockTypeQuickView

    Private m_DataFeed As DataFeed
    Private m_StockTypeIds As List(Of Integer)
    Private m_FlowDiagramSheet As DataSheet
    Private m_FlowDiagramData As DataTable
    Private m_FlowPathwayView As MultiRowDataFeedView
    Private m_FlowPathwayGrid As BaseDataGridView
    Private m_FlowPathwaySheet As DataSheet
    Private m_FlowPathwayData As DataTable
    Private m_ShowFlowsFrom As Boolean = True
    Private m_ShowFlowsTo As Boolean
    Private m_FromStratumVisible As Boolean
    Private m_FromStateClassVisible As Boolean
    Private m_FromMinAgeVisible As Boolean
    Private m_ToStratumVisible As Boolean
    Private m_ToStateClassVisible As Boolean
    Private m_ToMinAgeVisible As Boolean
    Private m_TransitionGroupVisible As Boolean
    Private m_StateAttributeTypeVisible As Boolean

    Public Sub LoadStockTypes(
        ByVal dataFeed As DataFeed,
        ByVal stockTypeIds As List(Of Integer))

        Me.m_DataFeed = dataFeed
        Me.m_StockTypeIds = stockTypeIds

        Dim sess As WinFormSession = CType(Me.Project.Library.Session, WinFormSession)

        Me.m_FlowDiagramSheet = Me.m_DataFeed.Scenario.GetDataSheet(DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME)
        Me.m_FlowDiagramData = Me.m_FlowDiagramSheet.GetData()

        Me.m_FlowPathwaySheet = Me.m_DataFeed.GetDataSheet(DATASHEET_FLOW_PATHWAY_NAME)
        Me.m_FlowPathwayData = Me.m_FlowPathwaySheet.GetData()

        Me.m_FlowPathwayView = CType(sess.CreateMultiRowDataFeedView(dataFeed.Scenario, dataFeed.Scenario), MultiRowDataFeedView)
        Me.m_FlowPathwayView.LoadDataFeed(Me.m_DataFeed, DATASHEET_FLOW_PATHWAY_NAME)

        Me.m_FlowPathwayGrid = Me.m_FlowPathwayView.GridControl()

        Me.FilterFlowPathways()
        Me.ConfigureContextMenus()
        Me.InitializeColumnVisiblity()
        Me.UpdateColumnVisibility()
        Me.ConfigureColumnsReadOnly()

        Me.PanelMain.Controls.Add(Me.m_FlowPathwayView)
        Me.m_FlowPathwayGrid.PaintGridBorders = False
        Me.m_FlowPathwayView.ManageOptionalColumns = False

        AddHandler Me.m_FlowPathwayGrid.CellBeginEdit, AddressOf OnGridCellBeginEdit
        AddHandler Me.m_FlowPathwayGrid.CellEndEdit, AddressOf OnGridCellEndEdit

    End Sub

    Private Sub FilterFlowPathways()

        Dim filter As String = Me.CreateGridFilterString()
        CType(Me.m_FlowPathwayGrid.DataSource, BindingSource).Filter = filter

    End Sub

    Private Function CreateGridFilterString() As String

        Dim Filter As String = CreateIntegerFilterSpec(Me.m_StockTypeIds)

        Dim FromFormatString As String = "FromStockTypeID IN ({0})"
        Dim ToFormatString As String = "ToStockTypeID IN ({0})"

        If (Me.m_ShowFlowsFrom) Then
            Return String.Format(CultureInfo.InvariantCulture, FromFormatString, Filter)
        Else
            Debug.Assert(Me.m_ShowFlowsTo)
            Return String.Format(CultureInfo.InvariantCulture, ToFormatString, Filter)
        End If

    End Function

    Private Sub InitializeColumnVisiblity()

        Me.m_FromStratumVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, FROM_STRATUM_ID_COLUMN_NAME)
        Me.m_FromStateClassVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, FROM_STATECLASS_ID_COLUMN_NAME)
        Me.m_FromMinAgeVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, FROM_MIN_AGE_COLUMN_NAME)
        Me.m_ToStratumVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, TO_STRATUM_ID_COLUMN_NAME)
        Me.m_ToStateClassVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, TO_STATECLASS_ID_COLUMN_NAME)
        Me.m_ToMinAgeVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, TO_MIN_AGE_COLUMN_NAME)
        Me.m_TransitionGroupVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, TRANSITION_GROUP_ID_COLUMN_NAME)
        Me.m_StateAttributeTypeVisible = DataTableUtilities.TableHasData(Me.m_FlowPathwayData, STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME)

    End Sub

    Private Sub UpdateColumnVisibility()

        If (Me.m_FlowPathwayGrid.CurrentCell IsNot Nothing) Then

            Dim ResetCurrentCell As Boolean = False
            Dim CurrentColumnIndex As Integer = Me.m_FlowPathwayGrid.CurrentCell.ColumnIndex
            Dim CurrentRowIndex As Integer = Me.m_FlowPathwayGrid.CurrentCell.RowIndex
            Dim CurrentColumnName As String = Me.m_FlowPathwayGrid.Columns(CurrentColumnIndex).Name

            If (CurrentColumnName = FROM_STRATUM_ID_COLUMN_NAME And (Not Me.m_FromStratumVisible)) Then
                ResetCurrentCell = True
            End If

            If (ResetCurrentCell) Then

                Me.m_FlowPathwayGrid.CurrentCell =
                    Me.m_FlowPathwayGrid.Rows(CurrentRowIndex).Cells(FROM_STOCK_TYPE_ID_COLUMN_NAME)

            End If

        End If

        Me.m_FlowPathwayGrid.Columns(FROM_STRATUM_ID_COLUMN_NAME).Visible = Me.m_FromStratumVisible
        Me.m_FlowPathwayGrid.Columns(FROM_STATECLASS_ID_COLUMN_NAME).Visible = Me.m_FromStateClassVisible
        Me.m_FlowPathwayGrid.Columns(FROM_MIN_AGE_COLUMN_NAME).Visible = Me.m_FromMinAgeVisible
        Me.m_FlowPathwayGrid.Columns(TO_STRATUM_ID_COLUMN_NAME).Visible = Me.m_ToStratumVisible
        Me.m_FlowPathwayGrid.Columns(TO_STATECLASS_ID_COLUMN_NAME).Visible = Me.m_ToStateClassVisible
        Me.m_FlowPathwayGrid.Columns(TO_MIN_AGE_COLUMN_NAME).Visible = Me.m_ToMinAgeVisible
        Me.m_FlowPathwayGrid.Columns(TRANSITION_GROUP_ID_COLUMN_NAME).Visible = Me.m_TransitionGroupVisible
        Me.m_FlowPathwayGrid.Columns(STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME).Visible = Me.m_StateAttributeTypeVisible

    End Sub

    Private Sub ConfigureContextMenus()

        For i As Integer = Me.m_FlowPathwayView.Commands.Count - 1 To 0 Step -1

            Dim c As Command = Me.m_FlowPathwayView.Commands(i)

            If (c.Name = "ssim_delete_all" Or c.Name = "ssim_import" Or
                c.Name = "ssim_export" Or c.Name = "ssim_export_all") Then

                Me.m_FlowPathwayView.Commands.RemoveAt(i)

            End If

        Next

        Dim StratumLabel As String = Me.GetStratumLabelTerminology()
        Dim FromStratum As String = String.Format(CultureInfo.CurrentCulture, "From {0}", StratumLabel)
        Dim ToStratum As String = String.Format(CultureInfo.CurrentCulture, "To {0}", StratumLabel)

        Me.m_FlowPathwayView.Commands.Add(New Command("Flows To", AddressOf OnExecuteFlowsToCommand, AddressOf OnUpdateFlowsToCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("Flows From", AddressOf OnExecuteFlowsFromCommand, AddressOf OnUpdateFlowsFromCommand))
        Me.m_FlowPathwayView.Commands.Add(Command.CreateSeparatorCommand())
        Me.m_FlowPathwayView.Commands.Add(New Command(FromStratum, AddressOf OnExecuteFromStratumCommand, AddressOf OnUpdateFromStratumCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("From State Class", AddressOf OnExecuteFromStateClassCommand, AddressOf OnUpdateFromStateClassCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("From Min Age", AddressOf OnExecuteFromMinAgeCommand, AddressOf OnUpdateFromMinAgeCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command(ToStratum, AddressOf OnExecuteToStratumCommand, AddressOf OnUpdateToStratumCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("To State Class", AddressOf OnExecuteToStateClassCommand, AddressOf OnUpdateToStateClassCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("To Min Age", AddressOf OnExecuteToMinAgeCommand, AddressOf OnUpdateToMinAgeCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("Transition Group", AddressOf OnExecuteTransitionGroupCommand, AddressOf OnUpdateTransitionGroupCommand))
        Me.m_FlowPathwayView.Commands.Add(New Command("State Attribute Type", AddressOf OnExecuteStateAttributeTypeCommand, AddressOf OnUpdateStateAttributeTypeCommand))

        Me.m_FlowPathwayView.RefreshContextMenuStrip()

        For i As Integer = Me.m_FlowPathwayGrid.ContextMenuStrip.Items.Count - 1 To 0 Step -1

            Dim item As ToolStripItem = Me.m_FlowPathwayGrid.ContextMenuStrip.Items(i)

            If (item.Name = "ssim_optional_column_separator" Or
                item.Name = "ssim_optional_column_item") Then

                Me.m_FlowPathwayGrid.ContextMenuStrip.Items.RemoveAt(i)

            End If

        Next

    End Sub

    Private Sub OnExecuteFlowsToCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_ShowFlowsFrom = False
        Me.m_ShowFlowsTo = True

        Me.FilterFlowPathways()
        Me.ConfigureColumnsReadOnly()
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateFlowsToCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_ShowFlowsTo

    End Sub

    Private Sub OnExecuteFlowsFromCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_ShowFlowsFrom = True
        Me.m_ShowFlowsTo = False

        Me.FilterFlowPathways()
        Me.ConfigureColumnsReadOnly()
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateFlowsFromCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_ShowFlowsFrom

    End Sub

    Private Sub OnExecuteFromStratumCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_FromStratumVisible = (Not Me.m_FromStratumVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateFromStratumCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_FromStratumVisible

    End Sub

    Private Sub OnExecuteFromStateClassCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_FromStateClassVisible = (Not Me.m_FromStateClassVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateFromStateClassCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_FromStateClassVisible

    End Sub

    Private Sub OnExecuteFromMinAgeCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_FromMinAgeVisible = (Not Me.m_FromMinAgeVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateFromMinAgeCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_FromMinAgeVisible

    End Sub

    Private Sub OnExecuteToStratumCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_ToStratumVisible = (Not Me.m_ToStratumVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateToStratumCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_ToStratumVisible

    End Sub

    Private Sub OnExecuteToStateClassCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_ToStateClassVisible = (Not Me.m_ToStateClassVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateToStateClassCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_ToStateClassVisible

    End Sub

    Private Sub OnExecuteToMinAgeCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_ToMinAgeVisible = (Not Me.m_ToMinAgeVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateToMinAgeCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_ToMinAgeVisible

    End Sub

    Private Sub OnExecuteTransitionGroupCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_TransitionGroupVisible = (Not Me.m_TransitionGroupVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateTransitionGroupCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_TransitionGroupVisible

    End Sub

    Private Sub OnExecuteStateAttributeTypeCommand(ByVal cmd As Command)

        If (Not Me.Validate()) Then
            Return
        End If

        Me.m_StateAttributeTypeVisible = (Not Me.m_StateAttributeTypeVisible)
        Me.UpdateColumnVisibility()

    End Sub

    Private Sub OnUpdateStateAttributeTypeCommand(ByVal cmd As Command)

        cmd.IsEnabled = True
        cmd.IsChecked = Me.m_StateAttributeTypeVisible

    End Sub

    Private Sub OnGridCellBeginEdit(ByVal sender As Object, ByVal e As DataGridViewCellCancelEventArgs)

        If (e.ColumnIndex = Me.m_FlowPathwayGrid.Columns(FROM_STOCK_TYPE_ID_COLUMN_NAME).Index) Then

            Me.FilterStockTypeCombo(
                FROM_STOCK_TYPE_ID_COLUMN_NAME,
                e.RowIndex,
                True)

        ElseIf (e.ColumnIndex = Me.m_FlowPathwayGrid.Columns(TO_STOCK_TYPE_ID_COLUMN_NAME).Index) Then

            Me.FilterStockTypeCombo(
                TO_STOCK_TYPE_ID_COLUMN_NAME,
                e.RowIndex,
                False)

        End If

    End Sub

    Private Sub OnGridCellEndEdit(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs)

        If (e.ColumnIndex = Me.m_FlowPathwayGrid.Columns(FROM_STOCK_TYPE_ID_COLUMN_NAME).Index) Then

            Me.ResetComboFilter(
                FROM_STOCK_TYPE_ID_COLUMN_NAME,
                e.RowIndex)

        ElseIf (e.ColumnIndex = Me.m_FlowPathwayGrid.Columns(TO_STOCK_TYPE_ID_COLUMN_NAME).Index) Then

            Me.ResetComboFilter(
                TO_STOCK_TYPE_ID_COLUMN_NAME,
                e.RowIndex)

        End If

    End Sub

    Private Sub ResetComboFilter(ByVal columnName As String, ByVal rowIndex As Integer)

        Dim dgv As DataGridViewRow = Me.m_FlowPathwayGrid.Rows(rowIndex)
        Dim DestStratumCell As DataGridViewComboBoxCell = CType(dgv.Cells(columnName), DataGridViewComboBoxCell)
        Dim DestStratumColumn As DataGridViewComboBoxColumn = CType(Me.m_FlowPathwayGrid.Columns(columnName), DataGridViewComboBoxColumn)

        DestStratumCell.DataSource = DestStratumColumn.DataSource
        DestStratumCell.ValueMember = DestStratumColumn.ValueMember
        DestStratumCell.DisplayMember = DestStratumColumn.DisplayMember

    End Sub

    Private Sub FilterStockTypeCombo(
        ByVal columnName As String,
        ByVal rowIndex As Integer,
        ByVal selectedTypesOnly As Boolean)

        Dim ds As DataSheet = Me.m_DataFeed.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim dgr As DataGridViewRow = Me.m_FlowPathwayGrid.Rows(rowIndex)
        Dim cell As DataGridViewComboBoxCell = CType(dgr.Cells(columnName), DataGridViewComboBoxCell)
        Dim filter As String

        If (selectedTypesOnly) Then
            filter = Me.CreateFromStockTypeFilter()
        Else
            filter = Me.CreateToStockTypeFilter()
        End If

        Dim dv As New DataView(ds.GetData(), filter, ds.DisplayMember, DataViewRowState.CurrentRows)

        cell.DataSource = dv
        cell.ValueMember = "StockTypeID"
        cell.DisplayMember = "Name"

    End Sub

    Private Function CreateFromStockTypeFilter() As String

        Dim spec As String = CreateIntegerFilterSpec(Me.m_StockTypeIds)
        Return String.Format(CultureInfo.InvariantCulture, "StockTypeID IN ({0})", spec)

    End Function

    Private Function CreateToStockTypeFilter() As String

        Dim lst As New List(Of Integer)

        For Each dr As DataRow In Me.m_FlowDiagramData.Rows

            If (dr.RowState <> DataRowState.Deleted) Then

                Dim Id As Integer = CInt(dr(STOCK_TYPE_ID_COLUMN_NAME))

                If (Not lst.Contains(Id)) Then
                    lst.Add(Id)
                End If

            End If

        Next

        If (lst.Count = 0) Then
            Return "StockTypeID=-1"
        Else

            Dim filter As String = CreateIntegerFilterSpec(lst)
            Return String.Format(CultureInfo.InvariantCulture, "StockTypeID IN ({0})", filter)

        End If

    End Function

    Private Shared Function CreateIntegerFilterSpec(ByVal ids As List(Of Integer)) As String

        Dim sb As New StringBuilder()

        For Each i As Integer In ids

            sb.Append(i.ToString(CultureInfo.InvariantCulture))
            sb.Append(",")

        Next

        Return sb.ToString.TrimEnd(CChar(","))

    End Function

    Private Function GetStratumLabelTerminology() As String

        Dim l As String = "Stratum"
        Dim dr As DataRow = Me.m_DataFeed.Project.GetDataSheet("STSim_Terminology").GetDataRow()

        If (dr IsNot Nothing) Then

            If (dr("PrimaryStratumLabel") IsNot DBNull.Value) Then
                l = CStr(dr("PrimaryStratumLabel"))
            End If

        End If

        Return l

    End Function

    Private Sub SetColumnReadOnly(ByVal columnName As String)

        Dim col As DataGridViewColumn = Me.m_FlowPathwayGrid.Columns(columnName)
        col.DefaultCellStyle.BackColor = Color.LightGray
        col.ReadOnly = True

    End Sub

    Private Sub ConfigureColumnsReadOnly()

        Debug.Assert(Not (Me.m_ShowFlowsTo And Me.m_ShowFlowsFrom))

        For Each c As DataGridViewColumn In Me.m_FlowPathwayGrid.Columns

            c.DefaultCellStyle.BackColor = Color.White
            c.ReadOnly = False

        Next

        If (Me.m_ShowFlowsTo) Then

            SetColumnReadOnly(TO_STOCK_TYPE_ID_COLUMN_NAME)
            SetColumnReadOnly(FROM_STOCK_TYPE_ID_COLUMN_NAME)

        End If

    End Sub

End Class
