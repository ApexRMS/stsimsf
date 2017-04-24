﻿'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Class DiagramFilterCriteria

    Private m_FlowTypes As New Dictionary(Of Integer, Boolean)

    Public ReadOnly Property FlowTypes As Dictionary(Of Integer, Boolean)
        Get
            Return Me.m_FlowTypes
        End Get
    End Property

End Class
