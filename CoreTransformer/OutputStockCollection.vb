'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Common
Imports System.Collections.ObjectModel

Friend Class OutputStockCollection
    Inherits KeyedCollection(Of FourIntegerLookupKey, OutputStock)

    Public Sub New()
        MyBase.New(New FourIntegerLookupKeyEqualityComparer)
    End Sub

    Protected Overrides Function GetKeyForItem(ByVal item As OutputStock) As FourIntegerLookupKey

        Return New FourIntegerLookupKey(
            item.StratumId,
            GetNullableKey(item.SecondaryStratumId),
            item.StateClassId,
            item.StockTypeId)

    End Function

End Class
