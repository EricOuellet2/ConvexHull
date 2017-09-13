using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConvexHullHelper
{
	public class HullStats
	{
		// ************************************************************************
		private List<HullStat> _items = new List<HullStat>();

		// ************************************************************************
		public List<HullStat> Items
		{
			get
			{
				if (_items == null)
				{
					_items = new List<HullStat>();
				}

				return _items;
			}
		}

		// ************************************************************************
		public void ToExcelFile(string path)
		{
			StringBuilder sb = new StringBuilder();

			if (Items != null && Items.Count > 0 && Items[0].StatEntries != null && Items[0].StatEntries.Count > 0)
			{
				// Header
				sb.Append("Count");
				foreach (HullStatEntry entry in this.Items[0].StatEntries.Values)
				{
					sb.Append(",");
					sb.Append(entry.HullType.ToString());
				}

				sb.Append("\r\n");

				// Data
				foreach (HullStat stat in Items)
				{
					sb.Append(stat.PointCount.ToString());
					sb.Append(", ");

					foreach (HullStatEntry entry in stat.StatEntries.Values)
					{
						sb.Append(entry.TimeSpan.TotalMilliseconds);
						sb.Append(", ");
					}

					sb.Append("\r\n");
				}

				using (StreamWriter outfile = new StreamWriter(path))
				{
					outfile.WriteLine(sb.ToString());
				}
			}
		}

		// ************************************************************************
	}
}
