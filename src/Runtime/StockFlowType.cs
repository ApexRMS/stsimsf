// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

namespace SyncroSim.STSimStockFlow
{
	internal class StockFlowType
	{
		private int m_Id;
        private string m_Name;

		public StockFlowType(int id, string name)
		{
			this.m_Id = id;
            this.m_Name = name;
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
    }
}
