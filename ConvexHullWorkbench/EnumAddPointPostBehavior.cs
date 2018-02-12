using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace ConvexHullWorkbench
{
	public enum EnumAddPointPostBehavior
	{
		GetResultAsArrayOfPointAlways = 0,
		GetResultAsArrayOfPointOnlyWhenPointIsConvexHullAdded = 1,
		QueryPreviousOrNextPointOnlyWhenPointIsConvexHullAdded = 2,
		DoNothing = 3,		
	}
}
