using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OuelletConvexHullAvl2
{
	public class Q4Comparer : IComparer<Point>
	{
		public int Compare(Point pt1, Point pt2)
		{
			if (pt1.X < pt2.X) // Increasing order
			{
				return -1;
			}
			if (pt1.X > pt2.X)
			{
				return 1;
			}

			return 0;
		}
	}
}
