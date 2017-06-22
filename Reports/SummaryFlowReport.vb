'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class SummaryFlowReport
    Inherits ExportTransformer

    Protected Overrides Sub Export(location As String, exportType As ExportType)
        Me.InternalExport(location, exportType, True)
    End Sub

    Friend Sub InternalExport(location As String, exportType As ExportType, ByVal showMessage As Boolean)

        Dim columns As ExportColumnCollection = Me.CreateColumnCollection()

        If (exportType = ExportType.ExcelFile) Then
            Me.ExcelExport(location, columns, Me.CreateReportQuery(False), "Flows")
        Else

            columns.Remove("ScenarioName")
            Me.CSVExport(location, columns, Me.CreateReportQuery(True))

            If (showMessage) Then
                FormsUtilities.InformationMessageBox("Data saved to '{0}'.", location)
            End If

        End If

    End Sub

    Private Function CreateColumnCollection() As ExportColumnCollection

        Dim c As New ExportColumnCollection()

        Dim dsterm As DataSheet = Me.Project.GetDataSheet(DATASHEET_TERMINOLOGY_NAME)
        Dim dstermSTSim As DataSheet = Me.Project.GetDataSheet("STSim_Terminology")
        Dim FlowUnits As String = GetTerminology(dsterm, STOCK_UNITS_COLUMN_NAME)
        Dim TimestepLabel As String = GetTimestepUnits(Me.Project)
        Dim PrimaryStratumLabel As String = Nothing
        Dim SecondaryStratumLabel As String = Nothing

        GetStratumLabelStrings(dstermSTSim, PrimaryStratumLabel, SecondaryStratumLabel)
        Dim TotalValue As String = String.Format(CultureInfo.InvariantCulture, "Total Value ({0})", FlowUnits)

        c.Add(New ExportColumn("ScenarioID", "Scenario ID"))
        c.Add(New ExportColumn("ScenarioName", "Scenario"))
        c.Add(New ExportColumn("Iteration", "Iteration"))
        c.Add(New ExportColumn("Timestep", TimestepLabel))
        c.Add(New ExportColumn("FromStratum", "From " & PrimaryStratumLabel))
        c.Add(New ExportColumn("FromSecondaryStratum", "From " & SecondaryStratumLabel))
        c.Add(New ExportColumn("FromStateClass", "From State Class"))
        c.Add(New ExportColumn("FromStock", "From Stock"))
        c.Add(New ExportColumn("TransitionType", "TransitionType"))
        c.Add(New ExportColumn("ToStratum", "To " & PrimaryStratumLabel))
        c.Add(New ExportColumn("ToStateClass", "To State Class"))
        c.Add(New ExportColumn("ToStock", "To Stock"))
        c.Add(New ExportColumn("FlowType", "Flow Type"))
        c.Add(New ExportColumn("Amount", TotalValue))

        c("Amount").DecimalPlaces = 2
        c("Amount").Alignment = Core.ColumnAlignment.Right

        Return c

    End Function

    Private Function CreateReportQuery(ByVal isCSV As Boolean) As String

        Dim ScenFilter As String = Me.CreateActiveResultScenarioFilter()

        If (isCSV) Then

            Return String.Format(CultureInfo.InvariantCulture,
                "SELECT " &
                "SF_OutputFlow.ScenarioID, " &
                "SF_OutputFlow.Iteration,  " &
                "SF_OutputFlow.Timestep,  " &
                "ST1.Name AS FromStratum, " &
                "SS1.Name AS FromSecondaryStratum, " &
                "SC1.Name AS FromStateClass, " &
                "STK1.Name AS FromStock, " &
                "STSim_TransitionType.Name AS TransitionType, " &
                "ST2.Name AS ToStratum, " &
                "SC2.Name AS ToStateClass, " &
                "STK2.Name AS ToStock, " &
                "SF_FlowType.Name as FlowType, " &
                "SF_OutputFlow.Amount " &
                "FROM SF_OutputFlow " &
                "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputFlow.FromStratumID " &
                "INNER JOIN STSim_Stratum AS ST2 ON ST2.StratumID = SF_OutputFlow.ToStratumID " &
                "LEFT JOIN STSim_SecondaryStratum AS SS1 ON SS1.SecondaryStratumID = SF_OutputFlow.FromSecondaryStratumID " &
                "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputFlow.FromStateClassID " &
                "INNER JOIN STSim_StateClass AS SC2 ON SC2.StateClassID = SF_OutputFlow.ToStateClassID " &
                "INNER JOIN SF_StockType AS STK1 ON STK1.StockTypeID = SF_OutputFlow.FromStockTypeID " &
                "INNER JOIN SF_StockType AS STK2 ON STK2.StockTypeID = SF_OutputFlow.ToStockTypeID " &
                "INNER JOIN SF_FlowType ON SF_FlowType.FlowTypeID = SF_OutputFlow.FlowTypeID " &
                "LEFT JOIN STSim_TransitionType ON STSim_TransitionType.TransitionTypeID = SF_OutputFlow.TransitionTypeID " &
                "WHERE SF_OutputFlow.ScenarioID IN ({0})  " &
                "ORDER BY " &
                "SF_OutputFlow.ScenarioID, " &
                "SF_OutputFlow.Iteration, " &
                "SF_OutputFlow.Timestep, " &
                "ST1.Name, " &
                "SS1.Name, " &
                "SC1.Name, " &
                "STK1.Name, " &
                "STSim_TransitionType.Name, " &
                "ST2.Name, " &
                "SC2.Name, " &
                "STK2.Name, " &
                "SF_FlowType.Name",
                ScenFilter)

        Else

            Return String.Format(CultureInfo.InvariantCulture,
                "SELECT " &
                "SF_OutputFlow.ScenarioID, " &
                "SSim_Scenario.Name AS ScenarioName, " &
                "SF_OutputFlow.Iteration,  " &
                "SF_OutputFlow.Timestep,  " &
                "ST1.Name AS FromStratum, " &
                "SS1.Name AS FromSecondaryStratum, " &
                "SC1.Name AS FromStateClass, " &
                "STK1.Name AS FromStock, " &
                "STSim_TransitionType.Name AS TransitionType, " &
                "ST2.Name AS ToStratum, " &
                "SC2.Name AS ToStateClass, " &
                "STK2.Name AS ToStock, " &
                "SF_FlowType.Name as FlowType, " &
                "SF_OutputFlow.Amount " &
                "FROM SF_OutputFlow " &
                "INNER JOIN SSim_Scenario ON SSim_Scenario.ScenarioID = SF_OutputFlow.ScenarioID " &
                "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputFlow.FromStratumID " &
                "INNER JOIN STSim_Stratum AS ST2 ON ST2.StratumID = SF_OutputFlow.ToStratumID " &
                "LEFT JOIN STSim_SecondaryStratum AS SS1 ON SS1.SecondaryStratumID = SF_OutputFlow.FromSecondaryStratumID " &
                "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputFlow.FromStateClassID " &
                "INNER JOIN STSim_StateClass AS SC2 ON SC2.StateClassID = SF_OutputFlow.ToStateClassID " &
                "INNER JOIN SF_StockType AS STK1 ON STK1.StockTypeID = SF_OutputFlow.FromStockTypeID " &
                "INNER JOIN SF_StockType AS STK2 ON STK2.StockTypeID = SF_OutputFlow.ToStockTypeID " &
                "INNER JOIN SF_FlowType ON SF_FlowType.FlowTypeID = SF_OutputFlow.FlowTypeID " &
                "LEFT JOIN STSim_TransitionType ON STSim_TransitionType.TransitionTypeID = SF_OutputFlow.TransitionTypeID " &
                "WHERE SF_OutputFlow.ScenarioID IN ({0})  " &
                "ORDER BY " &
                "SF_OutputFlow.ScenarioID, " &
                "SSim_Scenario.Name, " &
                "SF_OutputFlow.Iteration, " &
                "SF_OutputFlow.Timestep, " &
                "ST1.Name, " &
                "SS1.Name, " &
                "SC1.Name, " &
                "STK1.Name, " &
                "STSim_TransitionType.Name, " &
                "ST2.Name, " &
                "SC2.Name, " &
                "STK2.Name, " &
                "SF_FlowType.Name",
                ScenFilter)

        End If

    End Function

End Class
