/* File: throwaway.h
 * Author: Pat Morin
 * Description: Implementation of the throwaway heuristic
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#ifndef __THROWAWAY_H
#define __THROWAWAY_H
#define DllExport   __declspec( dllexport )

#include "point.h"

/* Compute the convex hull of the point set s.  The hull is stored at 
* location s+(return value) sorted in counterclockwise order
*/
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
	// used by C++ source code
#endif
	DllExport int throwaway_heuristic(point *s, int n);
#ifdef __cplusplus
}  // only need to export C interface if
// used by C++ source code
#endif


/* Preprocess the point set s by finding extreme points in 8
 * directions and eliminating points contained in the interior of the
 * convex hull of these 8 extreme points.  The eliminated points are
 * stored at the beginning of s.  The return value is the number of
 * points eliminated.  
 */

#endif /*__THROWAWAY_H*/
