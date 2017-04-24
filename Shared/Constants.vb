'************************************************************************************
' STSimStockFlow: A .NET library for simulating stocks and flows
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Module Constants

    'Report names
    Public Const STOCK_REPORT_NAME As String = "stocks"
    Public Const FLOW_REPORT_NAME As String = "flows"
    Public Const DEFAULT_FLOW_ORDER As Double = Double.MinValue

    'Categories and groups
    Public Const STOCK_TYPE_GROUP_NAME As String = "Stock Types"
    Public Const STOCK_GROUP_GROUP_NAME As String = "Stock Groups"
    Public Const FLOW_TYPE_GROUP_NAME As String = "Flow Types"
    Public Const FLOW_GROUP_GROUP_NAME As String = "Flow Groups"
    Public Const CATEGORY_ADVANCED_NAME As String = "Advanced"
    Public Const CATEGORY_DEFINITION_STOCKS_FLOWS_NAME As String = "Stocks and Flows"
    Public Const CATEGORY_ANALYZER_NAME As String = "Export"
    Public Const GROUP_SCENARIO_STOCKS_FLOWS_NAME As String = "Stocks and Flows"
    Public Const GROUP_ANALYZER_REPORTS_NAME As String = "Reports"
    Public Const GROUP_ANALYZER_MAPS_NAME As String = "Maps"

    'Data Feeds
    Public Const DATASHEET_STOCK_TYPE_NAME As String = "SF_StockType"
    Public Const DATASHEET_STOCK_GROUP_NAME As String = "SF_StockGroup"
    Public Const DATASHEET_FLOW_TYPE_NAME As String = "SF_FlowType"
    Public Const DATASHEET_FLOW_GROUP_NAME As String = "SF_FlowGroup"
    Public Const DATASHEET_TERMINOLOGY_NAME As String = "SF_Terminology"
    Public Const DATASHEET_INITIAL_STOCK_NON_SPATIAL As String = "SF_InitialStockNonSpatial"
    Public Const DATASHEET_INITIAL_STOCK_SPATIAL As String = "SF_InitialStockSpatial"
    Public Const DATASHEET_STOCK_LIMIT_NAME As String = "SF_StockLimit"
    Public Const DATASHEET_FLOW_PATHWAY_NAME As String = "SF_FlowPathway"
    Public Const DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME As String = "SF_FlowPathwayDiagram"
    Public Const DATASHEET_FLOW_MULTIPLIER_NAME As String = "SF_FlowMultiplier"
    Public Const DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME As String = "SF_FlowSpatialMultiplier"
    Public Const DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME As String = "SF_StockTypeGroupMembership"
    Public Const DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME As String = "SF_FlowTypeGroupMembership"
    Public Const DATASHEET_OUTPUT_FLOW_NAME As String = "SF_OutputFlow"
    Public Const DATASHEET_OUTPUT_STOCK_NAME As String = "SF_OutputStock"
    Public Const DATASHEET_FLOW_ORDER As String = "SF_FlowOrder"
    Public Const DATASHEET_FLOW_ORDER_OPTIONS As String = "SF_FlowOrderOptions"

    'Common column names
    Public Const STOCKFLOW_NAME_COLUMN_NAME As String = "Name"
    Public Const SCENARIO_ID_COLUMN_NAME As String = "ScenarioID"
    Public Const ITERATION_COLUMN_NAME As String = "Iteration"
    Public Const TIMESTEP_COLUMN_NAME As String = "Timestep"
    Public Const STRATUM_ID_COLUMN_NAME As String = "StratumID"
    Public Const SECONDARY_STRATUM_ID_COLUMN_NAME As String = "SecondaryStratumID"
    Public Const STATECLASS_ID_COLUMN_NAME As String = "StateClassID"
    Public Const STOCK_TYPE_ID_COLUMN_NAME As String = "StockTypeID"
    Public Const STOCK_GROUP_ID_COLUMN_NAME As String = "StockGroupID"
    Public Const FLOW_TYPE_ID_COLUMN_NAME As String = "FlowTypeID"
    Public Const FLOW_GROUP_ID_COLUMN_NAME As String = "FlowGroupID"
    Public Const MULTIPLIER_COLUMN_NAME As String = "Multiplier"
    Public Const MULTIPLIER_FILE_COLUMN_NAME As String = "MultiplierFilename"
    Public Const AMOUNT_COLUMN_NAME As String = "Amount"
    Public Const VALUE_COLUMN_NAME As String = "Value"
    Public Const UNITS_COLUMN_NAME As String = "Units"
    Public Const STOCK_MIN_COLUMN_NAME As String = "StockMinimum"
    Public Const STOCK_MAX_COLUMN_NAME As String = "StockMaximum"
    Public Const TRANSITION_GROUP_ID_COLUMN_NAME As String = "TransitionGroupID"
    Public Const TRANSITION_TYPE_ID_COLUMN_NAME As String = "TransitionTypeID"
    Public Const STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME As String = "StateAttributeTypeID"
    Public Const DISTRIBUTIONTYPE_COLUMN_NAME As String = "DistributionType"
    Public Const DISTRIBUTION_FREQUENCY_COLUMN_NAME As String = "DistributionFrequencyID"
    Public Const DISTRIBUTIONSD_COLUMN_NAME As String = "DistributionSD"
    Public Const DISTRIBUTIONMIN_COLUMN_NAME As String = "DistributionMin"
    Public Const DISTRIBUTIONMAX_COLUMN_NAME As String = "DistributionMax"

    Public Const FROM_STRATUM_ID_COLUMN_NAME As String = "FromStratumID"
    Public Const FROM_SECONDARY_STRATUM_ID_COLUMN_NAME As String = "FromSecondaryStratumID"
    Public Const FROM_STATECLASS_ID_COLUMN_NAME As String = "FromStateClassID"
    Public Const FROM_MIN_AGE_COLUMN_NAME As String = "FromAgeMin"
    Public Const FROM_STOCK_TYPE_ID_COLUMN_NAME As String = "FromStockTypeID"

    Public Const TO_STRATUM_ID_COLUMN_NAME As String = "ToStratumID"
    Public Const TO_STATECLASS_ID_COLUMN_NAME As String = "ToStateClassID"
    Public Const TO_MIN_AGE_COLUMN_NAME As String = "ToAgeMin"
    Public Const TO_STOCK_TYPE_ID_COLUMN_NAME As String = "ToStockTypeID"

    'Terminology
    Public Const STOCK_UNITS_COLUMN_NAME As String = "StockUnits"

    'Initial Stock
    Public Const RASTER_FILE_COLUMN_NAME As String = "RasterFileName"

    'Flow Pathway
    Public Const STOCK_TYPE_ID_SOURCE_COLUMN_NAME As String = "StockTypeIDSource"
    Public Const STOCK_TYPE_ID_DEST_COLUMN_NAME As String = "StockTypeIDDest"

    'Flow Diagram
    Public Const LOCATION_COLUMN_NAME As String = "Location"

    'Flow Order
    Public Const DATASHEET_FLOW_ORDER_OPTIONS_ABT_COLUMN_NAME As String = "ApplyBeforeTransitions"
    Public Const DATASHEET_FLOW_ORDER_OPTIONS_AERS_COLUMN_NAME As String = "ApplyEquallyRankedSimultaneously"
    Public Const DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME As String = "Order"

    'Output Options
    Public Const DATASHEET_OO_NAME As String = "SF_OutputOptions"
    Public Const DATASHEET_OO_DISPLAY_NAME As String = "Output Options"
    Public Const DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME As String = "SummaryOutputST"
    Public Const DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME As String = "SummaryOutputSTTimesteps"
    Public Const DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME As String = "SummaryOutputFL"
    Public Const DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME As String = "SummaryOutputFLTimesteps"
    Public Const DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME As String = "SpatialOutputST"
    Public Const DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME As String = "SpatialOutputSTTimesteps"
    Public Const DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME As String = "SpatialOutputFL"
    Public Const DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME As String = "SpatialOutputFLTimesteps"

    'Messages
    Public Const NO_SUMMARY_OUTPUT_OPTIONS_INFORMATION As String = "No summary output options specified for stocks and flows.  Using defaults."
    Public Const REPORT_EXCEL_TOO_MANY_ROWS As String = "There are too many rows for Excel.  Please try another format for your report."
    Public Const SPATIAL_FILE_STOCK_LOAD_WARNING As String = "The Initial Stocks Raster file '{0}' did not load, and will be ignored."
    Public Const SPATIAL_FILE_STOCK_METADATA_WARNING As String = "The Initial Stocks Raster file '{0}' metadata did not match that of the Initial Condition Primary Stratum raster file, and will be ignored. {1}."
    Public Const SPATIAL_FILE_STOCK_METADATA_INFO As String = "The Initial Stocks Raster file '{0}' metadata did not match that of the Initial Condition Primary Stratum raster file, but differences will be ignored. {1}."
    Public Const ERROR_NO_RESULT_SCENARIOS_FOR_REPORT As String = "There must be at least one selected result scenario to create this report."
    Public Const ERROR_FLOW_MULTIPLIERS_BETA_DISTRIBUTION_INVALID As String = "Flow Multipliers: The beta distribution parameters are invalid.  The multiplier will be used for all iterations."
    Public Const SPATIAL_METADATA_WARNING As String = "The Flow Spatial Multiplier file '{0}' number of rows and/or columns did not match that of ST-Sim's Initial Condition Primary Stratum raster file, and will be ignored."
    Public Const SPATIAL_METADATA_INFO As String = "The Flow Spatial Multiplier file '{0}' metadata did not match that of ST-Sim's Initial Condition Primary Stratum raster file, but differences will be ignored. {1}."
    Public Const SPATIAL_PROCESS_WARNING As String = "The Flow Spatial Multiplier file '{0}' did not load, and will be ignored."

    'Spatial Map file naming constants - Stock and Flow Type
    Public Const SPATIAL_MAP_EXPORT_STOCK_TYPE_VARIABLE_PREFIX As String = "Stk"
    Public Const SPATIAL_MAP_STOCK_TYPE_VARIABLE_PREFIX As String = "stk"
    Public Const SPATIAL_MAP_EXPORT_FLOW_TYPE_VARIABLE_PREFIX As String = "Flo"
    Public Const SPATIAL_MAP_FLOW_TYPE_VARIABLE_PREFIX As String = "flo"

    'Spatial Map file naming constants - Stock and Flow Group
    Public Const SPATIAL_MAP_EXPORT_STOCK_GROUP_VARIABLE_PREFIX As String = "Stkg"
    Public Const SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX As String = "stkg"
    Public Const SPATIAL_MAP_EXPORT_FLOW_GROUP_VARIABLE_PREFIX As String = "Flog"
    Public Const SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX As String = "flog"

    'Spatial Map file naming Regex filter, containing 1 ID value
    Public Const FILE_FILTER_ID_REGEX = "^(.*){0}-([\d]*)\.(tif|vrt)$"


    'Diagram
    Public Const DIAGRAM_MAX_ROWS As Integer = 256
    Public Const DIAGRAM_MAX_COLUMNS As Integer = 26
    Public Const DIAGRAM_ITEM_HEIGHT As Integer = 25
    Public Const DIAGRAM_TITLE_BAR_HEIGHT As Integer = 25
    Public Const DIAGRAM_SHAPE_PADDING As Integer = 44
    Public Const DIAGRAM_SHAPE_SIZE As Integer = (DIAGRAM_TITLE_BAR_HEIGHT + (2 * DIAGRAM_ITEM_HEIGHT))
    Public Const DIAGRAM_SPACE_BETWEEN_SHAPES As Integer = 2 * DIAGRAM_SHAPE_PADDING
    Public Const DIAGRAM_NUM_VERTICAL_CONNECTORS As Integer = 9
    Public Const DIAGRAM_NUM_HORIZONTAL_CONNECTORS As Integer = 9
    Public Const DIAGRAM_OFF_STRATUM_CUE_SIZE As Integer = 14
    Public Const DIAGRAM_LANES_BETWEEN_SHAPES As Integer = 11
    Public Const ZOOM_SAFE_PEN_WIDTH As Integer = -1

    Public DIAGRAM_SHAPE_TEXT_COLOR As System.Drawing.Color = Drawing.Color.Black
    Public DIAGRAM_SHAPE_READONLY_TEXT_COLOR As System.Drawing.Color = Drawing.Color.Gray
    Public DIAGRAM_SHAPE_BORDER_COLOR As System.Drawing.Color = Drawing.Color.Gray
    Public DIAGRAM_SHAPE_READONLY_BORDER_COLOR As System.Drawing.Color = Drawing.Color.LightGray
    Public DIAGRAM_SHAPE_BACKGROUND_COLOR As System.Drawing.Color = System.Drawing.Color.FromArgb(255, 240, 240, 240)
    Public DIAGRAM_FLOW_PATHWAY_LINE_COLOR As System.Drawing.Color = Drawing.Color.CornflowerBlue
    Public DIAGRAM_SHAPE_NORMAL_FONT As System.Drawing.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular)

End Module
