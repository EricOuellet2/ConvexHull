using System;

namespace ConvexHullWorkbench
{
    /// <summary>
    /// To get more accurate TimeSpan, it is better to get it from ticks.
    /// https://stackoverflow.com/questions/5450439/timespan-frommilliseconds-strange-implementation
    /// </summary>
    public static class TimeSpanHelper
	{
		public static TimeSpan MoreAccurateTimeSpanFromSeconds(double seconds)
		{
			return TimeSpan.FromTicks((long) (seconds*(double) TimeSpan.TicksPerSecond));
		}
	}
}
