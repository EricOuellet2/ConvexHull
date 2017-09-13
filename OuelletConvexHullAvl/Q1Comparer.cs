using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OuelletConvexHullAvl
{
	public class Q1Comparer : IComparer<Point>
	{
		public int Compare(Point pt1, Point pt2)
		{
			if (pt1.X > pt2.X) // Decreasing order
			{
				return -1;
			}
			if (pt1.X < pt2.X)
			{
				return 1;
			}

			return 0;
		}
	}
}
