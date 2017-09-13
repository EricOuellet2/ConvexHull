using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;

namespace ConvexHullHelper
{
	public class DifferencesInPath
	{
		// ************************************************************************
		public string Name { get; set; }
		public List<Point> MissingPoints { get; } = new List<Point>();
		public List<Point> UnwantedPoints { get; } = new List<Point>();
		public Point? FirstSequenceErrorDetectedNearPoint { get; set; } = null;
		public bool CountOfPointsIsDifferent { get; set; } = false;
		public bool HasClosedPath { get; set; } = false;

		private string _hint = null;
		public string Hint
		{
			get
			{
				if (_hint == null)
				{
					_hint = ConvexHullUtil.FormatPointsOnOneLine(PointsRef);
				}

				return _hint;
			}
			set { _hint = value; }
		}

		public Exception Exception { get; set; } = null;

		public IReadOnlyList<Point> SourcePoints { get; private set; }
		public IReadOnlyList<Point> PointsRef { get; private set; }
		public IReadOnlyList<Point> Points { get; private set; }

		// ************************************************************************
		public DifferencesInPath(string name, IReadOnlyList<Point> sourcePoints, IReadOnlyList<Point> pointsRef, IReadOnlyList<Point> points)
		{
			Name = name;
			SourcePoints = sourcePoints;
			PointsRef = pointsRef;
			Points = points;
		}

		// ************************************************************************
		public void PrintToDebugWindow(bool shortDescription = false)
		{
			Debug.Print("=============================== Diff results start ====================================");
			if (shortDescription)
			{
				Debug.Print(ShortDescription);
			}
			else
			{
				Debug.Print(Description);
			}
			Debug.Print("=============================== Diff results end ====================================");
		}

		// ************************************************************************
		public void PrintToConsole(bool shortDescription = false)
		{
			Console.WriteLine("=============================== Diff results start ====================================");
			if (shortDescription)
			{
				Debug.Print(ShortDescription);
			}
			else
			{
				Debug.Print(Description);
			}
			Console.WriteLine("=============================== Diff results end ====================================");
		}

		// ************************************************************************
		public bool HasErrors => CountOfPointsIsDifferent || FirstSequenceErrorDetectedNearPoint != null ||
			MissingPoints.Count > 0 || UnwantedPoints.Count > 0 || Exception != null;

		// ******************************************************************
		public string Description
		{
			get
			{
				if (Exception != null)
				{
					return $"Diffs in: '{Name}' : {Hint}. Exception: '{Exception}'.";
				}

				StringBuilder sb = new StringBuilder();

				sb.AppendLine($"Diffs in: '{Name}'");

				const string prefix = "    ";

				sb.Append(prefix);
				sb.AppendLine("Source points: " + ConvexHullUtil.FormatPointsOnOneLine(SourcePoints));

				sb.Append(prefix);
				sb.AppendLine("Points ref: " + ConvexHullUtil.FormatPointsOnOneLine(PointsRef));

				sb.Append(prefix);
				sb.AppendLine("Points: " + ConvexHullUtil.FormatPointsOnOneLine(Points));

				if (CountOfPointsIsDifferent)
				{
					sb.Append(prefix);
					sb.AppendLine("Count of Points is different.");
				}

				if (FirstSequenceErrorDetectedNearPoint != null)
				{
					sb.Append(prefix);
					sb.AppendLine("First sequence error detected near point: " + FirstSequenceErrorDetectedNearPoint);
				}

				if (MissingPoints.Count > 0)
				{
					sb.Append(prefix);
					sb.AppendLine("Missing Points: " + ConvexHullUtil.FormatPointsOnOneLine(MissingPoints));
				}

				if (UnwantedPoints.Count > 0)
				{
					sb.Append(prefix);
					sb.AppendLine("Unwanted Points: " + ConvexHullUtil.FormatPointsOnOneLine(UnwantedPoints));
				}

				if (HasClosedPath)
				{
					sb.Append(prefix);
					sb.AppendLine("Note: The path should be closed and it was not.");
				}

				return sb.ToString();
			}
		}

		// ******************************************************************
		public string ShortDescription
		{
			get
			{
				if (Exception != null)
				{
					return "Diffs in: " + Hint + ". " + Exception;
				}

				return $"Diffs in: {Hint}. Error detected near point: {FirstSequenceErrorDetectedNearPoint}. Missing {MissingPoints.Count } Points. Unwanted {UnwantedPoints.Count } Points.";
			}
		}

		// ************************************************************************
		public override string ToString()
		{
			return Description;
		}

		// ************************************************************************

	}
}