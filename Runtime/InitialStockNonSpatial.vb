'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' InitialStockNonSpatial
''' </summary>
''' <remarks></remarks>
Class InitialStockNonSpatial

    Private m_Id As Integer
    Private m_StockTypeId As Integer
    Private m_StateAttributeTypeId As Integer

    Public Sub New(ByVal id As Integer, ByVal stockTypeId As Integer, ByVal stateAttributeTypeId As Integer)

        Me.m_Id = id
        Me.m_StockTypeId = stockTypeId
        Me.m_StateAttributeTypeId = stateAttributeTypeId

    End Sub

    Public ReadOnly Property Id As Integer
        Get
            Return Me.m_Id
        End Get
    End Property

    Public ReadOnly Property StockTypeId As Integer
        Get
            Return Me.m_StockTypeId
        End Get
    End Property

    Public ReadOnly Property StateAttributeTypeId As Integer
        Get
            Return Me.m_StateAttributeTypeId
        End Get
    End Property

End Class
