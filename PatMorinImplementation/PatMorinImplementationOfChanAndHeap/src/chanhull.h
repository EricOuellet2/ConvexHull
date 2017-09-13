/* File: chanhull.h
* Author: Pat Morin
* Description: Implementation of Chan's output-sensitive convex 
*              hull algorithm
* Date: Sun Aug 19 16:15:58 EDT 2001
*/
#ifndef __CHANHULL_H
#define __CHANHULL_H
#define DllExport   __declspec( dllexport )

#include "point.h"

/* Compute the convex hull of the point set s.  The hull is stored at 
* location s+(return value) sorted in counterclockwise order
*/
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
	// used by C++ source code
#endif
	
	DllExport int chanhull(point *s, int n);
	DllExport int chanhullWithElapsedTime(point *s, int n, double* elapsedTime);

#ifdef __cplusplus
}  // only need to export C interface if
// used by C++ source code
#endif
#endif /*__CHANHULL_H*/

