using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MonotoneChain
{
	public static class PointExtension
	{
		public static double Cross(this Point a, Point b)
		{
			return (a.X * b.Y) - (a.Y * b.X);

		}

		public static Point Sub(this Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
	}
}
