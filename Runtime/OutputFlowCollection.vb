'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

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
