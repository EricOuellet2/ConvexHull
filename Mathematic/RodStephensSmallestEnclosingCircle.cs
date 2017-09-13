using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mathematic
{

	/// From Rod Stephens: http://csharphelper.com/blog/2014/08/find-a-minimal-bounding-circle-of-a-set-of-points-in-c/
	public class SmallestEnclosingCircle
	{
		// ******************************************************************
		// Find a minimal bounding circle.
		public static void FindMinimalBoundingCircle(IReadOnlyList<Point> points, out Point center, out double radius)
		{
			// The best solution so far.
			Point bestCenter = points[0];
			double bestRadius2 = double.MaxValue;

			// Look at pairs of hull points.
			for (int i = 0; i < points.Count - 1; i++)
			{
				for (int j = i + 1; j < points.Count; j++)
				{
					// Find the circle through these two points.
					Point testCenter = new Point(
						(points[i].X + points[j].X) / 2f,
						(points[i].Y + points[j].Y) / 2f);
					double dx = testCenter.X - points[i].X;
					double dy = testCenter.Y - points[i].Y;
					double testRadius2 = dx * dx + dy * dy;

					// See if this circle would be an improvement.
					if (testRadius2 < bestRadius2)
					{
						// See if this circle encloses all of the points.
						if (CircleEnclosesPoints(testCenter,
							testRadius2, points, i, j, -1))
						{
							// Save this solution.
							bestCenter = testCenter;
							bestRadius2 = testRadius2;
						}
					}
				} // for i
			} // for j

			// Look at triples of hull points.
			for (int i = 0; i < points.Count - 2; i++)
			{
				for (int j = i + 1; j < points.Count - 1; j++)
				{
					for (int k = j + 1; k < points.Count; k++)
					{
						// Find the circle through these three points.
						Point testCenter;
						double testRadius2;
						FindCircle(points[i], points[j], points[k],
							out testCenter, out testRadius2);

						// See if this circle would be an improvement.
						if (testRadius2 < bestRadius2)
						{
							// See if this circle encloses all the points.
							if (CircleEnclosesPoints(testCenter,
								testRadius2, points, i, j, k))
							{
								// Save this solution.
								bestCenter = testCenter;
								bestRadius2 = testRadius2;
							}
						}
					} // for k
				} // for i
			} // for j

			center = bestCenter;
			if (bestRadius2 == double.MaxValue)
				radius = 0;
			else
				radius = (double)Math.Sqrt(bestRadius2);
		}

		// ******************************************************************
		// Return true if the indicated circle encloses all of the points.
		private static bool CircleEnclosesPoints(Point center, double radius2, IReadOnlyList<Point> points, int skip1, int skip2, int skip3)
		{
			for (int i = 0; i < points.Count; i++)
			{
				if ((i != skip1) && (i != skip2) && (i != skip3))
				{
					Point point = points[i];
					double dx = center.X - point.X;
					double dy = center.Y - point.Y;
					double testRadius2 = dx * dx + dy * dy;
					if (testRadius2 > radius2) return false;
				}
			}
			return true;
		}

		// ******************************************************************
		// Find a circle through the three points.
		private static void FindCircle(Point a, Point b, Point c, out Point center, out double radius2)
		{
			// Get the perpendicular bisector of (x1, y1) and (x2, y2).
			double x1 = (b.X + a.X) / 2;
			double y1 = (b.Y + a.Y) / 2;
			double dy1 = b.X - a.X;
			double dx1 = -(b.Y - a.Y);

			// Get the perpendicular bisector of (x2, y2) and (x3, y3).
			double x2 = (c.X + b.X) / 2;
			double y2 = (c.Y + b.Y) / 2;
			double dy2 = c.X - b.X;
			double dx2 = -(c.Y - b.Y);

			// See where the lines intersect.
			bool isLinesIntersect, isSegmentsIntersect;
			Point intersection, closePt1, closePt2;
			FindIntersection(
				new Point(x1, y1),
				new Point(x1 + dx1, y1 + dy1),
				new Point(x2, y2),
				new Point(x2 + dx2, y2 + dy2),
				out isLinesIntersect,
				out isSegmentsIntersect,
				out intersection,
				out closePt1,
				out closePt2);

			center = intersection;
			double dx = center.X - a.X;
			double dy = center.Y - a.Y;
			radius2 = dx * dx + dy * dy;
		}

		// ******************************************************************
		// Find the point of intersection between
		// the lines p1 --> p2 and p3 --> p4.
		private static void FindIntersection(Point p1, Point p2, Point p3, Point p4,
			out bool isLinesIntersect, out bool isSegmentsIntersect,
			out Point intersection, out Point closePt1, out Point closePt2)
		{
			// Get the segments' parameters.
			double dx12 = p2.X - p1.X;
			double dy12 = p2.Y - p1.Y;
			double dx34 = p4.X - p3.X;
			double dy34 = p4.Y - p3.Y;

			// Solve for t1 and t2
			double denominator = (dy12 * dx34 - dx12 * dy34);

			double t1;
			try
			{
				t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
			}
			catch
			{
				// The lines are parallel (or close enough to it).
				isLinesIntersect = false;
				isSegmentsIntersect = false;
				intersection = new Point(double.NaN, double.NaN);
				closePt1 = new Point(double.NaN, double.NaN);
				closePt2 = new Point(double.NaN, double.NaN);
				return;
			}
			isLinesIntersect = true;

			double t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

			// Find the point of intersection.
			intersection = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);

			// The segments intersect if t1 and t2 are between 0 and 1.
			isSegmentsIntersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

			// Find the closest points on the segments.
			if (t1 < 0)
			{
				t1 = 0;
			}
			else if (t1 > 1)
			{
				t1 = 1;
			}

			if (t2 < 0)
			{
				t2 = 0;
			}
			else if (t2 > 1)
			{
				t2 = 1;
			}

			closePt1 = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);
			closePt2 = new Point(p3.X + dx34 * t2, p3.Y + dy34 * t2);
		}

		// ******************************************************************

	}
}
