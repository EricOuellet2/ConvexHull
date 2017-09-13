using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Unsafe
{
	public static class PointArrayUtilUnsafe
	{
		// ************************************************************************
		//[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		//public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
		public static unsafe extern void* MemCopy(void* dest, void* src, ulong count);


		[DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
		public static unsafe extern void* MemMove(void* dest, void* src, ulong count);
		
		// ************************************************************************
		/// <summary>
		/// Insert an "item" at "index" position into an "array" which could be bigger 
		/// than the real count of items ("countOfValidItems") in it.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="item"></param>
		/// <param name="index"></param>
		/// <param name="countOfValidItems"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		unsafe public static void InsertItem(ref Point[] array, Point item, int index, ref int countOfValidItems)
		{
			if (countOfValidItems >= array.Length)
			{
				Point[] dest = new Point[array.Length * 2];

				fixed (Point* s = array)
				{
					fixed (Point* d = dest)
					{
						MemCopy(d, s, (ulong)(index * sizeof(Point)));
						MemCopy(d + index + 1, s + index, (ulong)((countOfValidItems - index) * sizeof(Point)));
					}
				}

				array = dest;
			}
			else
			{

				fixed (Point* p = array)
				{
					MemMove(p + (index + 1), p + index, (ulong)((countOfValidItems - index) * sizeof(Point)));
				}
			}

			array[index] = item;
			countOfValidItems++;
		}

		// ************************************************************************
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		unsafe public static void RemoveRange(ref Point[] array, int index, int count, ref int countOfValidItems)
		{
			fixed (Point* p = array)
			{
				MemMove(p + index, p + index + count, (ulong)((countOfValidItems - index) * sizeof(Point)));
				// MemCopy(p + index, p + index + count, (ulong)((countOfValidItems - index) * sizeof(Point)));
			}

			countOfValidItems -= count;
		}

		// ************************************************************************
	}
}
