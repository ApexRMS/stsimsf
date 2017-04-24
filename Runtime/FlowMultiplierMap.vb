'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim
Imports SyncroSim.StochasticTime

Class FlowMultiplierMap
    Inherits StockFlowMapBase4(Of FlowMultiplier)

    Private m_DistributionProvider As STSimDistributionProvider

    Public Sub New(
        ByVal scenario As Scenario,
        ByVal items As FlowMultiplierCollection,
        ByVal provider As STSimDistributionProvider)

        MyBase.New(scenario)

        Me.m_DistributionProvider = provider

        For Each item As FlowMultiplier In items
            Me.TryAddMultiplier(item)
        Next

    End Sub

    Public Function GetFlowMultiplier(
        ByVal flowGroupId As Integer,
        ByVal stratumId As Integer,
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer) As Double

        Dim v As FlowMultiplier = Me.GetItem(
           flowGroupId,
           stratumId,
           secondaryStratumId,
           stateClassId,
           iteration,
           timestep)

        If (v Is Nothing) Then
            Return 1.0
        Else

            v.Sample(
                iteration,
                timestep,
                Me.m_DistributionProvider,
                DistributionFrequency.Always)

            Return v.CurrentValue.Value

        End If

    End Function

    Private Sub TryAddMultiplier(ByVal item As FlowMultiplier)

        Try

            MyBase.AddItem(
                item.FlowGroupId,
                item.StratumId,
                item.SecondaryStratumId,
                item.StateClassId,
                item.Iteration,
                item.Timestep,
                item)

        Catch ex As StockFlowMapDuplicateItemException

            Dim template As String =
                "A duplicate flow multiplier was detected: More information:" & vbCrLf &
                "Flow Group={0}, {1}={2}, {3}={4}, State Class={5}, Iteration={6}, Timestep={7}."

            ExceptionUtils.ThrowArgumentException(
                template,
                Me.GetFlowGroupName(item.FlowGroupId),
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
