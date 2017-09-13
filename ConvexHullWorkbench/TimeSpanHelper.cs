using System;

namespace ConvexHullWorkbench
{
	public static class TimeSpanHelper
	{
		public static TimeSpan MoreAccurateTimeSpanFromSeconds(double seconds)
		{
			return TimeSpan.FromTicks((long) (seconds*(double) TimeSpan.TicksPerSecond));
		}
	}
}
