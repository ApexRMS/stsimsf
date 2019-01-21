// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class StockLimit
	{
		private int? m_Iteration;
		private int? m_Timestep;
		private int m_StockTypeId;
		private int? m_StratumId;
		private int? m_SecondaryStratumId;
		private int? m_TertiaryStratumId;
		private int? m_StateClassId;
		private double m_StockMinimum;
		private double m_StockMaximum;

		public StockLimit(
            int? iteration, int? timestep, int stockTypeId, int? stratumId, int? secondaryStratumId, int? tertiaryStratumId, 
            int? stateClassId, double stockMinimum, double stockMaximum)
		{
			this.m_Iteration = iteration;
			this.m_Timestep = timestep;
			this.m_StockTypeId = stockTypeId;
			this.m_StratumId = stratumId;
			this.m_SecondaryStratumId = secondaryStratumId;
			this.m_TertiaryStratumId = tertiaryStratumId;
			this.m_StateClassId = stateClassId;
			this.m_StockMinimum = stockMinimum;
			this.m_StockMaximum = stockMaximum;
		}

		public int? Iteration
		{
			get
			{
				return this.m_Iteration;
			}
		}

		public int? Timestep
		{
			get
			{
				return this.m_Timestep;
			}
		}

		public int StockTypeId
		{
			get
			{
				return this.m_StockTypeId;
			}
		}

		public int? StratumId
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

		public int? StateClassId
		{
			get
			{
				return this.m_StateClassId;
			}
		}

		public double StockMinimum
		{
			get
			{
				return this.m_StockMinimum;
			}
		}

		public double StockMaximum
		{
			get
			{
				return this.m_StockMaximum;
			}
		}
	}
}