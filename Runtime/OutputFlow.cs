//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

namespace SyncroSim.STSimStockFlow
{
	internal class OutputFlow
	{
		private int m_FromStratumId;
		private int? m_FromSecondaryStratumId;
		private int? m_FromTertiaryStratumID;
		private int m_FromStateClassId;
		private int m_FromStockTypeId;
		private int? m_transitionTypeId;
		private int m_ToStratumId;
		private int m_ToStateClassId;
		private int m_ToStockTypeId;
		private int m_FlowTypeId;
		private double m_Amount;

		public OutputFlow(
            int fromStratumId, int? fromSecondaryStratumId, int? fromTertiaryStratumId, int fromStateClassId, int fromStockTypeId, 
            int? transitionTypeId, int toStratumId, int toStateClassId, int toStockTypeId, int flowTypeId, double amount)
		{
			this.m_FromStratumId = fromStratumId;
			this.m_FromSecondaryStratumId = fromSecondaryStratumId;
			this.m_FromTertiaryStratumID = fromTertiaryStratumId;
			this.m_FromStateClassId = fromStateClassId;
			this.m_FromStockTypeId = fromStockTypeId;
			this.m_transitionTypeId = transitionTypeId;
			this.m_ToStratumId = toStratumId;
			this.m_ToStateClassId = toStateClassId;
			this.m_ToStockTypeId = toStockTypeId;
			this.m_FlowTypeId = flowTypeId;
			this.m_Amount = amount;
		}

		public int FromStratumId
		{
			get
			{
				return this.m_FromStratumId;
			}
		}

		public int? FromSecondaryStratumId
		{
			get
			{
				return this.m_FromSecondaryStratumId;
			}
		}

		public int? FromTertiaryStratumId
		{
			get
			{
				return this.m_FromTertiaryStratumID;
			}
		}

		public int FromStateClassId
		{
			get
			{
				return this.m_FromStateClassId;
			}
		}

		public int FromStockTypeId
		{
			get
			{
				return this.m_FromStockTypeId;
			}
		}

		public int? TransitionTypeId
		{
			get
			{
				return this.m_transitionTypeId;
			}
		}

		public int ToStratumId
		{
			get
			{
				return this.m_ToStratumId;
			}
		}

		public int ToStateClassId
		{
			get
			{
				return this.m_ToStateClassId;
			}
		}

		public int ToStockTypeId
		{
			get
			{
				return this.m_ToStockTypeId;
			}
		}

		public int FlowTypeId
		{
			get
			{
				return this.m_FlowTypeId;
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