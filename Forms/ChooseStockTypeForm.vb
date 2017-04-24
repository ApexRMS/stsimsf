'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports System.Windows.Forms

Class ChooseStockTypeForm

    Private m_Diagram As FlowPathwayDiagram
    Private m_Project As Project
    Private m_StockTypeId As Integer

    Public ReadOnly Property StockTypeId As Integer
        Get
            Debug.Assert(Me.m_StockTypeId > 0)
            Return Me.m_StockTypeId
        End Get
    End Property

    Public Sub Initialize(ByVal diagram As FlowPathwayDiagram, ByVal project As Project)

        Me.m_Diagram = diagram
        Me.m_Project = project

        Dim ds As DataSheet = Me.m_Project.GetDataSheet(DATASHEET_STOCK_TYPE_NAME)
        Dim dv As New DataView(ds.GetData(), Nothing, ds.DisplayMember, DataViewRowState.CurrentRows)

        For Each drv As DataRowView In dv

            Me.ComboBoxStocks.Items.Add(
                New BaseValueDisplayListItem(
                    CInt(drv.Row(ds.ValueMember)),
                    CStr(drv.Row(ds.DisplayMember))))

        Next

        Debug.Assert(dv.Count > 0)
        Debug.Assert(Me.ComboBoxStocks.Items.Count = dv.Count)

        Me.ComboBoxStocks.SelectedIndex = 0

    End Sub

    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click

        Debug.Assert(Me.m_StockTypeId = 0)
        Dim id As Integer = CType(Me.ComboBoxStocks.SelectedItem, BaseValueDisplayListItem).Value

        If (Me.m_Diagram.GetStockTypeShape(id) IsNot Nothing) Then

            FormsUtilities.InformationMessageBox("The specified stock type has already been added to the diagram.")
            Return

        End If

        Me.m_StockTypeId = id
        Me.DialogResult = DialogResult.OK

        Me.Close()

    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click

        Debug.Assert(Me.m_StockTypeId = 0)
        Me.DialogResult = DialogResult.Cancel

        Me.Close()

    End Sub

End Class