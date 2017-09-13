/* File: chanhull.h
 * Author: Pat Morin
 * Description: Implementation of Chan's output-sensitive convex 
 *              hull algorithm
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#ifndef __CHANHULL_H
#define __CHANHULL_H

#include "point.h"

/* Compute the convex hull of the point set s.  The hull is stored at 
 * location s+(return value) sorted in counterclockwise order
 */
int chanhull(point *s, int n);

#endif /*__CHANHULL_H*/

