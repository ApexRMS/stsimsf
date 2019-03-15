// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Globalization;

namespace SyncroSim.STSimStockFlow
{
	internal static class ExceptionUtils
	{
		public static void ThrowArgumentException(string message)
		{
			ThrowArgumentException(message, new object[0]);
		}

		public static void ThrowArgumentException(string message, params object[] args)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, message, args));
		}

		public static void ThrowInvalidOperationException(string message)
		{
			ThrowInvalidOperationException(message, new object[0]);
		}

		public static void ThrowInvalidOperationException(string message, params object[] args)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, args));
		}
	}
}