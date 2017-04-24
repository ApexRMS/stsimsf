'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class OutputOptionsDataFeedView

    Const DEFAULT_TIMESTEP_VALUE As String = "1"

    Public Overrides Sub LoadDataFeed(dataFeed As DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Me.SetCheckBoxBinding(Me.CheckBoxSummaryST, DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME)
        Me.SetTextBoxBinding(Me.TextBoxSummarySTTimesteps, DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME)
        Me.SetCheckBoxBinding(Me.CheckBoxSummaryFL, DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME)
        Me.SetTextBoxBinding(Me.TextBoxSummaryFLTimesteps, DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME)
        Me.SetCheckBoxBinding(Me.CheckBoxSpatialST, DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME)
        Me.SetTextBoxBinding(Me.TextBoxSpatialSTTimesteps, DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME)
        Me.SetCheckBoxBinding(Me.CheckBoxSpatialFL, DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME)
        Me.SetTextBoxBinding(Me.TextBoxSpatialFLTimesteps, DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME)

        Me.MonitorDataSheet(
          "STSim_Terminology",
          AddressOf Me.OnTerminologyChanged,
          True)

    End Sub

    ''' <summary>
    ''' A callback for when the terminology changes
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnTerminologyChanged(ByVal e As DataSheetMonitorEventArgs)

        Dim t As String = CStr(e.GetValue(
            "TimestepUnits", "timestep")).ToLower(CultureInfo.CurrentCulture)

        Me.LabelSummarySTTimesteps.Text = t
        Me.LabelSummaryFLTimesteps.Text = t
        Me.LabelSpatialSTTimesteps.Text = t
        Me.LabelSpatialFLTimesteps.Text = t

    End Sub

    ''' <summary>
    ''' Overrides OnBoundCheckBoxChanged
    ''' </summary>
    ''' <param name="checkBox"></param>
    ''' <param name="columnName"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnBoundCheckBoxChanged(checkBox As System.Windows.Forms.CheckBox, columnName As String)

        MyBase.OnBoundCheckBoxChanged(checkBox, columnName)

        If (checkBox Is Me.CheckBoxSummaryST And Me.CheckBoxSummaryST.Checked And String.IsNullOrEmpty(Me.TextBoxSummarySTTimesteps.Text)) Then
            Me.SetTextBoxData(Me.TextBoxSummarySTTimesteps, DEFAULT_TIMESTEP_VALUE)
        ElseIf (checkBox Is Me.CheckBoxSummaryFL And Me.CheckBoxSummaryFL.Checked And String.IsNullOrEmpty(Me.TextBoxSummaryFLTimesteps.Text)) Then
            Me.SetTextBoxData(Me.TextBoxSummaryFLTimesteps, DEFAULT_TIMESTEP_VALUE)
        ElseIf (checkBox Is Me.CheckBoxSpatialST And Me.CheckBoxSpatialST.Checked And String.IsNullOrEmpty(Me.TextBoxSpatialSTTimesteps.Text)) Then
            Me.SetTextBoxData(Me.TextBoxSpatialSTTimesteps, DEFAULT_TIMESTEP_VALUE)
        ElseIf (checkBox Is Me.CheckBoxSpatialFL And Me.CheckBoxSpatialFL.Checked And String.IsNullOrEmpty(Me.TextBoxSpatialFLTimesteps.Text)) Then
            Me.SetTextBoxData(Me.TextBoxSpatialFLTimesteps, DEFAULT_TIMESTEP_VALUE)
        End If

        Me.EnableControls()

    End Sub

    ''' <summary>
    ''' Overrides EnableView
    ''' </summary>
    ''' <param name="enable"></param>
    ''' <remarks></remarks>
    Public Overrides Sub EnableView(enable As Boolean)

        MyBase.EnableView(enable)

        If (enable) Then
            Me.EnableControls()
        End If

    End Sub

    ''' <summary>
    ''' Enables all controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EnableControls()

        'Text Boxes
        Me.TextBoxSummarySTTimesteps.Enabled = Me.CheckBoxSummaryST.Checked
        Me.TextBoxSummaryFLTimesteps.Enabled = Me.CheckBoxSummaryFL.Checked
        Me.TextBoxSpatialSTTimesteps.Enabled = Me.CheckBoxSpatialST.Checked
        Me.TextBoxSpatialFLTimesteps.Enabled = Me.CheckBoxSpatialFL.Checked

        'Timesteps labels
        Me.LabelSummarySTTimesteps.Enabled = Me.CheckBoxSummaryST.Checked
        Me.LabelSummaryFLTimesteps.Enabled = Me.CheckBoxSummaryFL.Checked
        Me.LabelSpatialSTTimesteps.Enabled = Me.CheckBoxSpatialST.Checked
        Me.LabelSpatialFLTimesteps.Enabled = Me.CheckBoxSpatialFL.Checked

    End Sub

    ''' <summary>
    ''' Handles the Clicked event for the ClearAll button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ButtonClearAll_Click(sender As System.Object, e As System.EventArgs) Handles ButtonClearAll.Click

        Me.ResetBoundControls()
        Me.ClearDataSheets()
        Me.EnableControls()

    End Sub

End Class
