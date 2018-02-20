using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using General.AvlTreeSet;
using OuelletConvexHullAvl3.AvlTreeSet;
using OuelletConvexHullAvl3.Util;

namespace OuelletConvexHullAvl3
{
	// ******************************************************************
	public class ConvexHull : IReadOnlyCollection<Point>
	{
		// Quadrant: Q2 | Q1
		//	         -------
		//           Q3 | Q4

		internal Quadrant _q1;
		internal Quadrant _q2;
		internal Quadrant _q3;
		internal Quadrant _q4;

		internal Quadrant[] _quadrants;

		internal bool IsInitDone { get; set; } = false;

		// ******************************************************************
		private static Point[] _emptyListOfPoint = new Point[0];

		// ******************************************************************
		private void Init(IReadOnlyList<Point> points)
		{
			if (points == null || points.Count == 0)
			{
				return;
			}

			_q1 = new QuadrantSpecific1(this, points);
			_q2 = new QuadrantSpecific2(this, points);
			_q3 = new QuadrantSpecific3(this, points);
			_q4 = new QuadrantSpecific4(this, points);

			_quadrants = new Quadrant[] { _q1, _q2, _q3, _q4 };

			IsInitDone = true;
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
		/// Will reset any previous data, if any, and calculate a convex hull based on points.
		/// Results are kept per quadrant. To get result, either see: GetResultsAsArrayOfPoint or
		/// GetEnumerator
		/// </summary>
		public void CalcConvexHull(IReadOnlyList<Point> points)
		{
			Init(points);

			if (points == null || !points.Any())
			{
				return;
			}

			SetQuadrantLimitsOneThread(points);

			_q1.Prepare();
			_q2.Prepare();
			_q3.Prepare();
			_q4.Prepare();

			if (IsQuadrantAreDisjoint())
			{
				Debug.Print("Disjoint");
				ProcessPointsForDisjointQuadrant(points);
			}
			else
			{
				Debug.Print("Not disjoint");
				ProcessPointsForNotDisjointQuadrant(points);
			}
		}

		// ************************************************************************
		private void ProcessPointsForDisjointQuadrant(IReadOnlyList<Point> points)
		{
			Point point;
			Point q1Root = _q1.RootPoint;
			Point q2Root = _q2.RootPoint;
			Point q3Root = _q3.RootPoint;
			Point q4Root = _q4.RootPoint;

			int index = 0;
			int pointCount = points.Count;

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

			End:
			{ }
		}

		// ************************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ProcessPointsForNotDisjointQuadrant(IReadOnlyList<Point> points)
		{
			Point point;
			Point q1Root = _q1.RootPoint;
			Point q2Root = _q2.RootPoint;
			Point q3Root = _q3.RootPoint;
			Point q4Root = _q4.RootPoint;

			int index = 0;
			int pointCount = points.Count;

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

			End:
			{
			}
		}

		// ************************************************************************
		/// <summary>
		/// This function is called after a Point has been verified to be inside
		/// quadrant limits. When it is the case, it chcek if the point would 
		/// be participating to the convex hull if it would be added (but it is not).
		/// </summary>
		/// <param name="point"></param>
		/// <returns>1 = hull point, 0 = not a convex hull point, -1 convex hull point already exists</returns>		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private EnumConvexHullPoint EvaluatePointForPointInsideQuadrantLimits(Point point)
		{
			EnumConvexHullPoint result;

			if (point.X > _q1.RootPoint.X && point.Y > _q1.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
				{
					result = _q1.IsHullPoint(ref point);
					if (result != 0)
					{
						return result;
					}
				}

				if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
				{
					return _q3.IsHullPoint(ref point);
				}

				return EnumConvexHullPoint.NotConvexHullPoint;
			}

			if (point.X < _q2.RootPoint.X && point.Y > _q2.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
				{
					result = _q2.IsHullPoint(ref point);
					if (result != 0)
					{
						return result;
					}
				}

				if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
				{
					return _q4.IsHullPoint(ref point);
				}

				return EnumConvexHullPoint.NotConvexHullPoint;
			}

			if (point.X < _q3.RootPoint.X && point.Y < _q3.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
				{
					return _q3.IsHullPoint(ref point);
				}

				return EnumConvexHullPoint.NotConvexHullPoint;
			}

			if (point.X > _q4.RootPoint.X && point.Y < _q4.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
				{
					return _q4.IsHullPoint(ref point);
				}

				return EnumConvexHullPoint.NotConvexHullPoint;
			}

			return EnumConvexHullPoint.NotConvexHullPoint;
		}

		// ************************************************************************
		/// <summary>
		/// Try to insert a point into the proper quadrant (if appropriate).
		/// </summary>
		/// <param name="point"></param>
		/// <returns>1 = added, 0 = not a convex hull point, -1 convex hull point already exists</returns>		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private EnumConvexHullPoint ProcessPointForNotDisjointQuadrant(Point point)
		{
			EnumConvexHullPoint result;

			if (point.X > _q1.RootPoint.X && point.Y > _q1.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q1.FirstPoint, _q1.LastPoint, point))
				{
					result = _q1.ProcessPoint(ref point);
					if (result != 0)
					{
						return result;
					}
				}

				if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
				{
					return _q3.ProcessPoint(ref point);
				}

				return 0;
			}

			if (point.X < _q2.RootPoint.X && point.Y > _q2.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q2.FirstPoint, _q2.LastPoint, point))
				{
					result = _q2.ProcessPoint(ref point);
					if (result != 0)
					{
						return result;
					}
				}

				if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
				{
					return _q4.ProcessPoint(ref point);
				}

				return 0;
			}

			if (point.X < _q3.RootPoint.X && point.Y < _q3.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q3.FirstPoint, _q3.LastPoint, point))
				{
					return _q3.ProcessPoint(ref point);
				}

				return 0;
			}

			if (point.X > _q4.RootPoint.X && point.Y < _q4.RootPoint.Y)
			{
				if (Geometry.IsPointToTheRightOfOthers(_q4.FirstPoint, _q4.LastPoint, point))
				{
					return _q4.ProcessPoint(ref point);
				}

				return 0;
			}

			return 0;
		}



		#region Online (Dynamic add)

		// ************************************************************************
		// Start begin "Online" section (dynamic add point to the convex hull) 
		// ************************************************************************

		public double Top { get; private set; }
		public double Left { get; private set; }
		public double Bottom { get; private set; }
		public double Right { get; private set; }

		private Point[] _pointsArrayForDynamicCall = new Point[1];

		// ******************************************************************
		/// <summary>
		/// Check if a point is already part of the Convex Hull.
		/// In other word, it checks to see if the point is a duplicate.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>true if the point already exists, false otherwise</returns>
		public bool IsExists(Point pt)
		{
			Quadrant q = _q1;
			do
			{
				if (q.IsGoodQuadrantForPoint(pt))
				{
					if (q.Contains(pt))
					{
						return true;
					}
				}

				q = q.GetNextQuadrant();
			} while (q != _q1);

			return false;
		}

		// ******************************************************************
		/// <summary>
		/// Return the neighbors of the item if exists. 
		/// Otherwise it retuen default(Point).
		/// Not super efficient. If you wan tto iterate over all points. It is 
		/// better to call GetResultsAsArrayOfPoint() instead.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>Same point if only one. Neighbors if any.</returns>
		public Tuple<Point, Point> GetNeighbors(Point pt)
		{
			if (Count <= 1)
			{
				if (Count == 1)
				{
					if (_q1.FirstPoint == pt)
					{
						return new Tuple<Point, Point>(pt, pt);
					}
				}
			}
			else
			{
				AvlNode<Point> node = null;

				Quadrant q = _q1;
				do
				{
					if (q.IsGoodQuadrantForPoint(pt))
					{
						node = q.GetNode(pt);
						if (node != null)
						{
							var nodePrevious = node.GetPreviousNode();
							if (nodePrevious == null)
							{
								nodePrevious = GetPreviousNodeNotBeingPoint(q, pt);
							}

							var nodeNext = node.GetNextNode();
							if (nodeNext == null)
							{
								nodeNext = GetNextNodeNotBeingPoint(q, pt);
							}

							return new Tuple<Point, Point>(nodePrevious.Item, nodeNext.Item);
						}
					}

					q = q.GetNextQuadrant();
				} while (q != _q1);
			}

			return null;
		}

		// ******************************************************************
		/// <summary>
		/// Return the next neighbor of the item is exists. 
		/// Otherwise it retuen default(Point).
		/// Not super efficient. If you want to iterate over all points, it is 
		/// better to call either the iterator (iterate once) or 
		/// GetResultsAsArrayOfPoint() instead (iterate more than once).
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>Same point if only one. Next point if more than one and point exists. default(Point) otherwise (not found)</returns>
		public Point GetNextPoint(Point pt)
		{
			if (Count <= 1)
			{
				if (Count == 1)
				{
					if (_q1.FirstPoint == pt)
					{
						return pt;
					}
				}
			}
			else
			{
				AvlNode<Point> node = null;

				Quadrant q = _q1;
				do
				{
					if (q.IsGoodQuadrantForPoint(pt))
					{
						node = q.GetNode(pt);
						if (node != null)
						{
							node = node.GetNextNode();
							if (node != null)
							{
								return node.Item;
							}
							
							return GetNextNodeNotBeingPoint(q, pt).Item;
						}
					}

					q = q.GetNextQuadrant();
				} while (q != _q1);
			}

			return default(Point);
		}

		// ******************************************************************
		/// <summary>
		/// Return the previous neighbor of the item is exists. 
		/// Otherwise it retuen default(Point).
		/// Not super efficient. If you want to iterate over all points, it is 
		/// better to call either the iterator (iterate once) or 
		/// GetResultsAsArrayOfPoint() instead (iterate more than once).
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>Same point if only one. Next point if more than one and point exists. default(Point) otherwise (not found)</returns>
		public Point GetPreviousPoint(Point pt)
		{
			if (Count <= 1)
			{
				if (Count == 1)
				{
					if (_q1.FirstPoint == pt)
					{
						return pt;
					}
				}
			}
			else
			{
				AvlNode<Point> node = null;

				Quadrant q = _q1;
				do
				{
					if (q.IsGoodQuadrantForPoint(pt))
					{
						node = q.GetNode(pt);
						if (node != null)
						{
							node = node.GetPreviousNode();
							if (node != null)
							{
								return node.Item;
							}

							return GetPreviousNodeNotBeingPoint(q, pt).Item;
						}
					}

					q = q.GetPreviousQuadrant();
				} while (q != _q1);
			}

			return default(Point);
		}

		// ******************************************************************
		/// <summary>
		/// Can't be call for Hull count smaller or equal to 1 because it will loop forever.
		/// </summary>
		/// <param name="q">Quadrant where the point has no next node</param>
		/// <param name="pt">Last point of a quadrant</param>
		/// <returns></returns>
		private AvlNode<Point> GetNextNodeNotBeingPoint(Quadrant q, Point pt)
		{
			for (; ; )
			{
				q = q.GetNextQuadrant();
				AvlNode<Point> nodeNext = q.GetFirstNode();
				if (nodeNext.Item != pt)
				{
					return nodeNext;
				}

				nodeNext = nodeNext.GetNextNode();
				if (nodeNext != null)
				{
					return nodeNext;
				}
			}
		}

		// ******************************************************************
		/// <summary>
		/// Can't be call for Hull count smaller or equal to 1 because it will loop forever.
		/// </summary>
		/// <param name="q">Quadrant where the point has no next node</param>
		/// <param name="pt">Last point of a quadrant</param>
		/// <returns></returns>
		private AvlNode<Point> GetPreviousNodeNotBeingPoint(Quadrant q, Point pt)
		{
			for (; ; )
			{
				q = q.GetPreviousQuadrant();
				AvlNode<Point> nodePrevious = q.GetLastNode();
				if (nodePrevious.Item != pt)
				{
					return nodePrevious;
				}

				nodePrevious = nodePrevious.GetPreviousNode();
				if (nodePrevious != null)
				{
					return nodePrevious;
				}
			}
		}

		// ******************************************************************
		/// <summary>
		/// Verify if a point would be added or not as a part of the 
		/// current convex hull solution. 
		/// Duplication of code (partial or complete is intentional in order 
		/// keep best performance)
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>-1 point already exists, 0 point not part of the convex hull, 1 point will be added if asked for</returns>
		public EnumConvexHullPoint Evaluate(Point pt)
		{
			if (!IsInitDone)
			{
				return EnumConvexHullPoint.ConvexHullPoint;
			}

			//
			// Step 1/2: 
			// Find if the new point does affect quadrant boundary (limits).
			// If so, correct affected quadrants limits accordingly ...
			// (what should be done prior to try to insert any point in its (or 2) possible quadrant).
			// 

			LimitEnum limitAffectd = 0;

			if (pt.X >= Right)
			{
				// Should update both quadrant affected accordingly					

				//Q4
				if (pt.X > Right || pt.Y < _q4.LastPoint.Y)
				{
					if (pt.Y <= Bottom)
					{
						limitAffectd |= LimitEnum.Bottom;
					}

					limitAffectd |= LimitEnum.Right;
				}

				// Q1
				if (pt.X > Right || pt.Y > _q1.FirstPoint.Y)
				{
					if (pt.Y >= Top)
					{
						limitAffectd |= LimitEnum.Top;
					}

					limitAffectd |= LimitEnum.Right;
				}
			}

			if (pt.Y >= Top)
			{
				// Should update both quadrant affected accordingly					

				// Q1
				if (pt.Y > Top || pt.X > _q1.LastPoint.X)
				{
					if (pt.X >= Right)
					{
						limitAffectd |= LimitEnum.Right;
					}

					limitAffectd |= LimitEnum.Top;
				}

				//Q2
				if (pt.Y > Top || pt.X < _q2.FirstPoint.X)
				{
					if (pt.X <= Left)
					{
						limitAffectd |= LimitEnum.Left;
					}

					limitAffectd |= LimitEnum.Top;
				}
			}

			if (pt.X <= Left)
			{
				// Should update both quadrant affected accordingly					

				// Q2
				if (pt.X < Left || pt.Y > _q2.LastPoint.Y)
				{
					if (pt.Y >= Top)
					{
						limitAffectd |= LimitEnum.Top;
					}

					limitAffectd |= LimitEnum.Left;
				}

				//Q3
				if (pt.X < Left || pt.Y < _q3.FirstPoint.Y)
				{
					if (pt.Y <= Bottom)
					{
						limitAffectd |= LimitEnum.Bottom;
					}

					limitAffectd |= LimitEnum.Left;
				}
			}

			if (pt.Y <= Bottom)
			{
				// Should update both quadrant affected accordingly					

				// Q3
				if (pt.Y < Bottom || pt.X < _q3.LastPoint.X)
				{
					if (pt.X <= Left)
					{
						limitAffectd |= LimitEnum.Left;
					}

					limitAffectd |= LimitEnum.Bottom;
				}

				//Q4
				if (pt.Y < Bottom || pt.X > _q4.FirstPoint.X)
				{
					if (pt.X >= Right)
					{
						limitAffectd |= LimitEnum.Right;
					}

					limitAffectd |= LimitEnum.Bottom;
				}
			}

			if (limitAffectd != 0)
			{
				return EnumConvexHullPoint.ConvexHullPoint;
			}

			return EvaluatePointForPointInsideQuadrantLimits(pt);
		}

		// ************************************************************************
		/// <summary>
		/// Will add another point to the convex hull if appropriate.
		/// Return -1 point already exists, 0 point not part of the convex hull, 1 point added.
		/// </summary>
		/// Duplication of code (partial or complete is intentional in order 
		/// keep best performance)
		/// <param name="pt"></param>
		/// <returns>-1 point already exists, 0 point not part of the convex hull, 1 point added</returns>
		public EnumConvexHullPoint TryAddOnePoint(Point pt)
		{
			if (!IsInitDone)
			{
				_pointsArrayForDynamicCall[0] = pt;
				Init(_pointsArrayForDynamicCall);

				SetQuadrantLimitsOneThread(_pointsArrayForDynamicCall);

				_q1.Prepare();
				_q2.Prepare();
				_q3.Prepare();
				_q4.Prepare();
				return EnumConvexHullPoint.ConvexHullPoint;
			}

			//
			// Step 1/2: 
			// Find if the new point does affect quadrant boundary (limits).
			// If so, correct affected quadrants limits accordingly ...
			// (what should be done prior to try to insert any point in its (or 2) possible quadrant).
			// 

			LimitEnum limitAffectd = 0;

			if (pt.X >= Right)
			{
				// Should update both quadrant affected accordingly					

				//Q4
				if (pt.X > Right || pt.Y < _q4.LastPoint.Y)
				{
					if (pt.Y <= Bottom)
					{
						InvalidateAllQuadrantPoints(_q4, pt);
						limitAffectd |= LimitEnum.Bottom;
					}
					else
					{
						_q4.LastPoint = pt;
						_q4.RootPoint = new Point(_q4.FirstPoint.X, pt.Y);
						var avlPoint = _q4.AddOrUpdate(pt);
						_q4.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
					}

					limitAffectd |= LimitEnum.Right;
				}

				// Q1
				if (pt.X > Right || pt.Y > _q1.FirstPoint.Y)
				{
					if (pt.Y >= Top)
					{
						InvalidateAllQuadrantPoints(_q1, pt);
						limitAffectd |= LimitEnum.Top;
					}
					else
					{
						_q1.FirstPoint = pt;
						_q1.RootPoint = new Point(_q1.LastPoint.X, pt.Y);
						var avlPoint = _q1.AddOrUpdate(pt);
						_q1.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
					}

					limitAffectd |= LimitEnum.Right;
				}
			}

			if (pt.Y >= Top)
			{
				// Should update both quadrant affected accordingly					

				// Q1
				if (pt.Y > Top || pt.X > _q1.LastPoint.X)
				{
					if (pt.X >= Right)
					{
						InvalidateAllQuadrantPoints(_q1, pt);
						limitAffectd |= LimitEnum.Right;
					}
					else
					{
						_q1.LastPoint = pt;
						_q1.RootPoint = new Point(pt.X, _q1.FirstPoint.Y);
						var avlPoint = _q1.AddOrUpdate(pt);

						// Special case for top and bottom where sometimes, we should remove points between previous limit and new one.
						for (; ; )
						{
							AvlNode<Point> nodeToRemove = avlPoint.GetNextNode();
							if (nodeToRemove == null)
							{
								break;
							}
							_q1.RemoveNodeSafe(nodeToRemove);
						}

						_q1.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
					}

					limitAffectd |= LimitEnum.Top;
				}

				//Q2
				if (pt.Y > Top || pt.X < _q2.FirstPoint.X)
				{
					if (pt.X <= Left)
					{
						InvalidateAllQuadrantPoints(_q2, pt);
						limitAffectd |= LimitEnum.Left;
					}
					else
					{
						_q2.FirstPoint = pt;
						_q2.RootPoint = new Point(pt.X, _q2.LastPoint.Y);
						var avlPoint = _q2.AddOrUpdate(pt);

						// Special case for top and bottom where sometimes, we should remove points between previous limit and new one.
						for (; ; )
						{
							AvlNode<Point> nodeToRemove = avlPoint.GetPreviousNode();
							if (nodeToRemove == null)
							{
								break;
							}
							_q2.RemoveNodeSafe(nodeToRemove);
						}

						_q2.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
					}

					limitAffectd |= LimitEnum.Top;
				}
			}

			if (pt.X <= Left)
			{
				// Should update both quadrant affected accordingly					

				// Q2
				if (pt.X < Left || pt.Y > _q2.LastPoint.Y)
				{
					if (pt.Y >= Top)
					{
						InvalidateAllQuadrantPoints(_q2, pt);
						limitAffectd |= LimitEnum.Top;
					}
					else
					{
						_q2.LastPoint = pt;
						_q2.RootPoint = new Point(_q2.FirstPoint.X, pt.Y);
						var avlPoint = _q2.AddOrUpdate(pt);
						_q2.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
					}

					limitAffectd |= LimitEnum.Left;
				}

				//Q3
				if (pt.X < Left || pt.Y < _q3.FirstPoint.Y)
				{
					if (pt.Y <= Bottom)
					{
						InvalidateAllQuadrantPoints(_q3, pt);
						limitAffectd |= LimitEnum.Bottom;
					}
					else
					{
						_q3.FirstPoint = pt;
						_q3.RootPoint = new Point(_q3.LastPoint.X, pt.Y);
						var avlPoint = _q3.AddOrUpdate(pt);
						_q3.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
					}

					limitAffectd |= LimitEnum.Left;
				}
			}

			if (pt.Y <= Bottom)
			{
				// Should update both quadrant affected accordingly					

				// Q3
				if (pt.Y < Bottom || pt.X < _q3.LastPoint.X)
				{
					if (pt.X <= Left)
					{
						InvalidateAllQuadrantPoints(_q3, pt);
						limitAffectd |= LimitEnum.Left;
					}
					else
					{
						_q3.LastPoint = pt;
						_q3.RootPoint = new Point(pt.X, _q3.FirstPoint.Y);
						var avlPoint = _q3.AddOrUpdate(pt);

						// Special case for top and bottom where sometimes, we should remove points between previous limit and new one.
						for (; ; )
						{
							AvlNode<Point> nodeToRemove = avlPoint.GetNextNode();
							if (nodeToRemove == null)
							{
								break;
							}
							_q3.RemoveNodeSafe(nodeToRemove);
						}

						_q3.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
					}

					limitAffectd |= LimitEnum.Bottom;
				}

				//Q4
				if (pt.Y < Bottom || pt.X > _q4.FirstPoint.X)
				{
					if (pt.X >= Right)
					{
						InvalidateAllQuadrantPoints(_q4, pt);
						limitAffectd |= LimitEnum.Right;
					}
					else
					{
						_q4.FirstPoint = pt;
						_q4.RootPoint = new Point(pt.X, _q4.LastPoint.Y);
						var avlPoint = _q4.AddOrUpdate(pt);

						// Special case for top and bottom where sometimes, we should remove points between previous limit and new one.
						for (; ; )
						{
							AvlNode<Point> nodeToRemove = avlPoint.GetPreviousNode();
							if (nodeToRemove == null)
							{
								break;
							}
							_q4.RemoveNodeSafe(nodeToRemove);
						}

						_q4.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
					}

					limitAffectd |= LimitEnum.Bottom;
				}
			}

			if (limitAffectd != 0)
			{
				if (limitAffectd.HasFlag(LimitEnum.Right))
				{
					Right = pt.X;
				}

				if (limitAffectd.HasFlag(LimitEnum.Top))
				{
					Top = pt.Y;
				}

				if (limitAffectd.HasFlag(LimitEnum.Left))
				{
					Left = pt.X;
				}

				if (limitAffectd.HasFlag(LimitEnum.Bottom))
				{
					Bottom = pt.Y;
				}

				return EnumConvexHullPoint.ConvexHullPoint;
			}

			//
			// Step 2:
			// If point does not modify any actual quadrant boundary (which would not required any further action), then 
			// add it to its appropriate quadrant.
			//

			// Quadrant(s) limit did not change
			// We call method for non disjoint quadrant because it is not worth the time to calculate if quadrant
			// are disjoint or not, also this method is safe in both cases (disjoint or not) which is not the case 
			// for ProcessPointsForDisjointQuadrant where some optimisation are done which require quadrant to be disjoint.

			return ProcessPointForNotDisjointQuadrant(pt);
		}

		// ************************************************************************
		private void InvalidateAllQuadrantPoints(Quadrant q, Point pt)
		{
			// Invalidate all quadrant points
			q.FirstPoint = pt;
			q.LastPoint = pt;
			q.RootPoint = pt;
			q.Clear();
			q.Add(pt);
		}

		// ************************************************************************
		// End begin "Online" section (dynamic add point to the convex hull) 
		// ************************************************************************

		#endregion

		// ************************************************************************
		private void SetQuadrantLimitsOneThread(IReadOnlyList<Point> points)
		{
			Point ptFirst = points.First();

			// Find the quadrant limits (maximum x and y)

			double right, topLeft, topRight, left, bottomLeft, bottomRight;
			right = topLeft = topRight = left = bottomLeft = bottomRight = ptFirst.X;

			double top, rightTop, rightBottom, bottom, leftTop, leftBottom;
			top = rightTop = rightBottom = bottom = leftTop = leftBottom = ptFirst.Y;

			foreach (Point pt in points)
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

				Top = top;
				Left = left;
				Bottom = bottom;
				Right = right;

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
		private void SetQuadrantLimitsUsingAllThreads(IReadOnlyList<Point> points)
		{
			Point pt = points.First();
			_limit = new Limit(pt);

			int coreCount = Environment.ProcessorCount;

			Task[] tasks = new Task[coreCount];
			for (int n = 0; n < tasks.Length; n++)
			{
				int nLocal = n; // Prevent Lambda internal closure error.
				tasks[n] = Task.Factory.StartNew(() =>
				{
					Limit limit = _limit.Copy();
					FindLimits(points, nLocal, coreCount, limit);
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
		private object _findLimitFinalLock = new object();

		/// <summary>
		/// Utility function to calculate used when determining limits in a 
		/// multithreaded way.
		/// </summary>
		/// <param name="limit"></param>
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

		// ************************************************************************
		/// <summary>
		/// Return the count of convex hull points. That is the raw number of point.
		/// There is no additional point added to close the Convex Hull.
		/// </summary>
		public int Count
		{
			get
			{
				if (!IsInitDone)
				{
					return 0;
				}

				int countOfPoints = _q1.Count + _q2.Count + _q3.Count + _q4.Count;

				if (countOfPoints == 0)
				{
					return 0;
				}

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

					if (countOfPoints == 0)
					{
						countOfPoints = 1;
					}
				}

				return countOfPoints;
			}
		}

		// ******************************************************************
		/// <summary>
		/// Create an array with the result of the convex hull which is stored 
		/// into a tree for each quadrant.
		/// </summary>
		/// <param name="shouldCloseTheGraph"></param>
		/// <returns></returns>
		public Point[] GetResultsAsArrayOfPoint(bool shouldCloseTheGraph = true)
		{
			int countOfPoints = Count;

			if (!IsInitDone || countOfPoints == 0)
			{
				return new Point[0];
			}

			if (countOfPoints == 1) // Case where there is only one point
			{
				return new Point[] { _q1.FirstPoint };
			}

			if (shouldCloseTheGraph)
			{
				countOfPoints++;
			}

			Point[] results = new Point[countOfPoints];

			int resultIndex = -1;

			Point lastPoint = _q4.LastPoint;

			foreach (var quadrant in _quadrants)
			{
				var node = quadrant.GetFirstNode();
				if (node.Item == lastPoint)
				{
					node = node.GetNextNode();
				}

				while (node != null)
				{
					results[++resultIndex] = node.Item;
					node = node.GetNextNode();
				}

				lastPoint = quadrant.LastPoint;
			}

			if (shouldCloseTheGraph && results[resultIndex] != results[0])
			{
				resultIndex++;

				if (resultIndex >= countOfPoints)
				{
					// Array not big enough
					Dump();
					Debugger.Break();
					VerifyIntegrity();
				}

				results[resultIndex] = results[0];
			}

			// General.DebugUtil.Print(results);

			return results;

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
		/// <summary>
		/// This will provide an iterator adapter wrapper which could be use 
		/// to iterate over each convex hull result values. 
		/// Please note. That it is could for one use, but to read result many
		/// times, it is recommanded to use GetResultsAsArrayOfPoint in order 
		/// to get an array copy of the convex hull result and iterate over it.
		/// Otherwise, some performance penalty could occur due to the fact 
		/// the iterator has to iterate over each quadrant tree which imply 
		/// some slowness in regard to an iterating over an array.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Point> GetEnumerator()
		{
			return new ConvexHullEnumerator(this);
		}

		// ******************************************************************
		/// <summary>
		/// This will provide an iterator adapter wrapper which could be use 
		/// to iterate over each convex hull result values. 
		/// Please note. That it is could for one use, but to read result many
		/// times, it is recommanded to use GetResultsAsArrayOfPoint in order 
		/// to get an array copy of the convex hull result and iterate over it.
		/// Otherwise, some performance penalty could occur due to the fact 
		/// the iterator has to iterate over each quadrant tree which imply 
		/// some slowness in regard to an iterating over an array.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ConvexHullEnumerator(this);
		}

		// ******************************************************************
		private void VerifyIntegrity()
		{
			Assert(Left <= Right);
			Assert(Top >= Bottom);

			Assert(_q1.FirstPoint.X == Right);
			Assert(_q1.LastPoint.Y == Top);

			Assert(_q2.FirstPoint.Y == Top);
			Assert(_q2.LastPoint.X == Left);

			Assert(_q3.FirstPoint.X == Left);
			Assert(_q3.LastPoint.Y == Bottom);

			Assert(_q4.FirstPoint.Y == Bottom);
			Assert(_q4.LastPoint.X == Right);

			Assert(_q1.FirstPoint.Y >= _q4.LastPoint.Y);
			Assert(_q2.FirstPoint.X <= _q1.LastPoint.X);
			Assert(_q3.FirstPoint.Y <= _q2.LastPoint.Y);
			Assert(_q4.FirstPoint.X >= _q3.LastPoint.X);

			Assert(_q1.FirstPoint == _q1.GetFirstNode().Item);
			Assert(_q1.LastPoint == _q1.GetLastNode().Item);
			Assert(_q1.RootPoint.X == _q1.LastPoint.X);
			Assert(_q1.RootPoint.Y == _q1.FirstPoint.Y);

			Assert(_q2.FirstPoint == _q2.GetFirstNode().Item);
			Assert(_q2.LastPoint == _q2.GetLastNode().Item);
			Assert(_q2.RootPoint.X == _q2.FirstPoint.X);
			Assert(_q2.RootPoint.Y == _q2.LastPoint.Y);

			Assert(_q3.FirstPoint == _q3.GetFirstNode().Item);
			Assert(_q3.LastPoint == _q3.GetLastNode().Item);
			Assert(_q3.RootPoint.X == _q3.LastPoint.X);
			Assert(_q3.RootPoint.Y == _q3.FirstPoint.Y);

			Assert(_q4.FirstPoint == _q4.GetFirstNode().Item);
			Assert(_q4.LastPoint == _q4.GetLastNode().Item);
			Assert(_q4.RootPoint.X == _q4.FirstPoint.X);
			Assert(_q4.RootPoint.Y == _q4.LastPoint.Y);
		}

		// ******************************************************************
		[DebuggerHidden]
		private void Assert(bool isOk)
		{
			if (!isOk)
			{
				Dump();
				Debugger.Break();
				throw new ConvexHullResultIntegrityException();
			}
		}

		// ******************************************************************
		public void DumpVisual()
		{
			_q1.DumpVisual();
			_q2.DumpVisual();
			_q3.DumpVisual();
			_q4.DumpVisual();
		}

		// ******************************************************************
		public string DumpMaxHeight()
		{
			return $"Max Height: {_q1.GetMaxHeight()} | {_q2.GetMaxHeight()} | {_q3.GetMaxHeight()} | {_q4.GetMaxHeight()}";
		}


		// ******************************************************************
		public string DumpTreeNodeCount()
		{
			return $"Node count: {_q1.Count} | {_q2.Count} | {_q3.Count} | {_q4.Count}";
		}

		// ******************************************************************
		public void CheckNextNodePreviousNodeCoherence()
		{
			if (!IsInitDone)
			{
				return;
			}
			
			Point[] results = GetResultsAsArrayOfPoint();
			int index = 0;

			if (results.Length > 1)
			{
				if (_q1.FirstPoint == _q4.LastPoint)
				{
					index = results.Length -
							2; // Don't ask :-) ... Optimization in both place (GetResultsAsArrayOfPoint and Enumerator... generate a little incoherence between both func, diff start point sometimes)
				}
			}

			foreach (Point pt in this)
			{
				Debug.Assert(results[index] == pt);
				Point ptNext = GetNextPoint(pt);
				int indexNext = index >= results.Length - 1 ? 0 : index + 1;
				Debug.Assert(ptNext == results[indexNext]);

				Point ptPrevious = GetPreviousPoint(pt);
				int indexPrevious = index == 0 ? results.Length - 2 : index - 1;
				if (indexPrevious < 0) // When results.length = 1
				{
					indexPrevious = 0;
				}
				Debug.Assert(ptPrevious == results[indexPrevious]);

				var neighbors = GetNeighbors(pt);
				Debug.Assert(neighbors.Item1 == results[indexPrevious]);
				Debug.Assert(neighbors.Item2 == results[indexNext]);

				index++;
				if (index >= results.Length -1)
				{
					index = 0;
				}
			}
		}

		// ******************************************************************
	}
}

