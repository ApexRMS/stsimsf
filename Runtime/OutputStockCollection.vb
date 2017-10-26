'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Common
Imports System.Collections.ObjectModel

Friend Class OutputStockCollection
    Inherits KeyedCollection(Of FiveIntegerLookupKey, OutputStock)

    Public Sub New()
        MyBase.New(New FiveIntegerLookupKeyEqualityComparer)
    End Sub

    Protected Overrides Function GetKeyForItem(ByVal item As OutputStock) As FiveIntegerLookupKey

        Return New FiveIntegerLookupKey(
            item.StratumId,
            GetNullableKey(item.SecondaryStratumId),
            GetNullableKey(item.TertiaryStratumId),
            item.StateClassId,
            item.StockTypeId)

    End Function

End Class
