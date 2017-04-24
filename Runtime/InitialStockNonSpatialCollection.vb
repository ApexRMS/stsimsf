'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Collections.ObjectModel

''' <summary>
''' InitialStockNonSpatial Collection
''' </summary>
''' <remarks></remarks>
Class InitialStockNonSpatialCollection
    Inherits KeyedCollection(Of Integer, InitialStockNonSpatial)

    Protected Overrides Function GetKeyForItem(ByVal item As InitialStockNonSpatial) As Integer
        Return item.Id
    End Function

End Class
