// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class BaseValueDisplayListItem
	{
		private readonly int m_Value;
		private readonly string m_Display;

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