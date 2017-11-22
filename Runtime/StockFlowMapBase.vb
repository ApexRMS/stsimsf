'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

MustInherit Class StockFlowMapBase

    Private m_Scenario As Scenario
    Private m_PrimaryStratumLabel As String
    Private m_SecondaryStratumLabel As String
    Private m_TertiaryStratumLabel As String
    Private m_HasItems As Boolean

    Protected Sub New(ByVal scenario As Scenario)

        Me.m_Scenario = scenario

        GetStratumLabelStrings(
            scenario.Project.GetDataSheet("STSim_Terminology"),
            Me.m_PrimaryStratumLabel,
            Me.m_SecondaryStratumLabel,
            Me.m_TertiaryStratumLabel)

    End Sub

    Protected ReadOnly Property PrimaryStratumLabel As String
        Get
            Return Me.m_PrimaryStratumLabel
        End Get
    End Property

    Protected ReadOnly Property SecondaryStratumLabel As String
        Get
            Return Me.m_SecondaryStratumLabel
        End Get
    End Property

    Protected ReadOnly Property TertiaryStratumLabel As String
        Get
            Return Me.m_TertiaryStratumLabel
        End Get
    End Property

    Protected Sub SetHasItems()
        Me.m_HasItems = True
    End Sub

    Public ReadOnly Property HasItems As Boolean
        Get
            Return Me.m_HasItems
        End Get
    End Property

    Protected Shared Sub ThrowDuplicateItemException()
        Throw New StockFlowMapDuplicateItemException("An item with the same keys has already been added.")
    End Sub

    Protected Shared Function FormatValue(ByVal value As Nullable(Of Integer)) As String

        If (Not value.HasValue) Then
            Return "NULL"
        Else
            Return CStr(value)
        End If

    End Function

    Protected Function GetStratumName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName("STSim_Stratum", id)
    End Function

    Protected Function GetSecondaryStratumName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName("STSim_SecondaryStratum", id)
    End Function

    Protected Function GetTertiaryStratumName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName("STSim_TertiaryStratum", id)
    End Function

    Protected Function GetStateClassName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName("STSim_StateClass", id)
    End Function

    Protected Function GetTransitionGroupName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName("STSim_TransitionGroup", id)
    End Function

    Protected Function GetStockTypeName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName(DATASHEET_STOCK_TYPE_NAME, id)
    End Function

    Protected Function GetStockGroupName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName(DATASHEET_STOCK_GROUP_NAME, id)
    End Function

    Protected Function GetFlowGroupName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetProjectItemName(DATASHEET_FLOW_GROUP_NAME, id)
    End Function

    Protected Function GetProjectItemName(ByVal dataSheetName As String, ByVal id As Nullable(Of Integer)) As String

        If (Not id.HasValue) Then
            Return "NULL"
        Else

            Dim ds As DataSheet = Me.m_Scenario.Project.GetDataSheet(dataSheetName)
            Return ds.ValidationTable.GetDisplayName(id.Value)

        End If

    End Function

End Class
