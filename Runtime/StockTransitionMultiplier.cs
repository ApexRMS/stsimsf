﻿// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal class StockTransitionMultiplier : STSimDistributionBase
	{
		private int? m_StateClassId;
		private int m_TransitionGroupId;
		private int m_StockGroupId;
		private double m_StockValue;

		public StockTransitionMultiplier(
            int? iteration, int? timestep, int? stratumId, int? secondaryStratumId, int? tertiaryStratumId, 
            int? stateClassId, int transitionGroupId, int stockGroupId, double stockValue, double? multiplier, 
            int? distributionTypeId, DistributionFrequency? distributionFrequency, double? distributionSD, 
            double? distributionMin, double? distributionMax) : base(iteration, timestep, stratumId, secondaryStratumId, tertiaryStratumId, multiplier, distributionTypeId, distributionFrequency, distributionSD, distributionMin, distributionMax)
		{
			this.m_StateClassId = stateClassId;
			this.m_TransitionGroupId = transitionGroupId;
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

		public int TransitionGroupId
		{
			get
			{
				return this.m_TransitionGroupId;
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
			return new StockTransitionMultiplier(this.Iteration, this.Timestep, this.StratumId, this.SecondaryStratumId, this.TertiaryStratumId, this.StateClassId, this.TransitionGroupId, this.StockGroupId, this.StockValue, this.DistributionValue, this.DistributionTypeId, this.DistributionFrequency, this.DistributionSD, this.DistributionMin, this.DistributionMax);
		}
	}
}