'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class ResultLayoutProvider
    Inherits LayoutProvider

    Protected Overrides Sub ModifyLayout(layout As SyncroSimLayout)

        Dim ExportGroup As SyncroSimLayoutItem = layout.Items.GetItem("Export")

        If (ExportGroup IsNot Nothing) Then

            Dim ReportsGroup As SyncroSimLayoutItem = ExportGroup.Items.GetItem("Reports")
            Dim MapsGroup As SyncroSimLayoutItem = ExportGroup.Items.GetItem("Maps")

            If (ReportsGroup IsNot Nothing) Then

                ReportsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:summary-stock-report", "Stocks", False))
                ReportsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:summary-flow-report", "Flows", False))

            End If

            If (MapsGroup IsNot Nothing) Then

                MapsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:stock-raster-map", "Stock Types", False))
                MapsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:flow-raster-map", "Flow Types", False))
                MapsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:stock-group-raster-map", "Stock Groups", False))
                MapsGroup.Items.Add(New SyncroSimLayoutItem("stsim-stockflow:flow-group-raster-map", "Flow Groups", False))

            End If

        End If

    End Sub

End Class
