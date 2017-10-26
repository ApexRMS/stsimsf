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
Class OutputFlowDataSheet
    Inherits DataSheet

    Protected Overrides Sub OnDataFeedsRefreshed()

        MyBase.OnDataFeedsRefreshed()

        Dim s As String = Nothing
        Dim ss As String = Nothing
        Dim ts As String = Nothing

        GetStratumLabelStrings(Me.Project.GetDataSheet("STSim_Terminology"), s, ss, ts)

        Me.Columns(FROM_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "From {0}", s)
        Me.Columns(FROM_SECONDARY_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "From {0}", ss)
        Me.Columns(FROM_TERTIARY_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "From {0}", ts)
        Me.Columns(TO_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.InvariantCulture, "To {0}", s)

    End Sub

End Class
