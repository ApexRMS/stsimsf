'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.Core.Forms

Partial Class SFConsole

    Private Sub HandleCreateReportArgument()

        If (Me.Help) Then
            PrintCreateReportHelp()
        Else
            Me.CreateReport()
        End If

    End Sub

    Private Sub CreateReport()

        Dim n As String = Me.GetReportName()
        Dim f As String = Me.GetOutputFileName()
        Dim l As Library = Me.OpenLibrary()
        Dim sids As IEnumerable(Of Integer) = Me.GetMultiDatabaseIdArguments("sids")

        ValidateScenarios(sids, l)

        Dim p As Project = Me.ConfigureActiveProject(sids, l)

        If (n = STOCK_REPORT_NAME) Then

            Dim t As SummaryStockReport = CType(Me.Session.CreateTransformer(
                "stsim-stockflow:summary-stock-report", p.Library, p, Nothing), SummaryStockReport)

            t.InternalExport(f, ExportType.CSVFile, False)

        ElseIf (n = FLOW_REPORT_NAME) Then

            Dim t As SummaryFlowReport = CType(Me.Session.CreateTransformer(
                "stsim-stockflow:summary-flow-report", p.Library, p, Nothing), SummaryFlowReport)

            t.InternalExport(f, ExportType.CSVFile, False)

        End If

    End Sub

    Private Function GetReportName() As String

        Dim n As String = Me.GetRequiredArgument("name")

        If (n <> STOCK_REPORT_NAME And n <> FLOW_REPORT_NAME) Then
            ExceptionUtils.ThrowArgumentException("The report name is not valid.")
        End If

        Return n

    End Function

    Private Shared Sub ValidateScenarios(ByVal sids As IEnumerable(Of Integer), ByVal l As Library)

        Dim pids As New Dictionary(Of Integer, Boolean)

        For Each id As Integer In sids

            If (Not l.Scenarios.Contains(id)) Then
                ExceptionUtils.ThrowArgumentException("The scenario does not exist: {0}", id)
            End If

            Dim s As Scenario = l.Scenarios(id)

            If (Not s.IsResult) Then
                ExceptionUtils.ThrowArgumentException("The scenario is not a result scenario: {0}", id)
            End If

            If (Not pids.ContainsKey(s.Project.Id)) Then
                pids.Add(s.Project.Id, True)
            End If

            If (pids.Count > 1) Then
                ExceptionUtils.ThrowArgumentException("The scenarios must belong to the same project: {0}", id)
            End If

        Next

    End Sub

    Private Function ConfigureActiveProject(ByVal sids As IEnumerable(Of Integer), ByVal l As Library) As Project

        Dim p As Project = l.Scenarios(sids.First).Project
        Me.Session.SetActiveProject(p)

        For Each id As Integer In sids

            Dim s As Scenario = l.Scenarios(id)
            s.IsActive = True

            p.Results.Add(s)

        Next

        Return p

    End Function

    Private Shared Sub PrintCreateReportHelp()

        System.Console.WriteLine("Creates a Stocks and Flows Report")
        System.Console.WriteLine("USAGE: --create-report [Arguments]")
        System.Console.WriteLine()
        System.Console.WriteLine("  --lib={name}           The library file name")
        System.Console.WriteLine("  --sids={ids}           The scenario IDs separated by commas")
        System.Console.WriteLine("  --name={name}          The name of the report to create")
        System.Console.WriteLine("  --file={name}          The file name for the report")
        System.Console.WriteLine()
        System.Console.WriteLine("Examples:")
        System.Console.WriteLine("  --create-report --lib=test.ssim --sids=1,2,3 --name=stocks --file=stocks.csv")
        System.Console.WriteLine("  --create-report --lib=""my lib.ssim"" --sids=1 --name=flows --file=""my data.csv""")

    End Sub

End Class
