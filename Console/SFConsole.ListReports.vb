'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Partial Class SFConsole

    Private Sub HandleListReportsArgument()

        If (Me.Help) Then
            System.Console.WriteLine("Lists available Stocks and Flows reports")
        Else

            System.Console.WriteLine("Available reports:")
            System.Console.WriteLine()
            System.Console.WriteLine(STOCK_REPORT_NAME)
            System.Console.WriteLine(FLOW_REPORT_NAME)

        End If

    End Sub

End Class
