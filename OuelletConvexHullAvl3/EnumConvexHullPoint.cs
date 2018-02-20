using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuelletConvexHullAvl3
{
	[Flags]
	public enum EnumConvexHullPoint
	{
		NotConvexHullPoint = 0,
		AlreadyExists = 1,
		ConvexHullPoint = 2
	}
}
