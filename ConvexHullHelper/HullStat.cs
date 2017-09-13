using System;
using System.Collections.Generic;
using System.Text;

namespace ConvexHullHelper
{
	public class HullStat
	{
		// ************************************************************************
		public int PointCount;
		public Dictionary<HullType, HullStatEntry> StatEntries = new Dictionary<HullType,HullStatEntry>();

		// ************************************************************************
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Points: " + PointCount);
			sb.Append("\r\n");

			foreach (var hullStatEntry in StatEntries.Values)
			{
				sb.Append(String.Format("{0,-20} : {1}", hullStatEntry.TimeSpan.ToString("c"), hullStatEntry.HullType.ToString()));
				sb.Append("\r\n");
			}

			sb.Append("--------------------------\r\n");

			return sb.ToString();
		}

		// ************************************************************************
	}
}
