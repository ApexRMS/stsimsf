'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Class InitialStockSpatial

    Private m_Iteration As Nullable(Of Integer) = Nothing
    Private m_StockTypeId As Integer
    Private m_filename As String

    Public Sub New(
        ByVal iteration As Nullable(Of Integer),
        ByVal stockTypeId As Integer,
        ByVal filename As String)

        Me.m_Iteration = iteration
        Me.m_StockTypeId = stockTypeId
        Me.m_filename = filename

    End Sub

    Public ReadOnly Property Iteration As Integer?
        Get
            Return m_Iteration
        End Get
    End Property

    Public ReadOnly Property StockTypeId As Integer
        Get
            Return Me.m_StockTypeId
        End Get
    End Property

    Public ReadOnly Property Filename As String
        Get
            Return m_filename
        End Get
    End Property

End Class
