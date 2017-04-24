'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Partial Class StockFlowTransformer

    ''' <summary>
    ''' Sets whether or not this is a spatial model run
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeSpatialRunFlag()

        Dim drrc As DataRow = Me.ResultScenario.GetDataSheet("STSim_RunControl").GetDataRow()
        Me.m_IsSpatial = DataTableUtilities.GetDataBool(drrc("IsSpatial"))

    End Sub

    ''' <summary>
    ''' Sets the Flow Order Options
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeFlowOrderOptions()

        Dim dr As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_ORDER_OPTIONS).GetDataRow()

        If (dr IsNot Nothing) Then

            Me.m_ApplyBeforeTransitions = DataTableUtilities.GetDataBool(dr, "ApplyBeforeTransitions")
            Me.m_ApplyEquallyRankedSimultaneously = DataTableUtilities.GetDataBool(dr, "ApplyEquallyRankedSimultaneously")

        End If

    End Sub

    ''' <summary>
    ''' Initializes the output options
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeOutputOptions()

        Dim droo As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OO_NAME).GetDataRow()

        Dim SafeInt = Function(o As Object) As Integer
                          If (o Is DBNull.Value) Then
                              Return 0
                          Else
                              Return CInt(o)
                          End If
                      End Function

        Me.m_CreateSummaryStockOutput = DataTableUtilities.GetDataBool(droo(DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME))
        Me.m_SummaryStockOutputTimesteps = SafeInt(droo(DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME))
        Me.m_CreateSummaryFlowOutput = DataTableUtilities.GetDataBool(droo(DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME))
        Me.m_SummaryFlowOutputTimesteps = SafeInt(droo(DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME))
        Me.m_CreateSpatialStockOutput = DataTableUtilities.GetDataBool(droo(DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME))
        Me.m_SpatialStockOutputTimesteps = SafeInt(droo(DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME))
        Me.m_CreateSpatialFlowOutput = DataTableUtilities.GetDataBool(droo(DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME))
        Me.m_SpatialFlowOutputTimesteps = SafeInt(droo(DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME))

    End Sub

    Private Sub InitializeDistributionValues()

        Try

            For Each t As FlowMultiplier In Me.m_FlowMultipliers

                t.Initialize(
                    Me.m_STSimTransformer.MinimumIteration,
                    Me.m_STSimTransformer.MinimumTimestep,
                    Me.m_STSimTransformer.DistributionProvider)

            Next

        Catch ex As Exception
            Throw New ArgumentException("Flow Multipliers" & " -> " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Initializes a separate list of Flow Types that can be shuffled
    ''' </summary>
    ''' <remarks>
    ''' The main list is keyed and cannot be shuffled, but we need a shuffled list for doing raster simulations
    ''' </remarks>
    Private Sub InitializeShufflableFlowTypes()

        Debug.Assert(Me.m_ShufflableFlowTypes.Count = 0)

        For Each ft As FlowType In Me.m_FlowTypes.Values
            Me.m_ShufflableFlowTypes.Add(ft)
        Next

    End Sub

End Class
