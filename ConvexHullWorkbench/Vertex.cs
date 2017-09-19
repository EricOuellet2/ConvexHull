using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHullWorkbench
{
	public class Vertex : IVertex
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Vertex"/> class.
		/// </summary>
		/// <param name="x">The x position.</param>
		/// <param name="y">The y position.</param>
		public Vertex(double x, double y)
		{
			Position = new double[2] { x, y };
		}

		public double[] Position { get; set; }
	}
}
