#pragma once

#include <math.h>
#include "Point.h"

using namespace System::Windows;

public ref class OuelletConvexHullCpp
{
public:
	array<Point>^ OuelletHullManaged(array<Point>^ points, bool closeThePath);
	array<Point>^ OuelletHullManagedWithElapsedTime(array<Point>^ points, bool closeThePath, double% elapsedTimeInSec);
};

class OuelletHull
{
private:
	static const int _quadrantHullPointArrayInitialCapacity = 1000;
	static const int _quadrantHullPointArrayGrowSize = 1000;

	point* _pPoints;
	int _countOfPoint;
	bool _shouldCloseTheGraph;

	point* q1pHullPoints;
	point* q1pHullLast;
	int q1hullCapacity;
	int q1hullCount = 0;

	point* q2pHullPoints;
	point* q2pHullLast;
	int q2hullCapacity;
	int q2hullCount = 0;

	point* q3pHullPoints;
	point* q3pHullLast;
	int q3hullCapacity;
	int q3hullCount = 0;

	point* q4pHullPoints;
	point* q4pHullLast;
	int q4hullCapacity;
	int q4hullCount = 0;

	void CalcConvexHull();

	inline static void InsertPoint(point*& pPoint, int index, point& pt, int& count, int& capacity);
	inline static void RemoveRange(point* pPoint, int indexStart, int indexEnd, int &count);

public:
	OuelletHull(point* points, int countOfPoint, bool shouldCloseTheGraph = true);
	~OuelletHull();
	point* GetResultAsArray(int& count);
};

int ouelletHullForTimeCheckOnly(point* pArrayOfPoint, int count);

extern "C" 
{
	point* ouelletHull(point* pArrayOfPoint, int count, bool closeThePath, int& resultCount);
//	array<ManagedPoint>^ ouelletHullManaged(point* pArrayOfPoint, int count);
}

