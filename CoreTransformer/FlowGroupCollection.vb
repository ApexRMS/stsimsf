'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports System.Collections.ObjectModel

Class FlowGroupCollection
    Inherits KeyedCollection(Of Integer, FlowGroup)

    Protected Overrides Function GetKeyForItem(ByVal item As FlowGroup) As Integer
        Return item.Id
    End Function

End Class
