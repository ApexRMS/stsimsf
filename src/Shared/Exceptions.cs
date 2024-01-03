// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2024 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Runtime.Serialization;

namespace SyncroSim.STSimStockFlow
{
	[Serializable()]
	public sealed class StockFlowMapDuplicateItemException : Exception
	{
		public StockFlowMapDuplicateItemException() : base("Duplicate Item Exception")
		{
		}

		public StockFlowMapDuplicateItemException(string message) : base(message)
		{
		}

		public StockFlowMapDuplicateItemException(string message, Exception innerException) : base(message, innerException)
		{
		}

		private StockFlowMapDuplicateItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}