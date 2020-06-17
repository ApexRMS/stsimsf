// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		/// <summary>
		/// Sets whether or not this is a spatial model run
		/// </summary>
		/// <remarks></remarks>
		private void InitializeSpatialRunFlag()
		{
			DataRow drrc = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STSIM_RUN_CONTROL).GetDataRow();
			this.m_IsSpatial = DataTableUtilities.GetDataBool(drrc["IsSpatial"]);
		}

		/// <summary>
		/// Initializes the flags for controlling SecondaryStratum and TertiaryStratum output
		/// </summary>
		private void Initialize_SS_TS_Flags()
		{
			DataRow dr = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STSIM_OUTPUT_OPTIONS).GetDataRow();

			if (dr != null)
			{
				this.m_SummaryOmitSecondaryStrata = DataTableUtilities.GetDataBool(dr, "SummaryOutputOmitSS");
				this.m_SummaryOmitTertiaryStrata = DataTableUtilities.GetDataBool(dr, "SummaryOutputOmitTS");
			}
		}

		/// <summary>
		/// Sets the Flow Order Options
		/// </summary>
		/// <remarks></remarks>
		private void InitializeFlowOrderOptions()
		{
			DataRow dr = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_ORDER_OPTIONS).GetDataRow();

			if (dr != null)
			{
				this.m_ApplyBeforeTransitions = DataTableUtilities.GetDataBool(dr, "ApplyBeforeTransitions");
				this.m_ApplyEquallyRankedSimultaneously = DataTableUtilities.GetDataBool(dr, "ApplyEquallyRankedSimultaneously");
			}
		}

		/// <summary>
		/// Initializes the output options
		/// </summary>
		/// <remarks></remarks>
		private void InitializeOutputOptions()
		{
			DataRow droo = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OO_NAME).GetDataRow();

			Func<object, int> SafeInt = (object o) =>
			{
				if (o == DBNull.Value)
				{
					return 0;
				}
				else
				{
					return Convert.ToInt32(o, CultureInfo.InvariantCulture);
				}
			};

			this.m_CreateSummaryStockOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME]);
			this.m_SummaryStockOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateSummaryFlowOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME]);
			this.m_SummaryFlowOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateSpatialStockOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME]);
			this.m_SpatialStockOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateSpatialFlowOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME]);
			this.m_SpatialFlowOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateLateralFlowOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_LATERAL_OUTPUT_FL_COLUMN_NAME]);
			this.m_LateralFlowOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_LATERAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateAvgSpatialStockOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_COLUMN_NAME]);
			this.m_AvgSpatialStockOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME]);
            this.m_AvgSpatialStockOutputAcrossTimesteps = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_ST_ACROSS_TIMESTEPS_COLUMN_NAME]);
			this.m_CreateAvgSpatialFlowOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_COLUMN_NAME]);
			this.m_AvgSpatialFlowOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME]);
            this.m_AvgSpatialFlowOutputAcrossTimesteps = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_FL_ACROSS_TIMESTEPS_COLUMN_NAME]);
            this.m_CreateAvgSpatialLateralFlowOutput = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_LFL_COLUMN_NAME]);
			this.m_AvgSpatialLateralFlowOutputTimesteps = SafeInt(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_LFL_TIMESTEPS_COLUMN_NAME]);
            this.m_AvgSpatialLateralFlowOutputAcrossTimesteps = DataTableUtilities.GetDataBool(droo[Constants.DATASHEET_OO_AVG_SPATIAL_OUTPUT_LFL_ACROSS_TIMESTEPS_COLUMN_NAME]);
        }

        /// <summary>
        /// Initializes all distribution values
        /// </summary>
        private void InitializeDistributionValues()
		{
			this.InitializeFlowMultiplierDistributionValues();
			this.InitializeStockTransitionMultiplierDistributionValues();
		}

		/// <summary>
		/// Initializes distribution values for the flow multipliers
		/// </summary>
		private void InitializeFlowMultiplierDistributionValues()
		{
			try
			{
				foreach (FlowMultiplier t in this.m_FlowMultipliers)
				{
					t.Initialize(
                        this.m_STSimTransformer.MinimumIteration, 
                        this.m_STSimTransformer.MinimumTimestep, 
                        this.m_STSimTransformer.DistributionProvider);
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Flow Multipliers" + " -> " + ex.Message);
			}
		}

		/// <summary>
		/// Initializes distribution values for the stock transition multipliers
		/// </summary>
		private void InitializeStockTransitionMultiplierDistributionValues()
		{
			try
			{
				foreach (StockTransitionMultiplier t in this.m_StockTransitionMultipliers)
				{
					t.Initialize(
                        this.m_STSimTransformer.MinimumIteration, 
                        this.m_STSimTransformer.MinimumTimestep, 
                        this.m_STSimTransformer.DistributionProvider);
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Stock Transition Multipliers" + " -> " + ex.Message);
			}
		}

		/// <summary>
		/// Initializes a separate list of Flow Types that can be shuffled
		/// </summary>
		/// <remarks>
		/// The main list is keyed and cannot be shuffled, but we need a shuffled list for doing raster simulations
		/// </remarks>
		private void InitializeShufflableFlowTypes()
		{
			Debug.Assert(this.m_ShufflableFlowTypes.Count == 0);

			foreach (FlowType ft in this.m_FlowTypes)
			{
				this.m_ShufflableFlowTypes.Add(ft);
			}
		}

        /// <summary>
        /// Adds automatic stock type/group linkages
        /// </summary>
        private void AddAutoStockTypeLinkages()
        {
            DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME);
            DataTable dt = ds.GetData();

            foreach (StockType t in this.m_StockTypes)
            {
                DataRow NewRow = dt.NewRow();

                NewRow[Constants.STOCK_TYPE_ID_COLUMN_NAME] = t.Id;
                NewRow[Constants.STOCK_GROUP_ID_COLUMN_NAME] = this.GetAutoGeneratedStockGroup(t).Id;

                dt.Rows.Add(NewRow);
            } 
#if DEBUG
            this.m_AutoStockLinkagesAdded = true;
#endif
        }

        /// <summary>
        /// Adds automatic flow type/group linkages
        /// </summary>
        private void AddAutoFlowTypeLinkages()
        {
            DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME);
            DataTable dt = ds.GetData();

            foreach (FlowType t in this.m_FlowTypes)
            {
                DataRow NewRow = dt.NewRow();

                NewRow[Constants.FLOW_TYPE_ID_COLUMN_NAME] = t.Id;
                NewRow[Constants.FLOW_GROUP_ID_COLUMN_NAME] = this.GetAutoGeneratedFlowGroup(t).Id;

                dt.Rows.Add(NewRow);
            }

#if DEBUG
            this.m_AutoFlowLinkagesAdded = true;
#endif
        }

        private StockGroup GetAutoGeneratedStockGroup(StockType t)
        {
            string n = DataTableUtilities.GetAutoGeneratedGroupName(t.Name);

            foreach (StockGroup g in this.m_StockGroups)
            {
                if (g.Name == n)
                {
                    return g;
                }
            }

            throw new ArgumentException("Auto-generated group not found for stock type: " + t.Name);
        }

        private FlowGroup GetAutoGeneratedFlowGroup(FlowType t)
        {
            string n = DataTableUtilities.GetAutoGeneratedGroupName(t.Name);

            foreach (FlowGroup g in this.m_FlowGroups)
            {
                if (g.Name == n)
                {
                    return g;
                }
            }

            throw new ArgumentException("Auto-generated group not found for flow type: " + t.Name);
        }

        private void InitializeAverageStockMap()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.STSimTransformer.MinimumTimestep > 0);

            if (!this.m_CreateAvgSpatialStockOutput)
            {
                return;
            }

            foreach (StockGroup sg in this.m_StockGroups)
            {
                Dictionary<int, double[]> dict = new Dictionary<int, double[]>();

                for (var timestep = this.STSimTransformer.MinimumTimestep; timestep <= this.STSimTransformer.MaximumTimestep; timestep++)
                {
                    if (this.m_STSimTransformer.IsOutputTimestepAverage(
                        timestep,
                        this.m_AvgSpatialStockOutputTimesteps,
                        this.m_CreateAvgSpatialStockOutput))
                    {
                        double[] values = new double[this.STSimTransformer.Cells.Count];

                        for (var i = 0; i < this.STSimTransformer.Cells.Count; i++)
                        {
                            values[i] = 0;
                        }

                        dict.Add(timestep, values);
                    }
                }

                if (!dict.ContainsKey(this.STSimTransformer.TimestepZero))
                {
                    double[] values = new double[this.STSimTransformer.Cells.Count];

                    for (var i = 0; i < this.STSimTransformer.Cells.Count; i++)
                    {
                        values[i] = 0;
                    }

                    dict.Add(this.STSimTransformer.TimestepZero, values);
                }

                this.m_AvgStockMap.Add(sg.Id, dict);
            }
        }

        private void InitializeAverageFlowMap()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.STSimTransformer.MinimumTimestep > 0);

            if (!this.m_CreateAvgSpatialFlowOutput)
            {
                return;
            }

            foreach (FlowGroup fg in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = new Dictionary<int, double[]>();

                for (var timestep = this.STSimTransformer.MinimumTimestep; timestep <= this.STSimTransformer.MaximumTimestep; timestep++)
                {
                    if (this.m_STSimTransformer.IsOutputTimestepAverage(
                        timestep,
                        this.m_AvgSpatialFlowOutputTimesteps, 
                        this.m_CreateAvgSpatialFlowOutput))
                    { 
                        double[] values = new double[this.STSimTransformer.Cells.Count];

                        for (var i = 0; i < this.STSimTransformer.Cells.Count; i++)
                        {
                            values[i] = 0;
                        }

                        dict.Add(timestep, values);
                    }
                }

                this.m_AvgFlowMap.Add(fg.Id, dict);
            }
        }

        private void InitializeAverageLateralFlowMap()
        {
            Debug.Assert(this.STSimTransformer.IsSpatial);
            Debug.Assert(this.STSimTransformer.MinimumTimestep > 0);

            if (!this.m_CreateAvgSpatialLateralFlowOutput)
            {
                return;
            }

            foreach (FlowGroup fg in this.m_FlowGroups)
            {
                Dictionary<int, double[]> dict = new Dictionary<int, double[]>();

                for (var timestep = this.STSimTransformer.MinimumTimestep; timestep <= this.STSimTransformer.MaximumTimestep; timestep++)
                {
                    if (this.m_STSimTransformer.IsOutputTimestepAverage(
                        timestep,
                        this.m_AvgSpatialLateralFlowOutputTimesteps,
                        this.m_CreateAvgSpatialLateralFlowOutput))
                    {
                        double[] values = new double[this.STSimTransformer.Cells.Count];

                        for (var i = 0; i < this.STSimTransformer.Cells.Count; i++)
                        {
                            values[i] = 0;
                        }

                        dict.Add(timestep, values);
                    }
                }

                this.m_AvgLateralFlowMap.Add(fg.Id, dict);
            }
        }
    }
}
