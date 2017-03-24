'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

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
