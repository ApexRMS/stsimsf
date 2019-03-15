// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Diagnostics;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		private StockLimitMap m_StockLimitMap;
		private StockTransitionMultiplierMap m_StockTransitionMultiplierMap;
		private FlowPathwayMap m_FlowPathwayMap;
		private InitialStockSpatialMap m_InitialStockSpatialMap;
		private FlowOrderMap m_FlowOrderMap;

		private void CreateStockLimitMap()
		{
			Debug.Assert(this.m_StockLimitMap == null);
			this.m_StockLimitMap = new StockLimitMap(this.ResultScenario, this.m_StockLimits);
		}

		private void CreateStockTransitionMultiplierMap()
		{
			Debug.Assert(this.m_StockTransitionMultiplierMap == null);
			this.m_StockTransitionMultiplierMap = new StockTransitionMultiplierMap(this.ResultScenario, this.m_StockTransitionMultipliers, this.m_STSimTransformer.DistributionProvider);
		}

		private void CreateFlowPathwayMap()
		{
			Debug.Assert(this.m_FlowPathwayMap == null);
			this.m_FlowPathwayMap = new FlowPathwayMap(this.m_FlowPathways);
		}

		private void CreateInitialStockSpatialMap()
		{
			Debug.Assert(this.m_InitialStockSpatialMap == null);
			this.m_InitialStockSpatialMap = new InitialStockSpatialMap(this.m_InitialStocksSpatial);
		}

		private void CreateFlowOrderMap()
		{
			Debug.Assert(this.m_FlowOrderMap == null);
			this.m_FlowOrderMap = new FlowOrderMap(this.m_FlowOrders);
		}

        private void CreateMultiplierTypeMaps()
        {
            foreach (FlowMultiplier tm in this.m_FlowMultipliers)
            {
                FlowMultiplierType mt = this.GetFlowMultiplierType(tm.FlowMultiplierTypeId);
                mt.AddFlowMultiplier(tm);
            }

            foreach (FlowSpatialMultiplier tm in this.m_FlowSpatialMultipliers)
            {
                FlowMultiplierType mt = this.GetFlowMultiplierType(tm.FlowMultiplierTypeId);
                mt.AddFlowSpatialMultiplier(tm);
            }  

            foreach (FlowMultiplierType tmt in this.m_FlowMultiplierTypes)
            {
                tmt.CreateFlowMultiplierMap();
                tmt.CreateSpatialFlowMultiplierMap(); 
            }
        }
    }
}
