// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using SyncroSim.Core;
using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowMultiplierMap : StockFlowMapBase5<FlowMultiplier>
	{
		private STSimDistributionProvider m_DistributionProvider;

		public FlowMultiplierMap(Scenario scenario, FlowMultiplierCollection items, STSimDistributionProvider provider) : base(scenario)
		{
			this.m_DistributionProvider = provider;

			foreach (FlowMultiplier item in items)
			{
				this.TryAddMultiplier(item);
			}
		}

		public double GetFlowMultiplier(int flowGroupId, int stratumId, int? secondaryStratumId, int? tertiaryStratumId, int stateClassId, int iteration, int timestep)
		{
			FlowMultiplier v = this.GetItem(flowGroupId, stratumId, secondaryStratumId, tertiaryStratumId, stateClassId, iteration, timestep);

			if (v == null)
			{
				return 1.0;
			}
			else
			{
				v.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);
				return v.CurrentValue.Value;
			}
		}

		private void TryAddMultiplier(FlowMultiplier item)
		{
			try
			{
				base.AddItem(item.FlowGroupId, item.StratumId, item.SecondaryStratumId, item.TertiaryStratumId, item.StateClassId, item.Iteration, item.Timestep, item);
			}
			catch (StockFlowMapDuplicateItemException)
			{
				string template = "A duplicate flow multiplier was detected: More information:" + Environment.NewLine + "Flow Group={0}, {1}={2}, {3}={4}, {5}={6}, State Class={7}, Iteration={8}, Timestep={9}.";
				ExceptionUtils.ThrowArgumentException(template, this.GetFlowGroupName(item.FlowGroupId), this.PrimaryStratumLabel, this.GetStratumName(item.StratumId), this.SecondaryStratumLabel, this.GetSecondaryStratumName(item.SecondaryStratumId), this.TertiaryStratumLabel, this.GetTertiaryStratumName(item.TertiaryStratumId), this.GetStateClassName(item.StateClassId), StockFlowMapBase.FormatValue(item.Iteration), StockFlowMapBase.FormatValue(item.Timestep));         
			}
		}
	}
}