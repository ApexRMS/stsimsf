'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Drawing
Imports SyncroSim.Common.Forms

Class StockTypeShape
    Inherits BoxDiagramShape

    Private m_StockTypeId As Integer
    Private m_OutgoingPathways As New List(Of FlowPathway)
    Private m_IsReadOnly As Boolean

    Public Sub New(
        ByVal stockTypeId As Integer,
        ByVal displayName As String)

        MyBase.New(
            DIAGRAM_NUM_VERTICAL_CONNECTORS,
            DIAGRAM_NUM_HORIZONTAL_CONNECTORS)

        Me.TitleBarText = displayName
        Me.m_StockTypeId = stockTypeId

        Debug.Assert(Me.m_StockTypeId > 0)

    End Sub

    Public ReadOnly Property StockTypeId As Integer
        Get
            Debug.Assert(Me.m_StockTypeId > 0)
            Return Me.m_StockTypeId
        End Get
    End Property

    Public ReadOnly Property OutgoingFlowPathways As List(Of FlowPathway)
        Get
            Return Me.m_OutgoingPathways
        End Get
    End Property

    Public Property IsReadOnly As Boolean
        Get
            Return Me.m_IsReadOnly
        End Get
        Set(value As Boolean)
            Me.m_IsReadOnly = value
        End Set
    End Property

    Public Overrides Function GetToolTipText() As String
        Return Me.TitleBarText
    End Function

    Public Overrides Sub Render(g As Drawing.Graphics)

        Using b As New SolidBrush(DIAGRAM_SHAPE_BACKGROUND_COLOR)
            g.FillRectangle(b, Me.Bounds)
        End Using

        Dim TextColor As Color = DIAGRAM_SHAPE_TEXT_COLOR
        Dim BorderColor As Color = DIAGRAM_SHAPE_BORDER_COLOR

        If (Me.IsReadOnly) Then
            TextColor = DIAGRAM_SHAPE_READONLY_TEXT_COLOR
            BorderColor = DIAGRAM_SHAPE_READONLY_BORDER_COLOR
        End If

        Using p As New Pen(BorderColor, ZOOM_SAFE_PEN_WIDTH)
            g.DrawRectangle(p, Me.Bounds)
        End Using

        Dim rc As New Rectangle(
            Me.Bounds.Left ,
            Me.Bounds.Top,
            Me.Bounds.Width,
            Me.Bounds.Height)

        rc.Inflate(-4, -4)

        Using sf As New StringFormat()

            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Center

            If (Not Me.TitleBarText.Contains(" ")) Then

                sf.FormatFlags = StringFormatFlags.NoWrap
                sf.Trimming = StringTrimming.EllipsisCharacter

            End If

            Using b As New SolidBrush(TextColor)
                g.DrawString(Me.TitleBarText, DIAGRAM_SHAPE_NORMAL_FONT, b, rc, sf)
            End Using

        End Using

    End Sub

End Class
