'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core

MustInherit Class StockFlowMapBase

    Private m_Scenario As Scenario
    Private m_PrimaryStratumLabel As String
    Private m_SecondaryStratumLabel As String
    Private m_HasItems As Boolean

    Protected Sub New(ByVal scenario As Scenario)

        Me.m_Scenario = scenario

        GetStratumLabelStrings(
            scenario.Project.GetDataSheet("STSim_Terminology"),
            Me.m_PrimaryStratumLabel,
            Me.m_SecondaryStratumLabel)

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
        Return Me.GetDefinitionName("STSim_Stratum", id)
    End Function

    Protected Function GetSecondaryStratumName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetDefinitionName("STSim_SecondaryStratum", id)
    End Function

    Protected Function GetStateClassName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetDefinitionName("STSim_StateClass", id)
    End Function

    Protected Function GetStockTypeName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetDefinitionName(DATASHEET_STOCK_TYPE_NAME, id)
    End Function

    Protected Function GetFlowGroupName(ByVal id As Nullable(Of Integer)) As String
        Return Me.GetDefinitionName(DATASHEET_FLOW_GROUP_NAME, id)
    End Function

    Protected Function GetDefinitionName(ByVal dataSheetName As String, ByVal id As Nullable(Of Integer)) As String

        If (Not id.HasValue) Then
            Return "NULL"
        Else

            Dim ds As DataSheet = Me.m_Scenario.Project.GetDataSheet(dataSheetName)
            Return ds.ValidationTable.GetDisplayName(id.Value)

        End If

    End Function

End Class
