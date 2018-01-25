using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuelletConvexHullAvl2Online
{
	[Flags]
	public enum LimitEnum
	{
		Right = 1,
		Top = 2,
		Left = 4,
		Bottom = 8,
	}
}
