'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports System.Collections.ObjectModel

Class FlowGroupCollection
    Inherits KeyedCollection(Of Integer, FlowGroup)

    Protected Overrides Function GetKeyForItem(ByVal item As FlowGroup) As Integer
        Return item.Id
    End Function

End Class
