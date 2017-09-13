using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvexHullWorkbench
{
	public class LogEntry
	{
		public DateTime DateTime { get; set; }
		public string Message { get; private set; }
		public int Index { get; private set; }

		public LogEntry(int index, string message)
		{
			DateTime = DateTime.Now;
			Index = index;
			Message = message;
		}

		public override string ToString()
		{
			return DateTime.ToString("HH:mm:ss.fff") + "  " + Message;
		}
	}
}
