'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.StochasticTime.Forms
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class SFMapProvider
    Inherits MapProvider

    Public Overrides Sub RefreshMapCriteria(layout As SyncroSimLayout, project As Project)

        'Stock Types
        Dim StockTypesGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("StockTypes", "Stock Types", True)

        AddStockTypeMapVariables(project, StockTypesGroup.Items)

        If (StockTypesGroup.Items.Count > 0) Then
            layout.Items.Add(StockTypesGroup)
        End If

        'Flow Types
        Dim FlowTypesGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("FlowTypes", "Flow Types", True)

        AddFlowTypeMapVariables(project, FlowTypesGroup.Items)

        If (FlowTypesGroup.Items.Count > 0) Then
            layout.Items.Add(FlowTypesGroup)
        End If

        'Stock Groups
        Dim StockGroupsGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("StockGroups", "Stock Groups", True)

        AddStockGroupMapVariables(project, StockGroupsGroup.Items)

        If (StockGroupsGroup.Items.Count > 0) Then
            layout.Items.Add(StockGroupsGroup)
        End If

        'Flow Groups
        Dim FlowGroupsGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("FlowGroups", "Flow Groups", True)

        AddFlowGroupMapVariables(project, FlowGroupsGroup.Items)

        If (FlowGroupsGroup.Items.Count > 0) Then
            layout.Items.Add(FlowGroupsGroup)
        End If

    End Sub

    Private Shared Sub AddStockTypeMapVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))
            Dim VarName As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", SPATIAL_MAP_STOCK_TYPE_VARIABLE_PREFIX, id)
            Dim Item As New SyncroSimLayoutItem(VarName, DisplayName, False)

            items.Add(Item)

        Next

    End Sub

    Private Shared Sub AddStockGroupMapVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_STOCK_GROUP_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))
            Dim VarName As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX, id)
            Dim Item As New SyncroSimLayoutItem(VarName, DisplayName, False)

            items.Add(Item)

        Next

    End Sub

    Private Shared Sub AddFlowTypeMapVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_FLOW_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))
            Dim VarName As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", SPATIAL_MAP_FLOW_TYPE_VARIABLE_PREFIX, id)

            Dim Item As New SyncroSimLayoutItem(VarName, DisplayName, False)
            items.Add(Item)

        Next

    End Sub

    Private Shared Sub AddFlowGroupMapVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_FLOW_GROUP_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))
            Dim VarName As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX, id)

            Dim Item As New SyncroSimLayoutItem(VarName, DisplayName, False)
            items.Add(Item)

        Next

    End Sub

End Class
