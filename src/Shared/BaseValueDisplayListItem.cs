// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class BaseValueDisplayListItem
	{
		private int m_Value;
		private string m_Display;

		public BaseValueDisplayListItem(int value, string display)
		{
			this.m_Value = value;
			this.m_Display = display;
		}

		public int Value
		{
			get
			{
				return this.m_Value;
			}
		}

		public override string ToString()
		{
			return this.m_Display;
		}
	}
}