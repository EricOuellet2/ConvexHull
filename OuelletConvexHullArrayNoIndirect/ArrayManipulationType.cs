using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuelletConvexHullArrayNoIndirect
{
	public enum PointArrayManipulationType
	{
		ArrayCopy = 0,
		// BufferCopy = 1,
		UnsafeCMemCopy = 2,
		ArrayCopyImmutable = 3,
	}
}
