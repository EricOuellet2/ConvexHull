using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Converters;

namespace ConvexHullHelper
{
	public class ConvexHullUtil
	{
		// ******************************************************************
		public static DifferencesInPath GetPathDifferences(string algoName, IReadOnlyList<Point> ptsSource, IReadOnlyList<Point> ptsReference, IReadOnlyList<Point> ptsToCompare)
		{
			ConvexHullComparer comparer = new ConvexHullComparer(algoName);
			return comparer.GetPathDifferences(ptsSource, ptsReference, ptsToCompare);
		}

		// ******************************************************************
		public static void PrintPointsToDebugWindow(string name, IReadOnlyList<Point> points, int maxCount = -1)
		{
			Debug.Print("--------------------------------------");
			Debug.Print($"{name}");
			Debug.Print(FormatPoints(points, maxCount, "\r\n"));
			Debug.Print($"Count of points: {points.Count}");
			Debug.Print("--------------------------------------");
		}

		// ******************************************************************
		public static void PrintPointsToConsole(string name, IReadOnlyList<Point> points, int maxCount = -1)
		{
			Console.WriteLine("--------------------------------------");
			Console.WriteLine($"{name}");
			Console.WriteLine(FormatPoints(points, maxCount, "\r\n"));
			Console.WriteLine($"Count of points: {points.Count}");
			Console.WriteLine("--------------------------------------");
		}

		public static String FormatPoints(IReadOnlyList<Point> points, int maxPoints = 20, string pointSeparator = ", ")
		{
			StringBuilder sb = new StringBuilder();
			FormatPointsWithLimit(sb, points, maxPoints, pointSeparator);
			return sb.ToString();
		}

		// ************************************************************************
		public static String FormatPointsOnOneLine(IReadOnlyList<Point> points, int maxPoints = 20, string pointSeparator = ", ")
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			FormatPointsWithLimit(sb, points, maxPoints, pointSeparator);
			sb.Append("}");

			return sb.ToString();
		}

		// ************************************************************************
		public static StringBuilder FormatPointsWithLimit(StringBuilder sb, IReadOnlyList<Point> points, int maxPoints = 20, string pointSeparator = ", ")
		{
			if (points.Count <= maxPoints)
			{
				FormatPoints(sb, points);
			}
			else
			{
				FormatPoints(sb, points.Take(maxPoints - 1));
				sb.Append(", ...");
			}

			return sb;
		}

		// ************************************************************************
		public static void FormatPoints(StringBuilder sb, IEnumerable<Point> points, string pointSeparator = ", ")
		{
			if (points == null || !points.Any())
			{
				sb.Append("Empty");
			}

			bool firstPoint = true;
			foreach (Point pt in points)
			{
				if (firstPoint)
				{
					firstPoint = false;
				}
				else
				{
					sb.Append(pointSeparator);
				}

				sb.Append("[");
				sb.Append(pt.ToString(CultureInfo.InvariantCulture));
				sb.Append("]");
			}
		}

		// ******************************************************************
		public static void InvertCoordinate(Point[] points, bool invertX, bool invertY)
		{
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new Point(invertX ? -points[i].X : points[i].X, invertY ? -points[i].Y : points[i].Y);
			}
		}

		// ******************************************************************
	}
}
