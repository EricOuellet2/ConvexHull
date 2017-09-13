using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using General.AvlTreeSet;

namespace OuelletConvexHullAvl
{
	public class QuadrantSpecific1 : Quadrant
	{
		// ************************************************************************
		public const string QuadrantName = "Quadrant 1";

		// ************************************************************************
		public QuadrantSpecific1(IReadOnlyList<Point> listOfPoint) : base(listOfPoint, new Q1Comparer())
		{
			Name = QuadrantName;
		}

		// ******************************************************************
		protected override void SetQuadrantLimits()
		{
			Point firstPoint = this.ListOfPoint.First();

			double rightX = firstPoint.X;
			double rightY = firstPoint.Y;

			double topX = rightX;
			double topY = rightY;

			foreach (var point in ListOfPoint)
			{
				if (point.X >= rightX)
				{
					if (point.X == rightX)
					{
						if (point.Y > rightY)
						{
							rightY = point.Y;
						}
					}
					else
					{
						rightX = point.X;
						rightY = point.Y;
					}
				}

				if (point.Y >= topY)
				{
					if (point.Y == topY)
					{
						if (point.X > topX)
						{
							topX = point.X;
						}
					}
					else
					{
						topX = point.X;
						topY = point.Y;
					}
				}
			}

			FirstPoint = new Point(rightX, rightY);
			LastPoint = new Point(topX, topY);
			RootPoint = new Point(topX, rightY);
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsGoodQuadrantForPoint(Point pt)
		{
			if (pt.X > this.RootPoint.X && pt.Y > this.RootPoint.Y)
			{
				return true;
			}

			return false;
		}

		// ******************************************************************
		/// <summary>
		/// Iterate over each points to see if we can add it has a ConvexHull point.
		/// It is specific by Quadrant to improve efficiency.
		/// </summary>
		protected override void ProcessQuadrantSpecific()
		{
			// Main Loop to extract ConvexHullPoints
			foreach (Point point in ListOfPoint)
			{
				if (!IsGoodQuadrantForPoint(point))
				{
					continue;
				}

				CurrentNode = Root;
				AvlNode<Point> currentPrevious = null;
				AvlNode<Point> currentNext = null;

				// Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());

				while (CurrentNode != null)
				{
					if (CanQuickReject(point, CurrentNode.Item))
					{
						break;
					}

					var insertionSide = Side.Unknown;
					if (point.X > CurrentNode.Item.X)
					{
						if (CurrentNode.Left != null)
						{
							CurrentNode = CurrentNode.Left;
							continue;
						}

						currentPrevious = CurrentNode.GetPreviousNode();
						if (CanQuickReject(point, currentPrevious.Item))
						{
							break;
						}

						if (!IsPointToTheRightOfOthers(currentPrevious.Item, CurrentNode.Item, point))
						{
							break;
						}

						if (CurrentNode.Item == point) // Ensure to have no duplicate
						{
							continue;
						}

						insertionSide = Side.Left;
					}
					else if (point.X < CurrentNode.Item.X)
					{
						if (CurrentNode.Right != null)
						{
							CurrentNode = CurrentNode.Right;
							continue;
						}

						currentNext = CurrentNode.GetNextNode();
						if (CanQuickReject(point, currentNext.Item))
						{
							break;
						}

						if (!IsPointToTheRightOfOthers(CurrentNode.Item, currentNext.Item, point))
						{
							break;
						}

						if (CurrentNode.Item == point) // Ensure to have no duplicate
						{
							continue;
						}

						insertionSide = Side.Right;
					}
					else
					{
						if (point.Y <= CurrentNode.Item.Y)
						{
							break; // invalid point
						}

						// Replace CurrentNode point with point
						// Debug.Assert(CurrentNode.Parent == null || !point.Equals(CurrentNode.Parent.Item));
						CurrentNode.Item = point;

						InvalidateNeighbors(CurrentNode.GetPreviousNode(), CurrentNode, CurrentNode.GetNextNode());
						break;
					}

					//We should insert the point

					// Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());

					// Try to optimize and verify if can replace a node instead insertion to minimize tree balancing
					if (insertionSide == Side.Right)
					{
						currentPrevious = CurrentNode.GetPreviousNode();
						if (currentPrevious != null && !IsPointToTheRightOfOthers(currentPrevious.Item, point, CurrentNode.Item))
						{
							// DebugEnsureTreeIsValid();
							//Debug.Assert(CurrentNode.Item != FirstPoint && CurrentNode.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(CurrentNode.Parent == null || !point.Equals(CurrentNode.Parent.Item));
							CurrentNode.Item = point;
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//DebugEnsureTreeIsValid();
							break;
						}

						var nextNext = currentNext.GetNextNode();
						if (nextNext != null && !IsPointToTheRightOfOthers(point, nextNext.Item, currentNext.Item))
						{
							//DebugEnsureTreeIsValid();
							//Debug.Assert(currentNext.Item != FirstPoint && currentNext.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(CurrentNode.Parent == null || !point.Equals(CurrentNode.Parent.Item));
							currentNext.Item = point;
							//Debug.Assert(currentNext.Item != FirstPoint && currentNext.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							InvalidateNeighbors(null, currentNext, nextNext);
							//Debug.Assert(currentNext.Item != FirstPoint && currentNext.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//DebugEnsureTreeIsValid();
							break;
						}
					}
					else // Left
					{
						currentNext = CurrentNode.GetNextNode();
						if (currentNext != null && !IsPointToTheRightOfOthers(point, currentNext.Item, CurrentNode.Item))
						{
							//DebugEnsureTreeIsValid();
							//Debug.Assert(CurrentNode.Item != FirstPoint && CurrentNode.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(CurrentNode.Parent == null || !point.Equals(CurrentNode.Parent.Item));
							CurrentNode.Item = point;
							//Debug.Assert(CurrentNode.Item != FirstPoint && CurrentNode.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
							//Debug.Assert(CurrentNode.Item != FirstPoint && CurrentNode.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//DebugEnsureTreeIsValid();
							break;
						}

						var previousPrevious = currentPrevious.GetPreviousNode();
						if (previousPrevious != null && !IsPointToTheRightOfOthers(previousPrevious.Item, point, currentPrevious.Item))
						{
							//DebugEnsureTreeIsValid();
							//Debug.Assert(currentPrevious.Item != FirstPoint && currentPrevious.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//Debug.Assert(CurrentNode.Parent == null || !point.Equals(CurrentNode.Parent.Item));
							currentPrevious.Item = point;
							//Debug.Assert(currentPrevious.Item != FirstPoint && currentPrevious.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							InvalidateNeighbors(previousPrevious, currentPrevious, null);
							//Debug.Assert(currentPrevious.Item != FirstPoint && currentPrevious.Item != LastPoint);
							//Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
							//DebugEnsureTreeIsValid();
							break;
						}
					}

					// Should insert but no invalidation is required. (That's why we need to insert... can't replace an adjacent neightbor)
					AvlNode<Point> newNode = new AvlNode<Point>();
					if (insertionSide == Side.Right)
					{
						newNode.Parent = CurrentNode;
						newNode.Item = point;
						CurrentNode.Right = newNode;
						this.AddBalance(newNode.Parent, -1);
					}
					else // Left
					{
						newNode.Parent = CurrentNode;
						newNode.Item = point;
						CurrentNode.Left = newNode;
						this.AddBalance(newNode.Parent, 1);
					}

					// DebugEnsureTreeIsValid();
					break;
				}
				// Debug.Assert(FirstPoint == GetFirstItem() && LastPoint == GetLastItem());
			}

			DebugEnsureTreeIsSorted();
			// Dump2("", Name);
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool CanQuickReject(Point pt, Point ptHull)
		{
			if (pt.X <= ptHull.X && pt.Y <= ptHull.Y)
			{
				return true;
			}

			return false;
		}

		// ******************************************************************

	}
}

