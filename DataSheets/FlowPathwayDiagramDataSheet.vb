'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class FlowPathwayDiagramDataSheet
    Inherits DataSheet

    Public Const ERROR_INVALID_CELL_ADDRESS As String =
        "The value must be a valid cell address (a valid cell address is a letter from 'A' to 'Z' followed by a number from 1 to 255.  Example: 'A25')"

    Public Overrides Sub Validate(ByVal proposedValue As Object, columnName As String)

        MyBase.Validate(proposedValue, columnName)

        If (columnName = LOCATION_COLUMN_NAME) Then

            If (Not IsValidLocation(proposedValue)) Then
                Throw New DataException(ERROR_INVALID_CELL_ADDRESS)
            End If

        End If

    End Sub

    Public Overrides Function GetDeleteRowsConfirmation() As String

        Dim m As String = MyBase.GetDeleteRowsConfirmation()

        If (String.IsNullOrWhiteSpace(m)) Then
            m = "Associated flows will also be deleted.  Continue?"
        End If

        Return m

    End Function

    Protected Overrides Sub OnRowsDeleted(sender As Object, e As SyncroSim.Core.DataSheetRowEventArgs)

        Dim DeletedRows As Boolean = False
        Dim RemainingStockTypes As Dictionary(Of Integer, Boolean) = CreateRecordLookup(Me, STOCK_TYPE_ID_COLUMN_NAME)
        Dim FlowPathwaySheet As DataSheet = Me.GetDataSheet(DATASHEET_FLOW_PATHWAY_NAME)
        Dim FlowPathwayData As DataTable = FlowPathwaySheet.GetData()

        For i As Integer = FlowPathwayData.Rows.Count - 1 To 0 Step -1

            Dim dr As DataRow = FlowPathwayData.Rows(i)

            If (dr.RowState = DataRowState.Deleted) Then
                Continue For
            End If

            Dim FromStockTypeId As Integer = CInt(dr(FROM_STOCK_TYPE_ID_COLUMN_NAME))
            Dim ToStockTypeId As Integer = CInt(dr(TO_STOCK_TYPE_ID_COLUMN_NAME))

            If ((Not RemainingStockTypes.ContainsKey(FromStockTypeId)) Or
                (Not RemainingStockTypes.ContainsKey(ToStockTypeId))) Then

                DataTableUtilities.DeleteTableRow(FlowPathwayData, dr)
                DeletedRows = True

            End If

        Next

        If (DeletedRows) Then

            FlowPathwaySheet.Changes.Add(
                New ChangeRecord(Me, "Diagram data deleted rows"))

        End If

        MyBase.OnRowsDeleted(sender, e)

    End Sub

    Private Shared Function IsValidLocation(ByVal proposedLocation As Object) As Boolean

        If (proposedLocation Is Nothing) Then
            Return False
        End If

        Dim Location As String = CStr(proposedLocation)

        If (String.IsNullOrEmpty(Location)) Then
            Return False
        End If

        Dim LocUpper As String = Location.ToUpper(CultureInfo.CurrentCulture).Trim

        If (String.IsNullOrEmpty(LocUpper)) Then
            Return False
        End If

        If (LocUpper.Length < 2) Then
            Return False
        End If

        Dim CharPart As String = Mid(LocUpper, 1, 1)
        Dim NumPart As String = Mid(LocUpper, 2, LocUpper.Length - 1)

        If (String.IsNullOrEmpty(CharPart) Or String.IsNullOrEmpty(NumPart)) Then
            Return False
        End If

        If (Asc(CharPart) < Asc("A") Or Asc(CharPart) > Asc("Z")) Then
            Return False
        End If

        Dim n As Integer = 0
        If (Not Integer.TryParse(NumPart, n)) Then
            Return False
        End If

        If (n <= 0 Or n > 256) Then
            Return False
        End If

        Return True

    End Function

End Class
