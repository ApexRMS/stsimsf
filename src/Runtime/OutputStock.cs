// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class OutputStock
	{
		private int m_StratumId;
		private int? m_SecondaryStratumId;
		private int? m_TertiaryStratumId;
		private int m_StateClassId;
		private int m_StockTypeId;
		private double m_Amount;

		public OutputStock(int stratumId, int? secondaryStratumId, int? tertiaryStratumId, int stateClassId, int stockTypeId, double amount)
		{
			this.m_StratumId = stratumId;
			this.m_SecondaryStratumId = secondaryStratumId;
			this.m_TertiaryStratumId = tertiaryStratumId;
			this.m_StateClassId = stateClassId;
			this.m_StockTypeId = stockTypeId;
			this.m_Amount = amount;
		}

		public int StratumId
		{
			get
			{
				return this.m_StratumId;
			}
		}

		public int? SecondaryStratumId
		{
			get
			{
				return this.m_SecondaryStratumId;
			}
		}

		public int? TertiaryStratumId
		{
			get
			{
				return this.m_TertiaryStratumId;
			}
		}

		public int StateClassId
		{
			get
			{
				return this.m_StateClassId;
			}
		}

		public int StockTypeId
		{
			get
			{
				return this.m_StockTypeId;
			}
		}

		public double Amount
		{
			get
			{
				return this.m_Amount;
			}
			set
			{
				this.m_Amount = value;
			}
		}
	}
}