// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		private Dictionary<int, FlowType> m_FlowTypes = new Dictionary<int, FlowType>();
		private FlowGroupCollection m_FlowGroups = new FlowGroupCollection();
		private StockTypeCollection m_StockTypes = new StockTypeCollection();
		private InitialStockNonSpatialCollection m_InitialStocksNonSpatial = new InitialStockNonSpatialCollection();
		private InitialStockSpatialCollection m_InitialStocksSpatial = new InitialStockSpatialCollection();
		private Dictionary<string, StochasticTimeRaster> m_InitialStockSpatialRasters = new Dictionary<string, StochasticTimeRaster>();
		private StockLimitCollection m_StockLimits = new StockLimitCollection();
		private StockTransitionMultiplierCollection m_StockTransitionMultipliers = new StockTransitionMultiplierCollection();
		private FlowPathwayCollection m_FlowPathways = new FlowPathwayCollection();
		private FlowMultiplierCollection m_FlowMultipliers = new FlowMultiplierCollection();
		private FlowSpatialMultiplierCollection m_FlowSpatialMultipliers = new FlowSpatialMultiplierCollection();
		private Dictionary<string, StochasticTimeRaster> m_FlowSpatialMultiplierRasters = new Dictionary<string, StochasticTimeRaster>();
		private FlowOrderCollection m_FlowOrders = new FlowOrderCollection();

#if DEBUG
		private bool m_FlowGroupsFilled;
		private bool m_FlowTypesFilled;
#endif

		private void FillFlowGroups()
		{
			Debug.Assert(this.m_FlowGroups.Count == 0);
			DataSheet ds = this.Project.GetDataSheet(Constants.DATASHEET_FLOW_GROUP_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				this.m_FlowGroups.Add(new FlowGroup(
                    Convert.ToInt32(dr[ds.PrimaryKeyColumn.Name], CultureInfo.InvariantCulture)));
			}

#if DEBUG
			this.m_FlowGroupsFilled = true;
#endif
		}

		private void FillFlowTypes()
		{       
			Debug.Assert(this.m_FlowTypes.Count == 0);
			DataSheet ds = this.Project.GetDataSheet(Constants.DATASHEET_FLOW_TYPE_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int id = Convert.ToInt32(dr[ds.PrimaryKeyColumn.Name], CultureInfo.InvariantCulture);
				FlowType ft = new FlowType(id);
				this.m_FlowTypes.Add(id, ft);
			}

#if DEBUG
			this.m_FlowTypesFilled = true;
#endif

		}

		private void FillFlowTypeGroups()
		{

#if DEBUG
			Debug.Assert(this.m_FlowGroupsFilled);
			Debug.Assert(this.m_FlowTypesFilled);
#endif

			DataTable dt = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_TYPE_GROUP_MEMBERSHIP_NAME).GetData();

			foreach (FlowType ft in this.m_FlowTypes.Values)
			{
				string q = string.Format(CultureInfo.InvariantCulture, "{0}={1}", Constants.FLOW_TYPE_ID_COLUMN_NAME, ft.Id);
				DataRow[] rows = dt.Select(q, null);

				foreach (DataRow dr in rows)
				{
					int gid = Convert.ToInt32(dr[Constants.FLOW_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

					Debug.Assert(!ft.FlowGroups.Contains(gid));
					ft.FlowGroups.Add(this.m_FlowGroups[gid]);
				}
			}
		}

		private void FillStockTypes()
		{
			Debug.Assert(this.m_StockTypes.Count == 0);
			DataSheet ds = this.Project.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				this.m_StockTypes.Add(
                    new StockType(Convert.ToInt32(dr[ds.PrimaryKeyColumn.Name], CultureInfo.InvariantCulture)));
			}
		}

		private void FillInitialStocksNonSpatial()
		{
			Debug.Assert(this.m_InitialStocksNonSpatial.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_INITIAL_STOCK_NON_SPATIAL);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				this.m_InitialStocksNonSpatial.Add(
                    new InitialStockNonSpatial(
                        Convert.ToInt32(dr[ds.PrimaryKeyColumn.Name], CultureInfo.InvariantCulture), 
                        Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture),
                        Convert.ToInt32(dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture)));
			}
		}

		private void FillInitialStocksSpatial()
		{
			Debug.Assert(this.m_IsSpatial);
			Debug.Assert(this.m_InitialStocksSpatial.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_INITIAL_STOCK_SPATIAL);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				string stockFileName = Convert.ToString(dr[Constants.RASTER_FILE_COLUMN_NAME], CultureInfo.InvariantCulture);
				string fullFilename = Spatial.GetSpatialInputFileName(ds, stockFileName, false);
				int? Iteration = null;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

                //Load Initial Stock raster file
                StochasticTimeRaster raster;

				try
				{
					raster = new StochasticTimeRaster(fullFilename, RasterDataType.DTDouble);
				}
				catch (Exception)
				{
					string message = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_FILE_STOCK_LOAD_WARNING, stockFileName);
					throw new ArgumentException(message);
				}

				//Compare the Stock raster metadata to standard to make rasters match
				string cmpMsg = "";
				var cmpResult = this.STSimTransformer.InputRasters.CompareMetadata(raster, ref cmpMsg);

				if (cmpResult == STSim.CompareMetadataResult.ImportantDifferences)
				{
					string message = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_FILE_STOCK_METADATA_WARNING, stockFileName, cmpMsg);
					this.RecordStatus(StatusType.Warning, message);
				}
				else
				{
					if (cmpResult == STSim.CompareMetadataResult.UnimportantDifferences)
					{
						string message = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_FILE_STOCK_METADATA_INFO, stockFileName, cmpMsg);
						this.RecordStatus(StatusType.Information, message);
					}

					this.m_InitialStocksSpatial.Add(
                        new InitialStockSpatial(
                            Iteration, 
                            Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture), 
                            stockFileName));

					//Only loading single instance of a particular raster, as a way to converse memory

					if (!m_InitialStockSpatialRasters.ContainsKey(stockFileName))
					{
						this.m_InitialStockSpatialRasters.Add(stockFileName, raster);
					}
				}
			}
		}

		private void FillStockLimits()
		{
			Debug.Assert(this.m_StockLimits.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STOCK_LIMIT_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int? Iteration = null;
				int? Timestep = null;
				int StockTypeId = 0;
				int? StratumId = null;
				int? SecondaryStratumId = null;
				int? TertiaryStratumId = null;
				int? StateClassId = null;
				double StockMin = double.MinValue;
				double StockMax = double.MaxValue;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				StockTypeId = Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					StratumId = Convert.ToInt32(dr[Constants.STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					SecondaryStratumId = Convert.ToInt32(dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					TertiaryStratumId = Convert.ToInt32(dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
				{
					StateClassId = Convert.ToInt32(dr[Constants.STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STOCK_MIN_COLUMN_NAME] != DBNull.Value)
				{
					StockMin = Convert.ToDouble(dr[Constants.STOCK_MIN_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STOCK_MAX_COLUMN_NAME] != DBNull.Value)
				{
					StockMax = Convert.ToDouble(dr[Constants.STOCK_MAX_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				this.m_StockLimits.Add(new StockLimit(
                    Iteration, Timestep, StockTypeId, StratumId,
                    SecondaryStratumId, TertiaryStratumId, StateClassId, StockMin, StockMax));
			}
		}

		private void FillStockTransitionMultipliers()
		{
			Debug.Assert(this.m_StockTransitionMultipliers.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STOCK_TRANSITION_MULTIPLIER_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int? Iteration = null;
				int? Timestep = null;
				int? StratumId = null;
				int? SecondaryStratumId = null;
				int? TertiaryStratumId = null;
				int? StateClassId = null;
				int TransitionGroupId = Convert.ToInt32(dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				int StockGroupId = Convert.ToInt32(dr[Constants.STOCK_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				double StockValue = Convert.ToDouble(dr[Constants.STOCK_VALUE_COLUMN_NAME], CultureInfo.InvariantCulture);
				double? Multiplier = null;
				int? DistributionTypeId = null;
				DistributionFrequency? DistributionFrequency = null;
				double? DistributionSD = null;
				double? DistributionMin = null;
				double? DistributionMax = null;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					StratumId = Convert.ToInt32(dr[Constants.STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					SecondaryStratumId = Convert.ToInt32(dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					TertiaryStratumId = Convert.ToInt32(dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
				{
					StateClassId = Convert.ToInt32(dr[Constants.STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.MULTIPLIER_COLUMN_NAME] != DBNull.Value)
				{
					Multiplier = Convert.ToDouble(dr[Constants.MULTIPLIER_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONTYPE_COLUMN_NAME] != DBNull.Value)
				{
					DistributionTypeId = Convert.ToInt32(dr[Constants.DISTRIBUTIONTYPE_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTION_FREQUENCY_COLUMN_NAME] != DBNull.Value)
				{
					DistributionFrequency = (DistributionFrequency)(long)dr[Constants.DISTRIBUTION_FREQUENCY_COLUMN_NAME];
				}

				if (dr[Constants.DISTRIBUTIONSD_COLUMN_NAME] != DBNull.Value)
				{
					DistributionSD = Convert.ToDouble(dr[Constants.DISTRIBUTIONSD_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONMIN_COLUMN_NAME] != DBNull.Value)
				{
					DistributionMin = Convert.ToDouble(dr[Constants.DISTRIBUTIONMIN_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONMAX_COLUMN_NAME] != DBNull.Value)
				{
					DistributionMax = Convert.ToDouble(dr[Constants.DISTRIBUTIONMAX_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				try
				{
					StockTransitionMultiplier Item = new StockTransitionMultiplier(
                        Iteration, Timestep, StratumId, SecondaryStratumId, TertiaryStratumId, StateClassId, 
                        TransitionGroupId, StockGroupId, StockValue, Multiplier, DistributionTypeId, DistributionFrequency, 
                        DistributionSD, DistributionMin, DistributionMax);

					this.m_STSimTransformer.DistributionProvider.Validate(
                        Item.DistributionTypeId, Item.DistributionValue, Item.DistributionSD, 
                        Item.DistributionMin, Item.DistributionMax);

					this.m_StockTransitionMultipliers.Add(Item);

				}
				catch (Exception ex)
				{
					throw new ArgumentException(ds.DisplayName + " -> " + ex.Message);
				}
			}
		}

		private void FillFlowPathways()
		{      
			Debug.Assert(this.m_FlowPathways.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int? Iteration = null;
				int? Timestep = null;
				int? FromStratumId = null;
				int? FromStateClassId = null;
				int? FromMinimumAge = null;
				int FromStockTypeId = 0;
				int? ToStratumId = null;
				int? ToStateClassId = null;
				int? ToMinimumAge = null;
				int ToStockTypeId = 0;
				int TransitionGroupId = 0;
				int? StateAttributeTypeId = null;
				int FlowTypeId = 0;
				double Multiplier = 0;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.FROM_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					FromStratumId = Convert.ToInt32(dr[Constants.FROM_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
				{
					FromStateClassId = Convert.ToInt32(dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.FROM_MIN_AGE_COLUMN_NAME] != DBNull.Value)
				{
					FromMinimumAge = Convert.ToInt32(dr[Constants.FROM_MIN_AGE_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				FromStockTypeId = Convert.ToInt32(dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.TO_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					ToStratumId = Convert.ToInt32(dr[Constants.TO_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TO_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
				{
					ToStateClassId = Convert.ToInt32(dr[Constants.TO_STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TO_MIN_AGE_COLUMN_NAME] != DBNull.Value)
				{
					ToMinimumAge = Convert.ToInt32(dr[Constants.TO_MIN_AGE_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				ToStockTypeId = Convert.ToInt32(dr[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME] != DBNull.Value)
				{
					TransitionGroupId = Convert.ToInt32(dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}
				else
				{
					TransitionGroupId = 0;
				}

				if (dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME] != DBNull.Value)
				{
					StateAttributeTypeId = Convert.ToInt32(dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				FlowTypeId = Convert.ToInt32(dr[Constants.FLOW_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				Multiplier = Convert.ToDouble(dr[Constants.MULTIPLIER_COLUMN_NAME], CultureInfo.InvariantCulture);

				this.m_FlowPathways.Add(
                    new FlowPathway(
                        Iteration, Timestep, FromStratumId, FromStateClassId, 
                        FromMinimumAge, FromStockTypeId, ToStratumId, ToStateClassId, ToMinimumAge, 
                        ToStockTypeId, TransitionGroupId, StateAttributeTypeId, FlowTypeId, Multiplier));
			}      
		}

		private void FillFlowMultipliers()
		{
			Debug.Assert(this.m_FlowMultipliers.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_MULTIPLIER_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int? Iteration = null;
				int? Timestep = null;
				int? StratumId = null;
				int? SecondaryStratumId = null;
				int? TertiaryStratumId = null;
				int? StateClassId = null;
				int FlowGroupId = 0;
				double? MultiplierAmount = null;
				int? DistributionTypeId = null;
				DistributionFrequency? DistributionFrequency = null;
				double? DistributionSD = null;
				double? DistributionMin = null;
				double? DistributionMax = null;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					StratumId = Convert.ToInt32(dr[Constants.STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					SecondaryStratumId = Convert.ToInt32(dr[Constants.SECONDARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
				{
					TertiaryStratumId = Convert.ToInt32(dr[Constants.TERTIARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
				{
					StateClassId = Convert.ToInt32(dr[Constants.STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				FlowGroupId = Convert.ToInt32(dr[Constants.FLOW_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.VALUE_COLUMN_NAME] != DBNull.Value)
				{
					MultiplierAmount = Convert.ToDouble(dr[Constants.VALUE_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONTYPE_COLUMN_NAME] != DBNull.Value)
				{
					DistributionTypeId = Convert.ToInt32(dr[Constants.DISTRIBUTIONTYPE_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTION_FREQUENCY_COLUMN_NAME] != DBNull.Value)
				{
					DistributionFrequency = (DistributionFrequency)(long)dr[Constants.DISTRIBUTION_FREQUENCY_COLUMN_NAME];
				}

				if (dr[Constants.DISTRIBUTIONSD_COLUMN_NAME] != DBNull.Value)
				{
					DistributionSD = Convert.ToDouble(dr[Constants.DISTRIBUTIONSD_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONMIN_COLUMN_NAME] != DBNull.Value)
				{
					DistributionMin = Convert.ToDouble(dr[Constants.DISTRIBUTIONMIN_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.DISTRIBUTIONMAX_COLUMN_NAME] != DBNull.Value)
				{
					DistributionMax = Convert.ToDouble(dr[Constants.DISTRIBUTIONMAX_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				try
				{
					FlowMultiplier Item = new FlowMultiplier(
                        Iteration, Timestep, StratumId, SecondaryStratumId, TertiaryStratumId, StateClassId, 
                        FlowGroupId, MultiplierAmount, DistributionTypeId, DistributionFrequency, 
                        DistributionSD, DistributionMin, DistributionMax);

					this.m_STSimTransformer.DistributionProvider.Validate(
                        Item.DistributionTypeId, Item.DistributionValue, Item.DistributionSD, 
                        Item.DistributionMin, Item.DistributionMax);

					this.m_FlowMultipliers.Add(Item);

				}
				catch (Exception ex)
				{
					throw new ArgumentException(ds.DisplayName + " -> " + ex.Message);
				}
			}
		}

		private void FillFlowOrders()
		{       
			Debug.Assert(this.m_FlowOrders.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_ORDER);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int? Iteration = null;
				int? Timestep = null;
				int FlowTypeId = 0;
				double? Order = null;

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				FlowTypeId = Convert.ToInt32(dr[Constants.FLOW_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME] != DBNull.Value)
				{
					Order = Convert.ToDouble(dr[Constants.DATASHEET_FLOW_ORDER_ORDER_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				try
				{
					FlowOrder Item = new FlowOrder(Iteration, Timestep, FlowTypeId, Order);
					this.m_FlowOrders.Add(Item);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(ds.DisplayName + " -> " + ex.Message);
				}
			}
		}

		private void FillFlowSpatialMultipliers()
		{
			Debug.Assert(this.m_IsSpatial);
			Debug.Assert(this.m_FlowSpatialMultipliers.Count == 0);
			Debug.Assert(this.m_FlowSpatialMultiplierRasters.Count == 0);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME);

			foreach (DataRow dr in ds.GetData().Rows)
			{
				int FlowGroupId = Convert.ToInt32(dr[Constants.FLOW_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
				int? Iteration = null;
				int? Timestep = null;
				string FileName = Convert.ToString(dr[Constants.MULTIPLIER_FILE_COLUMN_NAME], CultureInfo.InvariantCulture);

				if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
				{
					Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
				{
					Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
				}

				FlowSpatialMultiplier Multiplier = new FlowSpatialMultiplier(FlowGroupId, Iteration, Timestep, FileName);
				string FullFilename = Spatial.GetSpatialInputFileName(ds, FileName, false);
                StochasticTimeRaster MultiplierRaster;

				try
				{
					MultiplierRaster = new StochasticTimeRaster(FullFilename, RasterDataType.DTDouble);
				}
				catch (Exception)
				{
					string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_PROCESS_WARNING, FullFilename);
					throw new ArgumentException(msg);
				}

				this.m_FlowSpatialMultipliers.Add(Multiplier);

				//Only load a single instance of a each unique filename to conserve memory

				if (!this.m_FlowSpatialMultiplierRasters.ContainsKey(FileName))
				{
					this.m_FlowSpatialMultiplierRasters.Add(FileName, MultiplierRaster);
				}
			}
		}
	}
}