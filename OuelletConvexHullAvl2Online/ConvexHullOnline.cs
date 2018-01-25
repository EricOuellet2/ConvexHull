using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using General.AvlTreeSet;

namespace OuelletConvexHullAvl2Online
{
	// ******************************************************************
	public class ConvexHullOnline : IReadOnlyCollection<Point>
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

		private IReadOnlyList<Point> _listOfPoint;

		// ******************************************************************
		public ConvexHullOnline()
		{
		}

		// ******************************************************************
		public ConvexHullOnline(IReadOnlyList<Point> listOfPoint) : this()
		{
			Init(listOfPoint);
		}

		// ******************************************************************
		private void Init(IReadOnlyList<Point> listOfPoint)
		{
			_listOfPoint = listOfPoint;

			_q1 = new QuadrantSpecific1(_listOfPoint);
			_q2 = new QuadrantSpecific2(_listOfPoint);
			_q3 = new QuadrantSpecific3(_listOfPoint);
			_q4 = new QuadrantSpecific4(_listOfPoint);

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
		/// 
		/// </summary>
		/// <param name="threadUsage">Using ConvexHullThreadUsage.All will only use all thread for the first pass (se quadrant limits) then use only 4 threads for pass 2 (which is the actual limit).</param>
		public void CalcConvexHull()
		{
			if (!IsInitDone)
			{
				throw new InvalidOperationException($"To calc convex hull, the class should have be initialized with points, either by the constructor or method: '{nameof(Init)}'");
			}

			if (IsZeroData())
			{
				return;
			}

			SetQuadrantLimitsOneThread();

			_q1.Prepare();
			_q2.Prepare();
			_q3.Prepare();
			_q4.Prepare();

			// Main Loop to extract ConvexHullPoints
			Point[] points = _listOfPoint as Point[];

			if (points != null)
			{
				Point point;

				if (IsQuadrantAreDisjoint())
				{
					Debug.Print("Disjoint");
					ProcessPointsForDisjointQuadrant(points);
				}
				else // Not disjoint ***********************************************************************************
				{
					Debug.Print("Not disjoint");
					ProcessPointsForNotDisjointQuadrant(points);
				}
			}
		}

		// ************************************************************************
		private void ProcessPointsForDisjointQuadrant(Point[] points)
		{
			Point point;
			Point q1Root = _q1.RootPoint;
			Point q2Root = _q2.RootPoint;
			Point q3Root = _q3.RootPoint;
			Point q4Root = _q4.RootPoint;

			int index = 0;
			int pointCount = points.Length;

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
		private void ProcessPointsForNotDisjointQuadrant(Point[] points)
		{
			Point point;
			Point q1Root = _q1.RootPoint;
			Point q2Root = _q2.RootPoint;
			Point q3Root = _q3.RootPoint;
			Point q4Root = _q4.RootPoint;

			int index = 0;
			int pointCount = points.Length;

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
		static private int _iteration = 0;


		/// <summary>
		/// Will add another point to the convex hull if appropriate.
		/// "Init" should have been called prior to use this method.
		/// Duplication of code (partial or complete is intentional in order to save some call)
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>Return true if added, false otherwise</returns>
		public bool DynamicallyAddAnotherPointToConvexHullIfAppropriate(Point pt)
		{
			bool isHullAsBeenModified = false;

			if (!IsInitDone)
			{
				_pointsArrayForDynamicCall[0] = pt;
				Init(_pointsArrayForDynamicCall);

				SetQuadrantLimitsOneThread();

				_q1.Prepare();
				_q2.Prepare();
				_q3.Prepare();
				_q4.Prepare();

				return true;
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
						if (avlPoint != null) // if already exists, do nothing
						{
							_q4.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
						}
					}

					isHullAsBeenModified = true;
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

						if (avlPoint != null) // if already exists, do nothing
						{
							_q1.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
						}
					}

					isHullAsBeenModified = true;
				}

				limitAffectd |= LimitEnum.Right;
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

						if (avlPoint != null) // if already exists, do nothing
						{
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

							//_q1.RemoveNode(nodeToRemoveAutomatically);
							//nodeToRemoveAutomatically = nodeToRemoveAutomatically.GetNextNode();

							_q1.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
						}
					}

					isHullAsBeenModified = true;
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

						if (avlPoint != null) // if already exists, do nothing
						{
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

							//AvlNode<Point> nodeToRemoveAutomatically = avlPoint.GetPreviousNode();
							//while (nodeToRemoveAutomatically != null)
							//{
							//	AvlNode<Point> nodeTemp = nodeToRemoveAutomatically.GetPreviousNode();
							//	_q2.RemoveNode(nodeToRemoveAutomatically);
							//	nodeToRemoveAutomatically = nodeTemp;

							//	//_q2.RemoveNode(nodeToRemoveAutomatically);
							//	//nodeToRemoveAutomatically = nodeToRemoveAutomatically.GetPreviousNode();
							//}

							_q2.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
						}
					}

					isHullAsBeenModified = true;
				}

				limitAffectd |= LimitEnum.Top;
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
						if (avlPoint != null) // if already exists, do nothing
						{
							_q2.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
						}
					}

					isHullAsBeenModified = true;
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
						if (avlPoint != null) // if already exists, do nothing
						{
							_q3.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
						}
					}

					isHullAsBeenModified = true;
				}

				limitAffectd |= LimitEnum.Left;
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

						if (avlPoint != null) // if already exists, do nothing
						{
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

							//AvlNode<Point> nodeToRemoveAutomatically = avlPoint.GetNextNode();
							//while (nodeToRemoveAutomatically != null)
							//{
							//	AvlNode<Point> nodeTemp = nodeToRemoveAutomatically.GetNextNode();
							//	_q3.RemoveNode(nodeToRemoveAutomatically);
							//	nodeToRemoveAutomatically = nodeTemp;

							//	//_q3.RemoveNode(nodeToRemoveAutomatically);
							//	//nodeToRemoveAutomatically = nodeToRemoveAutomatically.GetNextNode();
							//}

							_q3.InvalidateNeighbors(avlPoint.GetPreviousNode(), avlPoint, null);
						}
					}

					isHullAsBeenModified = true;
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

						if (avlPoint != null) // if already exists, do nothing
						{
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


							//AvlNode<Point> nodeToRemoveAutomatically = avlPoint.GetPreviousNode();
							//while (nodeToRemoveAutomatically != null)
							//{
							//	AvlNode<Point> nodeTemp = nodeToRemoveAutomatically.GetPreviousNode();
							//	_q4.RemoveNode(nodeToRemoveAutomatically);
							//	nodeToRemoveAutomatically = nodeTemp;

							//	//_q4.RemoveNode(nodeToRemoveAutomatically);
							//	//nodeToRemoveAutomatically = nodeToRemoveAutomatically.GetPreviousNode();
							//}

							_q4.InvalidateNeighbors(null, avlPoint, avlPoint.GetNextNode());
						}
					}

					isHullAsBeenModified = true;
				}

				limitAffectd |= LimitEnum.Bottom;
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
			}

			//
			// Step 2:
			// If point does not modify any actual quadrant boundary (which would not required any further action), then 
			// add it to its appropriate quadrant.
			//

			if (!isHullAsBeenModified) // Quadrant(s) limit changed
			{
				_pointsArrayForDynamicCall[0] = pt;

				int countBefore = Count;

				// We call method for non disjoint quadrant because it is not worth the time to calculate if quadrant
				// are disjoint or not and this method is safe in both cases (disjoint or not) which is not the case 
				// for ProcessPointsForDisjointQuadrant where some optimisation are done which require quadrant to be disjoint.
				ProcessPointsForNotDisjointQuadrant(_pointsArrayForDynamicCall);

				if (countBefore != Count)
				{
					isHullAsBeenModified = true;
				}
			}

			return isHullAsBeenModified;
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

		// ************************************************************************
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
		public IEnumerator<Point> GetEnumerator()
		{
			return new ConvexHullEnumerator(this);
		}

		// ******************************************************************
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
	}
}

