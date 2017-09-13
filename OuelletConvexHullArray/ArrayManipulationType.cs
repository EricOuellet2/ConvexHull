using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OuelletConvexHullArray
{
	// ******************************************************************
	internal delegate Point[] ArrayInsertImmutableDelegate(Point[] array, Point item, int index);
	internal delegate Point[] ArrayRemoveRangeImmutableDelegate(Point[] array, int index, int count);

	// ******************************************************************
	internal delegate void ArrayInsertDelegate(ref Point[] array, Point item, int index, ref int countOfValidItems);
	internal delegate void ArrayRemoveRangeDelegate(ref Point[] array, int index, int count, ref int countOfValidItems);

	// ******************************************************************
	public enum PointArrayManipulationType
	{
		ArrayCopy = 0,
		// BufferCopy = 1,     // Exception: Array must be of primitive type (does not support struct like Point)
		UnsafeCMemCopy = 2,
		ArrayCopyImmutable = 3,
	}

	// ******************************************************************

}
