'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

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

        GetStratumLabelStrings(Me.Project.GetDataSheet("STSim_Terminology"), s, ss)

        Me.Columns(FROM_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.CurrentCulture, "From {0}", s)
        Me.Columns(FROM_SECONDARY_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.CurrentCulture, "From {0}", ss)
        Me.Columns(TO_STRATUM_ID_COLUMN_NAME).DisplayName = String.Format(CultureInfo.CurrentCulture, "To {0}", s)

    End Sub

End Class
