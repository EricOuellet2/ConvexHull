//#pragma once
////include the necessary file
//#include "heaphull.h"
//#include "point.h"
//#include "chanhull.h"
//#include "throwaway.h"
//#include "GeneratePoints.h"
//
//using namespace std;
//
//
//namespace ConvexHullWrapper
//{
//  public ref class ConvexHull
//  {
//  public:
//	  int nbPoints;
//	  point *samplePoints;
//  
//	   ConvexHull()
//	   {
//	   }
//	   ConvexHull(point *points, int quantity)
//	   {
//		    samplePoints = points;
//			nbPoints = quantity;
//	   }
//
//      ConvexHull(int quantity)
//	   {
//		   nbPoints = quantity;
//		   samplePoints = new  point[nbPoints];		
//	   }
//
//
//	  ~ConvexHull()
//		{
//			this->!ConvexHull();
//		}
//
//		!ConvexHull()
//		{
//		}
//		int HeapHull();
//		int ChanHull();
//		int ThrowAway();
//		void GenerateDiskPoints();
//		void GenerateCirclePoints();
//        void GenerateHvlinePoints();
//        void GenerateHlinePoints();
//		void GenerateVlinePoints();
//		void GenerateSquarePoints();
//
//  };
//}