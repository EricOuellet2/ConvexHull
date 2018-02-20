using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using General.AvlTreeSet;
using OuelletConvexHullAvl3.AvlTreeSet;

namespace OuelletConvexHullAvl3
{
	public class QuadrantSpecific4 : Quadrant
	{
		// ************************************************************************
		public const string QuadrantName = "Quadrant 4";

		// ************************************************************************
		public QuadrantSpecific4(ConvexHull convexHull, IReadOnlyList<Point> listOfPoint) : base(convexHull, listOfPoint, new Q4Comparer())
		{
			Name = QuadrantName;
		}

		// ******************************************************************
		private QuadrantSpecific4()
		{
		}

		// ******************************************************************
		public override Quadrant Clone()
		{
			var q = new QuadrantSpecific4();
			this.CopyTo(q);
			return q;
		}

		// ******************************************************************
		protected override void SetQuadrantLimits()
		{
			Point firstPoint = this.ListOfPoint.First();

			double rightX = firstPoint.X;
			double rightY = firstPoint.Y;

			double bottomX = rightX;
			double bottomY = rightY;

			foreach (var point in ListOfPoint)
			{
				if (point.X >= rightX)
				{
					if (point.X == rightX)
					{
						if (point.Y < rightY)
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

				if (point.Y <= bottomY)
				{
					if (point.Y == bottomY)
					{
						if (point.X > bottomX)
						{
							bottomX = point.X;
						}
					}
					else
					{
						bottomX = point.X;
						bottomY = point.Y;
					}
				}
			}

			FirstPoint = new Point(bottomX, bottomY);
			LastPoint = new Point(rightX, rightY);
			RootPoint = new Point(bottomX, rightY);
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override bool IsGoodQuadrantForPoint(Point pt)
		{
			if (pt.X >= this.RootPoint.X && pt.Y <= this.RootPoint.Y)
			{
				return true;
			}

			return false;
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override EnumConvexHullPoint IsHullPoint(ref Point point)
		{
			CurrentNode = Root;
			AvlNode<Point> currentPrevious = null;
			AvlNode<Point> currentNext = null;

			while (CurrentNode != null)
			{
				if (point.X > CurrentNode.Item.X)
				{
					if (CurrentNode.Right != null)
					{
						CurrentNode = CurrentNode.Right;
						continue;
					}

					currentNext = CurrentNode.GetNextNode();
					if (CanQuickReject(ref point, ref currentNext.Item))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					if (!IsPointToTheRightOfOthers(CurrentNode.Item, currentNext.Item, point))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}
				}
				else if (point.X < CurrentNode.Item.X)
				{
					if (CurrentNode.Left != null)
					{
						CurrentNode = CurrentNode.Left;
						continue;
					}

					currentPrevious = CurrentNode.GetPreviousNode();
					if (CanQuickReject(ref point, ref currentPrevious.Item))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					if (!IsPointToTheRightOfOthers(currentPrevious.Item, CurrentNode.Item, point))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}
				}
				else
				{
					if (point.Y >= CurrentNode.Item.Y)
					{
						if (point.Y == CurrentNode.Item.Y)
						{
							return EnumConvexHullPoint .AlreadyExists;
						}

						return EnumConvexHullPoint.NotConvexHullPoint; // invalid point
					}

					return EnumConvexHullPoint.ConvexHullPoint;
				}

				return EnumConvexHullPoint.ConvexHullPoint;
			}

			return EnumConvexHullPoint.NotConvexHullPoint;
		}

		// ******************************************************************
		/// <summary>
		/// Iterate over each points to see if we can add it has a ConvexHull point.
		/// It is specific by Quadrant to improve efficiency.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override EnumConvexHullPoint ProcessPoint(ref Point point)
		{
			CurrentNode = Root;
			AvlNode<Point> currentPrevious = null;
			AvlNode<Point> currentNext = null;

			while (CurrentNode != null)
			{
				if (CanQuickReject(ref point, ref CurrentNode.Item))
				{
					return EnumConvexHullPoint.NotConvexHullPoint;
				}

				var insertionSide = Side.Unknown;
				if (point.X > CurrentNode.Item.X)
				{
					if (CurrentNode.Right != null)
					{
						CurrentNode = CurrentNode.Right;
						continue;
					}

					currentNext = CurrentNode.GetNextNode();
					if (CanQuickReject(ref point, ref currentNext.Item))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					if (!IsPointToTheRightOfOthers(CurrentNode.Item, currentNext.Item, point))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					insertionSide = Side.Right;
				}
				else if (point.X < CurrentNode.Item.X)
				{
					if (CurrentNode.Left != null)
					{
						CurrentNode = CurrentNode.Left;
						continue;
					}

					currentPrevious = CurrentNode.GetPreviousNode();
					if (CanQuickReject(ref point, ref currentPrevious.Item))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					if (!IsPointToTheRightOfOthers(currentPrevious.Item, CurrentNode.Item, point))
					{
						return EnumConvexHullPoint.NotConvexHullPoint;
					}

					insertionSide = Side.Left;
				}
				else
				{
					if (point.Y >= CurrentNode.Item.Y)
					{
						if (point.Y == CurrentNode.Item.Y)
						{
							return EnumConvexHullPoint .AlreadyExists;
						}

						return EnumConvexHullPoint.NotConvexHullPoint; // invalid point
					}

					// Replace CurrentNode point with point
					CurrentNode.Item = point;
					InvalidateNeighbors(CurrentNode.GetPreviousNode(), CurrentNode, CurrentNode.GetNextNode());
					return EnumConvexHullPoint.ConvexHullPoint;
				}

				//We should insert the point

				// Try to optimize and verify if can replace a node instead insertion to minimize tree balancing
				if (insertionSide == Side.Right)
				{
					currentPrevious = CurrentNode.GetPreviousNode();
					if (currentPrevious != null && !IsPointToTheRightOfOthers(currentPrevious.Item, point, CurrentNode.Item))
					{
						CurrentNode.Item = point;
						InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
						return EnumConvexHullPoint.ConvexHullPoint;
					}

					var nextNext = currentNext.GetNextNode();
					if (nextNext != null && !IsPointToTheRightOfOthers(point, nextNext.Item, currentNext.Item))
					{
						currentNext.Item = point;
						InvalidateNeighbors(null, currentNext, nextNext);
						return EnumConvexHullPoint.ConvexHullPoint;
					}
				}
				else // Left
				{
					currentNext = CurrentNode.GetNextNode();
					if (currentNext != null && !IsPointToTheRightOfOthers(point, currentNext.Item, CurrentNode.Item))
					{
						CurrentNode.Item = point;
						InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
						return EnumConvexHullPoint.ConvexHullPoint;
					}

					var previousPrevious = currentPrevious.GetPreviousNode();
					if (previousPrevious != null && !IsPointToTheRightOfOthers(previousPrevious.Item, point, currentPrevious.Item))
					{
						currentPrevious.Item = point;
						InvalidateNeighbors(previousPrevious, currentPrevious, null);
						return EnumConvexHullPoint.ConvexHullPoint;
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

				return EnumConvexHullPoint.ConvexHullPoint;
			}

			return EnumConvexHullPoint.NotConvexHullPoint;
		}

		// ******************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CanQuickReject(ref Point pt, ref Point ptHull)
		{
			if (pt.X <= ptHull.X && pt.Y >= ptHull.Y)
			{
				return true;
			}

			return false;
		}

		// ******************************************************************
		internal override Quadrant GetNextQuadrant()
		{
			return ConvexHull._q1;
		}

		// ******************************************************************
		internal override Quadrant GetPreviousQuadrant()
		{
			return ConvexHull._q3;
		}

		// ******************************************************************
	}
}
