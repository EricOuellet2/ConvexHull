using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Windows.Point;

// Ref: http://csharphelper.com/blog/2014/07/find-the-convex-hull-of-a-set-of-points-in-c/
// Thanks to Rod Stephens. Nice and simple algo in O(n2)
// 
// The class is slightly modified to used double instead of float. Also it use my Rectangle (included in project, with double) which is 
// Microsoft System.Drawing.rectangle modified to used double instead of int. Also using ~recent IList instead of List.

namespace RodStephens
{
    public static class Geometry
    {
        // For debugging.
        public static Point[] g_MinMaxCorners;
        public static Rectangle g_MinMaxBox;
        public static Point[] g_NonCulledPoints;

        // Find the points nearest the upper left, upper right,
        // lower left, and lower right corners.
        private static void GetMinMaxCorners(IList<Point> points, ref Point ul, ref Point ur, ref Point ll, ref Point lr)
        {
            // Start with the first point as the solution.
            ul = points[0];
            ur = ul;
            ll = ul;
            lr = ul;

            // Search the other points.
            foreach (Point pt in points)
            {
                if (-pt.X - pt.Y > -ul.X - ul.Y) ul = pt;
                if (pt.X - pt.Y > ur.X - ur.Y) ur = pt;
                if (-pt.X + pt.Y > -ll.X + ll.Y) ll = pt;
                if (pt.X + pt.Y > lr.X + lr.Y) lr = pt;
            }

            g_MinMaxCorners = new Point[] {ul, ur, lr, ll}; // For debugging.
        }

        // Find a box that fits inside the MinMax quadrilateral.
        private static Rectangle GetMinMaxBox(IList<Point> points)
        {
            // Find the MinMax quadrilateral.
            Point ul = new Point(0, 0), ur = ul, ll = ul, lr = ul;
            GetMinMaxCorners(points, ref ul, ref ur, ref ll, ref lr);

            // Get the coordinates of a box that lies inside this quadrilateral.
            double xmin, xmax, ymin, ymax;
            xmin = ul.X;
            ymin = ul.Y;

            xmax = ur.X;
            if (ymin < ur.Y) ymin = ur.Y;

            if (xmax > lr.X) xmax = lr.X;
            ymax = lr.Y;

            if (xmin < ll.X) xmin = ll.X;
            if (ymax > ll.Y) ymax = ll.Y;

            Rectangle result = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
            g_MinMaxBox = result;    // For debugging.
            return result;
        }

        // Cull points out of the convex hull that lie inside the
        // trapezoid defined by the vertices with smallest and
        // largest X and Y coordinates.
        // Return the points that are not culled.
        private static List<Point> HullCull(IList<Point> points)
        {
            // Find a culling box.
            Rectangle culling_box = GetMinMaxBox(points);

            // Cull the points.
            List<Point> results = new List<Point>();
            foreach (Point pt in points)
            {
                // See if (this point lies outside of the culling box.
                if (pt.X <= culling_box.Left ||
                    pt.X >= culling_box.Right ||
                    pt.Y <= culling_box.Top ||
                    pt.Y >= culling_box.Bottom)
                {
                    // This point cannot be culled.
                    // Add it to the results.
                    results.Add(pt);
                }
            }
            
            g_NonCulledPoints = new Point[results.Count];   // For debugging.
            results.CopyTo(g_NonCulledPoints);              // For debugging.
            return results;
        }

        // Return the points that make up a polygon's convex hull.
        // This method leaves the points list unchanged.
        public static List<Point> MakeConvexHull(IList<Point> points)
        {
            // Cull.
            points = HullCull(points);

            // Find the remaining point with the smallest Y value.
            // if (there's a tie, take the one with the smaller X value.
            Point best_pt = points[0];
            foreach (Point pt in points)
            {
                if ((pt.Y < best_pt.Y) ||
                   ((pt.Y == best_pt.Y) && (pt.X < best_pt.X)))
                {
                    best_pt = pt;
                }
            }

            // Move this point to the convex hull.
            List<Point> hull = new List<Point>();
            hull.Add(best_pt);
            points.Remove(best_pt);

			// Start wrapping up the other points.
			double sweep_angle = 0;
            for (;;)
            {
                // Find the point with smallest AngleValue
                // from the last point.
                double X = hull[hull.Count - 1].X;
				double Y = hull[hull.Count - 1].Y;
                best_pt = points[0];
				double best_angle = 3600;

                // Search the rest of the points.
                foreach (Point pt in points)
                {
					double test_angle = AngleValue(X, Y, pt.X, pt.Y);
                    if ((test_angle >= sweep_angle) &&
                        (best_angle > test_angle))
                    {
                        best_angle = test_angle;
                        best_pt = pt;
                    }
                }

				// See if the first point is better.
				// If so, we are done.
				double first_angle = AngleValue(X, Y, hull[0].X, hull[0].Y);
                if ((first_angle >= sweep_angle) &&
                    (best_angle >= first_angle))
                {
                    // The first point is better. We're done.
                    break;
                }

                // Add the best point to the convex hull.
                hull.Add(best_pt);
                points.Remove(best_pt);

                sweep_angle = best_angle;

                // If all of the points are on the hull, we're done.
                if (points.Count == 0) break;
            }

            return hull;
        }

        // Return a number that gives the ordering of angles
        // WRST horizontal from the point (x1, y1) to (x2, y2).
        // In other words, AngleValue(x1, y1, x2, y2) is not
        // the angle, but if:
        //   Angle(x1, y1, x2, y2) > Angle(x1, y1, x2, y2)
        // then
        //   AngleValue(x1, y1, x2, y2) > AngleValue(x1, y1, x2, y2)
        // this angle is greater than the angle for another set
        // of points,) this number for
        //
        // This function is dy / (dy + dx).
        private static double AngleValue(double x1, double y1, double x2, double y2)
        {
			double dx, dy, ax, ay, t;

            dx = x2 - x1;
            ax = Math.Abs(dx);
            dy = y2 - y1;
            ay = Math.Abs(dy);
            if (ax + ay == 0)
            {
                // if (the two points are the same, return 360.
                t = 360f / 9f;
            }
            else
            {
                t = dy / (ax + ay);
            }
            if (dx < 0)
            {
                t = 2 - t;
            }
            else if (dy < 0)
            {
                t = 4 + t;
            }
            return t * 90;
        }
    }
}
