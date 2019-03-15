// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowMultiplier : STSimDistributionBase
    {
		private int? m_StateClassId;
        private int? m_FlowMultiplierTypeId;
		private int m_FlowGroupId;

		public FlowMultiplier(
            int? iteration, int? timestep, int? stratumId, int? secondaryStratumId, int? tertiaryStratumId, 
            int? stateClassId, int? flowMultiplierTypeId, int flowGroupId, double? multiplierValue, int? distributionTypeId, 
            DistributionFrequency? distributionFrequency, double? distributionSD, 
            double? distributionMin, double? distributionMax) : base(iteration, timestep, stratumId, secondaryStratumId, tertiaryStratumId, multiplierValue, distributionTypeId, distributionFrequency, distributionSD, distributionMin, distributionMax)
		{
			this.m_StateClassId = stateClassId;
            this.m_FlowMultiplierTypeId = flowMultiplierTypeId;
			this.m_FlowGroupId = flowGroupId;
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
				return this.m_FlowGroupId;
			}
		}

		public override STSimDistributionBase Clone()
		{
			return new FlowMultiplier(
                this.Iteration, this.Timestep, this.StratumId, this.SecondaryStratumId, this.TertiaryStratumId, 
                this.StateClassId, this.FlowMultiplierTypeId, this.FlowGroupId, this.DistributionValue, this.DistributionTypeId, 
                this.DistributionFrequency, this.DistributionSD, this.DistributionMin, this.DistributionMax);
		}
	}
}
