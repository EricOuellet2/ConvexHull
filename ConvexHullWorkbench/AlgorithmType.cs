using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHullWorkbench
{
	public enum AlgorithmType
	{
		[Description("Convex Hull")]
		ConvexHull = 1,

		[Description("Convex Hull Online")]
		ConvexHullOnline = 2,

		[Description("Smallest Enclosing Circle")]
		SmallestEnclosingCircle = 3
	}
}
