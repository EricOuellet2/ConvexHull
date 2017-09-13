using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvexHullHelper;

namespace ConvexHullWorkbench
{
	public class AlgorithmStat
	{
		// ************************************************************************
		public int PointCount { get; set; }
		public int ResultCount { get; set; }
		// ************************************************************************
		// Time taken to calc ConvexHull directly in the original language without any conversion for C#
		public TimeSpan TimeSpanOriginal { get; set; }

		// ************************************************************************
		// Time taken to calc ConvexHull plus the time to convert it to C# if required
		public TimeSpan TimeSpanCSharp { get; set; }

		// ************************************************************************
		public TimeSpan BestTimeSpan
		{
			get
			{
				if (TimeSpanOriginal != default(TimeSpan))
				{
					return TimeSpanOriginal;
				}

				return TimeSpanCSharp;
			}
		}

		// ************************************************************************
	}
}
