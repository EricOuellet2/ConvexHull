using Loyc.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MonotoneChain
{
    public class MonotoneChainImplementation
    {
		public static IListSource<Point> ComputeConvexHull(IList<Point> points, bool sortInPlace = false)
		{
			if (!sortInPlace)
				points = new List<Point>(points);
			points.Sort((a, b) =>
				a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));

			// Importantly, DList provides O(1) insertion at beginning and end
			DList<Point> hull = new DList<Point>();
			int L = 0, U = 0; // size of lower and upper hulls

			// Builds a hull such that the output polygon starts at the leftmost point.
			for (int i = points.Count - 1; i >= 0; i--)
			{
				Point p = points[i], p1;

				// build lower hull (at end of output list)
				while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
				{
					hull.RemoveAt(hull.Count - 1);
					L--;
				}
				hull.PushLast(p);
				L++;

				// build upper hull (at beginning of output list)
				while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
				{
					hull.RemoveAt(0);
					U--;
				}
				if (U != 0) // when U=0, share the point added above
					hull.PushFirst(p);
				U++;
				Debug.Assert(U + L == hull.Count + 1);
			}
			hull.RemoveAt(hull.Count - 1);
			return hull;
		}

	}
}
