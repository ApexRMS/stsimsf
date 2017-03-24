'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core

Class FlowSpatialMultiplierMap
    Inherits StockFlowMapBase1(Of FlowSpatialMultiplier)

    Public Sub New(
        ByVal scenario As Scenario,
        ByVal items As FlowSpatialMultiplierCollection)

        MyBase.New(scenario)

        For Each Item As FlowSpatialMultiplier In items
            Me.TryAddItem(Item)
        Next

    End Sub

    Public Function GetFlowSpatialMultiplier(
        ByVal flowGroupId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer) As FlowSpatialMultiplier

        Return MyBase.GetItem(flowGroupId, iteration, timestep)

    End Function

    Private Sub TryAddItem(ByVal item As FlowSpatialMultiplier)

        Try

            Me.AddItem(
                item.FlowGroupId,
                item.Iteration,
                item.Timestep,
                item)

        Catch ex As StockFlowMapDuplicateItemException

            Dim template As String =
                "A duplicate flow spatial multiplier was detected: More information:" & vbCrLf &
                "Flow Group={0}, Iteration={1}, Timestep={2}"

            ExceptionUtils.ThrowArgumentException(
                template,
                Me.GetFlowGroupName(item.FlowGroupId),
                StockFlowMapBase.FormatValue(item.Iteration),
                StockFlowMapBase.FormatValue(item.Timestep))

        End Try

    End Sub

End Class
