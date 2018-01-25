using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OuelletConvexHullAvl2Online
{
	public class Geometry
	{
		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double CalcSlope(double x1, double y1, double x2, double y2)
		{
			//if (Math.Abs(x2 - x1) <= Double.Epsilon)
			//{
			//	return Double.NaN;
			//}

			return (y2 - y1) / (x2 - x1);
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPointToTheRightOfOthers(Point p1, Point p2, Point ptToCheck)
		{
			return ((p2.X - p1.X) * (ptToCheck.Y - p1.Y)) - ((p2.Y - p1.Y) * (ptToCheck.X - p1.X)) < 0;
		}

		// ******************************************************************

	}
}
