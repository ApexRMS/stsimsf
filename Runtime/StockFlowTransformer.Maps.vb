'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Partial Class StockFlowTransformer

    Private m_StockLimitMap As StockLimitMap
    Private m_FlowPathwayMap As FlowPathwayMap
    Private m_FlowMultiplierMap As FlowMultiplierMap
    Private m_FlowSpatialMultiplierMap As FlowSpatialMultiplierMap
    Private m_InitialStockSpatialMap As InitialStockSpatialMap
    Private m_FlowOrderMap As FlowOrderMap

    Private Sub CreateStockLimitMap()

        Debug.Assert(Me.m_StockLimitMap Is Nothing)

        Me.m_StockLimitMap = New StockLimitMap(
            Me.ResultScenario,
            Me.m_StockLimits)

    End Sub

    Private Sub CreateFlowPathwayMap()

        Debug.Assert(Me.m_FlowPathwayMap Is Nothing)
        Me.m_FlowPathwayMap = New FlowPathwayMap(Me.m_FlowPathways)

    End Sub

    Private Sub CreateFlowMultiplierMap()

        Debug.Assert(Me.m_FlowMultiplierMap Is Nothing)

        Me.m_FlowMultiplierMap = New FlowMultiplierMap(
            Me.ResultScenario,
            Me.m_FlowMultipliers,
            Me.m_STSimTransformer.DistributionProvider)

    End Sub

    Private Sub CreateFlowSpatialMultiplierMap()

        Debug.Assert(Me.m_FlowSpatialMultiplierMap Is Nothing)

        Me.m_FlowSpatialMultiplierMap = New FlowSpatialMultiplierMap(
            Me.ResultScenario,
            Me.m_FlowSpatialMultipliers)

    End Sub

    Private Sub CreateInitialStockSpatialMap()

        Debug.Assert(Me.m_InitialStockSpatialMap Is Nothing)
        Me.m_InitialStockSpatialMap = New InitialStockSpatialMap(Me.m_InitialStocksSpatial)

    End Sub

    Private Sub CreateFlowOrderMap()

        Debug.Assert(Me.m_FlowOrderMap Is Nothing)
        Me.m_FlowOrderMap = New FlowOrderMap(Me.m_FlowOrders)

    End Sub

End Class
