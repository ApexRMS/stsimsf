'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports System.Collections.ObjectModel

''' <summary>
''' StockType Collection
''' </summary>
''' <remarks></remarks>
Class StockTypeCollection
    Inherits KeyedCollection(Of Integer, StockType)

    Protected Overrides Function GetKeyForItem(ByVal item As StockType) As Integer
        Return item.Id
    End Function

End Class
