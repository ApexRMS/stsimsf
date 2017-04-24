'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Windows.Forms
Imports SyncroSim.Common.Forms

Class FlowPathwayTabStripItem
    Inherits TabStripItem

    Private m_Control As Control

    Public Sub New(ByVal text As String)
        MyBase.New(text)
    End Sub

    Public Property Control As Control
        Get
            Return Me.m_Control
        End Get
        Set(value As Control)
            Debug.Assert(Me.m_Control Is Nothing)
            Me.m_Control = value
        End Set
    End Property

    Protected Overrides Sub Dispose(disposing As Boolean)

        If (Me.m_Control IsNot Nothing) Then

            Me.m_Control.Dispose()
            Me.m_Control = Nothing

        End If

        MyBase.Dispose(disposing)

    End Sub

End Class
