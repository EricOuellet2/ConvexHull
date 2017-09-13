/* File: heaphull.h
* Author: Pat Morin
* Description: Implementation of HeapHull2 algorithm
* Date: Sun Aug 19 16:15:58 EDT 2001
*/
#ifndef __HEAPHULL_H
#define __HEAPHULL_H
#define DllExport   __declspec( dllexport )

#include "point.h"

/* Compute the convex hull of the point set s.  The hull is stored at 
* location s+(return value) sorted in counterclockwise order
*/
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
	// used by C++ source code
#endif

	/* Compute the upper (dir = 1) or lower (dir = -1) hull of the point
	* set s.  The hull is stored in counterclockwise order beginning at
	* s+(return value). 
	*/

	DllExport int heap_upperlower_hull(point *s, int n, int dir);
	
	/* Compute the convex hull of the point set s.  The hull is stored at 
	* location s+(return value) sorted in counterclockwise order
	*/

	DllExport int heaphull2(point *s, int n);

	DllExport int heaphull2WithElapsedTime(point *s, int n, double* elapsedTime);
	
#ifdef __cplusplus
}  // only need to export C interface if
// used by C++ source code
#endif



#endif /*__HEAPHULL_H*/

