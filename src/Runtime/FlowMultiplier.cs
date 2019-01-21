// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowMultiplier : STSimDistributionBase
    {
		private int? m_StateClassId;
		private int m_FlowGroupId;

		public FlowMultiplier(
            int? iteration, int? timestep, int? stratumId, int? secondaryStratumId, int? tertiaryStratumId, 
            int? stateClassId, int flowGroupId, double? multiplierValue, int? distributionTypeId, 
            DistributionFrequency? distributionFrequency, double? distributionSD, 
            double? distributionMin, double? distributionMax) : base(iteration, timestep, stratumId, secondaryStratumId, tertiaryStratumId, multiplierValue, distributionTypeId, distributionFrequency, distributionSD, distributionMin, distributionMax)
		{
			this.m_StateClassId = stateClassId;
			this.m_FlowGroupId = flowGroupId;
		}

		public int? StateClassId
		{
			get
			{
				return this.m_StateClassId;
			}
		}

		public int FlowGroupId
		{
			get
			{
				return this.m_FlowGroupId;
			}
		}

		public override STSimDistributionBase Clone()
		{
			return new FlowMultiplier(
                this.Iteration, this.Timestep, this.StratumId, this.SecondaryStratumId, this.TertiaryStratumId, 
                this.StateClassId, this.FlowGroupId, this.DistributionValue, this.DistributionTypeId, 
                this.DistributionFrequency, this.DistributionSD, this.DistributionMin, this.DistributionMax);
		}
	}
}