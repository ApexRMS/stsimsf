'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim
Imports SyncroSim.Common
Imports SyncroSim.StochasticTime
Imports System.Globalization

Partial Class StockFlowTransformer

    Private m_OutputStockTable As DataTable
    Private m_CreateSummaryStockOutput As Boolean
    Private m_SummaryStockOutputTimesteps As Integer
    Private m_SummaryOutputStockRecords As New OutputStockCollection
    Private m_OutputFlowTable As DataTable
    Private m_CreateSummaryFlowOutput As Boolean
    Private m_SummaryFlowOutputTimesteps As Integer
    Private m_SummaryOutputFlowRecords As New OutputFlowCollection
    Private m_SpatialOutputFlowDict As Dictionary(Of Integer, Double())
    Private m_CreateSpatialStockOutput As Boolean
    Private m_SpatialStockOutputTimesteps As Integer
    Private m_CreateSpatialFlowOutput As Boolean
    Private m_SpatialFlowOutputTimesteps As Integer

    ''' <summary>
    ''' Gets the output flow dictionary
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' We must lazy-load this dictionary because this transformer runs before ST-Sim's
    ''' and so the cell data is not there yet.
    ''' </remarks>
    Private Function GetOutputFlowDictionary() As Dictionary(Of Integer, Double())

        If (Me.m_SpatialOutputFlowDict Is Nothing) Then

            Me.m_SpatialOutputFlowDict = New Dictionary(Of Integer, Double())

            For Each ft As FlowType In Me.m_FlowTypes.Values

                Dim flowVals As Double()
                ReDim flowVals(Me.STSimTransformer.InputRasters.NumberCells - 1)

                Me.m_SpatialOutputFlowDict.Add(ft.Id, flowVals)

            Next

        End If

        Return Me.m_SpatialOutputFlowDict

    End Function

    ''' <summary>
    ''' Initializes the output data tables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeOutputDataTables()

        Debug.Assert(Me.m_OutputStockTable Is Nothing)
        Debug.Assert(Me.m_OutputFlowTable Is Nothing)

        Me.m_OutputStockTable = Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_STOCK_NAME).GetData()
        Me.m_OutputFlowTable = Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_FLOW_NAME).GetData()

        Debug.Assert(Me.m_OutputStockTable.Rows.Count = 0)
        Debug.Assert(Me.m_OutputFlowTable.Rows.Count = 0)

    End Sub

    ''' <summary>
    ''' Processes the current stock summary data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessStockSummaryData(ByVal iteration As Integer, ByVal timestep As Integer)

        For Each r As OutputStock In Me.m_SummaryOutputStockRecords

            Dim dr As DataRow = Me.m_OutputStockTable.NewRow

            dr(ITERATION_COLUMN_NAME) = iteration
            dr(TIMESTEP_COLUMN_NAME) = timestep
            dr(STRATUM_ID_COLUMN_NAME) = r.StratumId
            dr(SECONDARY_STRATUM_ID_COLUMN_NAME) = DataTableUtilities.GetNullableDatabaseValue(r.SecondaryStratumId)
            dr(STATECLASS_ID_COLUMN_NAME) = r.StateClassId
            dr(STOCK_TYPE_ID_COLUMN_NAME) = r.StockTypeId
            dr(AMOUNT_COLUMN_NAME) = r.Amount

            Me.m_OutputStockTable.Rows.Add(dr)

        Next

        Me.m_SummaryOutputStockRecords.Clear()

    End Sub

    ''' <summary>
    ''' Processes the current flow summary data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessFlowSummaryData(ByVal iteration As Integer, ByVal timestep As Integer)

        For Each r As OutputFlow In Me.m_SummaryOutputFlowRecords

            Dim dr As DataRow = Me.m_OutputFlowTable.NewRow

            dr(ITERATION_COLUMN_NAME) = iteration
            dr(TIMESTEP_COLUMN_NAME) = timestep
            dr(FROM_STRATUM_ID_COLUMN_NAME) = r.FromStratumId
            dr(FROM_SECONDARY_STRATUM_ID_COLUMN_NAME) = DataTableUtilities.GetNullableDatabaseValue(r.FromSecondaryStratumId)
            dr(FROM_STATECLASS_ID_COLUMN_NAME) = r.FromStateClassId
            dr(FROM_STOCK_TYPE_ID_COLUMN_NAME) = r.FromStockTypeId
            dr(TRANSITION_TYPE_ID_COLUMN_NAME) = DataTableUtilities.GetNullableDatabaseValue(r.TransitionTypeId)
            dr(TO_STRATUM_ID_COLUMN_NAME) = r.ToStratumId
            dr(TO_STATECLASS_ID_COLUMN_NAME) = r.ToStateClassId
            dr(TO_STOCK_TYPE_ID_COLUMN_NAME) = r.ToStockTypeId
            dr(FLOW_TYPE_ID_COLUMN_NAME) = r.FlowTypeId
            dr(AMOUNT_COLUMN_NAME) = r.Amount

            Me.m_OutputFlowTable.Rows.Add(dr)

        Next

        Me.m_SummaryOutputFlowRecords.Clear()

    End Sub

    ''' <summary>
    ''' Processes the Spatial Stock data. Create a raster file as a snapshot of the current Stock values.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessStockSpatialData(iteration As Integer, timestep As Integer)

        Debug.Assert(Me.m_IsSpatial)

        For Each s As StockType In Me.m_StockTypes

            Dim rastOutput As New StochasticTimeRaster
            ' Fetch the raster metadata from the InpRasters object
            Me.STSimTransformer.InputRasters.GetMetadata(rastOutput)
            rastOutput.InitDblCells()

            GetStockValues(s.Id, rastOutput)
            RasterFiles.SaveOutputRaster(rastOutput, Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_SPATIAL_STOCK_TYPE),RasterDataType.DTDouble,  iteration, timestep, SPATIAL_MAP_STOCK_TYPE_VARIABLE_PREFIX,s.Id,DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN)

        Next

    End Sub

    ''' <summary>
    ''' Get Stock Values for the specified Stock Type ID, placing then into the DblCells() in the specified raster.
    ''' </summary>
    ''' <param name="stockTypeId">The Stock Type ID that we want values for</param>
    ''' <param name="rastStockType">An object of type ApexRaster, where we will write the Stock Type values. The raster should be initialized with metadata and appropriate
    ''' array sizing.</param>
    ''' <remarks></remarks>
    Private Sub GetStockValues(stockTypeId As Integer, rastStockType As StochasticTimeRaster)

        Dim AmountPerCell As Double = Me.m_STSimTransformer.AmountPerCell

        For Each c As Cell In Me.STSimTransformer.Cells

            Dim StockAmounts As Dictionary(Of Integer, Double) = GetStockAmountDictionary(c)

            If StockAmounts.Count > 0 Then
                rastStockType.DblCells(c.CellId) = (StockAmounts(stockTypeId) / AmountPerCell)
            Else
                'I wouldnt expect to get here because of Stratum/StateClass test above
                Debug.Assert(False)
            End If

        Next

    End Sub

    ''' <summary>
    ''' Processes the Spatial Stock Group data. Create a raster file as a snapshot of the Stock Group values.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessStockGroupSpatialData(iteration As Integer, timestep As Integer)

        Using store As DataStore = Me.Library.CreateDataStore()
            Debug.Assert(Me.m_IsSpatial)

            ' Loop thru the Stock Groups
            Dim dsGrp As DataSheet = Me.Project.GetDataSheet(DATASHEET_STOCK_GROUP_NAME)
            For Each dr As DataRow In dsGrp.GetData.Rows

                Dim sgId = CInt(dr(dsGrp.ValidationTable.ValueMember))
                Dim sgName = dr(dsGrp.ValidationTable.DisplayMember)

                Dim rastOutput As New StochasticTimeRaster
                ' Fetch the raster metadata from the InpRasters object
                Me.STSimTransformer.InputRasters.GetMetadata(rastOutput)
                rastOutput.InitDblCells()

                ' Fetch the Stock Group Multipler table, and add the Stock Type values together, applying the multiplier
                Dim query As String = String.Format(CultureInfo.InvariantCulture,
                "select * from {0} where ScenarioId = {1} and {2}={3}", DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME, ResultScenario.Id, STOCK_GROUP_ID_COLUMN_NAME, sgId)
                Dim dtGrpTypes As DataTable = store.CreateDataTableFromQuery(query, "GrpTypeMultData")
                If dtGrpTypes.Rows.Count > 0 Then

                    For Each drGrpType As DataRow In dtGrpTypes.Rows
                        ' Find the Stock Types and associated multiplier that apply. Interpret and empty value (Null) as 1.0
                        Dim amt As Double
                        If IsDBNull(drGrpType("Value")) Then
                            amt = 1.0
                        Else
                            amt = CDbl(drGrpType("Value"))
                        End If
                        Dim stockTypeId As Integer = CInt(drGrpType(STOCK_TYPE_ID_COLUMN_NAME))
                        Debug.Print(String.Format(CultureInfo.CurrentCulture, "Group Name {0}, Group ID:{3}, Type:{1}, Amount:{2}", sgName, stockTypeId, amt, sgId))
                        Dim rastStockType As New StochasticTimeRaster
                        ' Fetch the raster metadata from the InpRasters object
                        Me.STSimTransformer.InputRasters.GetMetadata(rastStockType)
                        rastStockType.InitDblCells()
                        GetStockValues(stockTypeId, rastStockType)
                        rastStockType.ScaleDbl(amt)
                        rastOutput.AddDbl(rastStockType)
                    Next

                    RasterFiles.SaveOutputRaster(rastOutput, Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_SPATIAL_STOCK_GROUP),RasterDataType.DTDouble, iteration, timestep, SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX, sgId,DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN)
                End If
            Next
        End Using

    End Sub


    ''' <summary>
    ''' Processes the current flow spatial data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessFlowSpatialData(iteration As Integer, timestep As Integer)

        Debug.Assert(Me.m_IsSpatial)

        Dim rastOutput As New StochasticTimeRaster
        ' Fetch the raster metadata from the InpRasters object
        Me.STSimTransformer.InputRasters.GetMetadata(rastOutput)

        For Each flowType As FlowType In Me.m_FlowTypes.Values

            rastOutput.DblCells = GetOutputFlowDictionary()(flowType.Id)
            RasterFiles.SaveOutputRaster(rastOutput, Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_SPATIAL_FLOW_TYPE), RasterDataType.DTDouble, iteration, timestep, SPATIAL_MAP_FLOW_TYPE_VARIABLE_PREFIX, flowType.Id,DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN)

        Next


    End Sub

    ''' <summary>
    ''' Processes the current flow group spatial data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessFlowGroupSpatialData(iteration As Integer, timestep As Integer)

        Using store As DataStore = Me.Library.CreateDataStore()
            Debug.Assert(Me.m_IsSpatial)

            ' Loop thru the Flow Groups
            Dim dsGrp As DataSheet = Me.Project.GetDataSheet(DATASHEET_FLOW_GROUP_NAME)
            For Each dr As DataRow In dsGrp.GetData.Rows

                Dim fgId = CInt(dr(dsGrp.ValidationTable.ValueMember))
                Dim fgName = dr(dsGrp.ValidationTable.DisplayMember)

                Dim rastOutput As New StochasticTimeRaster
                ' Fetch the raster metadata from the InpRasters object
                Me.STSimTransformer.InputRasters.GetMetadata(rastOutput)
                rastOutput.InitDblCells()

                ' Loop through the Flow Group Multipler table, and add the Flow Type values together, applying the multiplier
                Dim query As String = String.Format(CultureInfo.InvariantCulture,
                "select * from {0} where scenarioId = {1} and {2}={3}", DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME, ResultScenario.Id, FLOW_GROUP_ID_COLUMN_NAME, fgId)
                Dim dtGrpTypes As DataTable = store.CreateDataTableFromQuery(query, "GrpTypeMultData")
                If dtGrpTypes.Rows.Count > 0 Then
                    For Each drGrpType As DataRow In dtGrpTypes.Rows
                        ' Find the Flow Types and associated multiplier that apply. Interpret and empty value (Null) as 1.0
                        Dim amt As Double
                        If IsDBNull(drGrpType("Value")) Then
                            amt = 1.0
                        Else
                            amt = CDbl(drGrpType("Value"))
                        End If

                        Dim flowTypeId As Integer = CInt(drGrpType(FLOW_TYPE_ID_COLUMN_NAME))
                        Debug.Print(String.Format(CultureInfo.CurrentCulture, "Group Name {0}, Group ID:{3}, Type:{1}, Amount:{2}", fgName, flowTypeId, amt, fgId))
                        Dim rastFlowType As New StochasticTimeRaster
                        ' Fetch the raster metadata from the InpRasters object
                        Me.STSimTransformer.InputRasters.GetMetadata(rastFlowType)
                        rastFlowType.InitDblCells()

                        rastFlowType.DblCells = CType(GetOutputFlowDictionary()(flowTypeId).Clone, Double())
                        rastFlowType.ScaleDbl(amt)
                        rastOutput.AddDbl(rastFlowType)
                    Next

                    RasterFiles.SaveOutputRaster(rastOutput, Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_SPATIAL_FLOW_GROUP),RasterDataType.DTDouble, iteration, timestep,SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX,  fgId,DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN)
                End If
            Next
        End Using



    End Sub


    ''' <summary>
    ''' Adds to the stock summary result collection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnSummaryStockOutput()

        For Each c As Cell In Me.STSimTransformer.Cells

            Dim StockAmounts As Dictionary(Of Integer, Double) = GetStockAmountDictionary(c)

            For Each id As Integer In StockAmounts.Keys

                Dim amount As Double = StockAmounts(id)
                Dim k As New FourIntegerLookupKey(c.StratumId, GetNullableKey(c.SecondaryStratumId), c.StateClassId, id)

                If (Me.m_SummaryOutputStockRecords.Contains(k)) Then

                    Dim r As OutputStock = Me.m_SummaryOutputStockRecords(k)
                    r.Amount += amount

                Else

                    Dim r As New OutputStock(c.StratumId, c.SecondaryStratumId, c.StateClassId, id, amount)
                    Me.m_SummaryOutputStockRecords.Add(r)

                End If

            Next

        Next

    End Sub

    ''' <summary>
    ''' Adds to the flow summary result collection
    ''' </summary>
    ''' <param name="timestep"></param>
    ''' <param name="cell"></param>
    ''' <param name="stockTypeId"></param>
    ''' <param name="flowTypeId"></param>
    ''' <param name="flowAmount"></param>
    ''' <param name="transitionPathway"></param>
    ''' <param name="flowPathway"></param>
    ''' <remarks></remarks>
    Private Sub OnSummaryFlowOutput(
        ByVal timestep As Integer,
        ByVal cell As Cell,
        ByVal deterministicPathway As DeterministicTransition,
        ByVal probabilisticPathway As Transition,
        ByVal flowPathway As FlowPathway,
        ByVal flowAmount As Double)

        Dim TransitionTypeId As Nullable(Of Integer) = Nothing
        Dim StratumIdDest As Integer = cell.StratumId
        Dim StateClassIdDest As Integer = cell.StateClassId

        If (probabilisticPathway IsNot Nothing) Then

            TransitionTypeId = probabilisticPathway.TransitionTypeId

            If (probabilisticPathway.StratumIdDestination.HasValue) Then
                StratumIdDest = probabilisticPathway.StratumIdDestination.Value
            End If

            If (probabilisticPathway.StateClassIdDestination.HasValue) Then
                StateClassIdDest = probabilisticPathway.StateClassIdDestination.Value
            End If

        Else

            If (deterministicPathway IsNot Nothing) Then

                If (deterministicPathway.StratumIdDestination.HasValue) Then
                    StratumIdDest = deterministicPathway.StratumIdDestination.Value
                End If

                If (deterministicPathway.StateClassIdDestination.HasValue) Then
                    StateClassIdDest = deterministicPathway.StateClassIdDestination.Value
                End If

            End If

        End If

        If (Me.m_STSimTransformer.IsOutputTimestep(timestep, Me.m_SummaryFlowOutputTimesteps, Me.m_CreateSummaryFlowOutput)) Then

            Dim k As New NineIntegerLookupKey(
                cell.StratumId,
                GetNullableKey(cell.SecondaryStratumId),
                cell.StateClassId,
                flowPathway.FromStockTypeId,
                GetNullableKey(TransitionTypeId),
                StratumIdDest,
                StateClassIdDest,
                flowPathway.ToStockTypeId,
                flowPathway.FlowTypeId)

            If (Me.m_SummaryOutputFlowRecords.Contains(k)) Then

                Dim r As OutputFlow = Me.m_SummaryOutputFlowRecords(k)
                r.Amount += flowAmount

            Else

                Dim r As New OutputFlow(
                    cell.StratumId,
                    cell.SecondaryStratumId,
                    cell.StateClassId,
                    flowPathway.FromStockTypeId,
                    TransitionTypeId,
                    StratumIdDest,
                    StateClassIdDest,
                    flowPathway.ToStockTypeId,
                    flowPathway.FlowTypeId,
                    flowAmount)

                Me.m_SummaryOutputFlowRecords.Add(r)

            End If

        End If

    End Sub

    ''' <summary>
    ''' Adds to the flow summary result collection
    ''' </summary>
    ''' <param name="timestep"></param>
    ''' <param name="cell"></param>
    ''' <param name="flowTypeId"></param>
    ''' <param name="flowAmount"></param>
    ''' <remarks></remarks>
    Private Sub OnSpatialFlowOutput(ByVal timestep As Integer, ByVal cell As Cell, ByVal flowTypeId As Integer, ByVal flowAmount As Double)

        If (Me.m_STSimTransformer.IsOutputTimestep(timestep, Me.m_SpatialFlowOutputTimesteps, Me.m_CreateSpatialFlowOutput) And Me.m_IsSpatial) Then

            If GetOutputFlowDictionary().ContainsKey(flowTypeId) Then

                Dim amt As Double = GetOutputFlowDictionary()(flowTypeId)(cell.CellId)

                If amt.Equals(StochasticTimeRaster.DefaultNoDataValue) Then
                    amt = 0
                End If

                amt += (flowAmount / Me.m_STSimTransformer.AmountPerCell)
                GetOutputFlowDictionary()(flowTypeId)(cell.CellId) = amt

            Else
                Debug.Assert(False, "I think we expected to find a m_SpatialOutputFlow object for the flowType " & flowTypeId.ToString("0000", CultureInfo.InvariantCulture))
            End If

        End If

    End Sub

End Class
