using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OuelletConvexHullArrayNoIndirect
{
	public class QuadrantSpecific1 : Quadrant
	{
		// ************************************************************************
		public QuadrantSpecific1(
			IReadOnlyList<Point> listOfPoint, int initialResultGuessSize) :
			base(listOfPoint, initialResultGuessSize)
		{
		}

		// ******************************************************************
		protected override void SetQuadrantLimits()
		{
			Point firstPoint = this._listOfPoint.First();

			double rightX = firstPoint.X;
			double rightY = firstPoint.Y;

			double topX = rightX;
			double topY = rightY;

			foreach (var point in _listOfPoint)
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
		protected override int TryAdd(Point pt)
		{
			int indexLow = 0;
			int indexHi = HullPointCount - 1;

			while (indexLow != indexHi - 1)
			{
				int index = ((indexHi - indexLow) >> 1) + indexLow;

				if (pt.X <= HullPoints[index].X && pt.Y <= HullPoints[index].Y)
				{
					return -1; // No calc needed
				}

				if (pt.X > HullPoints[index].X)
				{
					indexHi = index;
					continue;
				}

				if (pt.X < HullPoints[index].X)
				{
					indexLow = index;
					continue;
				}

				if (pt.X == HullPoints[index].X)
				{
					if (pt.Y > HullPoints[index].Y)
					{
						indexLow = index;
					}
					else
					{
						return -1;
					}
				}

				break;
			}

			if (pt.Y <= HullPoints[indexLow].Y)
			{
				return -1; // Eliminated without slope calc
			}

			return indexLow;
		}

		// ******************************************************************
	}
}

