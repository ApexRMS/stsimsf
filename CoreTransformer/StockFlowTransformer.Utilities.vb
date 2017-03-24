'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.STSim

Partial Class StockFlowTransformer

    Private Const STOCK_AMOUNT_KEY As String = "stockamountkey"

    ''' <summary>
    ''' Gets the ST-Sim transformer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSTSimTransformer() As STSimTransformer

        For Each t As Transformer In Me.Transformers

            If (t.Name = "stsim:runtime") Then
                Return CType(t, STSim.STSimTransformer)
            End If

        Next

        ExceptionUtils.ThrowInvalidOperationException("ST-Sim Transformer not found.  Fatal error!")
        Return Nothing

    End Function

    ''' <summary>
    ''' Gets the stock amount dictionary for the specified cell
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetStockAmountDictionary(ByVal cell As Cell) As Dictionary(Of Integer, Double)

        Dim StockAmounts As Dictionary(Of Integer, Double) =
            CType(cell.GetAssociatedObject(STOCK_AMOUNT_KEY), Global.System.Collections.Generic.Dictionary(Of Integer, Double))

        If (StockAmounts Is Nothing) Then

            StockAmounts = New Dictionary(Of Integer, Double)
            cell.SetAssociatedObject(STOCK_AMOUNT_KEY, StockAmounts)

        End If

        Return StockAmounts

    End Function

    ''' <summary>
    ''' Gets the state attribute value for the specified criteria
    ''' </summary>
    ''' <param name="stateAttributeTypeId"></param>
    ''' <param name="stratumId"></param>
    ''' <param name="secondaryStratumId"></param>
    ''' <param name="stateClassId"></param>
    ''' <param name="iteration"></param>
    ''' <param name="timestep"></param>
    ''' <param name="age"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAttributeValue(
        ByVal stateAttributeTypeId As Integer,
        ByVal stratumId As Integer,
        ByVal secondaryStratumId As Nullable(Of Integer),
        ByVal stateClassId As Integer,
        ByVal iteration As Integer,
        ByVal timestep As Integer,
        ByVal age As Integer) As Double

        Dim val As Double = 0.0

        Dim v As Nullable(Of Double) = Me.STSimTransformer.GetAttributeValueByAge(
            stateAttributeTypeId,
            stratumId,
            secondaryStratumId,
            stateClassId,
            iteration,
            timestep,
            age)

        If (Not v.HasValue) Then

            v = Me.STSimTransformer.GetAttributeValueNoAge(
                    stateAttributeTypeId,
                    stratumId,
                    secondaryStratumId,
                    stateClassId,
                    iteration,
                    timestep)

        End If

        If (v.HasValue) Then
            val = v.Value
        End If

        Return val

    End Function

    Protected Function AnyOutputOptionsSelected() As Boolean

        Dim dr As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OO_NAME).GetDataRow()

        If (dr Is Nothing) Then
            Return False
        End If

        If (DataTableUtilities.GetDataBool(dr, DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME) Or
            DataTableUtilities.GetDataBool(dr, DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME) Or
            DataTableUtilities.GetDataBool(dr, DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME) Or
            DataTableUtilities.GetDataBool(dr, DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME)) Then

            Return True

        End If

        Return False

    End Function

    ''' <summary>
    ''' Determines if it is possible to compute stocks and flows
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' (1.) If none of the stock-flow settings are provided then the user is not using the stock-flow feature.
    ''' (2.) If the flow pathways are missing then we can't compute stocks and flows.  However, log a message if any of the other settings are specified.
    ''' (3.) If flow pathways exist but no initial stocks are specified, log a warning that all stocks will be initialized to zero.
    ''' </remarks>
    Protected Function CanComputeStocksAndFlows() As Boolean

        Dim OutputOptionsExist As Boolean = Me.AnyOutputOptionsSelected()
        Dim FlowPathwaysExist As Boolean = (Me.ResultScenario.GetDataSheet(DATASHEET_FLOW_PATHWAY_NAME).GetData().Rows.Count > 0)
        Dim ICSpatialRecordsExist As Boolean = (Me.ResultScenario.GetDataSheet(DATASHEET_INITIAL_STOCK_SPATIAL).GetData().Rows.Count > 0)
        Dim ICNonSpatialRecordsExist As Boolean = (Me.ResultScenario.GetDataSheet(DATASHEET_INITIAL_STOCK_NON_SPATIAL).GetData().Rows.Count > 0)

        If (Not OutputOptionsExist And Not FlowPathwaysExist And Not ICSpatialRecordsExist And Not ICNonSpatialRecordsExist) Then
            Return False
        End If

        If (Not FlowPathwaysExist) Then

            If (ICSpatialRecordsExist Or ICNonSpatialRecordsExist Or OutputOptionsExist) Then
                Me.AddStatusRecord(StatusRecordType.Information, "Flow pathways not specified.  Not computing stocks and flows.")
            End If

            Return False

        End If

        If (Not ICSpatialRecordsExist And Not ICNonSpatialRecordsExist) Then
            Me.AddStatusRecord(StatusRecordType.Warning, "No initial stocks have been specified.  All stocks will be initialized to zero.")
        End If

        Return True

    End Function

End Class
