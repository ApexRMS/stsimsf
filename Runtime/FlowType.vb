'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Class FlowType
    Inherits StockFlowType

    Private m_FlowGroups As New FlowGroupCollection
    Private m_Order As Double = DEFAULT_FLOW_ORDER

    Public Sub New(ByVal id As Integer)
        MyBase.New(id)
    End Sub

    Public ReadOnly Property FlowGroups As FlowGroupCollection
        Get
            Return Me.m_FlowGroups
        End Get
    End Property

    Public Property Order As Double
        Get
            Return m_Order
        End Get
        Set(value As Double)
            m_Order = value
        End Set
    End Property

End Class
