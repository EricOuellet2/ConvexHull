/* File: heaphull.h
* Author: Pat Morin
* Description: Implementation of HeapHull2 algorithm
* Date: Sun Aug 19 16:15:58 EDT 2001
*/
#ifndef __GENERATEPOINTS_H
#define __GENERATEPOINTS_H
#define DllExport   __declspec( dllexport )

#include "point.h"
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
	// used by C++ source code
#endif

	/* Generate i.u.d. points in the unit disk
	*/
	DllExport void generate_disk_points(point *s, int n);

	/* Generate i.u.d. points in the unit disk
	*/
	DllExport void generate_circle_points(point *s, int n);

	/* Generate points on the line (0,0), (1,1)
	*/
	DllExport void generate_hvline_points(point *s, int n);

	/* Generate points on the line (0,0), (1,1)
	*/
	DllExport void generate_hline_points(point *s, int n);

	/* Generate points on the line (0,0), (1,1)
	*/
	DllExport void generate_vline_points(point *s, int n);
	/* Generate i.u.d. points in the x times y rectangle 
	*/
	DllExport void generate_square_points(point *s, int n);
#ifdef __cplusplus
}
#endif

#endif /*__GENERATEPOINTS_H*/

