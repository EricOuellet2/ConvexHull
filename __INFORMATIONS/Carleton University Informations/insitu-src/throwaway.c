/* File: throwaway.c
 * Author: Pat Morin
 * Description: Implementation of Throwaway2 algorithm
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#include <stdio.h>
#include <math.h>

#include "throwaway.h"

/* Utility for swapping two values */
#define swap(a, b, c) { c = (a); (a) = (b); (b) = c; }


/* Preprocess the point set s by finding extreme points in 8
 * directions and eliminating points contained in the interior of the
 * convex hull of these 8 extreme points.  The eliminated points are
 * stored at the beginning of s.  The return value is the number of
 * points eliminated.  
 */
int throwaway_heuristic(point *s, int n)
{
  int i, j, elim = 0, k;
  int maxi[8];
  double proj, maxs[8];
  double signs[8][2] = { {1, 0}, {1, 1}, {0, 1}, {-1, 1}, {-1, 0},
			 {-1, -1}, {0, -1}, {1, -1} };
  point hull8[8], tmp;

  for (i = 0; i < 8; i++) {
    maxs[i] = -HUGE_VAL;
  }

  /* find extreme elements in 8 directions */
  for (i = 0; i < n; i++) {
    for (j = 0; j < 8; j++) {
      proj = signs[j][0] * s[i].x + signs[j][1] * s[i].y;
      if (proj > maxs[j]) {
	maxs[j] = proj;
	maxi[j] = i;
      }
    }
  }
  hull8[0] = s[maxi[0]];
  for (k = 1, j = 1; j < 8; j++) {
    if (cmp(s[maxi[j]], hull8[k-1]) != 0) {
      hull8[k++] = s[maxi[j]];
    } 
  }
  if (k > 1 && cmp(hull8[0], hull8[k-1]) == 0) {
    k--;
  }
  i = 0;
  while (i < n) {    
    for (j = 0; j < k; j++) {
      if (right_turn(hull8[j], hull8[(j+1)%8], s[i])
	  || cmp(hull8[j], s[i]) == 0) {
	break;
      }
    }
    if (j == k) {
      /* eliminate this point */
      i++;
      elim++;
    } else {
      /* keep this point */
      n--;
      swap(s[n], s[i], tmp);
    }
  }
  return n;
  return 0;
}


