// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Diagnostics;

namespace SyncroSim.STSimStockFlow
{
	internal static class MathUtils
	{
		/// <summary>
		/// Linear Interpolation Y Value Calculation
		/// Y = ( ( X - X1 )( Y2 - Y1) / ( X2 - X1) ) + Y1
		/// </summary>
		/// <param name="x1">x value of 1st co-ordinate </param>
		/// <param name="y1">y value of 1st co-ordinate</param>
		/// <param name="x2">x value of 2nd co-ordinate</param>
		/// <param name="y2">y value of 2nd co-ordinate</param>
		/// <param name="x">X = Target x co-ordinate</param>
		/// <returns> Y = Interpolated y co-ordinate</returns>
		/// <remarks></remarks>
		public static double Interpolate(double x1, double y1, double x2, double y2, double x)
		{
			Debug.Assert(x > x1 && x < x2);

			double rise = y2 - y1;
			double run = x2 - x1;
			double slope = rise / run;
			double y = y1 + (x - x1) * slope;

			return y;
		}

		public static bool CompareDoublesEqual(double lhs, double rhs, double epsilon = double.Epsilon)
		{
			return (Math.Abs(lhs - rhs) < epsilon);
		}
	}
}