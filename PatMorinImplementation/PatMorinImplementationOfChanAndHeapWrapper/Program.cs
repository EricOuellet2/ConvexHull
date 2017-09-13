//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//// using MultiSim.Geometry;

////using ConvexHullWrapper = ConvexHullWrapper.ConvexHullWrapper;
//using PatMorinImplementationOfChanAndHeap;


//namespace TestConvexHull
//{
//	class Program
//	{
//		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
//		[return: MarshalAs(UnmanagedType.Bool)]
//		static extern bool SetDllDirectory(string lpPathName);

//		static void Main(string[] args)
//		{
//			int nbPoints = 15000000;

//			bool did_it_work;
//			//set the dll path so it can find the dlls
//			did_it_work = SetDllDirectory(@"C:\Users\ch7772\Documents\Visual Studio 2012\Projects\ConvexHull\Release");

//			//create convexHullWrapper
//			ConvexHullWrapper convexHull = new ConvexHullWrapper(nbPoints); 

//			//create an array of points using the generation functions;
//			convexHull.GenerateSquarePoints();
//			Stopwatch timer = new Stopwatch();

//			timer.Start();
//			//convexHull.ThrowAway();
//			//convexHull.NbPointThrown = convexHull.ThrowawayHeuristic();

			
//			convexHull.ChanHull();
//			timer.Stop();

//			TimeSpan ts = timer.Elapsed;
//			int testedPoints = convexHull.NbPoints - convexHull.NbPointThrown;
//			Console.WriteLine("Nb of Tested Points " + testedPoints);
//			Console.WriteLine("Nb of Hull Points " + convexHull.NbPointsHull);
//			// Format and display the TimeSpan value. 
//			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
//				ts.Hours, ts.Minutes, ts.Seconds,
//				ts.Milliseconds / 10);
//			Console.WriteLine("RunTime " + elapsedTime);
//			for (int i =0; i <convexHull.HullPoints.Length; i++)
//				Console.WriteLine(convexHull.HullPoints[i].X+"\t"+convexHull.HullPoints[i].Y);


//		}
//	}
//}
