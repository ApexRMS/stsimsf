//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using SyncroSim.Core;
using SyncroSim.STSim;
using SyncroSim.Common;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal partial class StockFlowTransformer : Transformer
	{
		private bool m_IsSpatial;
		private bool m_ApplyBeforeTransitions;
		private bool m_ApplyEquallyRankedSimultaneously;
		private bool m_SummaryOmitSecondaryStrata;
		private bool m_SummaryOmitTertiaryStrata;
		private STSimTransformer m_STSimTransformer;
		private bool m_CanComputeStocksAndFlows;
		private RandomGenerator m_RandomGenerator = new RandomGenerator();
		private List<FlowType> m_ShufflableFlowTypes = new List<FlowType>();

		/// <summary>
		/// Gets the ST-Sim Transformer
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		protected STSimTransformer STSimTransformer
		{
			get
			{
				return this.m_STSimTransformer;
			}
		}

		/// <summary>
		/// Overrides Configure
		/// </summary>
		/// <remarks></remarks>
		public override void Configure()
		{
			base.Configure();

			this.m_STSimTransformer = this.GetSTSimTransformer();
			this.m_CanComputeStocksAndFlows = this.CanComputeStocksAndFlows();

			if (this.m_CanComputeStocksAndFlows)
			{
				this.NormalizeOutputOptions();
			}
		}

		/// <summary>
		/// Overrides Initialize
		/// </summary>
		public override void Initialize()
		{
			this.m_STSimTransformer = this.GetSTSimTransformer();
			this.m_CanComputeStocksAndFlows = this.CanComputeStocksAndFlows();

			if (!this.m_CanComputeStocksAndFlows)
			{
				return;
			}

			this.InitializeSpatialRunFlag();
			this.Initialize_SS_TS_Flags();
			this.InitializeFlowOrderOptions();
			this.InitializeOutputOptions();
			this.InitializeOutputDataTables();

			this.FillFlowGroups();
			this.FillFlowTypes();
			this.FillFlowTypeGroups();
			this.FillStockTypes();
			this.FillInitialStocksNonSpatial();
			this.FillStockLimits();
			this.FillStockTransitionMultipliers();
			this.FillFlowPathways();
			this.FillFlowMultipliers();
			this.FillFlowOrders();

			if (this.m_IsSpatial)
			{
				this.FillInitialStocksSpatial();
				this.FillFlowSpatialMultipliers();
				this.ValidateFlowSpatialMultipliers();
			}

			this.NormalizeForUserDistributions();
			this.InitializeDistributionValues();
			this.InitializeShufflableFlowTypes();
			this.CreateStockLimitMap();
			this.CreateStockTransitionMultiplierMap();
			this.CreateFlowPathwayMap();
			this.CreateFlowMultiplierMap();
			this.CreateFlowSpatialMultiplierMap();
			this.CreateFlowOrderMap();

			if (this.m_IsSpatial)
			{
				this.CreateInitialStockSpatialMap();
			}
		}

		/// <summary>
		/// Overrides Transform
		/// </summary>
		/// <remarks>
		/// NOTE: We must initialize the ST-Sim transformer and the flag indicating whether or
		/// not we can do stocks and flows here because we might be loaded in a separate process as
		/// part of an MP run.
		/// </remarks>
		public override void Transform()
		{
			if (!this.m_CanComputeStocksAndFlows)
			{
				return;
			}

			this.STSimTransformer.CellInitialized += this.OnSTSimCellInitialized;
			this.STSimTransformer.CellsInitialized += this.OnSTSimAfterCellsInitialized;
			this.STSimTransformer.ChangingCellProbabilistic += this.OnSTSimBeforeChangeCellProbabilistic;
			this.STSimTransformer.ChangingCellDeterministic += this.OnSTSimBeforeChangeCellDeterministic;
			this.STSimTransformer.CellBeforeTransitions += this.OnSTSimCellBeforeTransitions;
			this.STSimTransformer.IterationStarted += this.OnSTSimBeforeIteration;
			this.STSimTransformer.TimestepStarted += this.OnSTSimBeforeTimestep;
			this.STSimTransformer.TimestepCompleted += this.OnSTSimAfterTimestep;

			if (this.m_StockTransitionMultipliers.Count > 0)
			{
				this.STSimTransformer.ApplyingTransitionMultipliers += this.OnApplyingTransitionMultipliers;
			}
		}

		/// <summary>
		/// Disposes this instance
		/// </summary>
		/// <param name="disposing"></param>
		/// <remarks></remarks>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.IsDisposed)
			{
				if (this.m_CanComputeStocksAndFlows)
				{
					this.STSimTransformer.CellInitialized -= this.OnSTSimCellInitialized;
					this.STSimTransformer.CellsInitialized -= this.OnSTSimAfterCellsInitialized;
					this.STSimTransformer.ChangingCellProbabilistic -= this.OnSTSimBeforeChangeCellProbabilistic;
					this.STSimTransformer.ChangingCellDeterministic -= this.OnSTSimBeforeChangeCellDeterministic;
					this.STSimTransformer.IterationStarted -= this.OnSTSimBeforeIteration;
					this.STSimTransformer.TimestepStarted -= this.OnSTSimBeforeTimestep;
					this.STSimTransformer.TimestepCompleted -= this.OnSTSimAfterTimestep;

					if (this.m_StockTransitionMultipliers.Count > 0)
					{
						this.STSimTransformer.ApplyingTransitionMultipliers -= this.OnApplyingTransitionMultipliers;
					}
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Overrides External Data Ready
		/// </summary>
		/// <param name="dataSheet"></param>
		/// <param name="previousData"></param>
		protected override void OnExternalDataReady(DataSheet dataSheet, DataTable previousData)
		{
			if (!this.m_CanComputeStocksAndFlows)
			{
				return;
			}

			if (dataSheet.Name == Constants.DATASHEET_FLOW_PATHWAY_NAME)
			{
				this.m_FlowPathways.Clear();
				this.FillFlowPathways();
				this.m_FlowPathwayMap = null;
				this.CreateFlowPathwayMap();
			}
			else if (dataSheet.Name == Constants.DATASHEET_FLOW_MULTIPLIER_NAME)
			{
				this.m_FlowMultipliers.Clear();
				this.FillFlowMultipliers();
				this.InitializeFlowMultiplierDistributionValues();
				this.m_FlowMultiplierMap = null;
				this.CreateFlowMultiplierMap();
			}
			else if (dataSheet.Name == Constants.DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME)
			{
				if (this.m_IsSpatial)
				{
					this.m_FlowSpatialMultipliers.Clear();
					this.m_FlowSpatialMultiplierRasters.Clear();
					this.m_FlowSpatialMultiplierMap = null;

					this.FillFlowSpatialMultipliers();
					this.ValidateFlowSpatialMultipliers();
					this.CreateFlowSpatialMultiplierMap();
				}
			}
			else if (dataSheet.Name == Constants.DATASHEET_FLOW_ORDER)
			{
				this.m_FlowOrders.Clear();
				this.FillFlowOrders();
				this.m_FlowOrderMap = null;
				this.CreateFlowOrderMap();
			}
			else if (dataSheet.Name == Constants.DATASHEET_STOCK_LIMIT_NAME)
			{
				this.m_StockLimits.Clear();
				this.FillStockLimits();
				this.m_StockLimitMap = null;
				this.CreateStockLimitMap();
			}
			else if (dataSheet.Name == Constants.DATASHEET_STOCK_TRANSITION_MULTIPLIER_NAME)
			{
				this.m_StockTransitionMultipliers.Clear();
				this.FillStockTransitionMultipliers();
				this.m_StockTransitionMultiplierMap = null;
				this.CreateStockTransitionMultiplierMap();
			}
			else
			{
				string msg = string.Format(CultureInfo.InvariantCulture, "External data is not supported for: {0}", dataSheet.Name);
				throw new TransformerFailedException(msg);
			}
		}

		/// <summary>
		/// Handles the BeforeIteration event. We run this raster verification code here
		/// as it depends of the STSim rasters having been loaded (it's a timing thing).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimBeforeIteration(object sender, IterationEventArgs e)
		{
			this.ResampleFlowMultiplierValues(
                e.Iteration, 
                this.m_STSimTransformer.MinimumTimestep, 
                DistributionFrequency.Iteration);
		}

		/// <summary>
		/// Handles the BeforeTimestep event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimBeforeTimestep(object sender, TimestepEventArgs e)
		{
			//Is it spatial flow output timestep.  If so, then iterate over flow types and initialize an output raster 
			//for each flow type Initialize to DEFAULT_NODATA_VALUE

			if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SpatialFlowOutputTimesteps, this.m_CreateSpatialFlowOutput))
			{
				foreach (FlowType ft in this.m_FlowTypes.Values)
				{             
					if (this.GetOutputFlowDictionary().ContainsKey(ft.Id))
					{
						double[] arr = GetOutputFlowDictionary()[ft.Id];

						for (var i = 0; i <= arr.GetUpperBound(0); i++)
						{
							arr[i] = StochasticTimeRaster.DefaultNoDataValue;
						}

#if DEBUG
						if (arr.Length > 0)
						{
							Debug.Assert(arr[0] == StochasticTimeRaster.DefaultNoDataValue);
							Debug.Assert(arr[arr.Length - 1] == StochasticTimeRaster.DefaultNoDataValue);
						}
#endif
					}
				}           
			}

			this.ResampleFlowMultiplierValues(e.Iteration, e.Timestep, DistributionFrequency.Timestep);
		}

		/// <summary>
		/// Handles the AfterTimestep event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimAfterTimestep(object sender, TimestepEventArgs e)
		{
			if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SummaryStockOutputTimesteps, this.m_CreateSummaryStockOutput))
			{
				this.OnSummaryStockOutput();
				this.ProcessStockSummaryData(e.Iteration, e.Timestep);
			}

			if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SummaryFlowOutputTimesteps, this.m_CreateSummaryFlowOutput))
			{
				this.ProcessFlowSummaryData(e.Iteration, e.Timestep);
			}

			if (this.m_IsSpatial)
			{
				if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SpatialStockOutputTimesteps, this.m_CreateSpatialStockOutput))
				{
					this.ProcessStockSpatialData(e.Iteration, e.Timestep);
					this.ProcessStockGroupSpatialData(e.Iteration, e.Timestep);
				}

				if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SpatialFlowOutputTimesteps, this.m_CreateSpatialFlowOutput))
				{
					this.ProcessFlowSpatialData(e.Iteration, e.Timestep);
					this.ProcessFlowGroupSpatialData(e.Iteration, e.Timestep);
				}
			}
		}

		/// <summary>
		/// Called when (non-spatial) multipliers are being applied
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplyingTransitionMultipliers(object sender, MultiplierEventArgs e)
		{
			Debug.Assert(this.m_StockTransitionMultipliers.Count > 0);

			double Multiplier = 1.0;
			DataSheet Groups = this.Project.GetDataSheet(Constants.DATASHEET_STOCK_GROUP_NAME);
			DataSheet TGMembership = this.ResultScenario.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_GROUP_MEMBERSHIP_NAME);
			Dictionary<int, double> StockAmounts = GetStockAmountDictionary(e.SimulationCell);
			var dtgroups = Groups.GetData();
			var dtmembership = TGMembership.GetData();

			foreach (DataRow dr in dtgroups.Rows)
			{
				double StockGroupValue = 0.0;
				int StockGroupId = Convert.ToInt32(dr[Groups.ValueMember]);
				string query = string.Format(CultureInfo.InvariantCulture, "StockGroupID={0}", StockGroupId);
				DataRow[] rows = dtmembership.Select(query);

				foreach (DataRow r in rows)
				{
					double ValueMultiplier = 1.0;
					int StockTypeId = Convert.ToInt32(r[Constants.STOCK_TYPE_ID_COLUMN_NAME]);
					double StockTypeAmount = 0.0;

					if (StockAmounts.ContainsKey(StockTypeId))
					{
						StockTypeAmount = StockAmounts[StockTypeId];
					}

					if (!Convert.IsDBNull(r[Constants.VALUE_COLUMN_NAME]))
					{
						ValueMultiplier = Convert.ToDouble(r[Constants.VALUE_COLUMN_NAME]);
					}

					StockGroupValue += ((StockTypeAmount * ValueMultiplier) / this.m_STSimTransformer.AmountPerCell);
				}

				Multiplier *= this.m_StockTransitionMultiplierMap.GetStockTransitionMultiplier(StockGroupId, e.SimulationCell.StratumId, e.SimulationCell.SecondaryStratumId, e.SimulationCell.TertiaryStratumId, e.SimulationCell.StateClassId, e.TransitionGroupId, e.Iteration, e.Timestep, StockGroupValue);
			}

			e.ApplyMultiplier(Multiplier);
		}

		/// <summary>
		/// Called when a cell has been initialized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimCellInitialized(object sender, CellEventArgs e)
		{
			Dictionary<int, double> StockAmounts = GetStockAmountDictionary(e.SimulationCell);

			foreach (StockType s in this.m_StockTypes)
			{
				if (StockAmounts.ContainsKey(s.Id))
				{
					StockAmounts[s.Id] = 0.0;
				}
				else
				{
					StockAmounts.Add(s.Id, 0.0);
				}
			}

			foreach (InitialStockNonSpatial s in this.m_InitialStocksNonSpatial)
			{
				StockLimit lim = this.m_StockLimitMap.GetStockLimit(s.StockTypeId, e.SimulationCell.StratumId, e.SimulationCell.SecondaryStratumId, e.SimulationCell.TertiaryStratumId, e.SimulationCell.StateClassId, e.Iteration, e.Timestep);
				double val = this.GetAttributeValue(s.StateAttributeTypeId, e.SimulationCell.StratumId, e.SimulationCell.SecondaryStratumId, e.SimulationCell.TertiaryStratumId, e.SimulationCell.StateClassId, e.Iteration, e.Timestep, e.SimulationCell.Age);

				double v = val * this.m_STSimTransformer.AmountPerCell;
				v = GetLimitBasedInitialStock(v, lim);

				StockAmounts[s.StockTypeId] = v;
			}

			if (this.m_InitialStockSpatialMap != null)
			{
				InitialStockSpatialCollection l = this.m_InitialStockSpatialMap.GetItem(e.Iteration);

				if (l != null)
				{
					foreach (InitialStockSpatial s in l)
					{
						StockLimit lim = this.m_StockLimitMap.GetStockLimit(
                            s.StockTypeId, e.SimulationCell.StratumId, e.SimulationCell.SecondaryStratumId, e.SimulationCell.TertiaryStratumId, 
                            e.SimulationCell.StateClassId, e.Iteration, e.Timestep);

						//The spatial value should take precedence over the non-spatial value.  Note that
						//we assume that raster values are the total amount not the density and don't need conversion.

						if (this.m_InitialStockSpatialRasters.ContainsKey(s.Filename))
						{
							double v = this.m_InitialStockSpatialRasters[s.Filename].DblCells[e.SimulationCell.CellId];

							//If a cell is a no data cell or if there is a -INF value for a cell, initialize the stock value to zero

							if (Math.Abs(v - this.m_InitialStockSpatialRasters[s.Filename].NoDataValue) < double.Epsilon | double.IsNegativeInfinity(v))
							{
								v = 0.0;
							}

							v = GetLimitBasedInitialStock(v, lim);
							StockAmounts[s.StockTypeId] = v;
						}
						else
						{
							Debug.Assert(false, "Where's the raster object ?");
						}
					}
				}
			}
		}

		/// <summary>
		/// Called after all cells have been initialized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimAfterCellsInitialized(object sender, CellEventArgs e)
		{
			this.OnSummaryStockOutput();
			this.ProcessStockSummaryData(e.Iteration, e.Timestep);

			if (this.m_IsSpatial)
			{
				if (this.m_STSimTransformer.IsOutputTimestep(e.Timestep, this.m_SpatialStockOutputTimesteps, this.m_CreateSpatialStockOutput))
				{
					this.ProcessStockSpatialData(e.Iteration, e.Timestep);
					this.ProcessStockGroupSpatialData(e.Iteration, e.Timestep);
				}
			}
		}

		/// <summary>
		/// Called before a cell changes for a probabilistic transition
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimBeforeChangeCellProbabilistic(object sender, CellChangeEventArgs e)
		{
			if (!this.m_FlowPathwayMap.HasRecords)
			{
				return;
			}

			this.ReorderShufflableFlowTypes(e.Iteration, e.Timestep);
			List<List<FlowType>> flowTypeLists = this.CreateListOfFlowTypeLists();

			foreach (List<FlowType> l in flowTypeLists)
			{
				foreach (StockType st in this.m_StockTypes)
				{
					this.ApplyTransitionFlows(l, st, e.SimulationCell, e.Iteration, e.Timestep, null, e.ProbabilisticPathway);
				}
			}
		}

		/// <summary>
		/// Called before a cell changes for a deterministic transition
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimBeforeChangeCellDeterministic(object sender, CellChangeEventArgs e)
		{
			if (this.m_ApplyBeforeTransitions == false)
			{
				ApplyAutomaticFlows(e);
			}
		}

		/// <summary>
		/// Called before a cell changes for a deterministic transition
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnSTSimCellBeforeTransitions(object sender, CellChangeEventArgs e)
		{
			if (this.m_ApplyBeforeTransitions == true)
			{
				ApplyAutomaticFlows(e);
			}
		}

		private void ValidateFlowSpatialMultipliers()
		{
			Debug.Assert(this.m_IsSpatial);
			DataSheet ds = this.ResultScenario.GetDataSheet(Constants.DATASHEET_FLOW_SPATIAL_MULTIPLIER_NAME);

			for (int i = this.m_FlowSpatialMultipliers.Count - 1; i >= 0; i--)
			{
				FlowSpatialMultiplier r = this.m_FlowSpatialMultipliers[i];

				if (!this.m_FlowSpatialMultiplierRasters.ContainsKey(r.FileName))
				{
					string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_PROCESS_WARNING, r.FileName);
					RecordStatus(StatusType.Warning, msg);

					continue;
				}

				string cmpMsg = "";
				var cmpRes = this.STSimTransformer.InputRasters.CompareMetadata(this.m_FlowSpatialMultiplierRasters[r.FileName], ref cmpMsg);
				string FullFilename = RasterFiles.GetInputFileName(ds, r.FileName, false);

				if (cmpRes == CompareMetadataResult.ImportantDifferences)
				{
					string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_WARNING, FullFilename);
					RecordStatus(StatusType.Warning, msg);

					this.m_FlowSpatialMultipliers.RemoveAt(i);
				}
				else
				{
					if (cmpRes == STSim.CompareMetadataResult.UnimportantDifferences)
					{
						string msg = string.Format(CultureInfo.InvariantCulture, Constants.SPATIAL_METADATA_INFO, FullFilename, cmpMsg);
						RecordStatus(StatusType.Information, msg);
					}
				}
			}
		}

		private List<List<FlowType>> CreateListOfFlowTypeLists()
		{
			List<List<FlowType>> FlowTypeLists = new List<List<FlowType>>();

			if (this.m_ApplyEquallyRankedSimultaneously)
			{
				SortedDictionary<double, List<FlowType>> FlowTypeOrderDictionary = new SortedDictionary<double, List<FlowType>>();

				foreach (FlowType ft in this.m_ShufflableFlowTypes)
				{
					if (!FlowTypeOrderDictionary.ContainsKey(ft.Order))
					{
						FlowTypeOrderDictionary.Add(ft.Order, new List<FlowType>());
					}

					FlowTypeOrderDictionary[ft.Order].Add(ft);
				}

				foreach (double order in FlowTypeOrderDictionary.Keys)
				{
					List<FlowType> l = FlowTypeOrderDictionary[order];
					FlowTypeLists.Add(l);
				}
			}
			else
			{
				foreach (FlowType ft in this.m_ShufflableFlowTypes)
				{
					List<FlowType> l = new List<FlowType>();
					l.Add(ft);
					FlowTypeLists.Add(l);
				}
			}

			return (FlowTypeLists);
		}

		private void ApplyAutomaticFlows(CellChangeEventArgs e)
		{
			if (!this.m_FlowPathwayMap.HasRecords)
			{
				return;
			}

			this.ReorderShufflableFlowTypes(e.Iteration, e.Timestep);
			List<List<FlowType>> flowTypeLists = this.CreateListOfFlowTypeLists();

			foreach (List<FlowType> l in flowTypeLists)
			{
				foreach (StockType st in this.m_StockTypes)
				{
					this.ApplyTransitionFlows(l, st, e.SimulationCell, e.Iteration, e.Timestep, e.DeterministicPathway, null);
				}
			}
		}

		private void ResampleFlowMultiplierValues(int iteration, int timestep, DistributionFrequency frequency)
		{
			try
			{
				foreach (FlowMultiplier t in this.m_FlowMultipliers)
				{
					t.Sample(iteration, timestep, this.m_STSimTransformer.DistributionProvider, frequency);
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Flow Multipliers" + " -> " + ex.Message);
			}
		}

		private void ApplyTransitionFlows(List<FlowType> ftList, StockType st, Cell cell, int iteration, int timestep, DeterministicTransition dtPathway, Transition ptPathway)
		{
			Debug.Assert(this.m_FlowPathwayMap.HasRecords);

			int DestStrat = 0;
			int DestStateClass = 0;
			int ToAge = 0;
			List<int> TGIds = new List<int>();

			if (ptPathway != null)
			{
				if (ptPathway.StratumIdDestination.HasValue)
				{
					DestStrat = ptPathway.StratumIdDestination.Value;
				}
				else
				{
					DestStrat = cell.StratumId;
				}

				if (ptPathway.StateClassIdDestination.HasValue)
				{
					DestStateClass = ptPathway.StateClassIdDestination.Value;
				}
				else
				{
					DestStateClass = cell.StateClassId;
				}

				ToAge = this.m_STSimTransformer.DetermineTargetAgeProbabilistic(cell.Age, DestStrat, DestStateClass, iteration, timestep, ptPathway);

				foreach (TransitionGroup tg in this.m_STSimTransformer.TransitionTypes[ptPathway.TransitionTypeId].TransitionGroups)
				{
					TGIds.Add(tg.TransitionGroupId);
				}
			}
			else
			{
				if (dtPathway == null)
				{

					DestStrat = cell.StratumId;
					DestStateClass = cell.StateClassId;
					ToAge = cell.Age + 1;
				}
				else
				{
					if (dtPathway.StratumIdDestination.HasValue)
					{
						DestStrat = dtPathway.StratumIdDestination.Value;
					}
					else
					{
						DestStrat = cell.StratumId;
					}

					if (dtPathway.StateClassIdDestination.HasValue)
					{
						DestStateClass = dtPathway.StateClassIdDestination.Value;
					}
					else
					{
						DestStateClass = cell.StateClassId;
					}

					ToAge = cell.Age + 1;

				}

				TGIds.Add(0);
			}

			foreach (int TransitionGroupId in TGIds)
			{
				List<FlowPathway> allFlowPathways = new List<FlowPathway>();

				foreach (FlowType ft in ftList)
				{
					List<FlowPathway> l = this.m_FlowPathwayMap.GetFlowPathwayList(
                        iteration, timestep, cell.StratumId, cell.StateClassId, st.Id, cell.Age, 
                        DestStrat, DestStateClass, TransitionGroupId, ft.Id, ToAge);

					if (l != null)
					{
						foreach (FlowPathway fp in l)
						{
							allFlowPathways.Add(fp);
						}
					}
				}

				foreach (FlowPathway fp in allFlowPathways)
				{
					fp.FlowAmount = this.CalculateFlowAmount(fp, cell, iteration, timestep);
				}

				foreach (FlowPathway fp in allFlowPathways)
				{
					Dictionary<int, double> d = GetStockAmountDictionary(cell);
					StockLimit limsrc = this.m_StockLimitMap.GetStockLimit(fp.FromStockTypeId, cell.StratumId, cell.SecondaryStratumId, cell.TertiaryStratumId, cell.StateClassId, iteration, timestep);
					StockLimit limdst = this.m_StockLimitMap.GetStockLimit(fp.ToStockTypeId, cell.StratumId, cell.SecondaryStratumId, cell.TertiaryStratumId, cell.StateClassId, iteration, timestep);

					if (!d.ContainsKey(fp.FromStockTypeId))
					{
						double val = GetLimitBasedInitialStock(0.0, limsrc);
						d.Add(fp.FromStockTypeId, val);
					}

					if (!d.ContainsKey(fp.ToStockTypeId))
					{
						double val = GetLimitBasedInitialStock(0.0, limdst);
						d.Add(fp.ToStockTypeId, val);
					}

					double fa = fp.FlowAmount;

					if (limsrc != null)
					{
						if ((d[fp.FromStockTypeId] - fa) < limsrc.StockMinimum)
						{
							fa = d[fp.FromStockTypeId] - limsrc.StockMinimum;
						}
					}

					if (limdst != null)
					{
						if ((d[fp.ToStockTypeId] + fa) > limdst.StockMaximum)
						{
							fa = limdst.StockMaximum - d[fp.ToStockTypeId];
						}
					}

					d[fp.FromStockTypeId] -= fa;
					d[fp.ToStockTypeId] += fa;

					this.OnSummaryFlowOutput(timestep, cell, dtPathway, ptPathway, fp, fa);
					this.OnSpatialFlowOutput(timestep, cell, fp.FlowTypeId, fa);
				}
			}
		}

		private static double GetLimitBasedInitialStock(double value, StockLimit limit)
		{
			double v = value;

			if (limit != null)
			{
				if (v < limit.StockMinimum)
				{
					v = limit.StockMinimum;
				}
				else if (v > limit.StockMaximum)
				{
					v = limit.StockMinimum;
				}
			}

			return v;
		}

		private double CalculateFlowAmount(FlowPathway fp, Cell cell, int iteration, int timestep)
		{      
			double FlowAmount = 0.0;
			FlowType ft = this.m_FlowTypes[fp.FlowTypeId];

			if (fp.StateAttributeTypeId.HasValue)
			{
				FlowAmount = this.GetAttributeValue(
                    fp.StateAttributeTypeId.Value, cell.StratumId, cell.SecondaryStratumId, 
                    cell.TertiaryStratumId, cell.StateClassId, iteration, timestep, cell.Age);

				FlowAmount *= this.m_STSimTransformer.AmountPerCell;
			}
			else
			{
				Dictionary<int, double> d = GetStockAmountDictionary(cell);

				if (!d.ContainsKey(fp.FromStockTypeId))
				{
					d.Add(fp.FromStockTypeId, 0.0);
				}

				FlowAmount = d[fp.FromStockTypeId];
			}

			FlowAmount *= fp.Multiplier;

			foreach (FlowGroup fg in ft.FlowGroups)
			{
				FlowAmount *= this.m_FlowMultiplierMap.GetFlowMultiplier(
                    fg.Id, cell.StratumId, cell.SecondaryStratumId, cell.TertiaryStratumId, 
                    cell.StateClassId, iteration, timestep);

				if (this.m_IsSpatial)
				{
					FlowAmount *= this.GetFlowSpatialMultiplier(cell.CellId, fg.Id, iteration, timestep);
				}
			}

			if (FlowAmount <= 0.0)
			{
				FlowAmount = 0.0;
			}

			return FlowAmount;
		}

		private double GetFlowSpatialMultiplier(int cellId, int flowGroupId, int iteration, int timestep)
		{
			Debug.Assert(this.m_IsSpatial);

			if (this.m_FlowSpatialMultipliers.Count == 0)
			{
				return 1.0;
			}

			FlowSpatialMultiplier m = this.m_FlowSpatialMultiplierMap.GetFlowSpatialMultiplier(flowGroupId, iteration, timestep);

			if (m == null)
			{
				return 1.0;
			}

			if (!this.m_FlowSpatialMultiplierRasters.ContainsKey(m.FileName))
			{
				return 1.0;
			}

			StochasticTimeRaster raster = this.m_FlowSpatialMultiplierRasters[m.FileName];
			double v = raster.DblCells[cellId];

			if ((v < 0.0) || (MathUtils.CompareDoublesEqual(v, raster.NoDataValue, double.Epsilon)))
			{
				return 1.0;
			}
			else
			{
				return v;
			}
		}

		/// <summary>
		/// Reorders the list of shufflable flow types
		/// </summary>
		/// <param name="iteration"></param>
		/// <param name="timestep"></param>
		/// <remarks></remarks>
		private void ReorderShufflableFlowTypes(int iteration, int timestep)
		{
			FlowOrderCollection orders = this.m_FlowOrderMap.GetOrders(iteration, timestep);

			if (orders == null)
			{
				ShuffleUtilities.ShuffleList(this.m_ShufflableFlowTypes, this.m_RandomGenerator.Random);
			}
			else
			{
				this.ReorderShufflableFlowTypes(orders);
			}
		}

		/// <summary>
		/// Reorders the list of shufflable Flow Types
		/// </summary>
		/// <param name="orders"></param>
		/// <remarks></remarks>
		private void ReorderShufflableFlowTypes(FlowOrderCollection orders)
		{
			//If there are less than two Flow Types there is no reason to continue

			if (this.m_ShufflableFlowTypes.Count <= 1)
			{
				return;
			}

			//Reset all Flow Type order values

			foreach (FlowType ft in this.m_ShufflableFlowTypes)
			{
				ft.Order = Constants.DEFAULT_FLOW_ORDER;
			}

			//Apply the new ordering from the order collection

			Debug.Assert(this.m_FlowTypes.Count == this.m_ShufflableFlowTypes.Count);

			foreach (FlowOrder order in orders)
			{
				if (this.m_FlowTypes.ContainsKey(order.FlowTypeId))
				{
					Debug.Assert(this.m_ShufflableFlowTypes.Contains(this.m_FlowTypes[order.FlowTypeId]));
					this.m_FlowTypes[order.FlowTypeId].Order = order.Order;
				}
			}

			//Sort by the Flow Types by the order value

			this.m_ShufflableFlowTypes.Sort((FlowType t1, FlowType t2) =>
			{
				return (t1.Order.CompareTo(t2.Order));
			});

			//Find the number of times each order appears.  If it appears more than
			//once then shuffle the subset of transtion groups with this order.

			Dictionary<double, int> OrderCounts = new Dictionary<double, int>();

			foreach (FlowOrder o in orders)
			{
				if (!OrderCounts.ContainsKey(o.Order))
				{
					OrderCounts.Add(o.Order, 1);
				}
				else
				{
					OrderCounts[o.Order] += 1;
				}
			}

			//If any order appears more than once then it is a subset
			//that we need to shuffle.  Note that there may be a subset
			//for the default order.

			foreach (double d in OrderCounts.Keys)
			{
				if (OrderCounts[d] > 1)
				{
					ShuffleUtilities.ShuffleSubList(
                        this.m_ShufflableFlowTypes, 
                        this.GetMinOrderIndex(d), 
                        this.GetMaxOrderIndex(d), 
                        this.m_RandomGenerator.Random);
				}
			}

			if (this.DefaultOrderHasSubset())
			{
				ShuffleUtilities.ShuffleSubList(
                    this.m_ShufflableFlowTypes, 
                    this.GetMinOrderIndex(
                    Constants.DEFAULT_FLOW_ORDER), 
                    this.GetMaxOrderIndex(Constants.DEFAULT_FLOW_ORDER), 
                    this.m_RandomGenerator.Random);
			}

		}

		private bool DefaultOrderHasSubset()
		{
			int c = 0;

			foreach (FlowType tg in this.m_ShufflableFlowTypes)
			{
				if (tg.Order == Constants.DEFAULT_FLOW_ORDER)
				{
					c += 1;

					if (c == 2)
					{
						return true;
					}
				}
			}

			return false;
		}

		private int GetMinOrderIndex(double order)
		{
			for (int Index = 0; Index < this.m_ShufflableFlowTypes.Count; Index++)
			{
				FlowType tg = this.m_ShufflableFlowTypes[Index];

				if (tg.Order == order)
				{
					return Index;
				}
			}

			throw new InvalidOperationException("Cannot find minimum Flow order!");
		}

		private int GetMaxOrderIndex(double order)
		{
			for (int Index = this.m_ShufflableFlowTypes.Count - 1; Index >= 0; Index--)
			{
				FlowType tg = this.m_ShufflableFlowTypes[Index];

				if (tg.Order == order)
				{
					return Index;
				}
			}

			throw new InvalidOperationException("Cannot find maximum Flow order!");
		}
	}
}