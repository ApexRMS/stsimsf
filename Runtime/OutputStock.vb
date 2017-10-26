'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Class OutputStock

    Private m_StratumId As Integer
    Private m_SecondaryStratumId As Nullable(Of Integer)
    Private m_TertiaryStratumId As Nullable(Of Integer)
    Private m_StateClassId As Integer
    Private m_StockTypeId As Integer
    Private m_Amount As Double

    Public Sub New(
        ByVal stratumId As Integer,
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal tertiaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Integer,
        ByVal stockTypeId As Integer,
        ByVal amount As Double)

        Me.m_StratumId = stratumId
        Me.m_SecondaryStratumId = secondaryStratumId
        Me.m_TertiaryStratumId = tertiaryStratumId
        Me.m_StateClassId = stateClassId
        Me.m_StockTypeId = stockTypeId
        Me.m_Amount = amount

    End Sub

    Public ReadOnly Property StratumId As Integer
        Get
            Return Me.m_StratumId
        End Get
    End Property

    Public ReadOnly Property SecondaryStratumId As Nullable(Of Integer)
        Get
            Return Me.m_SecondaryStratumId
        End Get
    End Property

    Public ReadOnly Property TertiaryStratumId As Nullable(Of Integer)
        Get
            Return Me.m_TertiaryStratumId
        End Get
    End Property

    Public ReadOnly Property StateClassId As Integer
        Get
            Return Me.m_StateClassId
        End Get
    End Property

    Public ReadOnly Property StockTypeId As Integer
        Get
            Return Me.m_StockTypeId
        End Get
    End Property

    Public Property Amount As Double
        Get
            Return Me.m_Amount
        End Get
        Set(ByVal value As Double)
            Me.m_Amount = value
        End Set
    End Property

End Class
