'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.Common.Forms
Imports System.Drawing
Imports System.Globalization

Class FlowPathwayDiagram
    Inherits BoxArrowDiagram

    Private m_DataFeed As DataFeed
    Private m_FlowDiagramSheet As DataSheet
    Private m_FlowDiagramData As DataTable
    Private m_FlowPathwaySheet As DataSheet
    Private m_FlowPathwayData As DataTable
    Private m_ShapeLookup As New Dictionary(Of Integer, StockTypeShape)
    Private m_IsFilterApplied As Boolean

    Public Sub New()

        Me.AutoScroll = False

        Me.Rows = DIAGRAM_MAX_ROWS
        Me.Columns = DIAGRAM_MAX_COLUMNS
        Me.CellPadding = DIAGRAM_SHAPE_PADDING
        Me.BoxSize = DIAGRAM_SHAPE_SIZE
        Me.LanesBetweenShapes = DIAGRAM_LANES_BETWEEN_SHAPES

    End Sub

    Public Sub Load(ByVal dataFeed As DataFeed)

        Debug.Assert(Me.m_DataFeed Is Nothing)

        Me.m_DataFeed = dataFeed

        Me.m_FlowDiagramSheet = dataFeed.Scenario.GetDataSheet(DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME)
        Me.m_FlowDiagramData = Me.m_FlowDiagramSheet.GetData()

        Me.m_FlowPathwaySheet = dataFeed.Scenario.GetDataSheet(DATASHEET_FLOW_PATHWAY_NAME)
        Me.m_FlowPathwayData = Me.m_FlowPathwaySheet.GetData()

        Me.RefreshDiagram()

    End Sub

    Public Sub RefreshDiagram()

        Dim Selected As New List(Of Integer)

        For Each s As StockTypeShape In Me.SelectedShapes
            Selected.Add(s.StockTypeId)
        Next

        Me.RefreshStockTypeShapes()
        Me.RefreshLocationLookups()
        Me.RefreshFlowPathwayLines()
        Me.ConfigureShapeReadOnlySetting()

        For Each id As Integer In Selected

            If (Me.m_ShapeLookup.ContainsKey(id)) Then
                Me.SelectStockTypeShape(id)
            End If

        Next

        Me.Invalidate()

    End Sub

    Public Sub FilterFlowPathways(ByVal criteria As DiagramFilterCriteria)

        Me.m_IsFilterApplied = IsFilterApplied(criteria)

        For Each l As FlowPathwayLine In Me.Lines
            l.IsVisible = criteria.FlowTypes(l.Pathway.FlowTypeId)
        Next

        Me.Invalidate()

    End Sub

    Public Function CanOpenStockTypes() As Boolean
        Return (Me.SelectedShapes.Count > 0 And Not Me.IsReadOnly)
    End Function

    Public Function CanCutStockTypes() As Boolean
        Return (Me.SelectedShapes.Count > 0 And Not Me.IsReadOnly)
    End Function

    Public Sub AddStockTypeShape(ByVal stockTypeId As Integer)

        Debug.Assert(Me.GetStockTypeShape(stockTypeId) Is Nothing)

        Dim Location As String = Me.GetNextLocation()

        If (Location Is Nothing) Then

            FormsUtilities.ErrorMessageBox(
                "There are no more available locations on the diagram.")

            Return

        End If

        Me.m_FlowDiagramSheet.BeginAddRows()
        Dim NewRow As DataRow = Me.m_FlowDiagramData.NewRow()

        NewRow(STOCK_TYPE_ID_COLUMN_NAME) = stockTypeId
        NewRow(LOCATION_COLUMN_NAME) = Location

        Me.m_FlowDiagramData.Rows.Add(NewRow)
        Me.m_FlowDiagramSheet.EndAddRows()

        Me.RefreshDiagram()
        Me.SelectStockTypeShape(stockTypeId)

    End Sub

    Public Sub SelectStockTypeShape(ByVal stockTypeId As Integer)

        If (Me.m_ShapeLookup.ContainsKey(stockTypeId)) Then

            Dim s As StockTypeShape = Me.m_ShapeLookup(stockTypeId)
            Me.SelectShape(s)

        End If

    End Sub

    Public Sub DeleteSelectedStockTypeShapes()

        Me.m_FlowDiagramSheet.BeginDeleteRows()
        Me.m_FlowPathwaySheet.BeginDeleteRows()

        For Each Shape As StockTypeShape In Me.SelectedShapes
            Me.DeleteStockTypeShape(Shape)
        Next

        Me.m_FlowDiagramSheet.EndDeleteRows()
        Me.m_FlowPathwaySheet.EndDeleteRows()

        Me.RefreshDiagram()

    End Sub

    Public Function GetStockTypeShape(ByVal stockTypeId As Integer) As StockTypeShape

#If DEBUG Then

        Dim q As String = String.Format(CultureInfo.InvariantCulture, "StockTypeID={0}", stockTypeId)
        Dim r() As DataRow = Me.m_FlowDiagramData.Select(q)
        Debug.Assert(r.Count = 0 Or r.Count = 1)

#End If

        If (Me.m_ShapeLookup.ContainsKey(stockTypeId)) Then
            Return Me.m_ShapeLookup(stockTypeId)
        Else
            Return Nothing
        End If

    End Function

    Public Sub ConfigureShapeReadOnlySetting()

        For Each Shape As StockTypeShape In Me.Shapes
            Shape.IsReadOnly = Me.IsReadOnly
        Next

        Me.Invalidate()

    End Sub

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)

        MyBase.OnPaint(e)

        If (Me.m_IsFilterApplied) Then
            e.Graphics.DrawImage(My.Resources.Filter16x16, New Point(4, 4))
        End If

    End Sub

    Protected Overrides Sub OnDropSelectedShapes(e As DiagramDragEventArgs)

        MyBase.OnDropSelectedShapes(e)

        For Each Shape As StockTypeShape In Me.SelectedShapes

            Debug.Assert(Me.WorkspaceRectangle.Contains(Shape.Bounds))

            Dim row As DataRow = Me.GetStockTypeRecord(Shape.StockTypeId)
            row(LOCATION_COLUMN_NAME) = RowColToLocation(Shape.Row, Shape.Column)

        Next

        Me.RefreshLocationLookups()
        Me.RefreshFlowPathwayLines()

        'DEVTODO: It would be better not to do this, but there is no obvious way to tell
        'if the user is about to drop the shapes they are dragging.  Is there?

        Me.m_FlowDiagramSheet.BeginModifyRows()
        Me.m_FlowDiagramSheet.EndModifyRows()

        Me.Focus()

    End Sub

    Private Sub RefreshStockTypeShapes()

        Me.RemoveAllShapes()

        Dim l As List(Of StockTypeShape) = Me.CreateStockTypeShapes()

        For Each s As StockTypeShape In l

            Dim rc As Rectangle = Me.GetShapeRectangleFromRowCol(s.Row, s.Column)

            s.SetDimensions(rc.Width, rc.Height)
            s.SetLocation(rc.X, rc.Y)
            s.CreateConnectorPoints()

            Me.FillOutgoingPathways(s)
            Me.AddShape(s)

        Next

    End Sub

    Private Function CreateStockTypeShapes() As List(Of StockTypeShape)

        Dim l As New List(Of StockTypeShape)
        Dim ds As DataSheet = Me.m_DataFeed.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)

        For Each dr As DataRow In Me.m_FlowDiagramData.Rows

            If (dr.RowState <> DataRowState.Deleted) Then

                Dim StockTypeId As Integer = CInt(dr(STOCK_TYPE_ID_COLUMN_NAME))
                Dim DisplayName As String = ds.ValidationTable.GetDisplayName(StockTypeId)
                Dim ShapeRow As Integer = -1
                Dim ShapeColumn As Integer = -1
                Dim Location As String = CStr(dr(LOCATION_COLUMN_NAME))

                LocationToRowCol(Location, ShapeRow, ShapeColumn)

                If (ShapeRow > DIAGRAM_MAX_ROWS Or ShapeColumn > DIAGRAM_MAX_COLUMNS) Then
                    Debug.Assert(False)
                    Continue For
                End If

                Dim NewShape As New StockTypeShape(StockTypeId, DisplayName)

                NewShape.Row = ShapeRow
                NewShape.Column = ShapeColumn

                l.Add(NewShape)

            End If

        Next

        Return l

    End Function

    Private Sub DeleteStockTypeShape(ByVal shape As StockTypeShape)

        Dim dr As DataRow = Me.GetStockTypeRecord(shape.StockTypeId)

        If (dr.RowState = DataRowState.Added) Then
            Me.m_FlowDiagramData.Rows.Remove(dr)
        Else
            dr.Delete()
        End If

    End Sub

    Private Sub FillOutgoingPathways(ByVal shape As StockTypeShape)

        Dim OutgoingRows() As DataRow = Me.GetOutgoingPathways(shape.StockTypeId)

        For Each dr As DataRow In OutgoingRows

            Debug.Assert(CInt(dr(FROM_STOCK_TYPE_ID_COLUMN_NAME)) = shape.StockTypeId)

            Dim p As FlowPathway = CreateFlowPathway(dr)
            shape.OutgoingFlowPathways.Add(p)

        Next

    End Sub

    Private Function GetOutgoingPathways(ByVal stockTypeId As Integer) As DataRow()

        Dim Query As String = String.Format(CultureInfo.InvariantCulture,
            "{0}={1}", FROM_STOCK_TYPE_ID_COLUMN_NAME, stockTypeId)

        Return Me.m_FlowPathwayData.Select(Query, Nothing)

    End Function

    Private Shared Function CreateFlowPathway(ByVal dr As DataRow) As FlowPathway

        Dim Iteration As Nullable(Of Integer) = Nothing
        Dim Timestep As Nullable(Of Integer) = Nothing
        Dim FromStratumId As Nullable(Of Integer) = Nothing
        Dim FromStateClassId As Nullable(Of Integer) = Nothing
        Dim FromMinimumAge As Nullable(Of Integer) = Nothing
        Dim FromStockTypeId As Integer
        Dim ToStratumId As Nullable(Of Integer) = Nothing
        Dim ToStateClassId As Nullable(Of Integer) = Nothing
        Dim ToMinimumAge As Nullable(Of Integer) = Nothing
        Dim ToStockTypeId As Integer
        Dim TransitionGroupId As Integer
        Dim StateAttributeTypeId As Nullable(Of Integer) = Nothing
        Dim FlowTypeId As Integer
        Dim Multiplier As Double

        If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
            Iteration = CType(dr(ITERATION_COLUMN_NAME), Integer)
        End If

        If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
            Timestep = CType(dr(TIMESTEP_COLUMN_NAME), Integer)
        End If

        If (dr(FROM_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            FromStratumId = CType(dr(FROM_STRATUM_ID_COLUMN_NAME), Integer)
        End If

        If (dr(FROM_STATECLASS_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            FromStateClassId = CType(dr(FROM_STATECLASS_ID_COLUMN_NAME), Integer)
        End If

        If (dr(FROM_MIN_AGE_COLUMN_NAME) IsNot DBNull.Value) Then
            FromMinimumAge = CInt(dr(FROM_MIN_AGE_COLUMN_NAME))
        End If

        FromStockTypeId = CInt(dr(FROM_STOCK_TYPE_ID_COLUMN_NAME))

        If (dr(TO_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            ToStratumId = CType(dr(TO_STRATUM_ID_COLUMN_NAME), Integer)
        End If

        If (dr(TO_STATECLASS_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            ToStateClassId = CType(dr(TO_STATECLASS_ID_COLUMN_NAME), Integer)
        End If

        If (dr(TO_MIN_AGE_COLUMN_NAME) IsNot DBNull.Value) Then
            ToMinimumAge = CInt(dr(TO_MIN_AGE_COLUMN_NAME))
        End If

        ToStockTypeId = CInt(dr(TO_STOCK_TYPE_ID_COLUMN_NAME))

        If (dr(TRANSITION_GROUP_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            TransitionGroupId = CType(dr(TRANSITION_GROUP_ID_COLUMN_NAME), Integer)
        Else
            TransitionGroupId = 0
        End If

        If (dr(STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            StateAttributeTypeId = CType(dr(STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME), Integer)
        End If

        FlowTypeId = CInt(dr(FLOW_TYPE_ID_COLUMN_NAME))
        Multiplier = CDbl(dr(MULTIPLIER_COLUMN_NAME))

        Dim p As New FlowPathway(
            Iteration,
            Timestep,
            FromStratumId,
            FromStateClassId,
            FromMinimumAge,
            FromStockTypeId,
            ToStratumId,
            ToStateClassId,
            ToMinimumAge,
            ToStockTypeId,
            TransitionGroupId,
            StateAttributeTypeId,
            FlowTypeId,
            Multiplier)

        Return p

    End Function

    Private Sub RefreshLocationLookups()

        Me.m_ShapeLookup.Clear()

        For Each s As StockTypeShape In Me.Shapes
            Me.m_ShapeLookup.Add(s.StockTypeId, s)
        Next

        Debug.Assert(Me.m_ShapeLookup.Count = Me.Shapes.Count)

    End Sub

    Private Shared Function IsFilterApplied(ByVal criteria As DiagramFilterCriteria) As Boolean

        For Each b As Boolean In criteria.FlowTypes.Values

            If (Not b) Then
                Return True
            End If

        Next

        Return False

    End Function

    Private Function GetStockTypeRecord(ByVal stockTypeId As Integer) As DataRow

        For Each dr As DataRow In Me.m_FlowDiagramData.Rows

            If (dr.RowState <> DataRowState.Deleted) Then

                Dim id As Integer = CInt(dr(STOCK_TYPE_ID_COLUMN_NAME))

                If (id = stockTypeId) Then

                    Debug.Assert(Me.GetStockTypeShape(stockTypeId) IsNot Nothing)
                    Return dr

                End If

            End If

        Next

        Debug.Assert(Me.GetStockTypeShape(stockTypeId) IsNot Nothing)
        Return Nothing

    End Function

    Private Sub RefreshFlowPathwayLines()

        Me.ClearLines()

        If (Me.Shapes.Count = 0) Then
            Return
        End If

        For Each Shape As StockTypeShape In Me.Shapes
            Shape.ResetConnectorPoints()
        Next

        Dim AlreadyAdded As New Dictionary(Of String, FlowPathwayLine)

        For Each FromShape As StockTypeShape In Me.Shapes

            For Each Pathway As FlowPathway In FromShape.OutgoingFlowPathways

                Dim l As FlowPathwayLine = Nothing
                Dim ToShape As StockTypeShape = Me.m_ShapeLookup(Pathway.ToStockTypeId)

                Dim Lookup As String = String.Format(
                    CultureInfo.InvariantCulture, "k1{0}-k2{1}", FromShape.StockTypeId, Pathway.ToStockTypeId)

                If (AlreadyAdded.ContainsKey(Lookup)) Then

                    Dim ExistingLine As FlowPathwayLine = AlreadyAdded(Lookup)

                    l = New FlowPathwayLine(DIAGRAM_FLOW_PATHWAY_LINE_COLOR, Pathway)
                    l.RenderPath = ExistingLine.CloneRenderPath()

                Else

                    If (FromShape Is ToShape) Then
                        l = CreatePathwayLineToSelf(FromShape, Pathway)
                    Else

                        l = New FlowPathwayLine(DIAGRAM_FLOW_PATHWAY_LINE_COLOR, Pathway)
                        Me.FillLineSegments(FromShape, ToShape, l, BoxArrowDiagramConnectorMode.Vertical)

                    End If

                    AlreadyAdded.Add(Lookup, l)

                End If

                Me.AddLine(l)

            Next

        Next

    End Sub

    Private Shared Function CreatePathwayLineToSelf(ByVal shape As StockTypeShape, ByVal pathway As FlowPathway) As FlowPathwayLine

        Dim l As New FlowPathwayLine(DIAGRAM_FLOW_PATHWAY_LINE_COLOR, pathway)

        Const PT_CIRCLE_RADIUS As Integer = 10

        Dim lrx As Integer = shape.Bounds.X + shape.Bounds.Width - PT_CIRCLE_RADIUS
        Dim lry As Integer = shape.Bounds.Y + shape.Bounds.Height - PT_CIRCLE_RADIUS
        Dim rc As New Rectangle(lrx, lry, 2 * PT_CIRCLE_RADIUS, 2 * PT_CIRCLE_RADIUS)

        l.AddEllipse(rc)

        Return l

    End Function

    Private Function GetNextLocation() As String

        If (Me.GetShapeAt(Me.CurrentMouseRow, Me.CurrentMouseColumn) Is Nothing) Then

            Dim ColLetter As String = CStr(Chr(Asc("A") + Me.CurrentMouseColumn))
            Dim RowLetter As String = CStr(Me.CurrentMouseRow + 1)

            Return ColLetter & RowLetter

        End If

        For col As Integer = 0 To DIAGRAM_MAX_COLUMNS - 1

            For row As Integer = 0 To DIAGRAM_MAX_ROWS - 1

                If (Me.GetShapeAt(row, col) Is Nothing) Then

                    Dim ColLetter As String = CStr(Chr(Asc("A") + col))
                    Dim RowLetter As String = CStr(row + 1)

                    Return (ColLetter & RowLetter)

                End If

            Next

        Next

        Return Nothing

    End Function

    Private Shared Sub LocationToRowCol(ByVal location As String, ByRef row As Integer, ByRef column As Integer)

        Dim LocUpper As String = location.ToUpper(CultureInfo.InvariantCulture)

        Dim CharPart As String = Mid(LocUpper, 1, 1)
        Dim NumPart As String = Mid(LocUpper, 2, LocUpper.Length - 1)

        Dim chars() As Char = CharPart.ToCharArray()
        Dim c As Char = chars(0)
        Dim CharVal As Integer = (Asc(c) - Asc("A"))

        column = CharVal
        row = CInt(NumPart) - 1

        Debug.Assert(column >= 0 And row >= 0)

    End Sub

    Private Shared Function RowColToLocation(ByVal row As Integer, ByVal column As Integer) As String

        Debug.Assert(column < 26)

        Dim s As String = CStr(ChrW(Asc("A") + column))
        s = s & (row + 1).ToString(CultureInfo.InvariantCulture)

        Return s

    End Function

End Class
