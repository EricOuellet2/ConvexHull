using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mathematic
{
	public class CircleHelper
	{
		public static Point[] CreateCircleWithPoints(Point center, double radius, int countOfPoint)
		{
			if (radius < Double.Epsilon)
			{
				return new Point[0];
			}

			Point[] points = new Point[countOfPoint];


			for (int n = 0; n < countOfPoint; n++)
			{
				double angle = Math.PI * 2 * n / countOfPoint;

				points[n] = new Point(center.X + (Math.Cos(angle) * radius), center.Y + (Math.Sin(angle) * radius));
			}

			return points;
		}
	}
}
