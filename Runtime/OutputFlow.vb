'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Class OutputFlow

    Private m_FromStratumId As Integer
    Private m_FromSecondaryStratumId As Nullable(Of Integer)
    Private m_FromTertiaryStratumID As Nullable(Of Integer)
    Private m_FromStateClassId As Integer
    Private m_FromStockTypeId As Integer
    Private m_transitionTypeId As Nullable(Of Integer)
    Private m_ToStratumId As Integer
    Private m_ToStateClassId As Integer
    Private m_ToStockTypeId As Integer
    Private m_FlowTypeId As Integer
    Private m_Amount As Double

    Public Sub New(
        ByVal fromStratumId As Integer,
        ByVal fromSecondaryStratumId As Nullable(Of Integer),
        ByVal fromTertiaryStratumId As Nullable(Of Integer),
        ByVal fromStateClassId As Integer,
        ByVal fromStockTypeId As Integer,
        ByVal transitionTypeId As Nullable(Of Integer),
        ByVal toStratumId As Integer,
        ByVal toStateClassId As Integer,
        ByVal toStockTypeId As Integer,
        ByVal flowTypeId As Integer,
        ByVal amount As Double)

        Me.m_FromStratumId = fromStratumId
        Me.m_FromSecondaryStratumId = fromSecondaryStratumId
        Me.m_FromTertiaryStratumID = fromTertiaryStratumId
        Me.m_FromStateClassId = fromStateClassId
        Me.m_FromStockTypeId = fromStockTypeId
        Me.m_transitionTypeId = transitionTypeId
        Me.m_ToStratumId = toStratumId
        Me.m_ToStateClassId = toStateClassId
        Me.m_ToStockTypeId = toStockTypeId
        Me.m_FlowTypeId = flowTypeId
        Me.m_Amount = amount

    End Sub

    Public ReadOnly Property FromStratumId As Integer
        Get
            Return Me.m_FromStratumId
        End Get
    End Property

    Public ReadOnly Property FromSecondaryStratumId As Nullable(Of Integer)
        Get
            Return Me.m_FromSecondaryStratumId
        End Get
    End Property

    Public ReadOnly Property FromTertiaryStratumId As Nullable(Of Integer)
        Get
            Return Me.m_FromTertiaryStratumID
        End Get
    End Property

    Public ReadOnly Property FromStateClassId As Integer
        Get
            Return Me.m_FromStateClassId
        End Get
    End Property

    Public ReadOnly Property FromStockTypeId As Integer
        Get
            Return Me.m_FromStockTypeId
        End Get
    End Property

    Public ReadOnly Property TransitionTypeId As Nullable(Of Integer)
        Get
            Return Me.m_transitionTypeId
        End Get
    End Property

    Public ReadOnly Property ToStratumId As Integer
        Get
            Return Me.m_ToStratumId
        End Get
    End Property

    Public ReadOnly Property ToStateClassId As Integer
        Get
            Return Me.m_ToStateClassId
        End Get
    End Property

    Public ReadOnly Property ToStockTypeId As Integer
        Get
            Return Me.m_ToStockTypeId
        End Get
    End Property

    Public ReadOnly Property FlowTypeId As Integer
        Get
            Return Me.m_FlowTypeId
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
