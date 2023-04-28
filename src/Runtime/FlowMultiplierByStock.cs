﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
		internal class FlowMultiplierByStock : STSimDistributionBase
		{
				private readonly int? m_StateClassId;
				private readonly int? m_FlowMultiplierTypeId;
				private readonly int m_FlowGroupID;
				private readonly int m_StockGroupId;
				private readonly double m_StockValue;

				public FlowMultiplierByStock(
								int? iteration, int? timestep, int? stratumId, int? secondaryStratumId, int? tertiaryStratumId,
								int? stateClassId, int? flowMultiplierTypeId, int flowGroupId, int stockGroupId, double stockValue, double? multiplier,
								int? distributionTypeId, DistributionFrequency? distributionFrequency, double? distributionSD,
								double? distributionMin, double? distributionMax) : base(iteration, timestep, stratumId, secondaryStratumId, tertiaryStratumId, multiplier, distributionTypeId, distributionFrequency, distributionSD, distributionMin, distributionMax)
				{
						this.m_StateClassId = stateClassId;
						this.m_FlowMultiplierTypeId = flowMultiplierTypeId;
						this.m_FlowGroupID = flowGroupId;
						this.m_StockGroupId = stockGroupId;
						this.m_StockValue = stockValue;

				}

				public int? StateClassId
				{
						get
						{
								return this.m_StateClassId;
						}
				}

				public int? FlowMultiplierTypeId
				{
						get
						{
								return this.m_FlowMultiplierTypeId;
						}
				}

				public int FlowGroupId
				{
						get
						{
								return this.m_FlowGroupID;
						}
				}

				public int StockGroupId
				{
						get
						{
								return this.m_StockGroupId;
						}
				}

				public double StockValue
				{
						get
						{
								return this.m_StockValue;
						}
				}

				public override STSimDistributionBase Clone()
				{
						return new FlowMultiplierByStock(
								this.Iteration, this.Timestep, this.StratumId, this.SecondaryStratumId, this.TertiaryStratumId, 
								this.StateClassId, this.FlowMultiplierTypeId, this.FlowGroupId, this.StockGroupId, this.StockValue, 
								this.DistributionValue, this.DistributionTypeId, this.DistributionFrequency, this.DistributionSD, 
								this.DistributionMin, this.DistributionMax);
				}
		}
}

