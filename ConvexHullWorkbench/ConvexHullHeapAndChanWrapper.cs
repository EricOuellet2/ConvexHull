using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace ConvexHullWorkbench
{
	//Managed Structure to the point structure
	[StructLayout(LayoutKind.Sequential)]

	//internal class containing any unmanaged methods
	internal sealed class NativeConvexHullApi
	{
		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern int heaphull2([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern int chanhull([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern int throwaway_heuristic([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_disk_points([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_circle_points([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_hvline_points([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_hline_points([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_vline_points([In, Out] Point[] s, int n);

		//[DllImport("ConvexHullPatMorin.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void generate_square_points([In, Out] Point[] s, int n);
		

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int heaphull2([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int heaphull2WithElapsedTime([In, Out] Point[] s, int n, ref double elapsedTime);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chanhull([In, Out] Point[] s, bool closeThePath, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chanhullWithElapsedTime([In, Out] Point[] s, int n, ref double elapsedTime);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int throwaway_heuristic([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_disk_points([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_circle_points([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_hvline_points([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_hline_points([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_vline_points([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void generate_square_points([In, Out] Point[] s, int n);
		
	}

	public class ConvexHullHeapAndChanWrapper
	{
		//constructors
		public ConvexHullHeapAndChanWrapper(int quantity)
		{
			NbPoints = quantity;
			SamplePoints = new Point[NbPoints];
		}
		//-----------------------------------------------------------------------
		public ConvexHullHeapAndChanWrapper(Point[] points)
		{
			SamplePoints = points;
			NbPoints = points.Length;
		}

		//properties
		//------------------------------------------------------------------------
		public Point[] SamplePoints { get; set; }

		//------------------------------------------------------------------------
		public Point[] HullPoints { get; private set; }

		//-----------------------------------------------------------------------
		public int NbPoints { get; set; }

		//----------------------------------------------------------------------
		public int NbPointsHull { get; private set; }

		//---------------------------------------------------------------------
		public int NbPointThrown { get; set; }


		//------------------------------------------------------------------------
		//Implements the service methods calling the 
		public void HeapHull()
		{
			int nb = NbPoints - NbPointThrown;
			Point[] rawPoints = new Point[nb];
			System.Array.Copy(SamplePoints, NbPointThrown, rawPoints, 0, nb);

			double elapsedTime = 0;

			// ELapsed time come from c function: "double omp_get_wtime( );" which return a double that represent the amount of seconds.
			int indexHull = NativeConvexHullApi.heaphull2WithElapsedTime(rawPoints, nb, ref elapsedTime);

			TimeSpan = TimeSpanHelper.MoreAccurateTimeSpanFromSeconds(elapsedTime);

			//copy the data to the Hull Array
			NbPointsHull = nb - indexHull;
			HullPoints = new Point[NbPointsHull];
			System.Array.Copy(rawPoints, indexHull, HullPoints, 0, NbPointsHull);
		}

		//-----------------------------------------------------------------------
		//public void ChanHull()
		//{
		//	int nb = NbPoints - NbPointThrown;
		//	//Point[] rawPoints = new Point[nb];
		//	//System.Array.Copy(SamplePoints, NbPointThrown, rawPoints, 0, nb);

		//	int indexHull = NativeConvexHullApi.chanhull(SamplePoints, nb);
		//	//copy the data to the Hull Array
		//	NbPointsHull = nb - indexHull;
		//	HullPoints = new Point[NbPointsHull];
		//	System.Array.Copy(SamplePoints, indexHull, HullPoints, 0, NbPointsHull);
		//}

		public TimeSpan TimeSpan { get; private set; }
		
		//-----------------------------------------------------------------------
		public void ChanHull()
		{
			int nb = NbPoints - NbPointThrown;
			//Point[] rawPoints = new Point[nb];
			//System.Array.Copy(SamplePoints, NbPointThrown, rawPoints, 0, nb);

			double elapsedTime = 0;

			if (SamplePoints == null || SamplePoints.Length == 0)
			{
				NbPointsHull = 0;
				HullPoints = new Point[0];
				return;
			}

			int indexHull = NativeConvexHullApi.chanhullWithElapsedTime(SamplePoints, nb, ref elapsedTime);

			TimeSpan = TimeSpanHelper.MoreAccurateTimeSpanFromSeconds(elapsedTime);

			//copy the data to the Hull Array
			NbPointsHull = nb - indexHull;
			HullPoints = new Point[NbPointsHull];
			System.Array.Copy(SamplePoints, indexHull, HullPoints, 0, NbPointsHull);
		}

		//-----------------------------------------------------------------------
		public void ThrowAway()
		{
			NbPointThrown = NativeConvexHullApi.throwaway_heuristic(SamplePoints, NbPoints);
		}

		//-----------------------------------------------------------------------
		public void GenerateDiskPoints()
		{
			NativeConvexHullApi.generate_disk_points(SamplePoints, NbPoints);
		}

		//-----------------------------------------------------------------------
		public void GenerateCirclPoints()
		{
			NativeConvexHullApi.generate_circle_points(SamplePoints, NbPoints);
		}

		//-----------------------------------------------------------------------
		public void GenerateHlinePoints()
		{
			NativeConvexHullApi.generate_hline_points(SamplePoints, NbPoints);
		}

		//-----------------------------------------------------------------------
		public void GenerateHVlinePoints()
		{
			NativeConvexHullApi.generate_hvline_points(SamplePoints, NbPoints);
		}

		//-----------------------------------------------------------------------
		public void GenerateSquarePoints()
		{
			NativeConvexHullApi.generate_square_points(SamplePoints, NbPoints);
		}
		//-----------------------------------------------------------------------
		public void GenerateVline()
		{
			NativeConvexHullApi.generate_vline_points(SamplePoints, NbPoints);
		}

		//Tentative to translate the C code into C# 
		//-----------------------------------------------------------------------
		public int ThrowawayHeuristic()
		{
			Point[] s = SamplePoints;
			int n = NbPoints;
			int i;
			int j;
			int elim = 0;
			int k;
			int[] maxi = new int[8];
			double proj;
			double[] maxs = new double[8];
			double[,] signs = new double[,] { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
			Point[] hull8 = new Point[8];
			Point tmp = new Point();

			for (i = 0; i < 8; i++)
			{
				maxs[i] = double.MinValue;
			}

			/* find extreme elements in 8 directions */
			for (i = 0; i < SamplePoints.Length; i++)
			{
				for (j = 0; j < hull8.Length; j++)
				{
					proj = signs[j, 0] * s[i].X + signs[j, 1] * s[i].Y;
					if (proj > maxs[j])
					{
						maxs[j] = proj;
						maxi[j] = i;
					}
				}
			}
			hull8[0] = s[maxi[0]];
			for (k = 1, j = 1; j < hull8.Length; j++)
			{
				if (Comp(ref s[maxi[j]], ref hull8[k - 1]) != 0)
				{
					hull8[k++] = s[maxi[j]];
				}
			}
			if (k > 1 && Comp(ref hull8[0], ref hull8[k - 1]) == 0)
			{
				k--;
			}
			i = 0;
			while (i < n)
			{
				for (j = 0; j < k; j++)
				{
					if (RightTurn(ref hull8[j], ref hull8[(j + 1) % 8], ref s[i]) || Comp(ref hull8[j], ref s[i]) == 0)
					{
						break;
					}
				}
				if (j == k)
				{
					/* eliminate this point */
					i++;
					elim++;
				}
				else
				{
					/* keep this point */
					n--;
					Swap(ref s[n], ref s[i], ref tmp);
				}
			}
			return n;

		}
		
		//Static methods and services on Points
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static double Area(ref Point a, ref Point b, ref Point c)
		{
			return (((b).X - (a).X) * ((c).Y - (a).Y) - ((b).Y - (a).Y) * ((c).X - (a).X));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RightTurn(ref Point a, ref Point b, ref Point c)
		{
			return (Area(ref a, ref b, ref c) < 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LeftTurn(ref Point a, ref Point b, ref Point c)
		{
			return (Area(ref a, ref b, ref c) > 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Collinear(ref Point a, ref Point b, ref Point c)
		{
			return (Math.Abs(Area(ref a, ref b, ref c) - 0) < double.Epsilon);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(double x)
		{
			return (((x) < 0) ? -1 : (((x) > 0) ? 1 : 0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Comp(ref Point a, ref Point b)
		{

			//return ((Math.Abs((a).X - (b).X) < double.Epsilon) ? Sign((a).Y - (b).Y) : Sign((a).X - (b).X));
			return (((a).X == (b).X) ? Sign((a).Y - (b).Y) : Sign((a).X - (b).X));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap(ref Point a, ref Point b, ref Point c)
		{
			c = a;
			a = b;
			b = c;
		}

	}
}
