'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.StochasticTime
Imports System.Globalization

Partial Class StockFlowTransformer

    Private m_FlowTypes As New Dictionary(Of Integer, FlowType)

    Private m_FlowGroups As New FlowGroupCollection()
    Private m_StockTypes As New StockTypeCollection()
    Private m_InitialStocksNonSpatial As New InitialStockNonSpatialCollection()
    Private m_InitialStocksSpatial As New InitialStockSpatialCollection()
    Private m_InitialStockSpatialRasters As New Dictionary(Of String, StochasticTimeRaster)
    Private m_StockLimits As New StockLimitCollection()
    Private m_StockTransitionMultipliers As New StockTransitionMultiplierCollection()
    Private m_FlowPathways As New FlowPathwayCollection()
    Private m_FlowMultipliers As New FlowMultiplierCollection()
    Private m_FlowSpatialMultipliers As New FlowSpatialMultiplierCollection()
    Private m_FlowSpatialMultiplierRasters As New Dictionary(Of String, StochasticTimeRaster)
    Private m_FlowOrders As New FlowOrderCollection()

#If DEBUG Then
    Private m_FlowGroupsFilled As Boolean
    Private m_FlowTypesFilled As Boolean
#End If

    Private Sub FillFlowGroups()

        Debug.Assert(Me.m_FlowGroups.Count = 0)
        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_FLOW_GROUP_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Me.m_FlowGroups.Add(New FlowGroup(
                  CInt(dr(ds.PrimaryKeyColumn.Name))))

        Next

#If DEBUG Then
        Me.m_FlowGroupsFilled = True
#End If

    End Sub

    Private Sub FillFlowTypes()

        Debug.Assert(Me.m_FlowTypes.Count = 0)
        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_FLOW_TYPE_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Dim id As Integer = CInt(dr(ds.PrimaryKeyColumn.Name))
            Dim ft As New FlowType(id)

            Me.m_FlowTypes.Add(id, ft)

        Next

#If DEBUG Then
        Me.m_FlowTypesFilled = True
#End If

    End Sub

    Private Sub FillFlowTypeGroups()

#If DEBUG Then
        Debug.Assert(Me.m_FlowGroupsFilled)
        Debug.Assert(Me.m_FlowTypesFilled)
#End If

        Dim dt As DataTable = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME).GetData()

        For Each ft As FlowType In Me.m_FlowTypes.Values

            Dim q As String = String.Format(CultureInfo.InvariantCulture, "{0}={1}", FLOW_TYPE_ID_COLUMN_NAME, ft.Id)
            Dim rows() As DataRow = dt.Select(q, Nothing)

            For Each dr As DataRow In rows

                Dim gid As Integer = CInt(dr(FLOW_GROUP_ID_COLUMN_NAME))

                Debug.Assert(Not ft.FlowGroups.Contains(gid))
                ft.FlowGroups.Add(Me.m_FlowGroups(gid))

            Next

        Next

    End Sub

    Private Sub FillStockTypes()

        Debug.Assert(Me.m_StockTypes.Count = 0)
        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)

        For Each dr As DataRow In ds.GetData.Rows
            Me.m_StockTypes.Add(New StockType(CInt(dr(ds.PrimaryKeyColumn.Name))))
        Next

    End Sub

    Private Sub FillInitialStocksNonSpatial()

        Debug.Assert(Me.m_InitialStocksNonSpatial.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_INITIAL_STOCK_NON_SPATIAL)

        For Each dr As DataRow In ds.GetData.Rows

            Me.m_InitialStocksNonSpatial.Add(New InitialStockNonSpatial(
                CInt(dr(ds.PrimaryKeyColumn.Name)),
                CInt(dr(STOCK_TYPE_ID_COLUMN_NAME)),
                CInt(dr(STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME))))

        Next

    End Sub

    Private Sub FillInitialStocksSpatial()

        Debug.Assert(Me.m_IsSpatial)
        Debug.Assert(Me.m_InitialStocksSpatial.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_INITIAL_STOCK_SPATIAL)

        For Each dr As DataRow In ds.GetData.Rows

            Dim stockFileName As String = CStr(dr(RASTER_FILE_COLUMN_NAME))
            Dim fullFilename As String = RasterFiles.GetInputFileName(ds, stockFileName, False)
            Dim Iteration As Nullable(Of Integer) = Nothing
            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            'Load Initial Stock raster file
            Dim raster As New StochasticTimeRaster

            Try
                RasterFiles.LoadRasterFile(fullFilename, raster, RasterDataType.DTDouble)
            Catch ex As Exception

                Dim message As String = String.Format(CultureInfo.InvariantCulture, SPATIAL_FILE_STOCK_LOAD_WARNING, stockFileName)
                Throw New ArgumentException(message)

            End Try

            'Compare the Stock raster metadata to standard to make rasters match
            Dim cmpMsg As String = ""
            Dim cmpResult = Me.STSimTransformer.InputRasters.CompareMetadata(raster, cmpMsg)

            If cmpResult = STSim.CompareMetadataResult.ImportantDifferences Then

                Dim message As String = String.Format(CultureInfo.InvariantCulture, SPATIAL_FILE_STOCK_METADATA_WARNING, stockFileName, cmpMsg)
                Me.RecordStatus(StatusType.Warning, message)

            Else

                If cmpResult = STSim.CompareMetadataResult.UnimportantDifferences Then
                    Dim message As String = String.Format(CultureInfo.InvariantCulture, SPATIAL_FILE_STOCK_METADATA_INFO, stockFileName, cmpMsg)
                    Me.RecordStatus(StatusType.Information, message)
                End If

                Me.m_InitialStocksSpatial.Add(New InitialStockSpatial(
                    Iteration,
                    CInt(dr(STOCK_TYPE_ID_COLUMN_NAME)),
                    stockFileName))

                'Only loading single instance of a particular raster, as a way to converse memory

                If Not m_InitialStockSpatialRasters.ContainsKey(stockFileName) Then
                    Me.m_InitialStockSpatialRasters.Add(stockFileName, raster)
                End If

            End If

        Next

    End Sub

    Private Sub FillStockLimits()

        Debug.Assert(Me.m_StockLimits.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_STOCK_LIMIT_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Dim Iteration As Nullable(Of Integer) = Nothing
            Dim Timestep As Nullable(Of Integer) = Nothing
            Dim StockTypeId As Integer = 0
            Dim StratumId As Nullable(Of Integer) = Nothing
            Dim SecondaryStratumId As Nullable(Of Integer) = Nothing
            Dim TertiaryStratumId As Nullable(Of Integer) = Nothing
            Dim StateClassId As Nullable(Of Integer) = Nothing
            Dim StockMin As Double = Double.MinValue
            Dim StockMax As Double = Double.MaxValue

            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                Timestep = CInt(dr(TIMESTEP_COLUMN_NAME))
            End If

            StockTypeId = CInt(dr(STOCK_TYPE_ID_COLUMN_NAME))

            If (dr(STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StratumId = CType(dr(STRATUM_ID_COLUMN_NAME), Integer)
            End If

            If (dr(SECONDARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                SecondaryStratumId = CType(dr(SECONDARY_STRATUM_ID_COLUMN_NAME), Integer)
            End If

            If (dr(TERTIARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                TertiaryStratumId = CInt(dr(TERTIARY_STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(STATECLASS_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StateClassId = CType(dr(STATECLASS_ID_COLUMN_NAME), Integer)
            End If

            If (dr(STOCK_MIN_COLUMN_NAME) IsNot DBNull.Value) Then
                StockMin = CType(dr(STOCK_MIN_COLUMN_NAME), Double)
            End If

            If (dr(STOCK_MAX_COLUMN_NAME) IsNot DBNull.Value) Then
                StockMax = CType(dr(STOCK_MAX_COLUMN_NAME), Double)
            End If

            Me.m_StockLimits.Add(New StockLimit(
                Iteration,
                Timestep,
                StockTypeId,
                StratumId,
                SecondaryStratumId,
                TertiaryStratumId,
                StateClassId,
                StockMin,
                StockMax))

        Next

    End Sub

    Private Sub FillStockTransitionMultipliers()

        Debug.Assert(Me.m_StockTransitionMultipliers.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_STOCK_TRANSITION_MULTIPLIER_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Dim Iteration As Nullable(Of Integer) = Nothing
            Dim Timestep As Nullable(Of Integer) = Nothing
            Dim StratumId As Nullable(Of Integer) = Nothing
            Dim SecondaryStratumId As Nullable(Of Integer) = Nothing
            Dim TertiaryStratumId As Nullable(Of Integer) = Nothing
            Dim StateClassId As Nullable(Of Integer) = Nothing
            Dim TransitionGroupId As Integer = CInt(dr(TRANSITION_GROUP_ID_COLUMN_NAME))
            Dim StockGroupId As Integer = CInt(dr(STOCK_GROUP_ID_COLUMN_NAME))
            Dim StockValue As Double = CDbl(dr(STOCK_VALUE_COLUMN_NAME))
            Dim MultiplierValue As Nullable(Of Double) = Nothing
            Dim DistributionTypeId As Nullable(Of Integer) = Nothing
            Dim DistributionFrequency As Nullable(Of DistributionFrequency) = Nothing
            Dim DistributionSD As Nullable(Of Double) = Nothing
            Dim DistributionMin As Nullable(Of Double) = Nothing
            Dim DistributionMax As Nullable(Of Double) = Nothing

            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                Timestep = CInt(dr(TIMESTEP_COLUMN_NAME))
            End If

            If (dr(STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StratumId = CInt(dr(STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(SECONDARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                SecondaryStratumId = CInt(dr(SECONDARY_STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(TERTIARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                TertiaryStratumId = CInt(dr(TERTIARY_STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(STATECLASS_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StateClassId = CInt(dr(STATECLASS_ID_COLUMN_NAME))
            End If

            If (dr(VALUE_COLUMN_NAME) IsNot DBNull.Value) Then
                MultiplierValue = CDbl(dr(VALUE_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONTYPE_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionTypeId = CInt(dr(DISTRIBUTIONTYPE_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTION_FREQUENCY_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionFrequency = CType(dr(DISTRIBUTION_FREQUENCY_COLUMN_NAME), DistributionFrequency)
            End If

            If (dr(DISTRIBUTIONSD_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionSD = CDbl(dr(DISTRIBUTIONSD_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONMIN_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionMin = CDbl(dr(DISTRIBUTIONMIN_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONMAX_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionMax = CDbl(dr(DISTRIBUTIONMAX_COLUMN_NAME))
            End If

            Try

                Dim Item As New StockTransitionMultiplier(
                    Iteration,
                    Timestep,
                    StratumId,
                    SecondaryStratumId,
                    TertiaryStratumId,
                    StateClassId,
                    TransitionGroupId,
                    StockGroupId,
                    StockValue,
                    MultiplierValue,
                    DistributionTypeId,
                    DistributionFrequency,
                    DistributionSD,
                    DistributionMin,
                    DistributionMax)

                Me.m_STSimTransformer.DistributionProvider.Validate(
                    Item.DistributionTypeId,
                    Item.DistributionValue,
                    Item.DistributionSD,
                    Item.DistributionMin,
                    Item.DistributionMax)

                Me.m_StockTransitionMultipliers.Add(Item)

            Catch ex As Exception
                Throw New ArgumentException(ds.DisplayName & " -> " & ex.Message)
            End Try

        Next

    End Sub

    Private Sub FillFlowPathways()

        Debug.Assert(Me.m_FlowPathways.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_PATHWAY_NAME)

        For Each dr As DataRow In ds.GetData.Rows

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

            Me.m_FlowPathways.Add(New FlowPathway(
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
                Multiplier))

        Next

    End Sub

    Private Sub FillFlowMultipliers()

        Debug.Assert(Me.m_FlowMultipliers.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_MULTIPLIER_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Dim Iteration As Nullable(Of Integer) = Nothing
            Dim Timestep As Nullable(Of Integer) = Nothing
            Dim StratumId As Nullable(Of Integer) = Nothing
            Dim SecondaryStratumId As Nullable(Of Integer) = Nothing
            Dim TertiaryStratumId As Nullable(Of Integer) = Nothing
            Dim StateClassId As Nullable(Of Integer) = Nothing
            Dim FlowGroupId As Integer
            Dim MultiplierAmount As Nullable(Of Double) = Nothing
            Dim DistributionTypeId As Nullable(Of Integer) = Nothing
            Dim DistributionFrequency As Nullable(Of DistributionFrequency) = Nothing
            Dim DistributionSD As Nullable(Of Double) = Nothing
            Dim DistributionMin As Nullable(Of Double) = Nothing
            Dim DistributionMax As Nullable(Of Double) = Nothing

            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                Timestep = CInt(dr(TIMESTEP_COLUMN_NAME))
            End If

            If (dr(STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StratumId = CInt(dr(STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(SECONDARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                SecondaryStratumId = CInt(dr(SECONDARY_STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(TERTIARY_STRATUM_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                TertiaryStratumId = CInt(dr(TERTIARY_STRATUM_ID_COLUMN_NAME))
            End If

            If (dr(STATECLASS_ID_COLUMN_NAME) IsNot DBNull.Value) Then
                StateClassId = CInt(dr(STATECLASS_ID_COLUMN_NAME))
            End If

            FlowGroupId = CInt(dr(FLOW_GROUP_ID_COLUMN_NAME))

            If (dr(VALUE_COLUMN_NAME) IsNot DBNull.Value) Then
                MultiplierAmount = CDbl(dr(VALUE_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONTYPE_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionTypeId = CInt(dr(DISTRIBUTIONTYPE_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTION_FREQUENCY_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionFrequency = CType(dr(DISTRIBUTION_FREQUENCY_COLUMN_NAME), DistributionFrequency)
            End If

            If (dr(DISTRIBUTIONSD_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionSD = CDbl(dr(DISTRIBUTIONSD_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONMIN_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionMin = CDbl(dr(DISTRIBUTIONMIN_COLUMN_NAME))
            End If

            If (dr(DISTRIBUTIONMAX_COLUMN_NAME) IsNot DBNull.Value) Then
                DistributionMax = CDbl(dr(DISTRIBUTIONMAX_COLUMN_NAME))
            End If

            Try

                Dim Item As New FlowMultiplier(
                    Iteration,
                    Timestep,
                    StratumId,
                    SecondaryStratumId,
                    TertiaryStratumId,
                    StateClassId,
                    FlowGroupId,
                    MultiplierAmount,
                    DistributionTypeId,
                    DistributionFrequency,
                    DistributionSD,
                    DistributionMin,
                    DistributionMax)

                Me.m_STSimTransformer.DistributionProvider.Validate(
                    Item.DistributionTypeId,
                    Item.DistributionValue,
                    Item.DistributionSD,
                    Item.DistributionMin,
                    Item.DistributionMax)

                Me.m_FlowMultipliers.Add(Item)

            Catch ex As Exception
                Throw New ArgumentException(ds.DisplayName & " -> " & ex.Message)
            End Try

        Next

    End Sub

    Private Sub FillFlowOrders()

        Debug.Assert(Me.m_FlowOrders.Count = 0)
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_ORDER)

        For Each dr As DataRow In ds.GetData.Rows

            Dim Iteration As Nullable(Of Integer) = Nothing
            Dim Timestep As Nullable(Of Integer) = Nothing
            Dim FlowTypeId As Integer
            Dim Order As Nullable(Of Double) = Nothing

            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                Timestep = CInt(dr(TIMESTEP_COLUMN_NAME))
            End If

            FlowTypeId = CInt(dr(FLOW_TYPE_ID_COLUMN_NAME))

            If (dr(DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME) IsNot DBNull.Value) Then
                Order = CDbl(dr(DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME))
            End If

            Try

                Dim Item As New FlowOrder(
                    Iteration,
                    Timestep,
                    FlowTypeId,
                    Order)

                Me.m_FlowOrders.Add(Item)

            Catch ex As Exception
                Throw New ArgumentException(ds.DisplayName & " -> " & ex.Message)
            End Try

        Next

    End Sub

    Private Sub FillFlowSpatialMultipliers()

        Debug.Assert(Me.m_IsSpatial)
        Debug.Assert(Me.m_FlowSpatialMultipliers.Count = 0)
        Debug.Assert(Me.m_FlowSpatialMultiplierRasters.Count = 0)

        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME)

        For Each dr As DataRow In ds.GetData.Rows

            Dim FlowGroupId As Integer = CInt(dr(FLOW_GROUP_ID_COLUMN_NAME))
            Dim Iteration As Nullable(Of Integer) = Nothing
            Dim Timestep As Nullable(Of Integer) = Nothing
            Dim FileName As String = CStr(dr(MULTIPLIER_FILE_COLUMN_NAME))

            If (dr(ITERATION_COLUMN_NAME) IsNot DBNull.Value) Then
                Iteration = CInt(dr(ITERATION_COLUMN_NAME))
            End If

            If (dr(TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                Timestep = CInt(dr(TIMESTEP_COLUMN_NAME))
            End If

            Dim Multiplier As New FlowSpatialMultiplier(FlowGroupId, Iteration, Timestep, FileName)
            Dim FullFilename As String = RasterFiles.GetInputFileName(ds, FileName, False)
            Dim MultiplierRaster As New StochasticTimeRaster

            Try
                RasterFiles.LoadRasterFile(FullFilename, MultiplierRaster, RasterDataType.DTDouble)
            Catch ex As Exception

                Dim msg As String = String.Format(CultureInfo.InvariantCulture, SPATIAL_PROCESS_WARNING, FullFilename)
                Throw New ArgumentException(msg)

            End Try

            Me.m_FlowSpatialMultipliers.Add(Multiplier)

            'Only load a single instance of a each unique filename to conserve memory

            If Not Me.m_FlowSpatialMultiplierRasters.ContainsKey(FileName) Then
                Me.m_FlowSpatialMultiplierRasters.Add(FileName, MultiplierRaster)
            End If

        Next

    End Sub

End Class
