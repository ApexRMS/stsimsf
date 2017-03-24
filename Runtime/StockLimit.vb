'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Class StockLimit

    Private m_Iteration As Nullable(Of Integer)
    Private m_Timestep As Nullable(Of Integer)
    Private m_StockTypeId As Integer
    Private m_StratumId As Nullable(Of Integer)
    Private m_SecondaryStratumId As Nullable(Of Integer)
    Private m_StateClassId As Nullable(Of Integer)
    Private m_StockMinimum As Double
    Private m_StockMaximum As Double

    Public Sub New(
        ByVal iteration As Nullable(Of Integer),
        ByVal timestep As Nullable(Of Integer),
        ByVal stockTypeId As Integer,
        ByVal stratumId As Nullable(Of Integer),
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Nullable(Of Integer),
        ByVal stockMinimum As Double,
        ByVal stockMaximum As Double)

        Me.m_Iteration = iteration
        Me.m_Timestep = timestep
        Me.m_StockTypeId = stockTypeId
        Me.m_StratumId = stratumId
        Me.m_SecondaryStratumId = secondaryStratumId
        Me.m_StateClassId = stateClassId
        Me.m_StockMinimum = stockMinimum
        Me.m_StockMaximum = stockMaximum

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

    Public ReadOnly Property StockTypeId As Integer
        Get
            Return Me.m_StockTypeId
        End Get
    End Property

    Public ReadOnly Property StratumId As Nullable(Of Integer)
        Get
            Return Me.m_StratumId
        End Get
    End Property

    Public ReadOnly Property SecondaryStratumId As Nullable(Of Integer)
        Get
            Return Me.m_SecondaryStratumId
        End Get
    End Property

    Public ReadOnly Property StateClassId As Nullable(Of Integer)
        Get
            Return Me.m_StateClassId
        End Get
    End Property

    Public ReadOnly Property StockMinimum As Double
        Get
            Return Me.m_StockMinimum
        End Get
    End Property

    Public ReadOnly Property StockMaximum As Double
        Get
            Return Me.m_StockMaximum
        End Get
    End Property

End Class