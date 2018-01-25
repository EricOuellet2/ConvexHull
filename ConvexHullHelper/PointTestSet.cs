using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvexHullHelper
{
	public class TestSetOfPoint
	{
		// ******************************************************************
		public Point[] Points { get; private set; }
		public Point[] ExpectedResult { get; private set; }

		// ******************************************************************
		public TestSetOfPoint(Point[] points, Point[] expectedResult)
		{
			Points = points;
			ExpectedResult = expectedResult;
		}

		// ******************************************************************

	}
}
