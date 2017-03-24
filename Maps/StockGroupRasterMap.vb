'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.StochasticTime.Forms
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports SyncroSim.Core.Forms

Class StockGroupRasterMap
    Inherits StochasticTimeExportTransformer

    ReadOnly fileFilterRegex As String = String.Format(CultureInfo.CurrentCulture, FILE_FILTER_ID_REGEX, SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX)

    Protected Overrides Sub Export(location As String, exportType As ExportType)
        StochasticTimeExportTransformer.CopyRasterFiles(Me.GetActiveResultScenarios(), fileFilterRegex, location, AddressOf CreateExportFilename)
    End Sub

    ''' <summary>
    ''' Create a Spatial Stock Type filename using the export filename convention. This involves replacing the Id with the Stock Type
    ''' Name
    ''' </summary>
    ''' <param name="filename">The name of the file as it appears in the internal filenaming convention</param>
    ''' <returns>The filename as it appears in the external filenaming convention</returns>
    ''' <remarks>Internal file convention is Itx-Tsy-stk-z.tif. External convention is ...Stk-StockName.tif</remarks>
    Private Function CreateExportFilename(ByVal filename As String) As String

        ' Pull the Id out of the filename, and convert a name
        Dim m As Match = Regex.Match(filename, fileFilterRegex)
        If Not m.Success Or m.Groups.Count <> 4 Then
            ' Something wrong here, so just return the original filename
            Debug.Assert(False, "Error parsing the Spatial Stock Group internal filename.")
            Return filename
        End If

        Dim id As Integer = CInt(m.Groups(2).Value)
        Dim name As String = id.ToString(CultureInfo.InvariantCulture)

        Dim ds As DataSheet = Me.Project.GetDataSheet(DATASHEET_STOCK_GROUP_NAME)

        For Each dr As DataRow In ds.GetData.Rows
            If CInt(dr(ds.PrimaryKeyColumn.Name)) = id Then
                name = CStr(dr("NAME"))
                Exit For
            End If
        Next

        Return String.Format(CultureInfo.InvariantCulture, "{0}{1}-{2}.{3}", m.Groups(1), SPATIAL_MAP_EXPORT_STOCK_GROUP_VARIABLE_PREFIX, name, m.Groups(3))

    End Function

End Class
