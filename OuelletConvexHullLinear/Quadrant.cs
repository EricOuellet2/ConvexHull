using General;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OuelletConvexHullLinear
{
	public abstract class Quadrant
	{
		// ************************************************************************
		public Point FirstPoint;
		public Point LastPoint;
		public Point RootPoint;

		public Point[] HullPoints = null;
		public int HullPointCount
		{
			get
			{
				return HullPoints.Length;
			}
		}

		protected IReadOnlyList<Point> _listOfPoint;

		// ************************************************************************
		// Very important the Quadrant should be always build in a way where dpiFirst has minus slope to center and dpiLast has maximum slope to center
		public Quadrant(IReadOnlyList<Point> listOfPoint, int initialResultGuessSize)
		{
			_listOfPoint = listOfPoint;
			HullPoints = new Point[0];
		}

		// ************************************************************************
		/// <summary>
		/// Initialize every values needed to extract values that are parts of the convex hull.
		/// This is where the first pass of all values is done the get maximum in every directions (x and y).
		/// </summary>
		protected abstract void SetQuadrantLimits();

		// ************************************************************************
		public void PrepareQuadrant(bool isSkipSetQuadrantLimits = false)
		{
			if (!_listOfPoint.Any())
			{
				// There is no points at all. Hey don't try to crash me.
				return;
			}

			if (!isSkipSetQuadrantLimits)
			{
				SetQuadrantLimits();
			}

			// Begin : General Init
			// HullPoints[0] = (FirstPoint);
			HullPoints = HullPoints.ImmutableInsertItem(FirstPoint, 0);


			if (FirstPoint.Equals(LastPoint))
			{
				return; // Case where for weird distribution (like triangle or diagonal) there could be one or more quadrants without points.
			}

			// HullPoints[1] = LastPoint;
			HullPoints = HullPoints.ImmutableInsertItem(LastPoint, 1);









			_countOfWorkerThread = Environment.ProcessorCount / 4 - 1;
			_countOfWorkerThreadRunning = _countOfWorkerThread;

			Task taskMain = Task.Run(new Action(() => ProcessCandidate()));
			for (int threadId = 1; threadId <= _countOfWorkerThread; threadId++)
			{
				Task.Run(new Action(() => FindCandidate()));
			}

			taskMain.Wait();
		}

		// ************************************************************************
		protected abstract bool IsQuickInterpolationCandidate(Point point);

		internal int _threadIdMaster = 0;
//		internal int _countOfWorkerThread = 0;
//		internal int _countOfWorkerThreadRunning = 0;
		internal int _indexPointCurrent = -1;

		// ************************************************************************
		public void ResolveConvexHull(int threadId)
		{
			try
			{
				int pointCount = _listOfPoint.Count;

				for (;;)
				{
					int index = Interlocked.Increment(ref _indexPointCurrent);
					if (index >= pointCount)
					{
						break;
					}

					Point point = _listOfPoint[index];

					if (!IsGoodQuadrantForPoint(point))
					{
						continue;
					}

					if (IsQuickInterpolationCandidate(point)) // Should put in Candidates if candidate
					{
						bool gotLock = false;
						try
						{
							_spinLockCandidates.Enter(ref gotLock);

							Candidates.Push(point);
						}
						finally
						{
							if (gotLock)
							{
								_spinLockCandidates.Exit();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Oops: Ex: {ex}");
				Debug.Print($"Oops: Ex: {ex}");
			}

			Interlocked.Decrement(ref _countOfWorkerThreadRunning);

			//if (Interlocked.Decrement(ref _countOfWorkerThreadRunning) == 0)
			//{
			//	int count = -1;

			//	bool gotLock = false;
			//	try
			//	{
			//		_spinLockCandidates.Enter(ref gotLock);

			//		count = Candidates.Count;
			//	}
			//	finally
			//	{
			//		if (gotLock)
			//		{
			//			_spinLockCandidates.Exit();
			//		}
			//	}

			//	Console.WriteLine($"Candidates left in {this.GetType().Name}: {count}");
			//}
		}

		// ************************************************************************
		public void ProcessCandidate(int tthreadId)
		{
			int pointCount = _listOfPoint.Count;

			Point point = default(Point);

			for (;;)
			{
				bool isPointFound = false;
				bool gotLock = false;

				try
				{
					_spinLockCandidates.Enter(ref gotLock);

					if (Candidates.Count > 0)
					{
						point = Candidates.Pop();
						isPointFound = true;
					}
				}
				finally
				{
					if (gotLock)
					{
						_spinLockCandidates.Exit();
					}
				}

				if (!isPointFound)
				{
					if (_indexPointCurrent < pointCount)
					{
						int index = Interlocked.Increment(ref _indexPointCurrent);
						if (index < pointCount)
						{
							point = _listOfPoint[index];
							isPointFound = true;
						}
					}
				}

				if (isPointFound)
				{
					if (!IsGoodQuadrantForPoint(point))
					{
						continue;
					}

					int indexLow = TryAdd(point);

					if (indexLow == -1)
					{
						continue;
					}

					Point p1 = HullPoints[indexLow];
					Point p2 = HullPoints[indexLow + 1];

					if (!IsPointToTheRightOfOthers(p1, p2, point))
					{
						continue;
					}

					int indexHi = indexLow + 1;

					// Find lower bound (remove point invalidate by the new one that come before)
					while (indexLow > 0)
					{
						if (IsPointToTheRightOfOthers(HullPoints[indexLow - 1], point, HullPoints[indexLow]))
						{
							break; // We found the lower index limit of points to keep. The new point should be added right after indexLow.
						}
						indexLow--;
					}

					// Find upper bound (remove point invalidate by the new one that come after)
					int maxIndexHi = HullPointCount - 1;
					while (indexHi < maxIndexHi)
					{
						if (IsPointToTheRightOfOthers(point, HullPoints[indexHi + 1], HullPoints[indexHi]))
						{
							break; // We found the higher index limit of points to keep. The new point should be added right before indexHi.
						}
						indexHi++;
					}

					if (indexLow + 1 == indexHi)
					{
						// Insert Point
						// HullPoints.Insert(indexHi, point);
						HullPoints = ArrayUtil.ImmutableInsertItem(HullPoints, point, indexHi);
					}
					else
					{
						HullPoints[indexLow + 1] = point;

						// Remove any invalidated points if any
						if (indexLow + 2 < indexHi)
						{
							// HullPoints.RemoveRange(indexLow + 2, indexHi - indexLow - 2);
							HullPoints = ArrayUtil.ImmutableRemoveRange(HullPoints, indexLow + 2, indexHi - indexLow - 2);
						}
					}
				}
				else
				{
					if (_countOfWorkerThreadRunning == 0) // Required to prevent race condition !!!
					{
						if (Candidates.Count == 0)
						{
							return;
						}
					}
				}
			}
		}

		// ************************************************************************
		/// <summary>
		/// To know if to the right. It is meaninful when p1 is first and p2 is next.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="ptToCheck"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool IsPointToTheRightOfOthers(Point p1, Point p2, Point ptToCheck)
		{
			return ((p2.X - p1.X) * (ptToCheck.Y - p1.Y)) - ((p2.Y - p1.Y) * (ptToCheck.X - p1.X)) < 0;
		}

		// ************************************************************************
		/// <summary>
		/// Tell if should try to add and where. -1 ==> Should not add.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract int TryAdd(Point pt);

		// ************************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract bool IsGoodQuadrantForPoint(Point pt);

		// ************************************************************************

	}
}
