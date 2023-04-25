// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using SyncroSim.Core;
using SyncroSim.STSim;
using SyncroSim.StochasticTime;

namespace SyncroSim.STSimStockFlow
{
		internal class StockFlowMultiplierMap : StockFlowMapBase6<SortedList<double, StockFlowMultiplier>>
		{
				private STSimDistributionProvider m_DistributionProvider;

				public StockFlowMultiplierMap(
								Scenario scenario,
								StockFlowMultiplierCollection items,
								STSimDistributionProvider provider) : base(scenario)
				{
						this.m_DistributionProvider = provider;

						foreach (StockFlowMultiplier item in items)
						{
								this.TryAddMultiplier(item);
						}
				}

				public double GetStockFlowMultiplier(
								int stockGroupId, int stratumId, int? secondaryStratumId, int? tertiaryStratumId,
								int stateClassId, int flowGroupId, int iteration, int timestep, double stockValue)
				{
						SortedList<double, StockFlowMultiplier> lst = this.GetItem(
											stockGroupId, stratumId, secondaryStratumId, tertiaryStratumId,
											stateClassId, flowGroupId, iteration, timestep);

						if (lst == null)
						{
								return 1.0;
						}

						if (lst.Count == 1)
						{
								StockFlowMultiplier tsm = lst.First().Value;
								tsm.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);

								return tsm.CurrentValue.Value;
						}

						if (lst.ContainsKey(stockValue))
						{
								StockFlowMultiplier tsm = lst[stockValue];
								tsm.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);

								return tsm.CurrentValue.Value;
						}

						double PrevKey = double.MinValue;
						double ThisKey = double.MinValue;

						foreach (double k in lst.Keys)
						{
								Debug.Assert(k != stockValue);

								if (k > stockValue)
								{
										ThisKey = k;
										break;
								}

								PrevKey = k;
						}

						if (PrevKey == double.MinValue)
						{
								StockFlowMultiplier tsm = lst.First().Value;
								tsm.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);

								return tsm.CurrentValue.Value;
						}

						if (ThisKey == double.MinValue)
						{
								StockFlowMultiplier tsm = lst.Last().Value;
								tsm.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);

								return tsm.CurrentValue.Value;
						}

						StockFlowMultiplier PrevMult = lst[PrevKey];
						StockFlowMultiplier ThisMult = lst[ThisKey];

						PrevMult.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);
						ThisMult.Sample(iteration, timestep, this.m_DistributionProvider, DistributionFrequency.Always);

						return MathUtils.Interpolate(PrevKey, PrevMult.CurrentValue.Value, ThisKey, ThisMult.CurrentValue.Value, stockValue);
				}

				private void TryAddMultiplier(StockFlowMultiplier item)
				{
						SortedList<double, StockFlowMultiplier> l = this.GetItemExact(
											item.StockGroupId, item.StratumId, item.SecondaryStratumId, item.TertiaryStratumId,
											item.StateClassId, item.FlowGroupId, item.Iteration, item.Timestep);

						if (l == null)
						{
								l = new SortedList<double, StockFlowMultiplier>();

								this.AddItem(
														item.StockGroupId, item.StratumId, item.SecondaryStratumId, item.TertiaryStratumId,
														item.StateClassId, item.FlowGroupId, item.Iteration, item.Timestep, l);
						}

						l.Add(item.StockValue, item);
						Debug.Assert(this.HasItems);
				}
		}
}