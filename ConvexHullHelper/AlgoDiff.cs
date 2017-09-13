using System;
using System.Windows;

namespace ConvexHullHelper
{
	public class AlgoDiff
	{
		public Point[] Points1 { private set; get; }
		public Point[] Points2 { private set; get; }

		public String Algo1 { private set; get; }
		public String Algo2 { private set; get; }

		public Point[] PointsDiff { private set; get; }

		public AlgoDiff(string algo1, Point[] points1, string algo2, Point[] points2, Point[] pointsDiff)
		{
			Algo1 = algo1;
			Points1 = points1;
			Algo2 = algo2;
			Points2 = points2;
			PointsDiff = pointsDiff;
		}
	}
}
