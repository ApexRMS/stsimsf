// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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