'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Module MathUtils

    Public Function CompareDoublesEqual(lhs As Double, rhs As Double, Optional epsilon As Double = [Double].Epsilon) As Boolean
        Return (Math.Abs(lhs - rhs) < epsilon)
    End Function

End Module
