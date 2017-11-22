'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim
Imports SyncroSim.StochasticTime

Class StockTransitionMultiplierMap
    Inherits StockFlowMapBase6(Of SortedList(Of Double, StockTransitionMultiplier))

    Private m_DistributionProvider As STSimDistributionProvider

    Public Sub New(
        ByVal scenario As Scenario,
        ByVal items As StockTransitionMultiplierCollection,
        ByVal provider As STSimDistributionProvider)

        MyBase.New(scenario)

        Me.m_DistributionProvider = provider

        For Each item As StockTransitionMultiplier In items
            Me.TryAddMultiplier(item)
        Next

    End Sub

    Public Function GetStockTransitionMultiplier(
        ByVal stockGroupId As Integer,
        ByVal stratumId As Integer,
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal tertiaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Integer,
        ByVal transitionGroupId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal stockValue As Double) As Double

        Dim lst As SortedList(Of Double, StockTransitionMultiplier) =
            Me.GetItem(
                stockGroupId,
                stratumId,
                secondaryStratumId,
                tertiaryStratumId,
                stateClassId,
                transitionGroupId,
                iteration,
                timestep)

        If (lst Is Nothing) Then
            Return 1.0
        End If

        If (lst.Count = 1) Then

            Dim tsm As StockTransitionMultiplier = lst.First.Value
            tsm.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)

            Return tsm.CurrentValue.Value

        End If

        If (lst.ContainsKey(stockValue)) Then

            Dim tsm As StockTransitionMultiplier = lst(stockValue)
            tsm.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)

            Return tsm.CurrentValue.Value

        End If

        Dim PrevKey As Double = Double.MinValue
        Dim ThisKey As Double = Double.MinValue

        For Each k As Double In lst.Keys

            Debug.Assert(k <> stockValue)

            If (k > stockValue) Then

                ThisKey = k
                Exit For

            End If

            PrevKey = k

        Next

        If (PrevKey = Double.MinValue) Then

            Dim tsm As StockTransitionMultiplier = lst.First.Value
            tsm.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)

            Return tsm.CurrentValue.Value

        End If

        If (ThisKey = Double.MinValue) Then

            Dim tsm As StockTransitionMultiplier = lst.Last.Value
            tsm.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)

            Return tsm.CurrentValue.Value

        End If

        Dim PrevMult As StockTransitionMultiplier = lst(PrevKey)
        Dim ThisMult As StockTransitionMultiplier = lst(ThisKey)

        PrevMult.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)
        ThisMult.Sample(iteration, timestep, Me.m_DistributionProvider, DistributionFrequency.Always)

        Return Interpolate(
            PrevKey,
            PrevMult.CurrentValue.Value,
            ThisKey,
            ThisMult.CurrentValue.Value,
            stockValue)

    End Function

    Private Sub TryAddMultiplier(ByVal item As StockTransitionMultiplier)

        Dim l As SortedList(Of Double, StockTransitionMultiplier) =
            Me.GetItemExact(
                item.StockGroupId,
                item.StratumId,
                item.SecondaryStratumId,
                item.TertiaryStratumId,
                item.StateClassId,
                item.TransitionGroupId,
                item.Iteration,
                item.Timestep)

        If (l Is Nothing) Then

            l = New SortedList(Of Double, StockTransitionMultiplier)

            Me.AddItem(
                item.StockGroupId,
                item.StratumId,
                item.SecondaryStratumId,
                item.TertiaryStratumId,
                item.StateClassId,
                item.TransitionGroupId,
                item.Iteration,
                item.Timestep,
                l)

        End If

        l.Add(item.StockValue, item)
        Debug.Assert(Me.HasItems())

    End Sub

End Class
