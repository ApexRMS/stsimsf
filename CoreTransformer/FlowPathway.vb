'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Class FlowPathway

    Private m_Iteration As Nullable(Of Integer)
    Private m_Timestep As Nullable(Of Integer)
    Private m_FromStratumId As Nullable(Of Integer)
    Private m_FromStateClassId As Nullable(Of Integer)
    Private m_FromMinimumAge As Nullable(Of Integer)
    Private m_FromStockTypeId As Integer
    Private m_ToStratumId As Nullable(Of Integer)
    Private m_ToStateClassId As Nullable(Of Integer)
    Private m_ToMinimumAge As Nullable(Of Integer)
    Private m_ToStockTypeId As Integer
    Private m_TransitionGroupId As Integer
    Private m_StateAttributeTypeId As Nullable(Of Integer)
    Private m_FlowTypeId As Integer
    Private m_Multiplier As Double
    Private m_FlowAmount As Double

    Public Sub New(
        ByVal iteration As Nullable(Of Integer),
        ByVal timestep As Nullable(Of Integer),
        ByVal fromStratumId As Nullable(Of Integer),
        ByVal fromStateClassId As Nullable(Of Integer),
        ByVal fromMinimumAge As Nullable(Of Integer),
        ByVal fromStockTypeId As Integer,
        ByVal toStratumId As Nullable(Of Integer),
        ByVal toStateClassId As Nullable(Of Integer),
        ByVal toMinimumAge As Nullable(Of Integer),
        ByVal toStockTypeId As Integer,
        ByVal transitionGroupId As Integer,
        ByVal stateAttributeTypeId As Nullable(Of Integer),
        ByVal flowTypeId As Integer,
        ByVal multiplier As Double)

        Me.m_Iteration = iteration
        Me.m_Timestep = timestep
        Me.m_FromStratumId = fromStratumId
        Me.m_FromStateClassId = fromStateClassId
        Me.m_FromMinimumAge = fromMinimumAge
        Me.m_FromStockTypeId = fromStockTypeId
        Me.m_ToStratumId = toStratumId
        Me.m_ToStateClassId = toStateClassId
        Me.m_ToMinimumAge = toMinimumAge
        Me.m_ToStockTypeId = toStockTypeId
        Me.m_TransitionGroupId = transitionGroupId
        Me.m_StateAttributeTypeId = stateAttributeTypeId
        Me.m_FlowTypeId = flowTypeId
        Me.m_Multiplier = multiplier

    End Sub

    Public ReadOnly Property Iteration As Nullable(Of Integer)
        Get
            Return Me.m_Iteration
        End Get
    End Property

    Public ReadOnly Property Timestep As Nullable(Of Integer)
        Get
            Return Me.m_Timestep
        End Get
    End Property

    Public ReadOnly Property FromStratumId As Nullable(Of Integer)
        Get
            Return Me.m_FromStratumId
        End Get
    End Property

    Public ReadOnly Property FromStateClassId As Nullable(Of Integer)
        Get
            Return Me.m_FromStateClassId
        End Get
    End Property

    Public ReadOnly Property FromMinimumAge As Nullable(Of Integer)
        Get
            Return Me.m_FromMinimumAge
        End Get
    End Property

    Public ReadOnly Property FromStockTypeId As Integer
        Get
            Return Me.m_FromStockTypeId
        End Get
    End Property

    Public ReadOnly Property ToStratumId As Nullable(Of Integer)
        Get
            Return Me.m_ToStratumId
        End Get
    End Property

    Public ReadOnly Property ToStateClassId As Nullable(Of Integer)
        Get
            Return Me.m_ToStateClassId
        End Get
    End Property

    Public ReadOnly Property ToMinimumAge As Nullable(Of Integer)
        Get
            Return Me.m_ToMinimumAge
        End Get
    End Property

    Public ReadOnly Property ToStockTypeId As Integer
        Get
            Return Me.m_ToStockTypeId
        End Get
    End Property

    Public ReadOnly Property TransitionGroupId As Integer
        Get
            Return Me.m_TransitionGroupId
        End Get
    End Property

    Public ReadOnly Property StateAttributeTypeId As Nullable(Of Integer)
        Get
            Return Me.m_StateAttributeTypeId
        End Get
    End Property

    Public ReadOnly Property FlowTypeId As Integer
        Get
            Return Me.m_FlowTypeId
        End Get
    End Property

    Public ReadOnly Property Multiplier As Double
        Get
            Return Me.m_Multiplier
        End Get
    End Property

    Public Property FlowAmount As Double
        Get
            Return Me.m_FlowAmount
        End Get
        Set(value As Double)
            Me.m_FlowAmount = value
        End Set
    End Property

End Class
