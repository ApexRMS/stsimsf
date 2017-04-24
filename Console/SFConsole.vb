'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports System.Reflection
Imports SyncroSim.Core

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class SFConsole
    Inherits SyncroSimConsole

    Public Sub New(ByVal session As Session, ByVal args As String())
        MyBase.New(session, args)
    End Sub

    Public Overrides Sub Execute()

        If (Me.GetArguments.Count = 1) Then
            System.Console.WriteLine("Use the --help switch to see available options.")
            Return
        End If

        If (Me.IsSwitchArgument("list-reports")) Then
            Me.HandleListReportsArgument()
        ElseIf (Me.IsSwitchArgument("create-report")) Then
            Me.HandleCreateReportArgument()
        Else
            If (Me.Help) Then
                PrintConsoleHelp()
            End If
        End If

    End Sub

    Private Shared Sub PrintConsoleHelp()

        System.Console.WriteLine("Stocks and Flows Console [Arguments]")
        System.Console.WriteLine()
        System.Console.WriteLine("  --list-reports     Lists available Stocks and Flows reports")
        System.Console.WriteLine("  --create-report    Creates a Stocks and Flows report")
        System.Console.WriteLine("  --help             Prints help for an argument")

    End Sub

End Class
