// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Drawing;

namespace SyncroSim.STSimStockFlow
{
	internal static class Constants
	{
        //ST-Sim Transformer
        public const string STSIM_TRANSFORMER_NAME = "stsim_Runtime";

		//Report names
		public const string STOCK_REPORT_NAME = "stocks";
		public const string FLOW_REPORT_NAME = "flows";
		public const double DEFAULT_FLOW_ORDER = double.MinValue;
		public const int OUTPUT_COLLECTION_WILDCARD_KEY = 0;

		//Categories and groups
		public const string STOCK_TYPE_GROUP_NAME = "Stock Types";
		public const string STOCK_GROUP_GROUP_NAME = "Stock Groups";
		public const string FLOW_TYPE_GROUP_NAME = "Flow Types";
		public const string FLOW_GROUP_GROUP_NAME = "Flow Groups";
		public const string CATEGORY_ADVANCED_NAME = "Advanced";
		public const string CATEGORY_DEFINITION_STOCKS_FLOWS_NAME = "Stocks and Flows";
		public const string CATEGORY_ANALYZER_NAME = "Export";
		public const string GROUP_SCENARIO_STOCKS_FLOWS_NAME = "Stocks and Flows";
		public const string GROUP_ANALYZER_REPORTS_NAME = "Reports";
		public const string GROUP_ANALYZER_MAPS_NAME = "Maps";

		//Data Feeds
		public const string DATASHEET_STOCK_TYPE_NAME = "stsimsf_StockType";
		public const string DATASHEET_STOCK_GROUP_NAME = "stsimsf_StockGroup";
		public const string DATASHEET_FLOW_TYPE_NAME = "stsimsf_FlowType";
		public const string DATASHEET_FLOW_MULTIPLIER_TYPE_NAME = "stsimsf_FlowMultiplierType";
		public const string DATASHEET_FLOW_GROUP_NAME = "stsimsf_FlowGroup";
		public const string DATASHEET_TERMINOLOGY_NAME = "stsimsf_Terminology";
		public const string DATASHEET_INITIAL_STOCK_NON_SPATIAL = "stsimsf_InitialStockNonSpatial";
		public const string DATASHEET_INITIAL_STOCK_SPATIAL = "stsimsf_InitialStockSpatial";
		public const string DATASHEET_STOCK_LIMIT_NAME = "stsimsf_StockLimit";
		public const string DATASHEET_STOCK_TRANSITION_MULTIPLIER_NAME = "stsimsf_StockTransitionMultiplier";
		public const string DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME = "stsimsf_StockTypeGroupMembership";
		public const string DATASHEET_FLOW_PATHWAY_NAME = "stsimsf_FlowPathway";
		public const string DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME = "stsimsf_FlowPathwayDiagram";
		public const string DATASHEET_FLOW_MULTIPLIER_NAME = "stsimsf_FlowMultiplier";
		public const string DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME = "stsimsf_FlowSpatialMultiplier";
		public const string DATASHEET_FLOW_LATERAL_MULTIPLIER_NAME = "stsimsf_FlowLateralMultiplier";
		public const string DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME = "stsimsf_FlowTypeGroupMembership";
		public const string DATASHEET_FLOW_ORDER = "stsimsf_FlowOrder";
		public const string DATASHEET_FLOW_ORDER_OPTIONS = "stsimsf_FlowOrderOptions";
		public const string DATASHEET_OUTPUT_FLOW_NAME = "stsimsf_OutputFlow";
		public const string DATASHEET_OUTPUT_STOCK_NAME = "stsimsf_OutputStock";
		public const string DATASHEET_OUTPUT_SPATIAL_STOCK_GROUP = "stsimsf_OutputSpatialStockGroup";
		public const string DATASHEET_OUTPUT_SPATIAL_FLOW_GROUP = "stsimsf_OutputSpatialFlowGroup";
		public const string DATASHEET_OUTPUT_LATERAL_FLOW_GROUP = "stsimsf_OutputLateralFlowGroup";
		public const string DATASHEET_OUTPUT_AVG_SPATIAL_STOCK_GROUP = "stsimsf_OutputAverageSpatialStockGroup";
		public const string DATASHEET_OUTPUT_AVG_SPATIAL_FLOW_GROUP = "stsimsf_OutputAverageSpatialFlowGroup";

        public const string DATASHEET_STSIM_STRATUM = "stsim_Stratum";
        public const string DATASHEET_STSIM_SECONDARY_STRATUM = "stsim_SecondaryStratum";
        public const string DATASHEET_STSIM_TERTIARY_STRATUM = "stsim_TertiaryStratum";
        public const string DATASHEET_STSIM_STATE_CLASS = "stsim_StateClass";
        public const string DATASHEET_STSIM_TERMINOLOGY = "stsim_Terminology";
        public const string DATASHEET_STSIM_RUN_CONTROL = "stsim_RunControl";
        public const string DATASHEET_STSIM_OUTPUT_OPTIONS = "stsim_OutputOptions";

		//Common column names
		public const string NAME_COLUMN_NAME = "Name";
		public const string SCENARIO_ID_COLUMN_NAME = "ScenarioID";
		public const string ITERATION_COLUMN_NAME = "Iteration";
		public const string TIMESTEP_COLUMN_NAME = "Timestep";
		public const string STRATUM_ID_COLUMN_NAME = "StratumID";
		public const string SECONDARY_STRATUM_ID_COLUMN_NAME = "SecondaryStratumID";
		public const string TERTIARY_STRATUM_ID_COLUMN_NAME = "TertiaryStratumID";
		public const string STATECLASS_ID_COLUMN_NAME = "StateClassID";
        public const string AGE_MIN_COLUMN_NAME = "AgeMin";
        public const string AGE_MAX_COLUMN_NAME = "AgeMax";
        public const string STOCK_TYPE_ID_COLUMN_NAME = "StockTypeID";
		public const string STOCK_GROUP_ID_COLUMN_NAME = "StockGroupID";
		public const string FLOW_TYPE_ID_COLUMN_NAME = "FlowTypeID";
		public const string FLOW_GROUP_ID_COLUMN_NAME = "FlowGroupID";
		public const string FLOW_MULTIPLIER_TYPE_ID_COLUMN_NAME = "FlowMultiplierTypeID";
		public const string MULTIPLIER_COLUMN_NAME = "Multiplier";
		public const string MULTIPLIER_FILE_COLUMN_NAME = "MultiplierFilename";
		public const string AMOUNT_COLUMN_NAME = "Amount";
		public const string VALUE_COLUMN_NAME = "Value";
		public const string UNITS_COLUMN_NAME = "Units";
		public const string STOCK_VALUE_COLUMN_NAME = "StockValue";
		public const string STOCK_MIN_COLUMN_NAME = "StockMinimum";
		public const string STOCK_MAX_COLUMN_NAME = "StockMaximum";
		public const string TRANSITION_GROUP_ID_COLUMN_NAME = "TransitionGroupID";
		public const string TRANSITION_TYPE_ID_COLUMN_NAME = "TransitionTypeID";
		public const string STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME = "StateAttributeTypeID";
		public const string DISTRIBUTIONTYPE_COLUMN_NAME = "DistributionType";
		public const string DISTRIBUTION_FREQUENCY_COLUMN_NAME = "DistributionFrequencyID";
		public const string DISTRIBUTIONSD_COLUMN_NAME = "DistributionSD";
		public const string DISTRIBUTIONMIN_COLUMN_NAME = "DistributionMin";
		public const string DISTRIBUTIONMAX_COLUMN_NAME = "DistributionMax";
        public const string DATASHEET_OUTPUT_SPATIAL_FILENAME_COLUMN = "Filename";
        public const string SUM_OF_AMOUNT_COLUMN_NAME = "SumOfAmount";

        public const string FROM_STRATUM_ID_COLUMN_NAME = "FromStratumID";
		public const string FROM_SECONDARY_STRATUM_ID_COLUMN_NAME = "FromSecondaryStratumID";
		public const string FROM_TERTIARY_STRATUM_ID_COLUMN_NAME = "FromTertiaryStratumID";
		public const string FROM_STATECLASS_ID_COLUMN_NAME = "FromStateClassID";
		public const string FROM_MIN_AGE_COLUMN_NAME = "FromAgeMin";
		public const string FROM_STOCK_TYPE_ID_COLUMN_NAME = "FromStockTypeID";
		public const string TO_STRATUM_ID_COLUMN_NAME = "ToStratumID";
		public const string TO_STATECLASS_ID_COLUMN_NAME = "ToStateClassID";
		public const string TO_MIN_AGE_COLUMN_NAME = "ToAgeMin";
		public const string TO_STOCK_TYPE_ID_COLUMN_NAME = "ToStockTypeID";

		public const string TRANSFER_TO_STRATUM_ID_COLUMN_NAME = "TransferToStratumID";
		public const string TRANSFER_TO_SECONDARY_STRATUM_ID_COLUMN_NAME = "TransferToSecondaryStratumID";
		public const string TRANSFER_TO_TERTIARY_STRATUM_ID_COLUMN_NAME = "TransferToTertiaryStratumID";
		public const string TRANSFER_TO_STATECLASS_ID_COLUMN_NAME = "TransferToStateClassID";
		public const string TRANSFER_TO_MIN_AGE_COLUMN_NAME = "TransferToAgeMin";

		public const string END_STRATUM_ID_COLUMN_NAME = "EndStratumID";
		public const string END_SECONDARY_STRATUM_ID_COLUMN_NAME = "EndSecondaryStratumID";
		public const string END_TERTIARY_STRATUM_ID_COLUMN_NAME = "EndTertiaryStratumID";
		public const string END_STATECLASS_ID_COLUMN_NAME = "EndStateClassID";
		public const string END_MIN_AGE_COLUMN_NAME = "EndMinAge";

        public const string IS_AUTO_COLUMN_NAME = "IsAuto";
        public const string AUTO_COLUMN_SUFFIX = "[Type]";

        //Terminology
        public const string STOCK_UNITS_COLUMN_NAME = "StockUnits";

		//Initial Stock
		public const string RASTER_FILE_COLUMN_NAME = "RasterFileName";

		//Flow Pathway
		public const string STOCK_TYPE_ID_SOURCE_COLUMN_NAME = "StockTypeIDSource";
		public const string STOCK_TYPE_ID_DEST_COLUMN_NAME = "StockTypeIDDest";

		//Flow Diagram
		public const string LOCATION_COLUMN_NAME = "Location";

		//Flow Order
		public const string DATASHEET_FLOW_ORDER_OPTIONS_ABT_COLUMN_NAME = "ApplyBeforeTransitions";
		public const string DATASHEET_FLOW_ORDER_OPTIONS_AERS_COLUMN_NAME = "ApplyEquallyRankedSimultaneously";
		public const string DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME = "Order";

		//Output Options
		public const string DATASHEET_OO_NAME = "stsimsf_OutputOptions";
		public const string DATASHEET_OO_DISPLAY_NAME = "Output Options";
		public const string DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME = "SummaryOutputST";
		public const string DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME = "SummaryOutputSTTimesteps";
		public const string DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME = "SummaryOutputFL";
		public const string DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME = "SummaryOutputFLTimesteps";
		public const string DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME = "SpatialOutputST";
		public const string DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME = "SpatialOutputSTTimesteps";
		public const string DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME = "SpatialOutputFL";
		public const string DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME = "SpatialOutputFLTimesteps";
		public const string DATASHEET_OO_LATERAL_OUTPUT_FL_COLUMN_NAME = "LateralOutputFL";
		public const string DATASHEET_OO_LATERAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME = "LateralOutputFLTimesteps";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_COLUMN_NAME = "AvgSpatialOutputST";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME = "AvgSpatialOutputSTTimesteps";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_ACROSS_TIMESTEPS_COLUMN_NAME = "AvgSpatialOutputSTAcrossTimesteps";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_COLUMN_NAME = "AvgSpatialOutputFL";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME = "AvgSpatialOutputFLTimesteps";
		public const string DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_ACROSS_TIMESTEPS_COLUMN_NAME = "AvgSpatialOutputFLAcrossTimesteps";

		//Messages
		public const string NO_SUMMARY_OUTPUT_OPTIONS_INFORMATION = "No summary output options specified for stocks and flows.  Using defaults.";
		public const string REPORT_EXCEL_TOO_MANY_ROWS = "There are too many rows for Excel.  Please try another format for your report.";
		public const string SPATIAL_FILE_STOCK_LOAD_WARNING = "The Initial Stocks Raster file '{0}' did not load, and will be ignored.";
		public const string SPATIAL_FILE_STOCK_METADATA_WARNING = "The Initial Stocks Raster file '{0}' metadata did not match that of the Initial Condition Primary Stratum raster file, and will be ignored. {1}.";
		public const string SPATIAL_FILE_STOCK_METADATA_INFO = "The Initial Stocks Raster file '{0}' metadata did not match that of the Initial Condition Primary Stratum raster file, but differences will be ignored. {1}.";
		public const string ERROR_NO_RESULT_SCENARIOS_FOR_REPORT = "There must be at least one selected result scenario to create this report.";
		public const string ERROR_FLOW_MULTIPLIERS_BETA_DISTRIBUTION_INVALID = "Flow Multipliers: The beta distribution parameters are invalid.  The multiplier will be used for all iterations.";
		public const string SPATIAL_METADATA_WARNING = "The Flow Spatial Multiplier file '{0}' number of rows and/or columns did not match that of ST-Sim's Initial Condition Primary Stratum raster file, and will be ignored.";
		public const string SPATIAL_METADATA_INFO = "The Flow Spatial Multiplier file '{0}' metadata did not match that of ST-Sim's Initial Condition Primary Stratum raster file, but differences will be ignored. {1}.";
		public const string SPATIAL_PROCESS_WARNING = "The Flow Spatial Multiplier file '{0}' did not load, and will be ignored.";

        //Spatial Map file naming constants - Stock and Flow Group
		public const string SPATIAL_MAP_STOCK_GROUP_VARIABLE_PREFIX = "stkg";
		public const string SPATIAL_MAP_FLOW_GROUP_VARIABLE_PREFIX = "flog";
		public const string SPATIAL_MAP_LATERAL_FLOW_GROUP_VARIABLE_PREFIX = "lflog";
		public const string SPATIAL_MAP_AVG_STOCK_GROUP_VARIABLE_PREFIX = "avgstkg";
		public const string SPATIAL_MAP_AVG_FLOW_GROUP_VARIABLE_PREFIX = "avgflog";

		//Spatial Map file naming Regex filter, containing 1 ID value
		public const string FILE_FILTER_ID_REGEX = "^(.*){0}-([\\d]*)\\.(tif|vrt)$";

		//Diagram
		public const int DIAGRAM_MAX_ROWS = 256;
		public const int DIAGRAM_MAX_COLUMNS = 26;
		public const int DIAGRAM_ITEM_HEIGHT = 25;
		public const int DIAGRAM_TITLE_BAR_HEIGHT = 25;
		public const int DIAGRAM_SHAPE_PADDING = 44;
		public const int DIAGRAM_SHAPE_SIZE = (DIAGRAM_TITLE_BAR_HEIGHT + (2 * DIAGRAM_ITEM_HEIGHT));
		public const int DIAGRAM_SPACE_BETWEEN_SHAPES = 2 * DIAGRAM_SHAPE_PADDING;
		public const int DIAGRAM_NUM_VERTICAL_CONNECTORS = 9;
		public const int DIAGRAM_NUM_HORIZONTAL_CONNECTORS = 9;
		public const int DIAGRAM_OFF_STRATUM_CUE_SIZE = 14;
		public const int DIAGRAM_LANES_BETWEEN_SHAPES = 11;
		public const int ZOOM_SAFE_PEN_WIDTH = -1;

        public static Color READONLY_COLUMN_COLOR = Color.FromArgb(232, 232, 232);
        public static Color DIAGRAM_SHAPE_TEXT_COLOR = Color.Black;
		public static Color DIAGRAM_SHAPE_READONLY_TEXT_COLOR = Color.Gray;
		public static Color DIAGRAM_SHAPE_BORDER_COLOR = Color.Gray;
		public static Color DIAGRAM_SHAPE_READONLY_BORDER_COLOR = Color.LightGray;
		public static Color DIAGRAM_SHAPE_BACKGROUND_COLOR = Color.FromArgb(255, 240, 240, 240);
		public static Color DIAGRAM_FLOW_PATHWAY_LINE_COLOR = Color.CornflowerBlue;
		public static Font DIAGRAM_SHAPE_NORMAL_FONT = new Font("Segoe UI", 9.0F, FontStyle.Regular);
	}
}