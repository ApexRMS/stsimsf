'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Module TerminologyUtilities

    Public Function GetTerminology(
        ByVal terminologySheet As DataSheet,
        ByVal columnName As String) As String

        Dim dr As DataRow = terminologySheet.GetDataRow()

        If (dr Is Nothing OrElse dr(columnName) Is DBNull.Value) Then
            Return "Units"
        Else
            Return CStr(dr(columnName))
        End If

    End Function

    Public Function GetTimestepUnits(ByVal project As Project) As String

        Dim dr As DataRow = project.GetDataSheet("STSim_Terminology").GetDataRow()

        If (dr Is Nothing OrElse dr("TimestepUnits") Is DBNull.Value) Then
            Return "Timestep"
        Else
            Return CStr(dr("TimestepUnits"))
        End If

    End Function

    Public Sub GetStratumLabelStrings(
        ByVal terminologyDataSheet As DataSheet,
        ByRef primaryStratumLabel As String,
        ByRef secondaryStratumLabel As String)

        Dim dr As DataRow = terminologyDataSheet.GetDataRow()

        primaryStratumLabel = "Stratum"
        secondaryStratumLabel = "Secondary Stratum"

        If (dr IsNot Nothing) Then

            If (dr("PrimaryStratumLabel") IsNot DBNull.Value) Then
                primaryStratumLabel = CStr(dr("PrimaryStratumLabel"))
            End If

            If (dr("SecondaryStratumLabel") IsNot DBNull.Value) Then
                secondaryStratumLabel = CStr(dr("SecondaryStratumLabel"))
            End If

        End If

    End Sub

End Module
