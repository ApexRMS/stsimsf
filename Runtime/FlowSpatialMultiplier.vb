'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.StochasticTime
Imports System.Collections.ObjectModel

Class FlowSpatialMultiplier

    Private m_FlowGroupId As Integer
    Private m_Iteration As Nullable(Of Integer)
    Private m_Timestep As Nullable(Of Integer)
    Private m_Filename As String

    Public Sub New(
        ByVal flowGroupId As Integer,
        ByVal iteration As Nullable(Of Integer),
        ByVal timestep As Nullable(Of Integer),
        ByVal fileName As String)

        If (fileName Is Nothing) Then
            Throw New ArgumentException("The filename parameter cannot be Null.")
        End If

        Me.m_FlowGroupId = flowGroupId
        Me.m_Iteration = iteration
        Me.m_Timestep = timestep
        Me.m_Filename = fileName

    End Sub

    ''' <summary>
    ''' Gets the flow group Id
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FlowGroupId As Integer
        Get
            Return Me.m_FlowGroupId
        End Get
    End Property

    ''' <summary>
    ''' Gets the iteration
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Iteration As Nullable(Of Integer)
        Get
            Return Me.m_Iteration
        End Get
    End Property

    ''' <summary>
    ''' Gets the timestep
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Timestep As Nullable(Of Integer)
        Get
            Return Me.m_Timestep
        End Get
    End Property

    ''' <summary>
    ''' Gets the file name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FileName As String
        Get
            Return Me.m_Filename
        End Get
    End Property


End Class
