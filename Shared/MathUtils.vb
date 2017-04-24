'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module MathUtils

    Public Function CompareDoublesEqual(lhs As Double, rhs As Double, Optional epsilon As Double = [Double].Epsilon) As Boolean
        Return (Math.Abs(lhs - rhs) < epsilon)
    End Function

End Module
