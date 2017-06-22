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
Class SummaryStockReport
    Inherits ExportTransformer

    Protected Overrides Sub Export(location As String, exportType As ExportType)
        Me.InternalExport(location, exportType, True)
    End Sub

    Friend Sub InternalExport(location As String, exportType As ExportType, ByVal showMessage As Boolean)

        Dim columns As ExportColumnCollection = Me.CreateColumnCollection()

        If (exportType = ExportType.ExcelFile) Then
            Me.ExcelExport(location, columns, Me.CreateReportQuery(False), "Stocks")
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
        Dim StockUnits As String = GetTerminology(dsterm, STOCK_UNITS_COLUMN_NAME)
        Dim TimestepLabel As String = GetTimestepUnits(Me.Project)
        Dim PrimaryStratumLabel As String = Nothing
        Dim SecondaryStratumLabel As String = Nothing

        GetStratumLabelStrings(dstermSTSim, PrimaryStratumLabel, SecondaryStratumLabel)
        Dim TotalValue As String = String.Format(CultureInfo.InvariantCulture, "Total Value ({0})", StockUnits)

        c.Add(New ExportColumn("ScenarioID", "Scenario ID"))
        c.Add(New ExportColumn("ScenarioName", "Scenario"))
        c.Add(New ExportColumn("Iteration", "Iteration"))
        c.Add(New ExportColumn("Timestep", TimestepLabel))
        c.Add(New ExportColumn("Stratum", PrimaryStratumLabel))
        c.Add(New ExportColumn("SecondaryStratum", SecondaryStratumLabel))
        c.Add(New ExportColumn("StateClass", "State Class"))
        c.Add(New ExportColumn("StockType", "Stock Type"))
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
                "SF_OutputStock.ScenarioID, " &
                "SF_OutputStock.Iteration,  " &
                "SF_OutputStock.Timestep,  " &
                "ST1.Name AS Stratum,  " &
                "ST2.Name AS SecondaryStratum,  " &
                "SC1.Name AS StateClass,  " &
                "SF_StockType.Name as StockType, " &
                "SF_OutputStock.Amount " &
                "FROM SF_OutputStock " &
                "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputStock.StratumID " &
                "LEFT JOIN STSim_SecondaryStratum AS ST2 ON ST2.SecondaryStratumID = SF_OutputStock.SecondaryStratumID " &
                "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputStock.StateClassID " &
                "INNER JOIN SF_StockType ON SF_StockType.StockTypeID = SF_OutputStock.StockTypeID " &
                "WHERE SF_OutputStock.ScenarioID IN ({0})  " &
                "ORDER BY " &
                "SF_OutputStock.ScenarioID, " &
                "SF_OutputStock.Iteration, " &
                "SF_OutputStock.Timestep, " &
                "ST1.Name, " &
                "ST2.Name, " &
                "SC1.Name, " &
                "SF_StockType.Name",
                ScenFilter)

        Else

            Return String.Format(CultureInfo.InvariantCulture,
                "SELECT " &
                "SF_OutputStock.ScenarioID, " &
                "SSim_Scenario.Name AS ScenarioName,  " &
                "SF_OutputStock.Iteration,  " &
                "SF_OutputStock.Timestep,  " &
                "ST1.Name AS Stratum,  " &
                "ST2.Name AS SecondaryStratum,  " &
                "SC1.Name AS StateClass,  " &
                "SF_StockType.Name as StockType, " &
                "SF_OutputStock.Amount " &
                "FROM SF_OutputStock " &
                "INNER JOIN SSim_Scenario ON SSim_Scenario.ScenarioID = SF_OutputStock.ScenarioID " &
                "INNER JOIN STSim_Stratum AS ST1 ON ST1.StratumID = SF_OutputStock.StratumID " &
                "LEFT JOIN STSim_SecondaryStratum AS ST2 ON ST2.SecondaryStratumID = SF_OutputStock.SecondaryStratumID " &
                "INNER JOIN STSim_StateClass AS SC1 ON SC1.StateClassID = SF_OutputStock.StateClassID " &
                "INNER JOIN SF_StockType ON SF_StockType.StockTypeID = SF_OutputStock.StockTypeID " &
                "WHERE SF_OutputStock.ScenarioID IN ({0})  " &
                "ORDER BY " &
                "SF_OutputStock.ScenarioID, " &
                "SSim_Scenario.Name, " &
                "SF_OutputStock.Iteration, " &
                "SF_OutputStock.Timestep, " &
                "ST1.Name, " &
                "ST2.Name, " &
                "SC1.Name, " &
                "SF_StockType.Name",
                ScenFilter)

        End If

    End Function

End Class
