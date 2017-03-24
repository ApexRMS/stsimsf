'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports System.Drawing
Imports SyncroSim.Common.Forms

Class FlowPathwayLine
    Inherits ConnectorLine

    Private m_Pathway As FlowPathway

    Public Sub New(ByVal lineColor As Color, ByVal pathway As FlowPathway)

        MyBase.New(lineColor)
        Me.m_Pathway = pathway

    End Sub

    Public ReadOnly Property Pathway As FlowPathway
        Get
            Return Me.m_Pathway
        End Get
    End Property

End Class
