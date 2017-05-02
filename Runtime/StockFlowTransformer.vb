'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim
Imports SyncroSim.StochasticTime
Imports System.Globalization
Imports SyncroSim.Common
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class StockFlowTransformer
    Inherits Transformer

    Private m_IsSpatial As Boolean
    Private m_ApplyBeforeTransitions As Boolean
    Private m_ApplyEquallyRankedSimultaneously As Boolean
    Private m_STSimTransformer As STSimTransformer
    Private m_CanComputeStocksAndFlows As Boolean
    Private m_ValidatedMultipliers As Boolean
    Private m_RandomGenerator As New RandomGenerator()
    Private m_ShufflableFlowTypes As New List(Of FlowType)
    Private m_IsInitialized As Boolean

    ''' <summary>
    ''' Gets the ST-Sim Transformer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property STSimTransformer As STSimTransformer
        Get
            Return Me.m_STSimTransformer
        End Get
    End Property

    ''' <summary>
    ''' Overrides Configure
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Configure()

        MyBase.Configure()

        Me.m_STSimTransformer = Me.GetSTSimTransformer()
        Me.m_CanComputeStocksAndFlows = Me.CanComputeStocksAndFlows()

        If (Me.m_CanComputeStocksAndFlows) Then
            Me.NormalizeOutputOptions()
        End If

    End Sub

    ''' <summary>
    ''' Overrides Transform
    ''' </summary>
    ''' <remarks>
    ''' NOTE: We must initialize the ST-Sim transformer and the flag indicating whether or
    ''' not we can do stocks and flows here because we might be loaded in a separate process as
    ''' part of an MP run.
    ''' </remarks>
    Public Overrides Sub Transform()

        MyBase.Transform()

        Me.m_STSimTransformer = Me.GetSTSimTransformer()
        Me.m_CanComputeStocksAndFlows = Me.CanComputeStocksAndFlows()

        If (Not Me.m_CanComputeStocksAndFlows) Then
            Return
        End If

        AddHandler Me.STSimTransformer.CellInitialized, AddressOf Me.OnSTSimCellInitialized
        AddHandler Me.STSimTransformer.CellsInitialized, AddressOf Me.OnSTSimAfterCellsInitialized
        AddHandler Me.STSimTransformer.ChangingCellProbabilistic, AddressOf Me.OnSTSimBeforeChangeCellProbabilistic
        AddHandler Me.STSimTransformer.ChangingCellDeterministic, AddressOf Me.OnSTSimBeforeChangeCellDeterministic
        AddHandler Me.STSimTransformer.CellBeforeTransitions, AddressOf Me.OnSTSimCellBeforeTransitions
        AddHandler Me.STSimTransformer.IterationStarted, AddressOf Me.OnSTSimBeforeIteration
        AddHandler Me.STSimTransformer.TimestepStarted, AddressOf Me.OnSTSimBeforeTimestep
        AddHandler Me.STSimTransformer.TimestepCompleted, AddressOf Me.OnSTSimAfterTimestep

    End Sub

    ''' <summary>
    ''' Disposes this instance
    ''' </summary>
    ''' <param name="disposing"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Dispose(disposing As Boolean)

        If (disposing And Not Me.IsDisposed()) Then

            If (Me.m_CanComputeStocksAndFlows) Then

                RemoveHandler Me.STSimTransformer.CellInitialized, AddressOf Me.OnSTSimCellInitialized
                RemoveHandler Me.STSimTransformer.CellsInitialized, AddressOf Me.OnSTSimAfterCellsInitialized
                RemoveHandler Me.STSimTransformer.ChangingCellProbabilistic, AddressOf Me.OnSTSimBeforeChangeCellProbabilistic
                RemoveHandler Me.STSimTransformer.ChangingCellDeterministic, AddressOf Me.OnSTSimBeforeChangeCellDeterministic
                RemoveHandler Me.STSimTransformer.IterationStarted, AddressOf Me.OnSTSimBeforeIteration
                RemoveHandler Me.STSimTransformer.TimestepStarted, AddressOf Me.OnSTSimBeforeTimestep
                RemoveHandler Me.STSimTransformer.TimestepCompleted, AddressOf Me.OnSTSimAfterTimestep

            End If

        End If

        MyBase.Dispose(disposing)

    End Sub

    ''' <summary>
    ''' Handles the BeforeIteration event. We run this raster verification code here as it depends of the STSim rasters having been loaded ( it's a timing thing)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimBeforeIteration(ByVal sender As Object, ByVal e As IterationEventArgs)

        If (Me.m_IsSpatial And (Not Me.m_ValidatedMultipliers)) Then

            Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME)

            For i As Integer = Me.m_FlowSpatialMultipliers.Count - 1 To 0 Step -1

                Dim r As FlowSpatialMultiplier = Me.m_FlowSpatialMultipliers(i)

                If Not Me.m_FlowSpatialMultiplierRasters.ContainsKey(r.FileName) Then

                    Dim msg As String = String.Format(CultureInfo.CurrentCulture, SPATIAL_PROCESS_WARNING, r.FileName)
                    RecordStatus(StatusType.Warning, msg)

                    Continue For

                End If

                Dim cmpMsg As String = ""
                Dim cmpRes = Me.STSimTransformer.InputRasters.CompareMetadata(Me.m_FlowSpatialMultiplierRasters(r.FileName), cmpMsg)
                Dim FullFilename As String = RasterFiles.GetInputFileName(ds, r.FileName, False)

                If (cmpRes = STSim.CompareMetadataResult.ImportantDifferences) Then

                    Dim msg As String = String.Format(CultureInfo.CurrentCulture, SPATIAL_METADATA_WARNING, FullFilename)
                    RecordStatus(StatusType.Warning, msg)

                    Me.m_FlowSpatialMultipliers.RemoveAt(i)

                Else

                    If (cmpRes = STSim.CompareMetadataResult.UnimportantDifferences) Then

                        Dim msg As String = String.Format(CultureInfo.CurrentCulture, SPATIAL_METADATA_INFO, FullFilename, cmpMsg)
                        RecordStatus(StatusType.Information, msg)

                    End If

                End If

            Next

            Me.m_ValidatedMultipliers = True

        End If

        Me.ResampleFlowMultiplierValues(
            e.Iteration,
            Me.m_STSimTransformer.MinimumTimestep,
            DistributionFrequency.Iteration)

    End Sub

    ''' <summary>
    ''' Handles the BeforeTimestep event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimBeforeTimestep(ByVal sender As Object, ByVal e As TimestepEventArgs)

        'Is it spatial flow output timestep.  If so, then iterate over flow types and initialize an output raster 
        'for each flow type Initialize to DEFAULT_NODATA_VALUE

        If (Me.m_STSimTransformer.IsOutputTimestep(
            e.Timestep, Me.m_SpatialFlowOutputTimesteps, Me.m_CreateSpatialFlowOutput)) Then

            For Each ft As FlowType In Me.m_FlowTypes.Values

                If Me.GetOutputFlowDictionary().ContainsKey(ft.Id) Then

                    Dim arr As Double() = GetOutputFlowDictionary().Item(ft.Id)

                    For i = 0 To arr.GetUpperBound(0)
                        arr(i) = StochasticTimeRaster.DefaultNoDataValue
                    Next

#If DEBUG Then
                    If (arr.Length > 0) Then

                        Debug.Assert(arr(0) = StochasticTimeRaster.DefaultNoDataValue)
                        Debug.Assert(arr(arr.Length - 1) = StochasticTimeRaster.DefaultNoDataValue)

                    End If
#End If

                End If

            Next

        End If

        Me.ResampleFlowMultiplierValues(
            e.Iteration,
            e.Timestep,
            DistributionFrequency.Timestep)

    End Sub

    ''' <summary>
    ''' Handles the AfterTimestep event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimAfterTimestep(ByVal sender As Object, ByVal e As TimestepEventArgs)

        If (Me.m_STSimTransformer.IsOutputTimestep(
            e.Timestep, Me.m_SummaryStockOutputTimesteps, Me.m_CreateSummaryStockOutput)) Then

            Me.OnSummaryStockOutput()
            Me.ProcessStockSummaryData(e.Iteration, e.Timestep)

        End If

        If (Me.m_STSimTransformer.IsOutputTimestep(
            e.Timestep, Me.m_SummaryFlowOutputTimesteps, Me.m_CreateSummaryFlowOutput)) Then

            Me.ProcessFlowSummaryData(e.Iteration, e.Timestep)

        End If

        If (Me.m_IsSpatial) Then

            If (Me.m_STSimTransformer.IsOutputTimestep(
                e.Timestep, Me.m_SpatialStockOutputTimesteps, Me.m_CreateSpatialStockOutput)) Then

                Me.ProcessStockSpatialData(e.Iteration, e.Timestep)
                Me.ProcessStockGroupSpatialData(e.Iteration, e.Timestep)

            End If

            If (Me.m_STSimTransformer.IsOutputTimestep(
                e.Timestep, Me.m_SpatialFlowOutputTimesteps, Me.m_CreateSpatialFlowOutput)) Then

                Me.ProcessFlowSpatialData(e.Iteration, e.Timestep)
                Me.ProcessFlowGroupSpatialData(e.Iteration, e.Timestep)

            End If

        End If

    End Sub

    ''' <summary>
    ''' Called when a cell has been initialized
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimCellInitialized(ByVal sender As Object, ByVal e As CellEventArgs)

        If (Not Me.m_IsInitialized) Then

            'We are doing this here because some of the initialization code below needs
            'to interact with an initialized ST-Sim...

            Me.InitializeSpatialRunFlag()
            Me.InitializeFlowOrderOptions()
            Me.InitializeOutputOptions()
            Me.InitializeOutputDataTables()

            Me.FillFlowGroups()
            Me.FillFlowTypes()
            Me.FillFlowTypeGroups()
            Me.FillStockTypes()
            Me.FillInitialStocksNonSpatial()
            Me.FillStockLimits()
            Me.FillFlowPathways()
            Me.FillFlowMultipliers()
            Me.FillFlowOrders()

            If (Me.m_IsSpatial) Then
                Me.FillInitialStocksSpatial()
                Me.FillFlowSpatialMultipliers()
            End If

            Me.NormalizeForUserDistributions()
            Me.InitializeDistributionValues()
            Me.InitializeShufflableFlowTypes()
            Me.CreateStockLimitMap()
            Me.CreateFlowPathwayMap()
            Me.CreateFlowMultiplierMap()
            Me.CreateFlowSpatialMultiplierMap()
            Me.CreateFlowOrderMap()

            If (Me.m_IsSpatial) Then
                Me.CreateInitialStockSpatialMap()
            End If

            Me.m_IsInitialized = True

        End If

        Dim StockAmounts As Dictionary(Of Integer, Double) = GetStockAmountDictionary(e.Cell)

        For Each s As StockType In Me.m_StockTypes

            If (StockAmounts.ContainsKey(s.Id)) Then
                StockAmounts(s.Id) = 0.0
            Else
                StockAmounts.Add(s.Id, 0.0)
            End If

        Next

        For Each s As InitialStockNonSpatial In Me.m_InitialStocksNonSpatial

            Dim lim As StockLimit = Me.m_StockLimitMap.GetStockLimit(
                s.StockTypeId,
                e.Cell.StratumId,
                e.Cell.SecondaryStratumId,
                e.Cell.StateClassId,
                e.Iteration,
                e.Timestep)

            Dim val As Double = Me.GetAttributeValue(
                s.StateAttributeTypeId,
                e.Cell.StratumId,
                e.Cell.SecondaryStratumId,
                e.Cell.StateClassId,
                e.Iteration,
                e.Timestep,
                e.Cell.Age)

            Dim v As Double = val * e.AmountPerCell
            v = GetLimitBasedInitialStock(v, lim)

            StockAmounts(s.StockTypeId) = v

        Next

        If (Me.m_InitialStockSpatialMap IsNot Nothing) Then

            Dim l As InitialStockSpatialCollection = Me.m_InitialStockSpatialMap.GetItem(e.Iteration)

            If (l IsNot Nothing) Then

                For Each s As InitialStockSpatial In l

                    Dim lim As StockLimit = Me.m_StockLimitMap.GetStockLimit(
                        s.StockTypeId,
                        e.Cell.StratumId,
                        e.Cell.SecondaryStratumId,
                        e.Cell.StateClassId,
                        e.Iteration,
                        e.Timestep)

                    'The spatial value should take precedence over the non-spatial value.  Note that
                    'we assume that raster values are the total amount not the density and don't need conversion.

                    If Me.m_InitialStockSpatialRasters.ContainsKey(s.Filename) Then

                        Dim v As Double = Me.m_InitialStockSpatialRasters(s.Filename).DblCells(e.Cell.CellId)

                        'If a cell is a no data cell or if there is a -INF value for a cell, initialize the stock value to zero

                        If Math.Abs(v - Me.m_InitialStockSpatialRasters(s.Filename).NoDataValue) < Double.Epsilon Or Double.IsNegativeInfinity(v) Then
                            v = 0.0
                        End If

                        v = GetLimitBasedInitialStock(v, lim)
                        StockAmounts(s.StockTypeId) = v

                    Else
                        Debug.Assert(False, "Where's the raster object ?")
                    End If

                Next

            End If

        End If

    End Sub

    ''' <summary>
    ''' Called after all cells have been initialized
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimAfterCellsInitialized(ByVal sender As Object, ByVal e As CellEventArgs)

        Me.OnSummaryStockOutput()
        Me.ProcessStockSummaryData(e.Iteration, e.Timestep)

        If (Me.m_IsSpatial) Then

            If (Me.m_STSimTransformer.IsOutputTimestep(
                e.Timestep, Me.m_SpatialStockOutputTimesteps, Me.m_CreateSpatialStockOutput)) Then

                Me.ProcessStockSpatialData(e.Iteration, e.Timestep)
                Me.ProcessStockGroupSpatialData(e.Iteration, e.Timestep)

            End If

        End If

    End Sub

    ''' <summary>
    ''' Called before a cell changes for a probabilistic transition
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimBeforeChangeCellProbabilistic(ByVal sender As Object, ByVal e As CellChangeEventArgs)

        If (Not Me.m_FlowPathwayMap.HasRecords) Then
            Return
        End If

        Me.ReorderShufflableFlowTypes(e.Iteration, e.Timestep)

        Dim flowTypeLists As List(Of List(Of FlowType)) = Me.CreateListOfFlowTypeLists

        For Each l As List(Of FlowType) In flowTypeLists

            For Each st As StockType In Me.m_StockTypes

                Me.ApplyTransitionFlows(
                    l,
                    st,
                    e.Cell,
                    e.Iteration,
                    e.Timestep,
                    e.AmountPerCell,
                    Nothing,
                    e.ProbabilisticPathway)

            Next

        Next

    End Sub

    ''' <summary>
    ''' Called before a cell changes for a deterministic transition
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimBeforeChangeCellDeterministic(ByVal sender As Object, ByVal e As CellChangeEventArgs)

        If Me.m_ApplyBeforeTransitions = False Then
            ApplyAutomaticFlows(e)
        End If

    End Sub

    ''' <summary>
    ''' Called before a cell changes for a deterministic transition
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnSTSimCellBeforeTransitions(ByVal sender As Object, ByVal e As CellChangeEventArgs)

        If Me.m_ApplyBeforeTransitions = True Then
            ApplyAutomaticFlows(e)
        End If

    End Sub

    Private Function CreateListOfFlowTypeLists() As List(Of List(Of FlowType))

        Dim FlowTypeLists As New List(Of List(Of FlowType))

        If Me.m_ApplyEquallyRankedSimultaneously Then

            Dim FlowTypeOrderDictionary As New SortedDictionary(Of Double, List(Of FlowType))

            For Each ft As FlowType In Me.m_ShufflableFlowTypes

                If (Not FlowTypeOrderDictionary.ContainsKey(ft.Order)) Then
                    FlowTypeOrderDictionary.Add(ft.Order, New List(Of FlowType))
                End If

                FlowTypeOrderDictionary(ft.Order).Add(ft)

            Next

            For Each order As Double In FlowTypeOrderDictionary.Keys
                Dim l As List(Of FlowType) = FlowTypeOrderDictionary(order)
                FlowTypeLists.Add(l)
            Next

        Else

            For Each ft As FlowType In Me.m_ShufflableFlowTypes

                Dim l As New List(Of FlowType)
                l.Add(ft)
                FlowTypeLists.Add(l)

            Next

        End If

        Return (FlowTypeLists)

    End Function

    Private Sub ApplyAutomaticFlows(e As CellChangeEventArgs)

        If (Not Me.m_FlowPathwayMap.HasRecords) Then
            Return
        End If

        Me.ReorderShufflableFlowTypes(e.Iteration, e.Timestep)

        Dim flowTypeLists As List(Of List(Of FlowType)) = Me.CreateListOfFlowTypeLists

        For Each l As List(Of FlowType) In flowTypeLists

            For Each st As StockType In Me.m_StockTypes

                Me.ApplyTransitionFlows(
                    l,
                    st,
                    e.Cell,
                    e.Iteration,
                    e.Timestep,
                    e.AmountPerCell,
                    e.DeterministicPathway,
                    Nothing)

            Next

        Next
    End Sub

    Private Sub ResampleFlowMultiplierValues(
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal frequency As DistributionFrequency)

        Try

            For Each t As FlowMultiplier In Me.m_FlowMultipliers
                t.Sample(iteration, timestep, Me.m_STSimTransformer.DistributionProvider, frequency)
            Next

        Catch ex As Exception
            Throw New ArgumentException("Flow Multipliers" & " -> " & ex.Message)
        End Try

    End Sub

    Private Sub ApplyTransitionFlows(
        ByVal ftList As List(Of FlowType),
        ByVal st As StockType,
        ByVal cell As Cell,
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal amountPerCell As Double,
        ByVal dtPathway As DeterministicTransition,
        ByVal ptPathway As Transition)

        Debug.Assert(Me.m_FlowPathwayMap.HasRecords)

        Dim DestStrat As Integer
        Dim DestStateClass As Integer
        Dim ToAge As Integer
        Dim TGIds As New List(Of Integer)

        If (ptPathway IsNot Nothing) Then

            If (ptPathway.StratumIdDestination.HasValue) Then
                DestStrat = ptPathway.StratumIdDestination.Value
            Else
                DestStrat = cell.StratumId
            End If

            If (ptPathway.StateClassIdDestination.HasValue) Then
                DestStateClass = ptPathway.StateClassIdDestination.Value
            Else
                DestStateClass = cell.StateClassId
            End If

            ToAge = Me.m_STSimTransformer.DetermineTargetAgeProbabilistic(
                cell.Age,
                DestStrat,
                DestStateClass,
                iteration,
                timestep,
                ptPathway)

            For Each tg As TransitionGroup In Me.m_STSimTransformer.TransitionTypes(ptPathway.TransitionTypeId).TransitionGroups
                TGIds.Add(tg.TransitionGroupId)
            Next

        Else

            If (dtPathway Is Nothing) Then

                DestStrat = cell.StratumId
                DestStateClass = cell.StateClassId
                ToAge = cell.Age + 1

            Else

                If (dtPathway.StratumIdDestination.HasValue) Then
                    DestStrat = dtPathway.StratumIdDestination.Value
                Else
                    DestStrat = cell.StratumId
                End If

                If (dtPathway.StateClassIdDestination.HasValue) Then
                    DestStateClass = dtPathway.StateClassIdDestination.Value
                Else
                    DestStateClass = cell.StateClassId
                End If

                ToAge = cell.Age + 1

            End If

            TGIds.Add(0)

        End If

        For Each TransitionGroupId As Integer In TGIds

            Dim allFlowPathways As List(Of FlowPathway) = New List(Of FlowPathway)

            For Each ft As FlowType In ftList

                Dim l As List(Of FlowPathway) =
                    Me.m_FlowPathwayMap.GetFlowPathwayList(
                        iteration,
                        timestep,
                        cell.StratumId,
                        cell.StateClassId,
                        st.Id,
                        cell.Age,
                        DestStrat,
                        DestStateClass,
                        TransitionGroupId,
                        ft.Id,
                        ToAge)

                If l IsNot Nothing Then

                    For Each fp As FlowPathway In l
                        allFlowPathways.Add(fp)
                    Next

                End If

            Next

            For Each fp As FlowPathway In allFlowPathways
                fp.FlowAmount = Me.CalculateFlowAmount(fp, cell, iteration, timestep, amountPerCell)
            Next

            For Each fp As FlowPathway In allFlowPathways

                Dim d As Dictionary(Of Integer, Double) = GetStockAmountDictionary(cell)

                Dim limsrc As StockLimit = Me.m_StockLimitMap.GetStockLimit(
                    fp.FromStockTypeId, cell.StratumId, cell.SecondaryStratumId, cell.StateClassId, iteration, timestep)

                Dim limdst As StockLimit = Me.m_StockLimitMap.GetStockLimit(
                    fp.ToStockTypeId, cell.StratumId, cell.SecondaryStratumId, cell.StateClassId, iteration, timestep)

                If (Not d.ContainsKey(fp.FromStockTypeId)) Then

                    Dim val As Double = GetLimitBasedInitialStock(0.0, limsrc)
                    d.Add(fp.FromStockTypeId, val)

                End If

                If (Not d.ContainsKey(fp.ToStockTypeId)) Then

                    Dim val As Double = GetLimitBasedInitialStock(0.0, limdst)
                    d.Add(fp.ToStockTypeId, val)

                End If

                Dim fa As Double = fp.FlowAmount

                If (limsrc IsNot Nothing) Then

                    If ((d(fp.FromStockTypeId) - fa) < limsrc.StockMinimum) Then
                        fa = d(fp.FromStockTypeId) - limsrc.StockMinimum
                    End If

                End If

                If (limdst IsNot Nothing) Then

                    If ((d(fp.ToStockTypeId) + fa) > limdst.StockMaximum) Then
                        fa = limdst.StockMaximum - d(fp.ToStockTypeId)
                    End If

                End If

                d(fp.FromStockTypeId) -= fa
                d(fp.ToStockTypeId) += fa

                Me.OnSummaryFlowOutput(timestep, cell, dtPathway, ptPathway, fp, fa)
                Me.OnSpatialFlowOutput(timestep, cell, fp.FlowTypeId, fa)

            Next

        Next

    End Sub

    Private Shared Function GetLimitBasedInitialStock(ByVal value As Double, ByVal limit As StockLimit) As Double

        Dim v As Double = value

        If (limit IsNot Nothing) Then

            If (v < limit.StockMinimum) Then
                v = limit.StockMinimum
            ElseIf (v > limit.StockMaximum) Then
                v = limit.StockMinimum
            End If

        End If

        Return v

    End Function

    Private Function CalculateFlowAmount(
        ByVal fp As FlowPathway,
        ByVal cell As Cell,
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal amountPerCell As Double) As Double

        Dim FlowAmount As Double = 0.0
        Dim ft As FlowType = Me.m_FlowTypes(fp.FlowTypeId)

        If (fp.StateAttributeTypeId.HasValue) Then

            FlowAmount = Me.GetAttributeValue(
                fp.StateAttributeTypeId.Value,
                cell.StratumId,
                cell.SecondaryStratumId,
                cell.StateClassId,
                iteration,
                timestep,
                cell.Age)

            FlowAmount *= amountPerCell

        Else

            Dim d As Dictionary(Of Integer, Double) = GetStockAmountDictionary(cell)

            If (Not d.ContainsKey(fp.FromStockTypeId)) Then
                d.Add(fp.FromStockTypeId, 0.0)
            End If

            FlowAmount = d(fp.FromStockTypeId)

        End If

        FlowAmount *= fp.Multiplier

        For Each fg As FlowGroup In ft.FlowGroups

            FlowAmount *= Me.m_FlowMultiplierMap.GetFlowMultiplier(
                fg.Id, cell.StratumId, cell.SecondaryStratumId, cell.StateClassId, iteration, timestep)

            If (Me.m_IsSpatial) Then
                FlowAmount *= Me.GetFlowSpatialMultiplier(cell.CellId, fg.Id, iteration, timestep)
            End If

        Next

        If (FlowAmount <= 0.0) Then
            FlowAmount = 0.0
        End If

        Return FlowAmount

    End Function

    Private Function GetFlowSpatialMultiplier(
        ByVal cellId As Integer,
        ByVal flowGroupId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer) As Double

        Debug.Assert(Me.m_IsSpatial)

        If (Me.m_FlowSpatialMultipliers.Count = 0) Then
            Return 1.0
        End If

        Dim m As FlowSpatialMultiplier =
            Me.m_FlowSpatialMultiplierMap.GetFlowSpatialMultiplier(flowGroupId, iteration, timestep)

        If (m Is Nothing) Then
            Return 1.0
        End If

        If Not Me.m_FlowSpatialMultiplierRasters.ContainsKey(m.FileName) Then
            Return 1.0
        End If

        Dim raster As StochasticTimeRaster = Me.m_FlowSpatialMultiplierRasters(m.FileName)
        Dim v As Double = raster.DblCells(cellId)

        If ((v < 0.0) Or (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, Double.Epsilon))) Then
            Return 1.0
        Else
            Return v
        End If

    End Function

    ''' <summary>
    ''' Reorders the list of shufflable flow types
    ''' </summary>
    ''' <param name="iteration"></param>
    ''' <param name="timestep"></param>
    ''' <remarks></remarks>
    Private Sub ReorderShufflableFlowTypes(ByVal iteration As Integer, ByVal timestep As Integer)

        Dim orders As FlowOrderCollection = Me.m_FlowOrderMap.GetOrders(iteration, timestep)

        If (orders Is Nothing) Then
            ShuffleUtilities.ShuffleList(Me.m_ShufflableFlowTypes, Me.m_RandomGenerator.Random())
        Else
            Me.ReorderShufflableFlowTypes(orders)
        End If

    End Sub

    ''' <summary>
    ''' Reorders the list of shufflable Flow Types
    ''' </summary>
    ''' <param name="orders"></param>
    ''' <remarks></remarks>
    Private Sub ReorderShufflableFlowTypes(ByVal orders As FlowOrderCollection)

        'If there are less than two Flow Types there is no reason to continue

        If (Me.m_ShufflableFlowTypes.Count <= 1) Then
            Return
        End If

        'Reset all Flow Type order values

        For Each ft As FlowType In Me.m_ShufflableFlowTypes
            ft.Order = DEFAULT_FLOW_ORDER
        Next

        'Apply the new ordering from the order collection

        Debug.Assert(Me.m_FlowTypes.Count = Me.m_ShufflableFlowTypes.Count)

        For Each order As FlowOrder In orders

            If (Me.m_FlowTypes.ContainsKey(order.FlowTypeId)) Then

                Debug.Assert(Me.m_ShufflableFlowTypes.Contains(Me.m_FlowTypes(order.FlowTypeId)))
                Me.m_FlowTypes(order.FlowTypeId).Order = order.Order

            End If

        Next

        'Sort by the Flow Types by the order value

        Me.m_ShufflableFlowTypes.Sort(
            Function(t1 As FlowType, t2 As FlowType) As Integer
                Return (t1.Order.CompareTo(t2.Order))
            End Function)

        'Find the number of times each order appears.  If it appears more than
        'once then shuffle the subset of transtion groups with this order.

        Dim OrderCounts As New Dictionary(Of Double, Integer)

        For Each o As FlowOrder In orders

            If (Not OrderCounts.ContainsKey(o.Order)) Then
                OrderCounts.Add(o.Order, 1)
            Else
                OrderCounts(o.Order) += 1
            End If

        Next

        'If any order appears more than once then it is a subset
        'that we need to shuffle.  Note that there may be a subset
        'for the default order.

        For Each d As Double In OrderCounts.Keys

            If (OrderCounts(d) > 1) Then

                ShuffleUtilities.ShuffleSubList(
                    Me.m_ShufflableFlowTypes,
                    Me.GetMinOrderIndex(d),
                    Me.GetMaxOrderIndex(d),
                    Me.m_RandomGenerator.Random())

            End If

        Next

        If (Me.DefaultOrderHasSubset()) Then

            ShuffleUtilities.ShuffleSubList(
                Me.m_ShufflableFlowTypes,
                Me.GetMinOrderIndex(DEFAULT_FLOW_ORDER),
                Me.GetMaxOrderIndex(DEFAULT_FLOW_ORDER),
                Me.m_RandomGenerator.Random())

        End If

    End Sub

    Private Function DefaultOrderHasSubset() As Boolean

        Dim c As Integer = 0

        For Each tg As FlowType In Me.m_ShufflableFlowTypes

            If (tg.Order = DEFAULT_FLOW_ORDER) Then

                c += 1

                If (c = 2) Then
                    Return True
                End If

            End If

        Next

        Return False

    End Function

    Private Function GetMinOrderIndex(ByVal order As Double) As Integer

        For Index As Integer = 0 To Me.m_ShufflableFlowTypes.Count - 1

            Dim tg As FlowType = Me.m_ShufflableFlowTypes(Index)

            If (tg.Order = order) Then
                Return Index
            End If

        Next

        Throw New InvalidOperationException("Cannot find minimum Flow order!")
        Return -1

    End Function

    Private Function GetMaxOrderIndex(ByVal order As Double) As Integer

        For Index As Integer = Me.m_ShufflableFlowTypes.Count - 1 To 0 Step -1

            Dim tg As FlowType = Me.m_ShufflableFlowTypes(Index)

            If (tg.Order = order) Then
                Return Index
            End If

        Next

        Throw New InvalidOperationException("Cannot find maximum Flow order!")
        Return -1

    End Function


End Class
