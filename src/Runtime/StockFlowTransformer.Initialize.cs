// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;

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
			DataRow drrc = this.ResultScenario.GetDataSheet("STSim_RunControl").GetDataRow();
			this.m_IsSpatial = DataTableUtilities.GetDataBool(drrc["IsSpatial"]);
		}

		/// <summary>
		/// Initializes the flags for controlling SecondaryStratum and TertiaryStratum output
		/// </summary>
		private void Initialize_SS_TS_Flags()
		{
			DataRow dr = this.ResultScenario.GetDataSheet("STSim_OutputOptions").GetDataRow();

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

			foreach (FlowType ft in this.m_FlowTypes.Values)
			{
				this.m_ShufflableFlowTypes.Add(ft);
			}
		}
	}
}