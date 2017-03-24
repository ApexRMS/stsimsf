'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Class StockFlowType

    Private m_Id As Integer

    Public Sub New(ByVal id As Integer)
        Me.m_Id = id
    End Sub

    Public ReadOnly Property Id As Integer
        Get
            Return Me.m_Id
        End Get
    End Property

End Class
