'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class InitialStocksDataFeedView

    Private m_RasterFilesView As DataFeedView
    Private m_RasterFilesDataGrid As DataGridView
    Private Delegate Sub DelegateNoArgs()
    Private m_ColumnsInitialized As Boolean
    Private m_IsEnabled As Boolean = True

    Const BROWSE_BUTTON_TEXT As String = "..."
    Const FILE_NAME_COLUMN_INDEX As Integer = 2
    Const BROWSE_COLUMN_INDEX As Integer = 3

    Protected Overrides Sub InitializeView()

        MyBase.InitializeView()

        Dim v1 As DataFeedView = Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)
        Me.PanelNonSpatial.Controls.Add(v1)

        Me.m_RasterFilesView = Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)
        Me.PanelSpatial.Controls.Add(Me.m_RasterFilesView)
        Me.m_RasterFilesDataGrid = CType(Me.m_RasterFilesView, MultiRowDataFeedView).GridControl

        Me.ConfigureContextMenu()

    End Sub

    Public Overrides Sub LoadDataFeed(dataFeed As Core.DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Dim v1 As DataFeedView = CType(Me.PanelNonSpatial.Controls(0), DataFeedView)
        v1.LoadDataFeed(dataFeed, DATASHEET_INITIAL_STOCK_NON_SPATIAL)

        Dim v2 As DataFeedView = CType(Me.PanelSpatial.Controls(0), DataFeedView)
        v2.LoadDataFeed(dataFeed, DATASHEET_INITIAL_STOCK_SPATIAL)

        If (Not Me.m_ColumnsInitialized) Then

            'Add handlers
            AddHandler Me.m_RasterFilesDataGrid.CellEnter, AddressOf Me.OnGridCellEnter
            AddHandler Me.m_RasterFilesDataGrid.CellMouseDown, AddressOf Me.OnGridCellMouseDown
            AddHandler Me.m_RasterFilesDataGrid.DataBindingComplete, AddressOf Me.OnGridBindingComplete
            AddHandler Me.m_RasterFilesDataGrid.KeyDown, AddressOf Me.OnGridKeyDown

            'Configure columns
            Me.m_RasterFilesDataGrid.Columns(FILE_NAME_COLUMN_INDEX).DefaultCellStyle.BackColor = Color.LightGray

            'Add the browse button column
            Dim BrowseColumn As New DataGridViewButtonColumn()

            BrowseColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            BrowseColumn.Width = 40
            BrowseColumn.MinimumWidth = 40

            Me.m_RasterFilesDataGrid.Columns.Add(BrowseColumn)
            Me.m_ColumnsInitialized = True

        End If

    End Sub

    ''' <summary>
    ''' Overrides EnableView
    ''' </summary>
    ''' <param name="enable"></param>
    ''' <remarks>
    ''' We override this so that we can manually enable the nested data feed view.  If we don't do this
    ''' then the user will not be abled to interact with it at all if it is disabled and this is not really
    ''' what we want here.  Also, we want to have control over the enabled state of the buttons.
    ''' </remarks>
    Public Overrides Sub EnableView(enable As Boolean)

        If (Me.PanelNonSpatial.Controls.Count > 0) Then

            Dim v As DataFeedView = CType(Me.PanelNonSpatial.Controls(0), DataFeedView)
            v.EnableView(enable)

        End If

        If (Me.PanelSpatial.Controls.Count > 0) Then

            Dim v As DataFeedView = CType(Me.PanelSpatial.Controls(0), DataFeedView)
            v.EnableView(enable)

        End If

        Me.LabelNonSpatial.Enabled = enable
        Me.LabelSpatial.Enabled = enable

        Me.m_IsEnabled = enable

    End Sub

    ''' <summary>
    ''' Handles the cell enter event for the grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnGridCellEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs)

        If (e.ColumnIndex = FILE_NAME_COLUMN_INDEX) Then
            Me.Session.MainForm.BeginInvoke(New DelegateNoArgs(AddressOf Me.OnNewCellEnterAsync), Nothing)
        End If

    End Sub

    ''' <summary>
    ''' Handles the CellMouseDown event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnGridCellMouseDown(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)

        If (e.RowIndex >= 0) Then

            If (e.ColumnIndex = BROWSE_COLUMN_INDEX) Then
                Me.GetMultiplierFile(e.RowIndex)
            End If

        End If

    End Sub

    ''' <summary>
    ''' Handles the grid's KeyDown event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnGridKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)

        If (Me.m_RasterFilesDataGrid.CurrentCell IsNot Nothing) Then

            If (Me.m_RasterFilesDataGrid.CurrentCell.ColumnIndex = BROWSE_COLUMN_INDEX) Then

                If (e.KeyValue = Keys.Enter) Then

                    Me.GetMultiplierFile(Me.m_RasterFilesDataGrid.CurrentCell.RowIndex)
                    e.Handled = True

                End If

            End If

        End If

    End Sub

    ''' <summary>
    ''' Handles the grid's DataBindingComplete event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnGridBindingComplete(sender As System.Object, e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs)

        For Each dgr As DataGridViewRow In Me.m_RasterFilesDataGrid.Rows
            dgr.Cells(BROWSE_COLUMN_INDEX).Value = BROWSE_BUTTON_TEXT
        Next

    End Sub

    ''' <summary>
    ''' Handles the CellEnter event
    ''' </summary>
    ''' <param name="argument"></param>
    ''' <remarks></remarks>
    Private Sub OnNewCellEnterAsync()

        Dim Row As Integer = Me.m_RasterFilesDataGrid.CurrentCell.RowIndex
        Dim Col As Integer = Me.m_RasterFilesDataGrid.CurrentCell.ColumnIndex

        If (Col = FILE_NAME_COLUMN_INDEX) Then

            If (ModifierKeys = Keys.Shift) Then
                Col -= 1
            Else
                Col += 1
            End If

            Me.m_RasterFilesDataGrid.CurrentCell = Me.m_RasterFilesDataGrid.Rows(Row).Cells(Col)
            Me.m_RasterFilesDataGrid.BeginEdit(True)

        End If

    End Sub

    ''' <summary>
    ''' Gets a multiplier file from the user
    ''' </summary>
    ''' <param name="rowIndex"></param>
    ''' <remarks></remarks>
    Private Sub GetMultiplierFile(ByVal rowIndex As Integer)

        If (Not Me.m_IsEnabled) Then
            Return
        End If

        Dim ds As DataSheet = Me.Scenario.GetDataSheet(DATASHEET_INITIAL_STOCK_SPATIAL)
        Dim RasterFile As String = ChooseRasterFileName("Raster File")

        If (RasterFile Is Nothing) Then
            Return
        End If

        Dim OldMode As DataGridViewEditMode = Me.m_RasterFilesDataGrid.EditMode
        Me.m_RasterFilesDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically

        Me.m_RasterFilesDataGrid.CurrentCell = Me.m_RasterFilesDataGrid.Rows(rowIndex).Cells(FILE_NAME_COLUMN_INDEX)
        Me.m_RasterFilesDataGrid.Rows(rowIndex).Cells(RASTER_FILE_COLUMN_NAME).Value = Path.GetFileName(RasterFile)
        Me.m_RasterFilesDataGrid.NotifyCurrentCellDirty(True)

        Me.m_RasterFilesDataGrid.BeginEdit(False)
        Me.m_RasterFilesDataGrid.EndEdit()

        Me.m_RasterFilesDataGrid.CurrentCell = Me.m_RasterFilesDataGrid.Rows(rowIndex).Cells(BROWSE_COLUMN_INDEX)

        ds.AddExternalInputFile(RasterFile)

        Me.m_RasterFilesDataGrid.EditMode = OldMode

    End Sub

    ''' <summary>
    ''' Configures the context menu for this view
    ''' </summary>
    ''' <remarks>We only want a subset of the default commands, so remove the others</remarks>
    Private Sub ConfigureContextMenu()

        For i As Integer = Me.m_RasterFilesView.Commands.Count - 1 To 0 Step -1

            Dim c As Command = Me.m_RasterFilesView.Commands(i)

            If (c.Name <> "ssim_delete" And
                c.Name <> "ssim_delete_all" And
                c.Name <> "ssim_import" And
                c.Name <> "ssim_export_all") Then

                If (Not c.IsSeparator) Then
                    Me.m_RasterFilesView.Commands.RemoveAt(i)
                End If

            End If

            If (c.Name = "ssim_export_all") Then
                c.DisplayName = "Export..."
            End If

        Next

        Me.m_RasterFilesView.RefreshContextMenuStrip()

    End Sub

End Class
