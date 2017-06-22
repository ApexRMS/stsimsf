'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class FlowPathwayDataSheet
    Inherits DataSheet

    Protected Overrides Sub OnDataSheetChanged(e As DataSheetMonitorEventArgs)

        MyBase.OnDataSheetChanged(e)

        Dim p As String = e.GetValue("PrimaryStratumLabel", "Stratum")

        Me.Columns(FROM_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "From {0}", p)
        Me.Columns(TO_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "To {0}", p)

    End Sub

    Public Overrides Sub Validate(proposedRow As DataRow, transferMethod As DataTransferMethod)

        MyBase.Validate(proposedRow, transferMethod)

        Dim DiagramSheet As DataSheet = Me.GetDataSheet(DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME)
        Dim StockTypes As Dictionary(Of Integer, Boolean) = CreateRecordLookup(DiagramSheet, STOCK_TYPE_ID_COLUMN_NAME)

        Dim FromStockTypeId As Integer = CInt(proposedRow(FROM_STOCK_TYPE_ID_COLUMN_NAME))

        If (Not StockTypes.ContainsKey(FromStockTypeId)) Then
            Throw New DataException("The 'From Stock' does not exist for this scenario.")
        End If

        Dim ToStockTypeId As Integer = CInt(proposedRow(TO_STOCK_TYPE_ID_COLUMN_NAME))

        If (Not StockTypes.ContainsKey(ToStockTypeId)) Then
            Throw New DataException("The 'To Stock' does not exist for this scenario.")
        End If

    End Sub

    Public Overrides Sub Validate(proposedData As DataTable, transferMethod As DataTransferMethod)

        MyBase.Validate(proposedData, transferMethod)

        Dim StockTypeSheet As DataSheet = Me.Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim DiagramSheet As DataSheet = Me.GetDataSheet(DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME)
        Dim StockTypes As Dictionary(Of Integer, Boolean) = CreateRecordLookup(DiagramSheet, STOCK_TYPE_ID_COLUMN_NAME)

        For Each dr As DataRow In proposedData.Rows

            Dim FromStockTypeId As Integer = CInt(dr(FROM_STOCK_TYPE_ID_COLUMN_NAME))

            If (Not StockTypes.ContainsKey(FromStockTypeId)) Then

                Dim StockTypeName As String = CStr(DataTableUtilities.GetTableValue(StockTypeSheet.GetData(), StockTypeSheet.ValueMember, FromStockTypeId, StockTypeSheet.DisplayMember))
                Throw New DataException(String.Format(CultureInfo.InvariantCulture, "Cannot import flow pathways because the 'From Stock' does not exist in this scenario: {0}", StockTypeName))

            End If

            Dim ToStockTypeId As Integer = CInt(dr(TO_STOCK_TYPE_ID_COLUMN_NAME))

            If (Not StockTypes.ContainsKey(ToStockTypeId)) Then

                Dim StockTypeName As String = CStr(DataTableUtilities.GetTableValue(StockTypeSheet.GetData(), StockTypeSheet.ValueMember, ToStockTypeId, StockTypeSheet.DisplayMember))
                Throw New DataException(String.Format(CultureInfo.InvariantCulture, "Cannot import flow pathways because the 'To Stock' does not exist in this scenario: {0}", StockTypeName))

            End If

        Next

    End Sub

End Class
