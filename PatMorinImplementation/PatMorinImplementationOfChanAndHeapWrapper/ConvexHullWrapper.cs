// using HQ.Mathematic.Geometry;
using System.Runtime.InteropServices;
using System.Windows;

namespace PatMorinImplementationOfChanAndHeapWrapper
{
	//Managed Structure to the point structure
	//[StructLayout(LayoutKind.Sequential)]
	//public struct Point
	//{
	//	public double X;
	//	public double Y;
	//}

	//internal class containing any unmanaged methods
	internal sealed class NativeConvexHullApi
	{
		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int heaphull2([In, Out] Point[] s, int n);

		[DllImport("PatMorinImplementationOfChanAndHeap.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chanhull([In, Out] Point[] s, int n);

	}

	public class ConvexHullWrapper
	{
		//constructors
		public ConvexHullWrapper(int quantity)
		{
			NbPoints = quantity;
			SamplePoints = new Point[NbPoints];
		}
		//-----------------------------------------------------------------------
		public ConvexHullWrapper(Point[] points)
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

			int indexHull = NativeConvexHullApi.heaphull2(rawPoints, nb);
			//copy the data to the Hull Array
			NbPointsHull = nb - indexHull;
			HullPoints = new Point[NbPointsHull];
			System.Array.Copy(rawPoints, indexHull, HullPoints, 0, NbPointsHull);
		}

		//-----------------------------------------------------------------------
		public void ChanHull()
		{
			int nb = NbPoints - NbPointThrown;
			//Point[] rawPoints = new Point[nb];
			//System.Array.Copy(SamplePoints, NbPointThrown, rawPoints, 0, nb);

			int indexHull = NativeConvexHullApi.chanhull(SamplePoints, nb);
			//copy the data to the Hull Array
			NbPointsHull = nb - indexHull;
			HullPoints = new Point[NbPointsHull];
			System.Array.Copy(SamplePoints, indexHull, HullPoints, 0, NbPointsHull);
		}

		//-----------------------------------------------------------------------
	}
}
