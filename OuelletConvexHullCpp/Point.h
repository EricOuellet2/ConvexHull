/* File: point.h
 * Author: Pat Morin
 * Description: Defines the point structure and some macros
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#ifndef __POINT_H
#define __POINT_H

/* A number type */
typedef double number;

/* A 2-d point type */
typedef struct {
  number x; 
  number y;
} point;

/* Left-turn, right-turn and collinear predicates */
#define area(a, b, c) (((b).x-(a).x)*((c).y-(a).y) \
                             - ((b).y-(a).y)*((c).x-(a).x))
#define right_turn(a, b, c) (area(a, b, c) < 0)
#define left_turn(a, b, c) (area(a, b, c) > 0)
#define collinear(a, b, c) (area(a, b, c) == 0)

/* Macros for lexicographic comparison of two points */
#define sign(x) (((x) < 0) ? -1 : (((x) > 0) ? 1 : 0))
#define cmp(a, b) (((a).x == (b).x) ? sign((a).y-(b).y) : sign((a).x-(b).x))

#define compare_points(a, b) (((a).x == (b).x) && ((a).y == (b).y))

#endif /*__POINT_H*/
