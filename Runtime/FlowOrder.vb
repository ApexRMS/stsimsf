'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Class FlowOrder

    Private m_iteration As Nullable(Of Integer)
    Private m_timestep As Nullable(Of Integer)
    Private m_flowTypeId As Integer
    Private m_Order As Double = DEFAULT_FLOW_ORDER


    Public Sub New(
        ByVal iteration As Nullable(Of Integer),
        ByVal timestep As Nullable(Of Integer),
        ByVal flowTypeId As Integer,
        ByVal order As Nullable(Of Double))

        Me.m_iteration = iteration
        Me.m_timestep = timestep
        Me.m_flowTypeId = flowTypeId

        If (order.HasValue) Then
            Me.m_Order = order.Value
        End If

    End Sub

    Public ReadOnly Property Iteration As Integer?
        Get
            Return m_iteration
        End Get
    End Property

    Public ReadOnly Property Timestep As Integer?
        Get
            Return m_timestep
        End Get
    End Property

    Public ReadOnly Property FlowTypeId As Integer
        Get
            Return m_flowTypeId
        End Get
    End Property

    Public ReadOnly Property Order As Double
        Get
            Return m_Order
        End Get
    End Property
End Class

