using System.Collections.Generic;

namespace ConvexHullHelper
{
	public class PointGeneratorManager
	{
		public static PointGeneratorManager  Instance { get; } = new PointGeneratorManager();
		public List<PointGenerator> Generators { get; } = new List<PointGenerator>();

		private PointGeneratorManager()
		{
			Generators.Add(new PointGenerator("Circle", "Random points in a circle", PointGenerator.GeneratePointsInCircle));
			Generators.Add(new PointGenerator("5 Circles", "5 random circle of random size and distance from each other with random points in it", PointGenerator.GeneratePointsIn5Circles));
			Generators.Add(new PointGenerator("Rectangle", "Random points into a rectangle", PointGenerator.GeneratePointsInRectangle));
			Generators.Add(new PointGenerator("Throw away", "Random point, then go away at random distance and angle, then loop", PointGenerator.GeneeratePointsThrowAway));
			Generators.Add(new PointGenerator("Arc", "Random point on an edge only of a quarter of an ellipse (4th Quadrant)", PointGenerator.GeneratePointsArcQuadrant4));
		}
	}
}
