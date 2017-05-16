'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Text
Imports System.Reflection
Imports System.Globalization
Imports SyncroSim.Core
Imports SyncroSim.StochasticTime.Forms

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class SFChartProvider
    Inherits ChartProvider

    Const DENSITY_GROUP_NAME As String = "stockflow_density_group"

    Public Overrides Function GetData(
        ByVal store As DataStore,
        ByVal descriptor As ChartDescriptor,
        ByVal dataSheet As DataSheet) As DataTable

        If (descriptor.DataSheetName = "SF_OutputStock" Or
            descriptor.DataSheetName = "SF_OutputFlow") Then

            Dim v() As String = descriptor.VariableName.Split(CChar("-"))
            Dim VarName As String = v(0)

            If (VarName = "stocktype" Or
                VarName = "stocktypedensity" Or
                VarName = "flowtype" Or
                VarName = "flowtypedensity" Or
                VarName = "stockgroup" Or
                VarName = "stockgroupdensity" Or
                VarName = "flowgroup" Or
                VarName = "flowgroupdensity") Then

                Return CreateRawChartData(dataSheet, descriptor, store, VarName, CInt(v(1)))

            End If

        End If

        Return Nothing

    End Function

    Public Overrides Sub RefreshCriteria(layout As SyncroSimLayout, project As Project)

        'Stock Types
        Dim StockTypesGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("StockTypes", "Stock Types", True)

        StockTypesGroup.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
        StockTypesGroup.Properties.Add(New MetaDataProperty("column", "Amount"))
        StockTypesGroup.Properties.Add(New MetaDataProperty("filter", "StratumID|SecondaryStratumID|StateClassID"))

        AddStockTypeChartVariables(project, StockTypesGroup.Items)

        If (StockTypesGroup.Items.Count > 0) Then
            layout.Items.Add(StockTypesGroup)
        End If

        'Stock Groups
        Dim StockGroupsGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("StockGroups", "Stock Groups", True)

        StockGroupsGroup.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
        StockGroupsGroup.Properties.Add(New MetaDataProperty("column", "Amount"))
        StockGroupsGroup.Properties.Add(New MetaDataProperty("filter", "StratumID|SecondaryStratumID|StateClassID"))

        AddStockGroupChartVariables(project, StockGroupsGroup.Items)

        If (StockGroupsGroup.Items.Count > 0) Then
            layout.Items.Add(StockGroupsGroup)
        End If

        'Flow Types
        Dim FlowTypesGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("FlowTypes", "Flow Types", True)

        FlowTypesGroup.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
        FlowTypesGroup.Properties.Add(New MetaDataProperty("column", "Amount"))
        FlowTypesGroup.Properties.Add(New MetaDataProperty("filter", "FromStratumID|FromSecondaryStratumID|FromStateClassID|FromStockTypeID|TransitionTypeID|ToStratumID|ToStateClassID|ToStockTypeID"))

        AddFlowTypeChartVariables(project, FlowTypesGroup.Items)

        If (FlowTypesGroup.Items.Count > 0) Then
            layout.Items.Add(FlowTypesGroup)
        End If

        'Flow Groups
        Dim FlowGroupsGroup As SyncroSimLayoutItem = New SyncroSimLayoutItem("FlowGroups", "Flow Groups", True)

        FlowGroupsGroup.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
        FlowGroupsGroup.Properties.Add(New MetaDataProperty("column", "Amount"))
        FlowGroupsGroup.Properties.Add(New MetaDataProperty("filter", "FromStratumID|FromSecondaryStratumID|FromStateClassID|FromStockTypeID|TransitionTypeID|ToStratumID|ToStateClassID|ToStockTypeID"))

        AddFlowGroupChartVariables(project, FlowGroupsGroup.Items)

        If (FlowGroupsGroup.Items.Count > 0) Then
            layout.Items.Add(FlowGroupsGroup)
        End If

    End Sub

    Private Shared Sub AddStockTypeChartVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        Dim DensityGroup As New SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", True)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))

            'Normal
            Dim VarNameNormal As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stocktype", id)
            Dim ItemNormal As New SyncroSimLayoutItem(VarNameNormal, DisplayName, False)

            ItemNormal.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
            ItemNormal.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemNormal.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemNormal.Properties.Add(New MetaDataProperty("customTitle", "Stock Type " & DisplayName))
            ItemNormal.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            items.Add(ItemNormal)

            'Density
            Dim VarNameDensity As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stocktypedensity", id)
            Dim ItemDensity As New SyncroSimLayoutItem(VarNameDensity, DisplayName, False)

            ItemDensity.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
            ItemDensity.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemDensity.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemDensity.Properties.Add(New MetaDataProperty("customTitle", "(Density): Stock Type " & DisplayName))
            ItemDensity.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            DensityGroup.Items.Add(ItemDensity)

        Next

        If (DensityGroup.Items.Count > 0) Then
            items.Add(DensityGroup)
        End If

    End Sub

    Private Shared Sub AddStockGroupChartVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_STOCK_GROUP_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        Dim DensityGroup As New SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", True)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))

            'Normal
            Dim VarNameNormal As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stockgroup", id)
            Dim ItemNormal As New SyncroSimLayoutItem(VarNameNormal, DisplayName, False)

            ItemNormal.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
            ItemNormal.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemNormal.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemNormal.Properties.Add(New MetaDataProperty("customTitle", "Stock Group " & DisplayName))
            ItemNormal.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            items.Add(ItemNormal)

            'Density
            Dim VarNameDensity As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "stockgroupdensity", id)
            Dim ItemDensity As New SyncroSimLayoutItem(VarNameDensity, DisplayName, False)

            ItemDensity.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputStock"))
            ItemDensity.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemDensity.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemDensity.Properties.Add(New MetaDataProperty("customTitle", "(Density): Stock Group " & DisplayName))
            ItemDensity.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            DensityGroup.Items.Add(ItemDensity)

        Next

        If (DensityGroup.Items.Count > 0) Then
            items.Add(DensityGroup)
        End If

    End Sub

    Private Shared Sub AddFlowTypeChartVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_FLOW_TYPE_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        Dim DensityGroup As New SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", True)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))

            'Normal
            Dim VarNameNormal As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowtype", id)
            Dim ItemNormal As New SyncroSimLayoutItem(VarNameNormal, DisplayName, False)

            ItemNormal.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
            ItemNormal.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemNormal.Properties.Add(New MetaDataProperty("skipTimestepZero", "True"))
            ItemNormal.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemNormal.Properties.Add(New MetaDataProperty("customTitle", "Flow Type " & DisplayName))
            ItemNormal.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            items.Add(ItemNormal)

            'Density
            Dim VarNameDensity As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowtypedensity", id)
            Dim ItemDensity As New SyncroSimLayoutItem(VarNameDensity, DisplayName, False)

            ItemDensity.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
            ItemDensity.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemDensity.Properties.Add(New MetaDataProperty("skipTimestepZero", "True"))
            ItemDensity.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemDensity.Properties.Add(New MetaDataProperty("customTitle", "(Density): Flow Type " & DisplayName))
            ItemDensity.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            DensityGroup.Items.Add(ItemDensity)

        Next

        If (DensityGroup.Items.Count > 0) Then
            items.Add(DensityGroup)
        End If

    End Sub

    Private Shared Sub AddFlowGroupChartVariables(
        ByVal project As Project,
        ByVal items As SyncroSimLayoutItemCollection)

        Dim ds As DataSheet = project.GetDataSheet(DATASHEET_FLOW_GROUP_NAME)
        Dim dt As DataTable = ds.GetData()

        Dim dv As New DataView(
            dt,
            Nothing,
            ds.ValidationTable.DisplayMember,
            DataViewRowState.CurrentRows)

        Dim DensityGroup As New SyncroSimLayoutItem(DENSITY_GROUP_NAME, "Density", True)

        For Each drv As DataRowView In dv

            Dim id As Integer = CInt(drv.Row(ds.ValidationTable.ValueMember))
            Dim DisplayName As String = CStr(drv.Row(ds.ValidationTable.DisplayMember))

            'Normal
            Dim VarNameNormal As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowgroup", id)
            Dim ItemNormal As New SyncroSimLayoutItem(VarNameNormal, DisplayName, False)

            ItemNormal.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
            ItemNormal.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemNormal.Properties.Add(New MetaDataProperty("skipTimestepZero", "True"))
            ItemNormal.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemNormal.Properties.Add(New MetaDataProperty("customTitle", "Flow Group " & DisplayName))
            ItemNormal.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            items.Add(ItemNormal)

            'Density
            Dim VarNameDensity As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", "flowgroupdensity", id)
            Dim ItemDensity As New SyncroSimLayoutItem(VarNameDensity, DisplayName, False)

            ItemDensity.Properties.Add(New MetaDataProperty("dataSheet", "SF_OutputFlow"))
            ItemDensity.Properties.Add(New MetaDataProperty("column", "Amount"))
            ItemDensity.Properties.Add(New MetaDataProperty("skipTimestepZero", "True"))
            ItemDensity.Properties.Add(New MetaDataProperty("prefixFolderName", "False"))
            ItemDensity.Properties.Add(New MetaDataProperty("customTitle", "(Density): Flow Group " & DisplayName))
            ItemDensity.Properties.Add(New MetaDataProperty("defaultValue", "0.0"))

            DensityGroup.Items.Add(ItemDensity)

        Next

        If (DensityGroup.Items.Count > 0) Then
            items.Add(DensityGroup)
        End If

    End Sub

    Private Shared Function CreateRawChartData(
        ByVal dataSheet As DataSheet,
        ByVal descriptor As ChartDescriptor,
        ByVal store As DataStore,
        ByVal variableName As String,
        ByVal variableId As Integer) As DataTable

        Dim query As String = Nothing

        If (variableName = "stocktype" Or
            variableName = "stocktypedensity" Or
            variableName = "flowtype" Or
            variableName = "flowtypedensity") Then

            query = CreateRawChartDataQueryForType(dataSheet, descriptor, variableName, variableId)

        Else

            Debug.Assert(
                variableName = "stockgroup" Or
                variableName = "stockgroupdensity" Or
                variableName = "flowgroup" Or
                variableName = "flowgroupdensity")

            query = CreateRawChartDataQueryForGroup(dataSheet, descriptor, variableName, variableId)

        End If

        Dim dt As DataTable = store.CreateDataTableFromQuery(query, "RawData")

        If (variableName.EndsWith("density", StringComparison.Ordinal)) Then

            Dim dict As Dictionary(Of String, Double) =
                CreateAmountDictionary(dataSheet.Scenario, descriptor, variableName, store)

            If (dict.Count > 0) Then

                For Each dr As DataRow In dt.Rows

                    Dim it As Integer = CInt(dr("Iteration"))
                    Dim ts As Integer = CInt(dr("Timestep"))

                    Dim k As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", it, ts)
                    dr("SumOfAmount") = CDbl(dr("SumOfAmount")) / dict(k)

                Next

            End If

        End If

        Return dt

    End Function

    Private Shared Function CreateRawChartDataQueryForType(
        ByVal dataSheet As DataSheet,
        ByVal descriptor As ChartDescriptor,
        ByVal variableName As String,
        ByVal variableId As Integer) As String

        Debug.Assert(
            variableName = "stocktype" Or
            variableName = "stocktypedensity" Or
            variableName = "flowtype" Or
            variableName = "flowtypedensity")

        Dim TypeColumnName As String = Nothing

        If (variableName = "stocktype" Or variableName = "stocktypedensity") Then
            TypeColumnName = STOCK_TYPE_ID_COLUMN_NAME
        Else
            TypeColumnName = FLOW_TYPE_ID_COLUMN_NAME
        End If

        Dim ScenarioClause As String = String.Format(CultureInfo.InvariantCulture,
            "([{0}]={1})", SCENARIO_ID_COLUMN_NAME, dataSheet.Scenario.Id)

        Dim WhereClause As String = String.Format(CultureInfo.InvariantCulture,
            "{0} AND ([{1}]={2})", ScenarioClause, TypeColumnName, variableId)

        If (Not String.IsNullOrEmpty(descriptor.DisaggregateFilter)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, descriptor.DisaggregateFilter)
        End If

        If (Not String.IsNullOrEmpty(descriptor.IncludeDataFilter)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, descriptor.IncludeDataFilter)
        End If

        Dim query As String = String.Format(CultureInfo.InvariantCulture,
            "SELECT Iteration, Timestep, SUM(Amount) AS SumOfAmount FROM {0} WHERE ({1}) GROUP BY Iteration, Timestep",
            dataSheet.Name, WhereClause)

        Return query

    End Function

    Private Shared Function CreateRawChartDataQueryForGroup(
        ByVal dataSheet As DataSheet,
        ByVal descriptor As ChartDescriptor,
        ByVal variableName As String,
        ByVal variableId As Integer) As String

        Debug.Assert(
            variableName = "stockgroup" Or
            variableName = "stockgroupdensity" Or
            variableName = "flowgroup" Or
            variableName = "flowgroupdensity")

        Dim GroupColumnName As String = Nothing
        Dim JoinColumnName As String = Nothing
        Dim TypeGroupTableName As String = Nothing

        If (variableName = "stockgroup" Or variableName = "stockgroupdensity") Then

            GroupColumnName = STOCK_GROUP_ID_COLUMN_NAME
            JoinColumnName = STOCK_TYPE_ID_COLUMN_NAME
            TypeGroupTableName = DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME

        Else

            GroupColumnName = FLOW_GROUP_ID_COLUMN_NAME
            JoinColumnName = FLOW_TYPE_ID_COLUMN_NAME
            TypeGroupTableName = DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME

        End If

        Dim ScenarioClause As String = String.Format(CultureInfo.InvariantCulture,
            "({0}.{1}={2})", dataSheet.Name, SCENARIO_ID_COLUMN_NAME, dataSheet.Scenario.Id)

        Dim WhereClause As String = String.Format(CultureInfo.InvariantCulture,
            "{0} AND ({1}.{2}={3})", ScenarioClause, TypeGroupTableName, GroupColumnName, variableId)

        If (Not String.IsNullOrEmpty(descriptor.DisaggregateFilter)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, descriptor.DisaggregateFilter)
        End If

        If (Not String.IsNullOrEmpty(descriptor.IncludeDataFilter)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, descriptor.IncludeDataFilter)
        End If

        Dim query As String = String.Format(CultureInfo.InvariantCulture,
            "SELECT Iteration, Timestep, Sum({0}.Amount * CASE WHEN {1}.Value IS NULL THEN 1.0 ELSE {2}.Value END) AS SumOfAmount " &
            "FROM {3} INNER JOIN {4} ON {5}.{6} = {7}.{8} AND {9}.ScenarioID = {10}.ScenarioID " &
            "WHERE ({11}) GROUP BY Iteration, Timestep",
            dataSheet.Name, TypeGroupTableName, TypeGroupTableName, dataSheet.Name, TypeGroupTableName, dataSheet.Name, JoinColumnName, TypeGroupTableName,
            JoinColumnName, dataSheet.Name, TypeGroupTableName, WhereClause)

        Return query

    End Function

    Public Shared Function CreateAmountDictionary(
        ByVal scenario As Scenario,
        ByVal descriptor As ChartDescriptor,
        ByVal variableName As String,
        ByVal store As DataStore) As Dictionary(Of String, Double)

        Dim dict As New Dictionary(Of String, Double)
        Dim query As String = CreateAmountQuery(scenario, descriptor, variableName)
        Dim dt As DataTable = store.CreateDataTableFromQuery(query, "AmountData")

        For Each dr As DataRow In dt.Rows

            Dim it As Integer = CInt(dr("Iteration"))
            Dim ts As Integer = CInt(dr("Timestep"))

            Dim k As String = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", it, ts)
            dict.Add(k, CDbl(dr("SumOfAmount")))

        Next

        Return dict

    End Function

    Private Shared Function CreateAmountQuery(
        ByVal scenario As Scenario,
        ByVal descriptor As ChartDescriptor,
        ByVal variableName As String) As String

        Debug.Assert(variableName.EndsWith("density", StringComparison.Ordinal))

        Dim ScenarioClause As String = String.Format(CultureInfo.InvariantCulture,
            "([{0}]={1})", SCENARIO_ID_COLUMN_NAME, scenario.Id)

        Dim WhereClause As String = ScenarioClause

        Dim Disagg As String = RemoveUnwantedColumnReferences(descriptor.DisaggregateFilter, variableName)
        Dim IncData As String = RemoveUnwantedColumnReferences(descriptor.IncludeDataFilter, variableName)

        If (Not String.IsNullOrEmpty(Disagg)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, Disagg)
        End If

        If (Not String.IsNullOrEmpty(IncData)) Then
            WhereClause = String.Format(CultureInfo.InvariantCulture, "{0} AND ({1})", WhereClause, IncData)
        End If

        Dim query As String = String.Format(CultureInfo.InvariantCulture,
            "SELECT Iteration, Timestep, SUM(Amount) AS SumOfAmount FROM STSim_OutputStratum WHERE ({0}) GROUP BY Iteration, Timestep",
            WhereClause)

        Return query

    End Function

    Private Shared Function RemoveUnwantedColumnReferences(ByVal filter As String, ByVal variableName As String) As String

        If (filter Is Nothing) Then
            Return Nothing
        End If

        Dim AndSplit() As String = filter.Split({" AND "}, StringSplitOptions.None)
        Dim sb As New StringBuilder

        If (variableName.StartsWith("flow", StringComparison.Ordinal)) Then

            For Each s As String In AndSplit

                If (s.Contains("FromStratumID")) Then

                    s = s.Replace("FromStratumID", "StratumID")
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", s)

                ElseIf (s.Contains("FromSecondaryStratumID")) Then

                    s = s.Replace("FromSecondaryStratumID", "SecondaryStratumID")
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", s)

                End If

            Next

        Else

            For Each s As String In AndSplit

                If (s.Contains("StratumID") Or
                    s.Contains("SecondaryStratumID")) Then

                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} AND ", s)

                End If

            Next

        End If

        Dim final As String = sb.ToString

        If (final.Count > 0) Then

            Debug.Assert(final.Count >= 5)
            Return Mid(final, 1, final.Length - 5)

        Else
            Return Nothing
        End If

    End Function

End Class
