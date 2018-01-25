using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.SqlServer.Server;

namespace ConvexHullHelper
{
	public class ConvexHullTests
	{
		// ******************************************************************
		private Func<Point[], IReadOnlyList<Point>> _funcConvexHull;
		private Func<DifferencesInPath, ExecutionState> _funcShouldStopTesting = null; // Stop if return true

		private Point[] _points;
		private Point[] _referenceResults;
		private IReadOnlyList<Point> _results;

		private string _algoName = null;

		public string LatestTestName { get; private set; }
		public DifferencesInPath LastestDifferencesInPath { get; private set; }

		// ******************************************************************
		public ConvexHullTests(string algoName, Func<Point[], IReadOnlyList<Point>> funcConvexHull, Func<DifferencesInPath, ExecutionState> actionToDoOnErrorAndStopTestsIfReturnTrue)
		{
			_algoName = algoName;
			_funcConvexHull = funcConvexHull;
			_funcShouldStopTesting = actionToDoOnErrorAndStopTestsIfReturnTrue;
		}

		// ******************************************************************
		/// <summary>
		/// Return true to continue, false to stop
		/// </summary>
		private ExecutionState Test()
		{
			try
			{
				_results = _funcConvexHull(_points);
				var diff = ConvexHullUtil.GetPathDifferences(_algoName, _points, _referenceResults, _results);
				diff.Hint = LatestTestName;
				if (_funcShouldStopTesting != null)
				{
					return _funcShouldStopTesting(diff);
				}
			}
			catch (Exception ex)
			{
				if (_funcShouldStopTesting != null)
				{
					DifferencesInPath diff = new DifferencesInPath(_algoName, _points, _referenceResults, _results);
					diff.Hint = LatestTestName;
					diff.Exception = ex;
					return _funcShouldStopTesting(diff);
				}
			}

			return ExecutionState.Continue;
		}

		// ******************************************************************
		public ExecutionState TestSpecialCases()
		{
			LatestTestName = "No points";
			_points = new Point[0];
			_referenceResults = new Point[] { };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "Only one point";
			Point[] test5 = new Point[] { new Point(0, 0) };
			_referenceResults = new Point[] { new Point(0, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "2 points, the same";
			_points = new Point[] { new Point(0, 0), new Point(0, 0) };
			_referenceResults = new Point[] { new Point(0, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "2 points, same X";
			_points = new Point[] { new Point(0, 1), new Point(0, 0) };
			_referenceResults = new Point[] { new Point(0, 1), new Point(0, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "2 points, same Y";
			_points = new Point[] { new Point(0, 0), new Point(1, 0) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(1, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "2 points, diagonal";
			_points = new Point[] { new Point(0, 1), new Point(1, 0) };
			_referenceResults = new Point[] { new Point(0, 1), new Point(1, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "3 points, same X";
			_points = new Point[] { new Point(0, 0), new Point(0, 1), new Point(0, 2) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(0, 2) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "3 points, same Y";
			_points = new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(2, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "3 points, in one diagonal";
			_points = new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(2, 2) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "3 points";
			_points = new Point[] { new Point(0, 0), new Point(0, 1), new Point(2, 2) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(0, 1), new Point(2, 2) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "3 points, all the same";
			_points = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0) };
			_referenceResults = new Point[] { new Point(0, 0) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "4 points, in diagonal, twice same points";
			_points = new Point[] { new Point(0, 0), new Point(0, 0), new Point(2, 2), new Point(2, 2) };
			_referenceResults = new Point[] { new Point(0, 0), new Point(2, 2) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "4 points, in square";
			_points = new Point[] { new Point(1, 1), new Point(-1, -1), new Point(1, -1), new Point(-1, 1) };
			_referenceResults = new Point[] { new Point(1, 1), new Point(1, -1), new Point(-1, -1), new Point(-1, 1) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "4 points, 3 points same X";
			_points = new Point[] { new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(-1, 1) };
			_referenceResults = new Point[] { new Point(0, 1), new Point(0, 3), new Point(-1, 1) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			LatestTestName = "4 points, 3 points same Y";
			_points = new Point[] { new Point(1, 0), new Point(2, 0), new Point(-1, 0), new Point(-1, -1) };
			_referenceResults = new Point[] { new Point(2, 0), new Point(-1, 0), new Point(-1, -1) };
			if (Test() == ExecutionState.Stop) return ExecutionState.Stop;

			return ExecutionState.Continue;
		}

		/// <summary>
		/// Return a tuple of points, and expected convex hull result points
		/// </summary>
		public static TestSetOfPoint GetBasicTestSampleSet()
		{
			Point[] testPoint;

			testPoint = new Point[]
			{
				// first should be my Hull Points while others are wrong ones
				new Point(3,3),
				new Point(-3,3),
				new Point(-3,-3),
				new Point(3, -3),

				// Limit  where first points are the hull points

				new Point(1, 3),
				new Point(3,1),
				new Point(-1, -3),
				new Point(-3, -1),
				new Point(0, -3),
				new Point(-3, 0),
				new Point(3, 0),
				new Point(0, 3)
			};

			return new TestSetOfPoint(testPoint, testPoint.Take(4).ToArray());
		}

		// ******************************************************************
		public static TestSetOfPoint GetExtensiveTestSet()
		{
			Point[] testPoint = new Point[]
			{
				// first should be my Hull Points while others are wrong ones
				new Point(13, 0),
				new Point(12, 4),
				new Point(11, 7),
				new Point(10, 9),
				new Point(9, 10),
				new Point(0, 13),

				// Limit  where first points are the hull points

				new Point(12, 3),
				new Point(11, 6),
				new Point(10, 8),
				new Point(9, 9),
				new Point(8, 10),
			};

			return new TestSetOfPoint(testPoint, testPoint.Take(6).ToArray());
		}

		// ******************************************************************
		// REturn false if should stop
		public ExecutionState ExtensiveTests()
		{
			TestSetOfPoint testSet = GetBasicTestSampleSet();

			if (TestAllPossibilitesOnAllQuadrantAndCheckIfShouldStop(testSet) == ExecutionState.Stop)
			{
				return ExecutionState.Stop;
			}

			if (TestAllPossibilitesOnAllQuadrantAndCheckIfShouldStop(GetExtensiveTestSet()) == ExecutionState.Stop)
			{
				return ExecutionState.Stop;
			}

			return ExecutionState.Continue;
		}

		// ******************************************************************
		public ExecutionState TestAllPossibilitesOnAllQuadrantAndCheckIfShouldStop(TestSetOfPoint testSet)
		{
			var actionTestConvexHull = new Func<Point[], ExecutionState>((pts) => TestConvexHull(pts, testSet.ExpectedResult));

			Global.Instance.Iteration = 0;

			//Test proper behavior Q1
			Global.Instance.Quadrant = "Q1";
			if (Permutations.ForAllPermutation(testSet.Points, actionTestConvexHull) == ExecutionState.Stop) return ExecutionState.Stop;

			//Test proper behavior Q2
			Global.Instance.Quadrant = "Q2";
			ConvexHullUtil.InvertCoordinate(testSet.Points, true, false);
			ConvexHullUtil.InvertCoordinate(testSet.ExpectedResult, true, false);
			if (Permutations.ForAllPermutation(testSet.Points, actionTestConvexHull) == ExecutionState.Stop) return ExecutionState.Stop;

			//Test proper behavior Q3
			Global.Instance.Quadrant = "Q3";
			ConvexHullUtil.InvertCoordinate(testSet.Points, false, true);
			ConvexHullUtil.InvertCoordinate(testSet.ExpectedResult, false, true);
			if (Permutations.ForAllPermutation(testSet.Points, actionTestConvexHull) == ExecutionState.Stop) return ExecutionState.Stop;

			//Test proper behavior Q4
			Global.Instance.Quadrant = "Q4";
			ConvexHullUtil.InvertCoordinate(testSet.Points, true, false);
			ConvexHullUtil.InvertCoordinate(testSet.ExpectedResult, true, false);
			if (Permutations.ForAllPermutation(testSet.Points, actionTestConvexHull) == ExecutionState.Stop) return ExecutionState.Stop;

			return ExecutionState.Continue;
		}


		// ******************************************************************
		/// <summary>
		/// 
		/// </summary>
		/// <param name="indexesPosition"></param>
		/// <param name="testPoints"></param>
		/// <param name="expectedHullResult"></param>
		/// <returns>TRue if should continue testing or false to stop</returns>
		private ExecutionState TestConvexHull(Point[] points, Point[] expectedHullResult)
		{
			Global.Instance.Iteration++;

			IReadOnlyList<Point> convexHullPoints = _funcConvexHull(points);

			DifferencesInPath diffs = ConvexHullUtil.GetPathDifferences(_algoName, points, expectedHullResult, convexHullPoints);

			if (Global.Instance.IsCancel)
			{
				Global.Instance.ResetCancel();
				return ExecutionState.Stop; // Cancel
			}

			if (_funcShouldStopTesting != null)
			{
				return _funcShouldStopTesting(diffs);
			}

			return ExecutionState.Continue;
		}

		// ******************************************************************
		

	}
}
