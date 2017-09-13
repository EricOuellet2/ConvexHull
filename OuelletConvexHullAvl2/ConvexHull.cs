using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OuelletConvexHullAvl2
{
	// ******************************************************************
	public class ConvexHullAvl
	{
		// Quadrant: Q2 | Q1
		//	         -------
		//           Q3 | Q4

		private Quadrant _q1;
		private Quadrant _q2;
		private Quadrant _q3;
		private Quadrant _q4;

		private IReadOnlyList<Point> _listOfPoint;
		private bool _shouldCloseTheGraph;

		// ******************************************************************
		public ConvexHullAvl(IReadOnlyList<Point> listOfPoint, bool shouldCloseTheGraph = true, int initialResultGuessSize = 0)
		{
			Init(listOfPoint, shouldCloseTheGraph);
		}

		// ******************************************************************
		private void Init(IReadOnlyList<Point> listOfPoint, bool shouldCloseTheGraph)
		{
			_listOfPoint = listOfPoint;
			_shouldCloseTheGraph = shouldCloseTheGraph;

			_q1 = new QuadrantSpecific1(_listOfPoint);
			_q2 = new QuadrantSpecific2(_listOfPoint);
			_q3 = new QuadrantSpecific3(_listOfPoint);
			_q4 = new QuadrantSpecific4(_listOfPoint);
		}

		// ******************************************************************
		private bool IsQuadrantAreDisjoint()
		{
			if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, _q3.RootPoint))
			{
				return false;
			}

			if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, _q4.RootPoint))
			{
				return false;
			}

			if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, _q1.RootPoint))
			{
				return false;
			}

			if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, _q2.RootPoint))
			{
				return false;
			}

			return true;
		}

		// ******************************************************************
		/// <summary>
		/// 
		/// </summary>
		/// <param name="threadUsage">Using ConvexHullThreadUsage.All will only use all thread for the first pass (se quadrant limits) then use only 4 threads for pass 2 (which is the actual limit).</param>
		public void CalcConvexHull()
		{
			if (IsZeroData())
			{
				return;
			}

			SetQuadrantLimitsOneThread();

			_q1.Prepare();
			_q2.Prepare();
			_q3.Prepare();
			_q4.Prepare();

			Point q1Root = _q1.RootPoint;
			Point q2Root = _q2.RootPoint;
			Point q3Root = _q3.RootPoint;
			Point q4Root = _q4.RootPoint;

			// Main Loop to extract ConvexHullPoints
			IEnumerator<Point> enumPoint = _listOfPoint.GetEnumerator();
			Point[] points = _listOfPoint as Point[];
			int index = 0;
			int pointCount = points.Length;

			if (points != null)
			{
				Point point;

				if (IsQuadrantAreDisjoint())
				{
					Debug.Print("Disjoint");

					// ****************** Q1
					Q1First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							_q1.ProcessPoint(ref point);
							goto Q1First;
						}

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							_q2.ProcessPoint(ref point);
							goto Q2First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							_q3.ProcessPoint(ref point);
							goto Q3First;
						}

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							_q4.ProcessPoint(ref point);
							goto Q4First;
						}

						goto Q1First;
					}
					else
					{
						goto End;
					}

					// ****************** Q2
					Q2First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							_q2.ProcessPoint(ref point);
							goto Q2First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							_q3.ProcessPoint(ref point);
							goto Q3First;
						}

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							_q4.ProcessPoint(ref point);
							goto Q4First;
						}

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							_q1.ProcessPoint(ref point);
							goto Q1First;
						}

						goto Q2First;
					}
					else
					{
						goto End;
					}

					// ****************** Q3
					Q3First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							_q3.ProcessPoint(ref point);
							goto Q3First;
						}

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							_q4.ProcessPoint(ref point);
							goto Q4First;
						}

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							_q1.ProcessPoint(ref point);
							goto Q1First;
						}

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							_q2.ProcessPoint(ref point);
							goto Q2First;
						}

						goto Q3First;
					}
					else
					{
						goto End;
					}

					// ****************** Q4
					Q4First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							_q4.ProcessPoint(ref point);
							goto Q4First;
						}

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							_q1.ProcessPoint(ref point);
							goto Q1First;
						}

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							_q2.ProcessPoint(ref point);
							goto Q2First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							_q3.ProcessPoint(ref point);
							goto Q3First;
						}

						goto Q4First;
					}
					else
					{
						goto End;
					}
				}
				else  // Not disjoint ***********************************************************************************
				{
					Debug.Print("Not disjoint");

					// ****************** Q1
					Q1First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
							{
								_q1.ProcessPoint(ref point);
								goto Q1First;
							}

							if (point.X < q3Root.X && point.Y < q3Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
								{
									_q3.ProcessPoint(ref point);
								}

								goto Q3First;
							}

							goto Q1First;
						}

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
							{
								_q2.ProcessPoint(ref point);
								goto Q2First;
							}

							if (point.X > q4Root.X && point.Y < q4Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
								{
									_q4.ProcessPoint(ref point);
								}

								goto Q4First;
							}

							goto Q2First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
							{
								_q3.ProcessPoint(ref point);
							}

							goto Q3First;
						}
						else if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
							{
								_q4.ProcessPoint(ref point);
							}

							goto Q4First;
						}

						goto Q1First;
					}
					else
					{
						goto End;
					}

					// ****************** Q2
					Q2First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
							{
								_q2.ProcessPoint(ref point);
								goto Q2First;
							}

							if (point.X > q4Root.X && point.Y < q4Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
								{
									_q4.ProcessPoint(ref point);
								}

								goto Q4First;
							}

							goto Q2First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
							{
								_q3.ProcessPoint(ref point);
								goto Q3First;
							}

							if (point.X > q1Root.X && point.Y > q1Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
								{
									_q1.ProcessPoint(ref point);
								}

								goto Q1First;
							}

							goto Q3First;
						}

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
							{
								_q4.ProcessPoint(ref point);
							}

							goto Q4First;
						}
						else if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
							{
								_q1.ProcessPoint(ref point);
							}

							goto Q1First;
						}

						goto Q2First;
					}
					else
					{
						goto End;
					}

					// ****************** Q3
					Q3First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
							{
								_q3.ProcessPoint(ref point);
								goto Q3First;
							}

							if (point.X > q1Root.X && point.Y > q1Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
								{
									_q1.ProcessPoint(ref point);
								}

								goto Q1First;
							}

							goto Q3First;
						}

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
							{
								_q4.ProcessPoint(ref point);
								goto Q4First;
							}

							if (point.X < q2Root.X && point.Y > q2Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
								{
									_q2.ProcessPoint(ref point);
								}

								goto Q2First;
							}

							goto Q4First;
						}

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
							{
								_q1.ProcessPoint(ref point);
								goto Q1First;
							}
						}
						else if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
							{
								_q2.ProcessPoint(ref point);
								goto Q2First;
							}
						}

						goto Q3First;
					}
					else
					{
						goto End;
					}

					// ****************** Q4
					Q4First:
					if (index < pointCount)
					{
						point = points[index++];

						if (point.X > q4Root.X && point.Y < q4Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
							{
								_q4.ProcessPoint(ref point);
								goto Q4First;
							}

							if (point.X < q2Root.X && point.Y > q2Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
								{
									_q2.ProcessPoint(ref point);
								}

								goto Q2First;
							}

							goto Q4First;
						}

						if (point.X > q1Root.X && point.Y > q1Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
							{
								_q1.ProcessPoint(ref point);
								goto Q1First;
							}

							if (point.X < q3Root.X && point.Y < q3Root.Y)
							{
								if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
								{
									_q3.ProcessPoint(ref point);
								}

								goto Q3First;
							}

							goto Q1First;
						}

						if (point.X < q3Root.X && point.Y < q3Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
							{
								_q3.ProcessPoint(ref point);
								goto Q3First;
							}
						}

						if (point.X < q2Root.X && point.Y > q2Root.Y)
						{
							if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
							{
								_q2.ProcessPoint(ref point);
								goto Q2First;
							}
						}

						goto Q4First;
					}
					else
					{
						goto End;
					}
				}

				End:
				{ }
			}
		}

		// ******************************************************************
		private void SetQuadrantLimitsOneThread()
		{
			Point ptFirst = this._listOfPoint.First();

			// Find the quadrant limits (maximum x and y)

			double right, topLeft, topRight, left, bottomLeft, bottomRight;
			right = topLeft = topRight = left = bottomLeft = bottomRight = ptFirst.X;

			double top, rightTop, rightBottom, bottom, leftTop, leftBottom;
			top = rightTop = rightBottom = bottom = leftTop = leftBottom = ptFirst.Y;

			foreach (Point pt in _listOfPoint)
			{
				if (pt.X >= right)
				{
					if (pt.X == right)
					{
						if (pt.Y > rightTop)
						{
							rightTop = pt.Y;
						}
						else
						{
							if (pt.Y < rightBottom)
							{
								rightBottom = pt.Y;
							}
						}
					}
					else
					{
						right = pt.X;
						rightTop = rightBottom = pt.Y;
					}
				}

				if (pt.X <= left)
				{
					if (pt.X == left)
					{
						if (pt.Y > leftTop)
						{
							leftTop = pt.Y;
						}
						else
						{
							if (pt.Y < leftBottom)
							{
								leftBottom = pt.Y;
							}
						}
					}
					else
					{
						left = pt.X;
						leftBottom = leftTop = pt.Y;
					}
				}

				if (pt.Y >= top)
				{
					if (pt.Y == top)
					{
						if (pt.X < topLeft)
						{
							topLeft = pt.X;
						}
						else
						{
							if (pt.X > topRight)
							{
								topRight = pt.X;
							}
						}
					}
					else
					{
						top = pt.Y;
						topLeft = topRight = pt.X;
					}
				}

				if (pt.Y <= bottom)
				{
					if (pt.Y == bottom)
					{
						if (pt.X < bottomLeft)
						{
							bottomLeft = pt.X;
						}
						else
						{
							if (pt.X > bottomRight)
							{
								bottomRight = pt.X;
							}
						}
					}
					else
					{
						bottom = pt.Y;
						bottomRight = bottomLeft = pt.X;
					}
				}

				this._q1.FirstPoint = new Point(right, rightTop);
				this._q1.LastPoint = new Point(topRight, top);
				this._q1.RootPoint = new Point(topRight, rightTop);

				this._q2.FirstPoint = new Point(topLeft, top);
				this._q2.LastPoint = new Point(left, leftTop);
				this._q2.RootPoint = new Point(topLeft, leftTop);

				this._q3.FirstPoint = new Point(left, leftBottom);
				this._q3.LastPoint = new Point(bottomLeft, bottom);
				this._q3.RootPoint = new Point(bottomLeft, leftBottom);

				this._q4.FirstPoint = new Point(bottomRight, bottom);
				this._q4.LastPoint = new Point(right, rightBottom);
				this._q4.RootPoint = new Point(bottomRight, rightBottom);

			}
		}

		private Limit _limit = null;
		// ******************************************************************
		// For usage of Parallel func, I highly suggest: Stephen Toub: Patterns of parallel programming ==> Just Awsome !!!
		// But its only my own fault if I'm not using it at its full potential...
		private void SetQuadrantLimitsUsingAllThreads()
		{
			Point pt = this._listOfPoint.First();
			_limit = new Limit(pt);

			int coreCount = Environment.ProcessorCount;

			Task[] tasks = new Task[coreCount];
			for (int n = 0; n < tasks.Length; n++)
			{
				int nLocal = n; // Prevent Lambda internal closure error.
				tasks[n] = Task.Factory.StartNew(() =>
				{
					Limit limit = _limit.Copy();
					FindLimits(_listOfPoint, nLocal, coreCount, limit);
					AggregateLimits(limit);
				});
			}
			Task.WaitAll(tasks);

			_q1.FirstPoint = _limit.Q1Right;
			_q1.LastPoint = _limit.Q1Top;
			_q2.FirstPoint = _limit.Q2Top;
			_q2.LastPoint = _limit.Q2Left;
			_q3.FirstPoint = _limit.Q3Left;
			_q3.LastPoint = _limit.Q3Bottom;
			_q4.FirstPoint = _limit.Q4Bottom;
			_q4.LastPoint = _limit.Q4Right;

			_q1.RootPoint = new Point(_q1.LastPoint.X, _q1.FirstPoint.Y);
			_q2.RootPoint = new Point(_q2.FirstPoint.X, _q2.LastPoint.Y);
			_q3.RootPoint = new Point(_q3.LastPoint.X, _q3.FirstPoint.Y);
			_q4.RootPoint = new Point(_q4.FirstPoint.X, _q4.LastPoint.Y);
		}

		// ******************************************************************
		private Limit FindLimits(IReadOnlyList<Point> listOfPoint, int start, int offset, Limit limit)
		{
			for (int index = start; index < listOfPoint.Count; index += offset)
			{
				Point pt = listOfPoint[index];

				double x = pt.X;
				double y = pt.Y;

				// Top
				if (y >= limit.Q2Top.Y)
				{
					if (y == limit.Q2Top.Y) // Special
					{
						if (y == limit.Q1Top.Y)
						{
							if (x < limit.Q2Top.X)
							{
								limit.Q2Top.X = x;
							}
							else if (x > limit.Q1Top.X)
							{
								limit.Q1Top.X = x;
							}
						}
						else
						{
							if (x < limit.Q2Top.X)
							{
								limit.Q1Top.X = limit.Q2Top.X;
								limit.Q1Top.Y = limit.Q2Top.Y;

								limit.Q2Top.X = x;
							}
							else if (x > limit.Q1Top.X)
							{
								limit.Q1Top.X = x;
								limit.Q1Top.Y = y;
							}
						}
					}
					else
					{
						limit.Q2Top.X = x;
						limit.Q2Top.Y = y;
					}
				}

				// Bottom
				if (y <= limit.Q3Bottom.Y)
				{
					if (y == limit.Q3Bottom.Y) // Special
					{
						if (y == limit.Q4Bottom.Y)
						{
							if (x < limit.Q3Bottom.X)
							{
								limit.Q3Bottom.X = x;
							}
							else if (x > limit.Q4Bottom.X)
							{
								limit.Q4Bottom.X = x;
							}
						}
						else
						{
							if (x < limit.Q3Bottom.X)
							{
								limit.Q4Bottom.X = limit.Q3Bottom.X;
								limit.Q4Bottom.Y = limit.Q3Bottom.Y;

								limit.Q3Bottom.X = x;
							}
							else if (x > limit.Q3Bottom.X)
							{
								limit.Q4Bottom.X = x;
								limit.Q4Bottom.Y = y;
							}
						}
					}
					else
					{
						limit.Q3Bottom.X = x;
						limit.Q3Bottom.Y = y;
					}
				}

				// Right
				if (x >= limit.Q4Right.X)
				{
					if (x == limit.Q4Right.X) // Special
					{
						if (x == limit.Q1Right.X)
						{
							if (y < limit.Q4Right.Y)
							{
								limit.Q4Right.Y = y;
							}
							else if (y > limit.Q1Right.Y)
							{
								limit.Q1Right.Y = y;
							}
						}
						else
						{
							if (y < limit.Q4Right.Y)
							{
								limit.Q1Right.X = limit.Q4Right.X;
								limit.Q1Right.Y = limit.Q4Right.Y;

								limit.Q4Right.Y = y;
							}
							else if (y > limit.Q1Right.Y)
							{
								limit.Q1Right.X = x;
								limit.Q1Right.Y = y;
							}
						}
					}
					else
					{
						limit.Q4Right.X = x;
						limit.Q4Right.Y = y;
					}
				}

				// Left
				if (x <= limit.Q3Left.X)
				{
					if (x == limit.Q3Left.X) // Special
					{
						if (x == limit.Q2Left.X)
						{
							if (y < limit.Q3Left.Y)
							{
								limit.Q3Left.Y = y;
							}
							else if (y > limit.Q2Left.Y)
							{
								limit.Q2Left.Y = y;
							}
						}
						else
						{
							if (y < limit.Q3Left.Y)
							{
								limit.Q2Left.X = limit.Q3Left.X;
								limit.Q2Left.Y = limit.Q3Left.Y;

								limit.Q3Left.Y = y;
							}
							else if (y > limit.Q2Left.Y)
							{
								limit.Q2Left.X = x;
								limit.Q2Left.Y = y;
							}
						}
					}
					else
					{
						limit.Q3Left.X = x;
						limit.Q3Left.Y = y;
					}
				}

				if (limit.Q2Left.X != limit.Q3Left.X)
				{
					limit.Q2Left.X = limit.Q3Left.X;
					limit.Q2Left.Y = limit.Q3Left.Y;
				}

				if (limit.Q1Right.X != limit.Q4Right.X)
				{
					limit.Q1Right.X = limit.Q4Right.X;
					limit.Q1Right.Y = limit.Q4Right.Y;
				}

				if (limit.Q1Top.Y != limit.Q2Top.Y)
				{
					limit.Q1Top.X = limit.Q2Top.X;
					limit.Q1Top.Y = limit.Q2Top.Y;
				}

				if (limit.Q4Bottom.Y != limit.Q3Bottom.Y)
				{
					limit.Q4Bottom.X = limit.Q3Bottom.X;
					limit.Q4Bottom.Y = limit.Q3Bottom.Y;
				}
			}

			return limit;
		}

		// ******************************************************************
		private Limit FindLimits(Point pt, ParallelLoopState state, Limit limit)
		{
			double x = pt.X;
			double y = pt.Y;

			// Top
			if (y >= limit.Q2Top.Y)
			{
				if (y == limit.Q2Top.Y) // Special
				{
					if (y == limit.Q1Top.Y)
					{
						if (x < limit.Q2Top.X)
						{
							limit.Q2Top.X = x;
						}
						else if (x > limit.Q1Top.X)
						{
							limit.Q1Top.X = x;
						}
					}
					else
					{
						if (x < limit.Q2Top.X)
						{
							limit.Q1Top.X = limit.Q2Top.X;
							limit.Q1Top.Y = limit.Q2Top.Y;

							limit.Q2Top.X = x;
						}
						else if (x > limit.Q1Top.X)
						{
							limit.Q1Top.X = x;
							limit.Q1Top.Y = y;
						}
					}
				}
				else
				{
					limit.Q2Top.X = x;
					limit.Q2Top.Y = y;
				}
			}

			// Bottom
			if (y <= limit.Q3Bottom.Y)
			{
				if (y == limit.Q3Bottom.Y) // Special
				{
					if (y == limit.Q4Bottom.Y)
					{
						if (x < limit.Q3Bottom.X)
						{
							limit.Q3Bottom.X = x;
						}
						else if (x > limit.Q4Bottom.X)
						{
							limit.Q4Bottom.X = x;
						}
					}
					else
					{
						if (x < limit.Q3Bottom.X)
						{
							limit.Q4Bottom.X = limit.Q3Bottom.X;
							limit.Q4Bottom.Y = limit.Q3Bottom.Y;

							limit.Q3Bottom.X = x;
						}
						else if (x > limit.Q3Bottom.X)
						{
							limit.Q4Bottom.X = x;
							limit.Q4Bottom.Y = y;
						}
					}
				}
				else
				{
					limit.Q3Bottom.X = x;
					limit.Q3Bottom.Y = y;
				}
			}

			// Right
			if (x >= limit.Q4Right.X)
			{
				if (x == limit.Q4Right.X) // Special
				{
					if (x == limit.Q1Right.X)
					{
						if (y < limit.Q4Right.Y)
						{
							limit.Q4Right.Y = y;
						}
						else if (y > limit.Q1Right.Y)
						{
							limit.Q1Right.Y = y;
						}
					}
					else
					{
						if (y < limit.Q4Right.Y)
						{
							limit.Q1Right.X = limit.Q4Right.X;
							limit.Q1Right.Y = limit.Q4Right.Y;

							limit.Q4Right.Y = y;
						}
						else if (y > limit.Q1Right.Y)
						{
							limit.Q1Right.X = x;
							limit.Q1Right.Y = y;
						}
					}
				}
				else
				{
					limit.Q4Right.X = x;
					limit.Q4Right.Y = y;
				}
			}

			// Left
			if (x <= limit.Q3Left.X)
			{
				if (x == limit.Q3Left.X) // Special
				{
					if (x == limit.Q2Left.X)
					{
						if (y < limit.Q3Left.Y)
						{
							limit.Q3Left.Y = y;
						}
						else if (y > limit.Q2Left.Y)
						{
							limit.Q2Left.Y = y;
						}
					}
					else
					{
						if (y < limit.Q3Left.Y)
						{
							limit.Q2Left.X = limit.Q3Left.X;
							limit.Q2Left.Y = limit.Q3Left.Y;

							limit.Q3Left.Y = y;
						}
						else if (y > limit.Q2Left.Y)
						{
							limit.Q2Left.X = x;
							limit.Q2Left.Y = y;
						}
					}
				}
				else
				{
					limit.Q3Left.X = x;
					limit.Q3Left.Y = y;
				}
			}

			if (limit.Q2Left.X != limit.Q3Left.X)
			{
				limit.Q2Left.X = limit.Q3Left.X;
				limit.Q2Left.Y = limit.Q3Left.Y;
			}

			if (limit.Q1Right.X != limit.Q4Right.X)
			{
				limit.Q1Right.X = limit.Q4Right.X;
				limit.Q1Right.Y = limit.Q4Right.Y;
			}

			if (limit.Q1Top.Y != limit.Q2Top.Y)
			{
				limit.Q1Top.X = limit.Q2Top.X;
				limit.Q1Top.Y = limit.Q2Top.Y;
			}

			if (limit.Q4Bottom.Y != limit.Q3Bottom.Y)
			{
				limit.Q4Bottom.X = limit.Q3Bottom.X;
				limit.Q4Bottom.Y = limit.Q3Bottom.Y;
			}

			return limit;
		}

		// ******************************************************************
		private object _findLimitFinalLock = new object();

		private void AggregateLimits(Limit limit)
		{
			lock (_findLimitFinalLock)
			{
				if (limit.Q1Right.X >= _limit.Q1Right.X)
				{
					if (limit.Q1Right.X == _limit.Q1Right.X)
					{
						if (limit.Q1Right.Y > _limit.Q1Right.Y)
						{
							_limit.Q1Right = limit.Q1Right;
						}
					}
					else
					{
						_limit.Q1Right = limit.Q1Right;
					}
				}

				if (limit.Q4Right.X > _limit.Q4Right.X)
				{
					if (limit.Q4Right.X == _limit.Q4Right.X)
					{
						if (limit.Q4Right.Y < _limit.Q4Right.Y)
						{
							_limit.Q4Right = limit.Q4Right;
						}
					}
					else
					{
						_limit.Q4Right = limit.Q4Right;
					}
				}

				if (limit.Q2Left.X < _limit.Q2Left.X)
				{
					if (limit.Q2Left.X == _limit.Q2Left.X)
					{
						if (limit.Q2Left.Y > _limit.Q2Left.Y)
						{
							_limit.Q2Left = limit.Q2Left;
						}
					}
					else
					{
						_limit.Q2Left = limit.Q2Left;
					}
				}

				if (limit.Q3Left.X < _limit.Q3Left.X)
				{
					if (limit.Q3Left.X == _limit.Q3Left.X)
					{
						if (limit.Q3Left.Y > _limit.Q3Left.Y)
						{
							_limit.Q3Left = limit.Q3Left;
						}
					}
					else
					{
						_limit.Q3Left = limit.Q3Left;
					}
				}

				if (limit.Q1Top.Y > _limit.Q1Top.Y)
				{
					if (limit.Q1Top.Y == _limit.Q1Top.Y)
					{
						if (limit.Q1Top.X > _limit.Q1Top.X)
						{
							_limit.Q1Top = limit.Q1Top;
						}
					}
					else
					{
						_limit.Q1Top = limit.Q1Top;
					}
				}

				if (limit.Q2Top.Y > _limit.Q2Top.Y)
				{
					if (limit.Q2Top.Y == _limit.Q2Top.Y)
					{
						if (limit.Q2Top.X < _limit.Q2Top.X)
						{
							_limit.Q2Top = limit.Q2Top;
						}
					}
					else
					{
						_limit.Q2Top = limit.Q2Top;
					}
				}

				if (limit.Q3Bottom.Y < _limit.Q3Bottom.Y)
				{
					if (limit.Q3Bottom.Y == _limit.Q3Bottom.Y)
					{
						if (limit.Q3Bottom.X < _limit.Q3Bottom.X)
						{
							_limit.Q3Bottom = limit.Q3Bottom;
						}
					}
					else
					{
						_limit.Q3Bottom = limit.Q3Bottom;
					}
				}

				if (limit.Q4Bottom.Y < _limit.Q4Bottom.Y)
				{
					if (limit.Q4Bottom.Y == _limit.Q4Bottom.Y)
					{
						if (limit.Q4Bottom.X > _limit.Q4Bottom.X)
						{
							_limit.Q4Bottom = limit.Q4Bottom;
						}
					}
					else
					{
						_limit.Q4Bottom = limit.Q4Bottom;
					}
				}
			}
		}

		// ******************************************************************
		public Point[] GetResultsAsArrayOfPoint()
		{
			if (_listOfPoint == null || !_listOfPoint.Any())
			{
				return new Point[0];
			}

			int countOfPoints = _q1.Count + _q2.Count + _q3.Count + _q4.Count;

			if (_q1.LastPoint == _q2.FirstPoint)
			{
				countOfPoints--;
			}

			if (_q2.LastPoint == _q3.FirstPoint)
			{
				countOfPoints--;
			}

			if (_q3.LastPoint == _q4.FirstPoint)
			{
				countOfPoints--;
			}

			if (_q4.LastPoint == _q1.FirstPoint)
			{
				countOfPoints--;
			}

			if (countOfPoints == 0) // Case where there is only one point
			{
				return new Point[] { _q1.FirstPoint };
			}

			if (_shouldCloseTheGraph)
			{
				countOfPoints++;
			}

			Point[] results = new Point[countOfPoints];

			int resultIndex = -1;

			if (_q1.FirstPoint != _q4.LastPoint)
			{
				foreach (Point pt in _q1)
				{
					results[++resultIndex] = pt;
				}
			}
			else
			{
				var enumerator = _q1.GetEnumerator();
				enumerator.Reset();
				if (enumerator.MoveNext())
				{
					// Skip first (same as the last one as quadrant 4

					while (enumerator.MoveNext())
					{
						results[++resultIndex] = enumerator.Current;
					}
				}
			}

			if (_q2.Count == 1)
			{
				if (_q2.FirstPoint != _q1.LastPoint)
				{
					results[++resultIndex] = _q2.FirstPoint;
				}
			}
			else
			{
				var enumerator = _q2.GetEnumerator();
				enumerator.Reset();
				if (enumerator.MoveNext()) // Will always be true
				{
					if (enumerator.Current != _q1.LastPoint)
					{
						results[++resultIndex] = enumerator.Current;
					}

					while (enumerator.MoveNext())
					{
						results[++resultIndex] = enumerator.Current;
					}
				}
			}

			if (_q3.Count == 1)
			{
				if (_q3.FirstPoint != _q2.LastPoint)
				{
					results[++resultIndex] = _q3.FirstPoint;
				}
			}
			else
			{
				var enumerator = _q3.GetEnumerator();
				enumerator.Reset();
				if (enumerator.MoveNext()) // Will always be true
				{
					if (enumerator.Current != _q2.LastPoint)
					{
						results[++resultIndex] = enumerator.Current;
					}

					while (enumerator.MoveNext())
					{
						results[++resultIndex] = enumerator.Current;
					}
				}
			}

			if (_q4.Count == 1)
			{
				if (_q4.FirstPoint != _q3.LastPoint)
				{
					results[++resultIndex] = _q4.FirstPoint;
				}
			}
			else
			{
				var enumerator = _q4.GetEnumerator();
				enumerator.Reset();
				if (enumerator.MoveNext()) // Will always be true
				{
					if (enumerator.Current != _q3.LastPoint)
					{
						results[++resultIndex] = enumerator.Current;
					}

					while (enumerator.MoveNext())
					{
						results[++resultIndex] = enumerator.Current;
					}
				}
			}

			if (_shouldCloseTheGraph && results[resultIndex] != results[0])
			{
				results[++resultIndex] = results[0];
			}

			General.DebugUtil.Print(results);

			return results;

		}

		// ******************************************************************
		private bool IsZeroData()
		{
			return _listOfPoint == null || !_listOfPoint.Any();
		}

		// ******************************************************************
		public void Dump()
		{
			Debug.Print("Q1:");
			_q1.Dump();
			Debug.Print("Q2:");
			_q2.Dump();
			Debug.Print("Q3:");
			_q3.Dump();
			Debug.Print("Q4:");
			_q4.Dump();
		}

		// ******************************************************************
	}
}

