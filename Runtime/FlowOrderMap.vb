'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Common

Class FlowOrderMap

    Private m_HasItems As Boolean
    Private m_Map As New SortedKeyMap2(Of FlowOrderCollection)(SearchMode.ExactPrev)

    Public Sub New(ByVal orders As FlowOrderCollection)

        For Each o As FlowOrder In orders
            Me.AddOrder(o)
        Next

    End Sub

    Private Sub AddOrder(ByVal order As FlowOrder)

        Dim l As FlowOrderCollection = Me.m_Map.GetItemExact(order.Iteration, order.Timestep)

        If (l Is Nothing) Then

            l = New FlowOrderCollection()
            Me.m_Map.AddItem(order.Iteration, order.Timestep, l)

        End If

        l.Add(order)
        Me.m_HasItems = True

    End Sub

    Public Function GetOrders(ByVal iteration As Integer, ByVal timestep As Integer) As FlowOrderCollection

        If (Not Me.m_HasItems) Then
            Return Nothing
        End If

        Dim l As FlowOrderCollection = Me.m_Map.GetItem(iteration, timestep)

        If (l Is Nothing) Then
            Return Nothing
        End If

        Debug.Assert(l.Count > 0)
        Return l

    End Function

End Class

