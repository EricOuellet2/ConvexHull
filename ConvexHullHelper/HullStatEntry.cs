using System;

namespace ConvexHullHelper
{
	public class HullStatEntry
	{
		// ************************************************************************
		public HullType HullType;
		public TimeSpan TimeSpan;

		// ************************************************************************
		public HullStatEntry(HullType hullType, TimeSpan timeSpan)
		{
			HullType = hullType;
			TimeSpan = timeSpan;
		}

		// ************************************************************************

	}
}
