using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using General.AvlTreeSet;
using OuelletConvexHullAvl3.AvlTreeSet;

namespace OuelletConvexHullAvl3
{
	public abstract class Quadrant : AvlTreeSet<Point>
	{
		// ************************************************************************
		public Point FirstPoint;
		public Point LastPoint;
		public Point RootPoint;

		protected AvlNode<Point> CurrentNode = null;

		protected IReadOnlyList<Point> ListOfPoint;

		protected ConvexHull ConvexHull = null;

		// ************************************************************************
		/// <summary>
		/// 
		/// </summary>
		/// <param name="listOfPoint"></param>
		/// <param name="comparer">Comparer is only used to add the second point (the last point, which is compared against the first one).</param>
		public Quadrant(ConvexHull convexHull, IReadOnlyList<Point> listOfPoint, IComparer<Point> comparer) : base(comparer)
		{
			ConvexHull = convexHull;
			ListOfPoint = listOfPoint;
		}

		// ************************************************************************
		protected Quadrant()
		{
		}

		// ************************************************************************
		/// <summary>
		/// Initialize every values needed to extract values that are parts of the convex hull.
		/// This is where the first pass of all values is done the get maximum in every directions (x and y).
		/// </summary>
		protected abstract void SetQuadrantLimits();

		// ************************************************************************
		public void Prepare()
		{
			if (!ListOfPoint.Any())
			{
				// There is no points at all. Hey don't try to crash me.
				return;
			}

			// Begin : General Init
			Add(FirstPoint);
			if (FirstPoint.Equals(LastPoint))
			{
				return; // Case where for weird distribution like triangle or diagonal. This quadrant will have no point
			}

			Add(LastPoint);
		}

		// ************************************************************************
		/// <summary>
		/// To know if to the right. It is meaninful when p1 is first and p2 is next.
		/// 
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="ptToCheck"></param>
		/// <returns>Equivalent of tracing a line from p1 to p2 and tell if ptToCheck
		///  is to the right or left of that line taking p1 as reference point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool IsPointToTheRightOfOthers(Point p1, Point p2, Point ptToCheck)
		{
			return ((p2.X - p1.X) * (ptToCheck.Y - p1.Y)) - ((p2.Y - p1.Y) * (ptToCheck.X - p1.X)) < 0;
		}

		// ************************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal abstract EnumConvexHullPoint IsHullPoint(ref Point point);

		// ************************************************************************
		/// <summary>
		/// Tell if should try to add and where. -1 ==> Should not add.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>1 = added, 0 = not a convex hull point, -1 convex hull point already exists</returns>
		internal abstract EnumConvexHullPoint ProcessPoint(ref Point point);

		// ************************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal abstract bool IsGoodQuadrantForPoint(Point pt);

		// ************************************************************************
		// protected abstract bool CanQuickReject(Point pt, Point ptHull);

		// ******************************************************************
		/// <summary>
		/// Called after insertion in order to see if the newly added point invalidate one 
		/// or more neighbors and if so, remove it/them from the tree.
		/// </summary>
		/// <param name="pointPrevious">The previous point if you want to go that direction</param>
		/// <param name="pointNew">The new inserted point</param>
		/// <param name="pointNext">The next point if you wan tto go that direction</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InvalidateNeighbors(AvlNode<Point> pointPrevious, AvlNode<Point> pointNew, AvlNode<Point> pointNext)
		{
			bool invalidPoint;

			if (pointPrevious != null)
			{
				AvlNode<Point> previousPrevious = pointPrevious.GetPreviousNode();
				for (; ; )
				{
					if (previousPrevious == null)
					{
						break;
					}

					invalidPoint = !IsPointToTheRightOfOthers(previousPrevious.Item, pointNew.Item, pointPrevious.Item);
					if (!invalidPoint)
					{
						break;
					}
					Point ptPrevPrev = previousPrevious.Item;

					RemoveNode(pointPrevious);
					pointPrevious = this.GetNode(ptPrevPrev);
					previousPrevious = pointPrevious.GetPreviousNode();
				}
			}

			// Invalidate next(s)
			if (pointNext != null)
			{
				AvlNode<Point> nextNext = pointNext.GetNextNode();
				for (; ; )
				{
					if (nextNext == null)
					{
						break;
					}

					invalidPoint = !IsPointToTheRightOfOthers(pointNew.Item, nextNext.Item, pointNext.Item);
					if (!invalidPoint)
					{
						break;
					}
					Point ptNextNext = nextNext.Item;

					RemoveNode(pointNext);
					pointNext = GetNode(ptNextNext);

					nextNext = pointNext.GetNextNode();
				}
			}
		}

		// ************************************************************************
		internal abstract Quadrant GetNextQuadrant();

		// ******************************************************************
		internal abstract Quadrant GetPreviousQuadrant();

		// ************************************************************************
		public abstract Quadrant Clone();

		// ************************************************************************
		public void CopyTo(Quadrant q)
		{
			base.CopyTo(q);
			q.FirstPoint = this.FirstPoint;
			q.CurrentNode = this.CurrentNode;
			q.LastPoint = this.LastPoint;
			q.RootPoint = this.RootPoint;
			q.ListOfPoint = this.ListOfPoint;
		}

		// ************************************************************************
		public void Dump2(string prefix = null)
		{
			Debug.Print($"-------------------- Quadrant Dump of {Name}");
			Debug.Print($"FirstPoint: {FirstPoint}, LastPoint: {LastPoint}, Root: {RootPoint}");
			base.DumpVisual2(prefix, Name);
		}

		// ************************************************************************
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var q = obj as Quadrant;
			if (q == null)
			{
				return false;
			}

			if (FirstPoint != q.FirstPoint || LastPoint != q.LastPoint || RootPoint != q.RootPoint)
			{
				return false;
			}

			if (this.ListOfPoint != q.ListOfPoint)
			{
				return false;
			}

			if (!base.Equals(q))
			{
				return false;
			}

			return true;
		}

		// ************************************************************************
		public override void Dump()
		{
			Debug.Print($"Quadrant dump: {Name}, FirstPoint: {FirstPoint}, LastPoint: {LastPoint}");
			base.Dump();
		}

		// ************************************************************************
		public void DumpVisual()
		{
			base.DumpVisual(Name);
		}

		// ************************************************************************
	}
}
