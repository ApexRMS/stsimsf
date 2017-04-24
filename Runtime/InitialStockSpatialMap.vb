'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Common

Class InitialStockSpatialMap

    Private m_HasItems As Boolean
    Private m_Map As New SortedKeyMap1(Of InitialStockSpatialCollection)(SearchMode.ExactPrev)

    Public Sub New(ByVal icd As InitialStockSpatialCollection)

        For Each t As InitialStockSpatial In icd
            Me.AddISS(t)
        Next

    End Sub

    Private Sub AddISS(ByVal iss As InitialStockSpatial)

        Dim l As InitialStockSpatialCollection = Me.m_Map.GetItemExact(iss.Iteration)

        If (l Is Nothing) Then

            l = New InitialStockSpatialCollection()
            Me.m_Map.AddItem(iss.Iteration, l)

        End If

        l.Add(iss)
        Me.m_HasItems = True

    End Sub

    Public Function GetItem(ByVal iteration As Nullable(Of Integer)) As InitialStockSpatialCollection

        If (Not Me.m_HasItems) Then
            Return Nothing
        End If

        Dim l As InitialStockSpatialCollection = Me.m_Map.GetItem(iteration)

        If (l Is Nothing) Then
            Return Nothing
        End If

        Debug.Assert(l.Count > 0)
        Return l

    End Function

End Class
