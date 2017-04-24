'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

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
