﻿'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.STSim
Imports SyncroSim.StochasticTime

Class FlowMultiplier
    Inherits STSimDistributionBase

    Private m_StateClassId As Nullable(Of Integer)
    Private m_FlowGroupId As Integer

    Public Sub New(
        ByVal iteration As Nullable(Of Integer),
        ByVal timestep As Nullable(Of Integer),
        ByVal stratumId As Nullable(Of Integer),
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal tertiaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Nullable(Of Integer),
        ByVal flowGroupId As Integer,
        ByVal multiplierValue As Nullable(Of Double),
        ByVal distributionTypeId As Nullable(Of Integer),
        ByVal distributionFrequency As Nullable(Of DistributionFrequency),
        ByVal distributionSD As Nullable(Of Double),
        ByVal distributionMin As Nullable(Of Double),
        ByVal distributionMax As Nullable(Of Double))

        MyBase.New(
            iteration,
            timestep,
            stratumId,
            secondaryStratumId,
            tertiaryStratumId,
            multiplierValue,
            distributionTypeId,
            distributionFrequency,
            distributionSD,
            distributionMin,
            distributionMax)

        Me.m_StateClassId = stateClassId
        Me.m_FlowGroupId = flowGroupId

    End Sub

    Public ReadOnly Property StateClassId As Nullable(Of Integer)
        Get
            Return Me.m_StateClassId
        End Get
    End Property

    Public ReadOnly Property FlowGroupId As Integer
        Get
            Return Me.m_FlowGroupId
        End Get
    End Property

    Public Overrides Function Clone() As STSimDistributionBase

        Return New FlowMultiplier(
            Me.Iteration,
            Me.Timestep,
            Me.StratumId,
            Me.SecondaryStratumId,
            Me.TertiaryStratumId,
            Me.StateClassId,
            Me.FlowGroupId,
            Me.DistributionValue,
            Me.DistributionTypeId,
            Me.DistributionFrequency,
            Me.DistributionSD,
            Me.DistributionMin,
            Me.DistributionMax)

    End Function

End Class
