'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Common
Imports System.Collections.ObjectModel

Friend Class OutputFlowCollection
    Inherits KeyedCollection(Of TenIntegerLookupKey, OutputFlow)

    Public Sub New()
        MyBase.New(New TenIntegerLookupKeyEqualityComparer)
    End Sub

    Protected Overrides Function GetKeyForItem(ByVal item As OutputFlow) As TenIntegerLookupKey

        Return New TenIntegerLookupKey(
            item.FromStratumId,
            LookupKeyUtilities.GetOutputCollectionKey(item.FromSecondaryStratumId),
            LookupKeyUtilities.GetOutputCollectionKey(item.FromTertiaryStratumId),
            item.FromStateClassId,
            item.FromStockTypeId,
            LookupKeyUtilities.GetOutputCollectionKey(item.TransitionTypeId),
            item.ToStratumId,
            item.ToStateClassId,
            item.ToStockTypeId,
            item.FlowTypeId)

    End Function

End Class
