using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MIConvexHull;
using OuelletConvexHull;
using ConvexHull = OuelletConvexHull.ConvexHull;
using System.Windows;
using Mathematic;
using MonotoneChain;
using System.Windows.Media;

namespace ConvexHullWorkbench
{
	public class AlgorithmManager
	{
		public static AlgorithmManager Instance { get; } = new AlgorithmManager();

		private List<Algorithm> _algorithms = new List<Algorithm>();

		public IReadOnlyList<Algorithm> Algorithms { get { return _algorithms; } }

		private AlgorithmManager()
		{
			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Monotone chain", "A. M. Andrew, David Piepgrass (Qwerties)", "Very slow. Got on the web.", OxyPlot.OxyColors.Chocolate,
				(points, algorithmStat) =>
				{
					var stopwatch = Stopwatch.StartNew();
					var result = MonotoneChainImplementation.ComputeConvexHull(points);
					stopwatch.Stop();

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanOriginal = stopwatch.Elapsed;
					}
					return result.ToArray();
				}));

			AlgoIndexMonotoneChain = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Heap", "?, Pat Morin", "Very slow. Got on the web.", OxyPlot.OxyColors.Thistle,
				(points, algorithmStat) =>
				{
					ConvexHullHeapAndChanWrapper wrapper = new ConvexHullHeapAndChanWrapper(points);
					wrapper.HeapHull();
					var result = wrapper.HullPoints;

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanOriginal = wrapper.TimeSpan;
					}

					return result;
				}));

			AlgoIndexHeap = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "MI ConvexHUll (Delaunay/Voronoi)", "Delaunay and Voronoi, davecz", "Slow. From CodePlex. Could do 3D.", OxyPlot.OxyColors.Indigo,
				(points, algorithmStat) =>
				{
					var vertices = new Vertex[points.Length];
					int i = 0;
					foreach (var point in points)
					{
						vertices[i] = new Vertex(point.X, point.Y);
						i++;
					}

					var stopwatch = Stopwatch.StartNew();
					var convexHull = MIConvexHull.ConvexHull.Create(vertices);
					stopwatch.Stop();

					var hullPoints = new Point[convexHull.Points.Count()];
					i = 0;

					if (hullPoints.Length > 0)
					{
						var firstFace = convexHull.Faces.First();
						var face = firstFace;
						do
						{
							hullPoints[i++] = new Point(face.Vertices[0].Position[0], face.Vertices[0].Position[1]);

							face = face.Adjacency[0];
						} while (face != firstFace);
					}

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanOriginal = stopwatch.Elapsed;
					}

					return hullPoints;
				}));

			AlgoIndexMiConvexHull = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Chan", "Chan, Pat Morin", "C code. Has a little bug (happen rarely). Got on the web.", OxyPlot.OxyColors.Red,
				(points, algorithmStat) =>
				{
					Point[] result = null;
					try
					{
						ConvexHullHeapAndChanWrapper wrapper = new ConvexHullHeapAndChanWrapper(points);
						wrapper.ChanHull();
						result = wrapper.HullPoints;

						if (algorithmStat != null)
						{
							algorithmStat.TimeSpanOriginal = wrapper.TimeSpan;
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						Debugger.Break();
					}

					return result;
				}));

			AlgoIndexChan = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "LiuAndChen", "Liu and Chen, Eric Ouellet", "The same as Ouellet without my optimization", OxyPlot.OxyColors.LightSkyBlue,
				(points, algorithmStat) =>
				{
					LiuAndChen.ConvexHull convexHull = new LiuAndChen.ConvexHull(points);
					convexHull.CalcConvexHull(LiuAndChen.ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexLiuAndChen = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# ST", "Eric Ouellet, Eric Ouellet", "Highly tested.", OxyPlot.OxyColors.Green,
				(points, algorithmStat) =>
				{
					OuelletConvexHull.ConvexHull convexHull = new OuelletConvexHull.ConvexHull(points);
					convexHull.CalcConvexHull(ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullSingleThread = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# ST (array copy)", "Eric Ouellet, Eric Ouellet", "Array instead of a List.", OxyPlot.OxyColors.Purple,
				(points, algorithmStat) =>
				{
					OuelletConvexHullArray.ConvexHull convexHull = new OuelletConvexHullArray.ConvexHull(OuelletConvexHullArray.PointArrayManipulationType.ArrayCopy, points);
					convexHull.CalcConvexHull(OuelletConvexHullArray.ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullSingleThreadArray = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# ST (array C memcpy)", "Eric Ouellet, Eric Ouellet", "Array instead of a List.", OxyPlot.OxyColors.HotPink,
				(points, algorithmStat) =>
				{
					OuelletConvexHullArray.ConvexHull convexHull = new OuelletConvexHullArray.ConvexHull(OuelletConvexHullArray.PointArrayManipulationType.UnsafeCMemCopy, points);
					convexHull.CalcConvexHull(OuelletConvexHullArray.ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullSingleThreadArrayMemCpy = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# ST (array copy immu)", "Eric Ouellet, Eric Ouellet", "Array instead of a List.", OxyPlot.OxyColors.Olive,
				(points, algorithmStat) =>
				{
					OuelletConvexHullArray.ConvexHull convexHull = new OuelletConvexHullArray.ConvexHull(OuelletConvexHullArray.PointArrayManipulationType.ArrayCopyImmutable, points);
					convexHull.CalcConvexHull(OuelletConvexHullArray.ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullSingleThreadArrayImmu = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# ST (array memcpy no indirect)", "Eric Ouellet, Eric Ouellet", "Array instead of a List.", OxyPlot.OxyColors.Gold,
				(points, algorithmStat) =>
				{
					OuelletConvexHullArrayNoIndirect.ConvexHull convexHull = new OuelletConvexHullArrayNoIndirect.ConvexHull(OuelletConvexHullArrayNoIndirect.PointArrayManipulationType.ArrayCopyImmutable, points);
					convexHull.CalcConvexHull(OuelletConvexHullArrayNoIndirect.ConvexHullThreadUsage.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullSingleThreadArrayMemCpyNoIndirect = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# Avl", "Eric Ouellet, Eric Ouellet", "Same as ST but with Avl Tree instead of List.", OxyPlot.OxyColors.Orange,
				(points, algorithmStat) =>
				{
					OuelletConvexHullAvl.ConvexHullAvl convexHull = new OuelletConvexHullAvl.ConvexHullAvl(points);
					convexHull.CalcConvexHull(OuelletConvexHullAvl.ConvexHullThreadUsageAvl.OnlyOne);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullAvl = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# Avl v2", "Eric Ouellet, Eric Ouellet", "Same as AVL with some optimisation.", OxyPlot.OxyColors.Blue,
				(points, algorithmStat) =>
				{
					OuelletConvexHullAvl2.ConvexHullAvl convexHull = new OuelletConvexHullAvl2.ConvexHullAvl(points);
					convexHull.CalcConvexHull();
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullAvl2 = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# 4T", "Eric Ouellet, Eric Ouellet", "1 thread per quadrant for part 2", OxyPlot.OxyColors.SlateBlue,
				(points, algorithmStat) =>
				{
					OuelletConvexHull.ConvexHull convexHull = new OuelletConvexHull.ConvexHull(points);
					convexHull.CalcConvexHull(ConvexHullThreadUsage.FixedFour);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHull4Threads = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet C# MT", "Eric Ouellet, Eric Ouellet", "All thread part 1, 4 threads part 2, 1 thread part 3", OxyPlot.OxyColors.SaddleBrown,
				(points, algorithmStat) =>
				{
					OuelletConvexHull.ConvexHull convexHull = new OuelletConvexHull.ConvexHull(points);
					convexHull.CalcConvexHull(ConvexHullThreadUsage.All);
					return convexHull.GetResultsAsArrayOfPoint();
				}));

			AlgoIndexOuelletConvexHullMultiThreads = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.ConvexHull, "Ouellet CPP ST", "Eric Ouellet, Eric Ouellet", " thread, highly optimized, very ugly code", OxyPlot.OxyColors.YellowGreen,
				(points, algorithmStat) =>
				{
					double elapsedTime = 0;

					OuelletConvexHullCpp convexHull = new OuelletConvexHullCpp();
					var result = convexHull.OuelletHullManagedWithElapsedTime(points, true, ref elapsedTime);

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanOriginal = TimeSpanHelper.MoreAccurateTimeSpanFromSeconds(elapsedTime);
					}

					return result;
				}));

			AlgoIndexOuelletConvexHullCpp = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.SmallestEnclosingCircle, "Smallest Enclosing Circle", "Rod Stephens, Rod Stephens", "Slow Slow Slow. Select max 500 pts.", OxyPlot.OxyColors.Red,
				(points, algorithmStat) =>
				{
					var stopwatch = Stopwatch.StartNew();

					Point center = new Point(0, 0);
					double radius = 0;
					SmallestEnclosingCircle.FindMinimalBoundingCircle(points, out center, out radius);

					stopwatch.Stop();

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanCSharp = stopwatch.Elapsed;
					}

					Point[] result = CircleHelper.CreateCircleWithPoints(center, radius, 200); // 200 seems ok :-)

					return result;
				}));

			AlgoIndexSmallestEnclosingCircle = _algorithms.Count - 1;

			_algorithms.Add(new Algorithm(AlgorithmType.SmallestEnclosingCircle, "Smallest Enclosing Circle from ConvexHull (Ouellet ST)", "Rod Stephens, Rod Stephens", "Same algo as previous but points are first filtered by the Convex Hull algo", OxyPlot.OxyColors.Blue,
				(points, algorithmStat) =>
				{
					var stopwatch = Stopwatch.StartNew();


					ConvexHull convexHull = new ConvexHull(points);
					convexHull.CalcConvexHull(ConvexHullThreadUsage.OnlyOne);
					Point[] hullPoints = convexHull.GetResultsAsArrayOfPoint();


					Point center = new Point(0, 0);
					double radius = 0;
					SmallestEnclosingCircle.FindMinimalBoundingCircle(hullPoints, out center, out radius);

					stopwatch.Stop();

					if (algorithmStat != null)
					{
						algorithmStat.TimeSpanCSharp = stopwatch.Elapsed;
					}

					Point[] result = CircleHelper.CreateCircleWithPoints(center, radius, 200); // 200 seems ok :-)

					return result;
				}));

			AlgoIndexSmallestEnclosingCircle = _algorithms.Count - 1;

		}

		public int AlgoIndexMonotoneChain { get; private set; }
		public int AlgoIndexChan { get; private set; }
		public int AlgoIndexHeap { get; private set; }
		public int AlgoIndexLiuAndChen { get; private set; }
		public int AlgoIndexOuelletConvexHullSingleThread { get; private set; }
		public int AlgoIndexOuelletConvexHullSingleThreadArray { get; private set; }
		public int AlgoIndexOuelletConvexHullSingleThreadArrayMemCpy { get; private set; }
		public int AlgoIndexOuelletConvexHullSingleThreadArrayImmu { get; private set; }
		public int AlgoIndexOuelletConvexHullSingleThreadArrayMemCpyNoIndirect { get; private set; }
		public int AlgoIndexOuelletConvexHullAvl { get; private set; }
		public int AlgoIndexOuelletConvexHullAvl2 { get; private set; }
		public int AlgoIndexOuelletConvexHull4Threads { get; private set; }
		public int AlgoIndexOuelletConvexHullMultiThreads { get; private set; }
		public int AlgoIndexOuelletConvexHullCpp { get; private set; }
		public int AlgoIndexMiConvexHull { get; private set; }
		public int AlgoIndexSmallestEnclosingCircle { get; private set; }
		public int AlgoIndexSmallestEnclosingCircleFromConvexHull { get; private set; }
	}
}
