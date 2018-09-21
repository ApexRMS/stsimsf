//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

namespace SyncroSim.STSimStockFlow
{
	internal class InitialStockSpatial
	{
		private int? m_Iteration = null;
		private int m_StockTypeId;
		private string m_filename;

		public InitialStockSpatial(int? iteration, int stockTypeId, string filename)
		{
			this.m_Iteration = iteration;
			this.m_StockTypeId = stockTypeId;
			this.m_filename = filename;
		}

		public int? Iteration
		{
			get
			{
				return m_Iteration;
			}
		}

		public int StockTypeId
		{
			get
			{
				return this.m_StockTypeId;
			}
		}

		public string Filename
		{
			get
			{
				return m_filename;
			}
		}
	}
}