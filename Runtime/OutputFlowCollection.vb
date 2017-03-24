'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Common
Imports System.Collections.ObjectModel

Friend Class OutputFlowCollection
    Inherits KeyedCollection(Of NineIntegerLookupKey, OutputFlow)

    Public Sub New()
        MyBase.New(New NineIntegerLookupKeyEqualityComparer)
    End Sub

    Protected Overrides Function GetKeyForItem(ByVal item As OutputFlow) As NineIntegerLookupKey

        Return New NineIntegerLookupKey(
            item.FromStratumId,
            GetNullableKey(item.FromSecondaryStratumId),
            item.FromStateClassId,
            item.FromStockTypeId,
            GetNullableKey(item.TransitionTypeId),
            item.ToStratumId,
            item.ToStateClassId,
            item.ToStockTypeId,
            item.FlowTypeId)

    End Function

End Class
