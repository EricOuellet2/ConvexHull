using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvexHullHelper
{
	/// <summary>
	/// EO: 2016-04-14
	/// Generator of all permutations of an array of anything.
	/// Based on Heap's Algorithm. See: https://en.wikipedia.org/wiki/Heap%27s_algorithm#cite_note-3
	/// 
	/// EXTREMELY FAST. The fastest I know.
	/// 
	/// </summary>
	public static class Permutations
	{
		/// <summary>
		/// Heap's algorithm to find all pmermutations. Non recursive, more efficient.
		/// </summary>
		/// <param name="items">Items to permute in each possible ways</param>
		/// <param name="funcExecuteAndTellIfShouldStop"></param>
		/// <returns>Return true if cancelled</returns> 
		public static ExecutionState ForAllPermutation<T>(T[] items, Func<T[], ExecutionState> funcExecuteAndTellIfShouldStop)
		{
			int countOfItem = items.Length;

			if (countOfItem <= 1)
			{
				return funcExecuteAndTellIfShouldStop(items);
			}

			var indexes = new int[countOfItem];
			for (int i = 0; i < countOfItem; i++)
			{
				indexes[i] = 0;
			}

			if (funcExecuteAndTellIfShouldStop(items) == ExecutionState.Stop)
			{
#if DEBUG
					while (MessageBox.Show("Retry", "Debug", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
					{
						funcExecuteAndTellIfShouldStop(items);
					}
#endif

				return ExecutionState.Stop;
			}

			for (int i = 1; i < countOfItem;)
			{
				if (indexes[i] < i)
				{ // On the web there is an implementation with a multiplication which should be less efficient.
					if ((i & 1) == 1) // if (i % 2 == 1)  ... more efficient ??? At least the same.
					{
						Swap(ref items[i], ref items[indexes[i]]);
					}
					else
					{
						Swap(ref items[i], ref items[0]);
					}

					if (funcExecuteAndTellIfShouldStop(items) == ExecutionState.Stop)
					{
#if DEBUG
						while (MessageBox.Show("Retry", "Debug", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
							{
								funcExecuteAndTellIfShouldStop(items);
							}
#endif

						return ExecutionState.Stop;
					}

					indexes[i]++;
					i = 1;
				}
				else
				{
					indexes[i++] = 0;
				}
			}

			return ExecutionState.Continue;
		}

		/// <summary>
		/// This function is to show a linq way but is far less efficient
		/// From: StackOverflow user: Pengyang : http://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => new T[] { t });

			return GetPermutations(list, length - 1)
				.SelectMany(t => list.Where(e => !t.Contains(e)),
					(t1, t2) => t1.Concat(new T[] { t2 }));
		}

		/// <summary>
		/// Swap 2 elements of same type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Swap<T>(ref T a, ref T b)
		{
			T temp = a;
			a = b;
			b = temp;
		}

		/// <summary>
		/// Func to show how to call. It does a little test for an array of 4 items.
		/// </summary>
		public static void Test()
		{
			ForAllPermutation("123".ToCharArray(), (vals) =>
			{
				Console.WriteLine(String.Join("", vals));
				return ExecutionState.Continue;
			});

			int[] values = new int[] { 0, 1, 2, 4 };

			Console.WriteLine("Ouellet heap's algorithm implementation");
			ForAllPermutation(values, (vals) =>
			{
				Console.WriteLine(String.Join("", vals));
				return ExecutionState.Continue;
			});

			Console.WriteLine("Linq algorithm");
			foreach (var v in GetPermutations(values, values.Length))
			{
				Console.WriteLine(String.Join("", v));
			}

			// Performance Heap's against Linq version : huge differences
			int count = 0;

			values = new int[10];
			for (int n = 0; n < values.Length; n++)
			{
				values[n] = n;
			}

			Stopwatch stopWatch = new Stopwatch();

			ForAllPermutation(values, (vals) =>
			{
				foreach (var v in vals)
				{
					count++;
				}
				return ExecutionState.Continue;
			});

			stopWatch.Stop();
			Console.WriteLine($"Ouellet heap's algorithm implementation {count} items in {stopWatch.ElapsedMilliseconds} millisecs");

			count = 0;
			stopWatch.Reset();
			stopWatch.Start();

			foreach (var vals in GetPermutations(values, values.Length))
			{
				foreach (var v in vals)
				{
					count++;
				}
			}

			stopWatch.Stop();
			Console.WriteLine($"Linq {count} items in {stopWatch.ElapsedMilliseconds} millisecs");
		}
	}
}
