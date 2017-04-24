'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Common

Class FlowPathwayMap

    Private m_HasRecords As Boolean
    Private m_Map As New MultiLevelKeyMap7(Of SortedKeyMap4(Of List(Of FlowPathway)))

    Public Sub New(ByVal pathways As FlowPathwayCollection)

        For Each sa As FlowPathway In pathways
            Me.AddItem(sa)
        Next

    End Sub

    Public ReadOnly Property HasRecords As Boolean
        Get
            Return Me.m_HasRecords
        End Get
    End Property

    Public Sub AddItem(ByVal pathway As FlowPathway)

        Dim m1 As SortedKeyMap4(Of List(Of FlowPathway)) =
            Me.m_Map.GetItemExact(
                pathway.FromStratumId, pathway.FromStateClassId, pathway.FromStockTypeId,
                pathway.ToStratumId, pathway.ToStateClassId, pathway.TransitionGroupId, pathway.FlowTypeId)

        If (m1 Is Nothing) Then

            m1 = New SortedKeyMap4(Of List(Of FlowPathway))(SearchMode.ExactPrev)

            Me.m_Map.AddItem(
                pathway.FromStratumId, pathway.FromStateClassId, pathway.FromStockTypeId,
                pathway.ToStratumId, pathway.ToStateClassId, pathway.TransitionGroupId, pathway.FlowTypeId, m1)

        End If

        Dim l As List(Of FlowPathway) = m1.GetItemExact(
            pathway.FromMinimumAge, pathway.ToMinimumAge, pathway.Iteration, pathway.Timestep)

        If (l Is Nothing) Then

            l = New List(Of FlowPathway)
            m1.AddItem(pathway.FromMinimumAge, pathway.ToMinimumAge, pathway.Iteration, pathway.Timestep, l)

        End If

        l.Add(pathway)
        Me.m_HasRecords = True

    End Sub

    Public Function GetFlowPathwayList(
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal fromStratumId As Nullable(Of Integer),
        ByVal fromStateClassId As Integer,
        ByVal fromStockTypeId As Integer,
        ByVal fromMinimumAge As Integer,
        ByVal toStratumId As Nullable(Of Integer),
        ByVal toStateClassId As Nullable(Of Integer),
        ByVal transitionGroupId As Integer,
        ByVal flowTypeId As Integer,
        ByVal toMinimumAge As Integer) As List(Of FlowPathway)

        If (Not Me.m_HasRecords) Then
            Return Nothing
        End If

        Dim m1 As SortedKeyMap4(Of List(Of FlowPathway)) =
            Me.m_Map.GetItem(
                fromStratumId, fromStateClassId, fromStockTypeId,
                toStratumId, toStateClassId, transitionGroupId, flowTypeId)

        If (m1 Is Nothing) Then
            Return Nothing
        End If

        Dim l As List(Of FlowPathway) = m1.GetItem(fromMinimumAge, toMinimumAge, iteration, timestep)

        If (l Is Nothing OrElse l.Count = 0) Then
            Return Nothing
        End If

        Return l

    End Function

End Class
