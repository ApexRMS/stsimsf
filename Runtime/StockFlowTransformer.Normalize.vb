'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim
Imports System.Globalization

Partial Class StockFlowTransformer

    ''' <summary>
    ''' Normalizes the output options data feed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub NormalizeOutputOptions()

        Dim drrc As DataRow = Me.ResultScenario.GetDataSheet("STSim_RunControl").GetDataRow()
        Dim MaxTimestep As Integer = CInt(drrc("MaximumTimestep"))
        Dim dsoo As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_OO_NAME)
        Dim droo As DataRow = dsoo.GetDataRow()

        If (droo Is Nothing) Then

            droo = dsoo.GetData().NewRow()
            dsoo.GetData().Rows.Add(droo)

        End If

        If (Not AnyOutputOptionsSelected()) Then

            DataTableUtilities.SetRowValue(droo, DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME, CInt(True))
            DataTableUtilities.SetRowValue(droo, DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME, CInt(True))
            DataTableUtilities.SetRowValue(droo, DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, 1)
            DataTableUtilities.SetRowValue(droo, DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, 1)

            Me.RecordStatus(StatusType.Information, NO_SUMMARY_OUTPUT_OPTIONS_INFORMATION)

        End If

        Me.ValidateTimesteps(droo, DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME, DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, "Summary stocks", MaxTimestep)
        Me.ValidateTimesteps(droo, DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME, DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, "Summary flows", MaxTimestep)
        Me.ValidateTimesteps(droo, DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME, DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, "Spatial stocks", MaxTimestep)
        Me.ValidateTimesteps(droo, DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME, DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, "Spatial flows", MaxTimestep)

    End Sub

    ''' <summary>
    ''' Validates the timesteps for the specified column name and maximum timestep
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <param name="optionColumnName"></param>
    ''' <param name="timestepsColumnName"></param>
    ''' <param name="timestepsColumnHeaderText"></param>
    ''' <param name="maxTimestep"></param>
    ''' <remarks></remarks>
    Private Sub ValidateTimesteps(
        ByVal dr As DataRow,
        ByVal optionColumnName As String,
        ByVal timestepsColumnName As String,
        ByVal timestepsColumnHeaderText As String,
        ByVal maxTimestep As Integer)

        If (dr(optionColumnName) Is DBNull.Value) Then
            Return
        End If

        If (Not CBool(dr(optionColumnName))) Then
            Return
        End If

        If (dr(timestepsColumnName) Is DBNull.Value) Then

            Dim message As String = String.Format(CultureInfo.InvariantCulture,
                    "Stocks and Flows Timestep value for '{0}' is invalid.  Using default.", timestepsColumnHeaderText)

            Me.RecordStatus(StatusType.Warning, message)
            dr(timestepsColumnName) = 1

            Return

        End If

        Dim val As Integer = CInt(dr(timestepsColumnName))

        If (val > maxTimestep) Then

            Dim message As String = String.Format(CultureInfo.InvariantCulture,
                "Stocks and Flows Timestep value for '{0}' out of range.  Using default.", timestepsColumnHeaderText)

            Me.RecordStatus(StatusType.Warning, message)
            dr(timestepsColumnName) = maxTimestep

            Return

        End If

    End Sub

    Private Sub NormalizeForUserDistributions()

        If (Me.m_STSimTransformer.DistributionProvider.Values.Count > 0) Then

            Dim Expander As New STSimDistributionBaseExpander(Me.m_STSimTransformer.DistributionProvider)
            Me.ExpandFlowMultipliers(Expander)

        End If

    End Sub

    Private Sub ExpandFlowMultipliers(ByVal expander As STSimDistributionBaseExpander)

        If (Me.m_FlowMultipliers.Count > 0) Then

            Dim NewItems As IEnumerable(Of STSimDistributionBase) =
                expander.Expand(Me.m_FlowMultipliers)

            Me.m_FlowMultipliers.Clear()

            For Each t As FlowMultiplier In NewItems
                Me.m_FlowMultipliers.Add(t)
            Next

        End If

    End Sub

End Class
