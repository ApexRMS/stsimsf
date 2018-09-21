//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

namespace SyncroSim.STSimStockFlow
{
	internal class FlowPathway
	{
		private int? m_Iteration;
		private int? m_Timestep;
		private int? m_FromStratumId;
		private int? m_FromStateClassId;
		private int? m_FromMinimumAge;
		private int m_FromStockTypeId;
		private int? m_ToStratumId;
		private int? m_ToStateClassId;
		private int? m_ToMinimumAge;
		private int m_ToStockTypeId;
		private int m_TransitionGroupId;
		private int? m_StateAttributeTypeId;
		private int m_FlowTypeId;
		private double m_Multiplier;
		private double m_FlowAmount;

		public FlowPathway(int? iteration, int? timestep, int? fromStratumId, int? fromStateClassId, int? fromMinimumAge, int fromStockTypeId, int? toStratumId, int? toStateClassId, int? toMinimumAge, int toStockTypeId, int transitionGroupId, int? stateAttributeTypeId, int flowTypeId, double multiplier)
		{
			this.m_Iteration = iteration;
			this.m_Timestep = timestep;
			this.m_FromStratumId = fromStratumId;
			this.m_FromStateClassId = fromStateClassId;
			this.m_FromMinimumAge = fromMinimumAge;
			this.m_FromStockTypeId = fromStockTypeId;
			this.m_ToStratumId = toStratumId;
			this.m_ToStateClassId = toStateClassId;
			this.m_ToMinimumAge = toMinimumAge;
			this.m_ToStockTypeId = toStockTypeId;
			this.m_TransitionGroupId = transitionGroupId;
			this.m_StateAttributeTypeId = stateAttributeTypeId;
			this.m_FlowTypeId = flowTypeId;
			this.m_Multiplier = multiplier;
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

		public int? FromStratumId
		{
			get
			{
				return this.m_FromStratumId;
			}
		}

		public int? FromStateClassId
		{
			get
			{
				return this.m_FromStateClassId;
			}
		}

		public int? FromMinimumAge
		{
			get
			{
				return this.m_FromMinimumAge;
			}
		}

		public int FromStockTypeId
		{
			get
			{
				return this.m_FromStockTypeId;
			}
		}

		public int? ToStratumId
		{
			get
			{
				return this.m_ToStratumId;
			}
		}

		public int? ToStateClassId
		{
			get
			{
				return this.m_ToStateClassId;
			}
		}

		public int? ToMinimumAge
		{
			get
			{
				return this.m_ToMinimumAge;
			}
		}

		public int ToStockTypeId
		{
			get
			{
				return this.m_ToStockTypeId;
			}
		}

		public int TransitionGroupId
		{
			get
			{
				return this.m_TransitionGroupId;
			}
		}

		public int? StateAttributeTypeId
		{
			get
			{
				return this.m_StateAttributeTypeId;
			}
		}

		public int FlowTypeId
		{
			get
			{
				return this.m_FlowTypeId;
			}
		}

		public double Multiplier
		{
			get
			{
				return this.m_Multiplier;
			}
		}

		public double FlowAmount
		{
			get
			{
				return this.m_FlowAmount;
			}
			set
			{
				this.m_FlowAmount = value;
			}
		}
	}
}