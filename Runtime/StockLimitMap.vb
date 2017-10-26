'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Class StockLimitMap
    Inherits StockFlowMapBase5(Of StockLimit)

    Public Sub New(
        ByVal scenario As Scenario,
        ByVal items As StockLimitCollection)

        MyBase.New(scenario)

        For Each item As StockLimit In items
            Me.TryAddLimit(item)
        Next

    End Sub

    Public Function GetStockLimit(
        ByVal stockTypeId As Integer,
        ByVal stratumId As Integer,
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal tertiaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer) As StockLimit

        Return MyBase.GetItem(
            stockTypeId,
            stratumId,
            secondaryStratumId,
            tertiaryStratumId,
            stateClassId,
            iteration,
            timestep)

    End Function

    Private Sub TryAddLimit(ByVal item As StockLimit)

        Try

            MyBase.AddItem(
                item.StockTypeId,
                item.StratumId,
                item.SecondaryStratumId,
                item.TertiaryStratumId,
                item.StateClassId,
                item.Iteration,
                item.Timestep,
                item)

        Catch ex As StockFlowMapDuplicateItemException

            Dim template As String =
                "A duplicate stock limit was detected: More information:" & vbCrLf &
                "Stock Type={0}, {1}={2}, {3}={4}, {5}={6}, State Class={7}, Iteration={8}, Timestep={9}."

            ExceptionUtils.ThrowArgumentException(
                template,
                Me.GetStockTypeName(item.StockTypeId),
                Me.PrimaryStratumLabel,
                Me.GetStratumName(item.StratumId),
                Me.SecondaryStratumLabel,
                Me.GetSecondaryStratumName(item.SecondaryStratumId),
                Me.TertiaryStratumLabel,
                Me.GetTertiaryStratumName(item.TertiaryStratumId),
                Me.GetStateClassName(item.StateClassId),
                StockFlowMapBase.FormatValue(item.Iteration),
                StockFlowMapBase.FormatValue(item.Timestep))

        End Try

    End Sub

End Class
