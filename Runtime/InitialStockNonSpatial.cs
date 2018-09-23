// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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