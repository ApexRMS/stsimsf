// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class InitialStockNonSpatial
	{
		private int m_Id;
		private int m_StockTypeId;
		private int m_StateAttributeTypeId;

		public InitialStockNonSpatial(int id, int stockTypeId, int stateAttributeTypeId)
		{
			this.m_Id = id;
			this.m_StockTypeId = stockTypeId;
			this.m_StateAttributeTypeId = stateAttributeTypeId;
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public int StockTypeId
		{
			get
			{
				return this.m_StockTypeId;
			}
		}

		public int StateAttributeTypeId
		{
			get
			{
				return this.m_StateAttributeTypeId;
			}
		}
	}

}