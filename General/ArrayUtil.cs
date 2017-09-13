using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace General
{
	public static class ArrayUtil
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="count">Actual valid items count in the array</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InsertItem<T>(ref T[] array, T item, int index, ref int countOfValidItems) where T : struct
		{
			if (countOfValidItems >= array.Length)
			{
				T[] dest = new T[array.Length * 2];

				Array.Copy(array, 0, dest, 0, index);
				Array.Copy(array, index, dest, index + 1, countOfValidItems - index);

				array = dest;
			}
			else
			{
				Array.Copy(array, index, array, index + 1, countOfValidItems - index);
			}

			array[index] = item;
			countOfValidItems++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveRange<T>(ref T[] array, int index, int count, ref int countOfValidItems) where T : struct
		{
			Array.Copy(array, index + count, array, index, countOfValidItems - count - index);

			countOfValidItems -= count;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ImmutableInsertItem<T>(this T[] array, T item, int index)
		{
			T[] dest = new T[array.Length + 1];
			Array.Copy(array, 0, dest, 0, index);
			Array.Copy(array, index, dest, index + 1, array.Length - index);
			dest[index] = item;
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ImmutableRemoveRange<T>(this T[] array, int index, int count)
		{
			T[] dest = new T[array.Length - count];

			Array.Copy(array, 0, dest, 0, index);
			Array.Copy(array, index + count, dest, index, array.Length - count - index);
			return dest;
		}


		public static bool EqualsEx<T>(this T[] array1, T[] array2, int count)
		{
			if (array1.Length < count || array2.Length < count)
			{
				return false;
			}

			for (int index = 0; index < count; index++)
			{
				if (!array1[index].Equals(array2[index]))
				{
					return false;
				}
			}

			return true;
		}


		public static bool EqualsEx<T>(this T[] array, int array1ValidItemCount, IEnumerable<T> enum2)
		{
			if (array1ValidItemCount > array.Length)
			{
				return false;
			}

			int index = 0;
			foreach(T item in enum2)
			{
				if (index >= array1ValidItemCount)
				{
					return false;
				}

				if (! item.Equals(array[index]))
				{
					return false;
				}

				index++;
			}

			if (index != array1ValidItemCount)
			{
				return false;
			}

			return true;
		}

		public static bool EqualsEx<T>(this T[] array1, T[] array2)
		{
			if (array1.Length != array2.Length)
			{
				return false;
			}

			for (int index = 0; index < array1.Length; index++)
			{
				if (!array1[index].Equals(array2[index]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
