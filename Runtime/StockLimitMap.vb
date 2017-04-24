'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Class StockLimitMap
    Inherits StockFlowMapBase4(Of StockLimit)

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
        ByVal stateClassId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer) As StockLimit

        Return MyBase.GetItem(
            stockTypeId,
            stratumId,
            secondaryStratumId,
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
                item.StateClassId,
                item.Iteration,
                item.Timestep,
                item)

        Catch ex As StockFlowMapDuplicateItemException

            Dim template As String =
                "A duplicate stock limit was detected: More information:" & vbCrLf &
                "Stock Type={0}, {1}={2}, {3}={4}, State Class={5}, Iteration={6}, Timestep={7}."

            ExceptionUtils.ThrowArgumentException(
                template,
                Me.GetStockTypeName(item.StockTypeId),
                Me.PrimaryStratumLabel,
                Me.GetStratumName(item.StratumId),
                Me.SecondaryStratumLabel,
                Me.GetSecondaryStratumName(item.SecondaryStratumId),
                Me.GetStateClassName(item.StateClassId),
                StockFlowMapBase.FormatValue(item.Iteration),
                StockFlowMapBase.FormatValue(item.Timestep))

        End Try

    End Sub

End Class
