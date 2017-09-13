/* File: heaphull.c
 * Author: Pat Morin
 * Description: Implementation of HeapHull2 algorithm
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#include <stdio.h>
#include <math.h>

#include "heaphull.h"

/* Boolean values */
typedef enum { false = 0, true } boolean;

/* Macros for heaps */
#define parent(i) (((i)-1)/2)
#define left(i) (2*((i)+1)-1)
#define right(i) (2*((i)+1))

/* Utility for swapping two values */
#define swap(a, b, c) { c = (a); (a) = (b); (b) = c; }

/* Restore the heap property at the root of s */
#define reheap(s, n, dir) heapify(s, n, 0, dir)

/* Restore the heap property of the subtree of s rooted at node i. The
 * value of dir should be 1 if s is a min-heap and -1 if s is a
 * max-heap.  
 */
static void heapify(point *s, int n, int i, int dir)
{
  int min;
  boolean done;
  point tmp;

  do {
    done = false;
    min = i;
    if (left(i) < n && dir*cmp(s[left(i)], s[min]) < 0) {
      min = left(i);
    }
    if (right(i) < n && dir*cmp(s[right(i)], s[min]) < 0) {
      min = right(i);
    }
    if (min != i) {
      swap(s[min], s[i], tmp);
      i = min;
    } else {
      done = true;
    }
  } while (!done);
}

/* Build a heap of size n on the array s.  The value of dir determines
 * whether this is a max (dir=-1) or min (dir=1) heap. 
 */
static void build_heap(point *s, int n, int dir)
{
  int i;
  
  for (i = n/2; i >= 0; i--) {
    heapify(s, n, i, dir);
  }
}

/* Partitions the set s into two sets, one which contains all
 * candidates for the lower hull and one which contains all candidates
 * for the upper hull.  Returns the index where the second set begins.  
 */
static int partition(point *s, int n)
{
  int i, l = 0, r = 0;
  point a, b, tmp;

  /* find the highest leftmost point and lowest rightmost point */
  for (i = 1; i < n; i++) {
    if (cmp(s[i], s[l]) < 0) {
      l = i;
    }
    if (cmp(s[i], s[r]) > 0) {
      r = i;
    }
  }

  /* partitions the point set */
  a = s[l];
  b = s[r];
  i = 0;
  while (i < n) {
    if (right_turn(a, b, s[i])) {
      i++;
    } else {
      n--;
      swap(s[i], s[n], tmp);
    }
  }
  return n;
}

/* Construct the upper (dir = 1) or lower (dir = -1) hull of s and
 * store it beginning at s+tos and working backwards.  The value h
 * represents the number of points already stored at s+tos.
 */
static int heap_compute_hull(point *s, int n, int tos, int h, int dir)
{
  point tmp;

  build_heap(s, n, dir);
  while (n-- > 0) {
    while (h > 1 && !right_turn(s[tos+1], s[tos], s[0])) {
      tos++;
      h--;
    }
    tos--;
    h++;
    swap(s[0], s[tos], tmp);
    if (tos != n) {
      swap(s[n], s[0], tmp);
    }
    reheap(s, n, dir);
  }
  return tos;
}

/* Compute the convex hull of hte point set s.  The hull is stored at 
 * location s+(return value) sorted in counterclockwise order
 */
int heaphull2(point *s, int n)
{
  int i, j;

  i = partition(s, n);
  j = heap_compute_hull(s+i, n-i, n-i, 0, 1);     /* construct upper hull */
  i = heap_compute_hull(s, i, j+i, 1, -1);        /* construct lower hull */
  /* cleanup lower hull */
  while (i < n-2 && !right_turn(s[i+1], s[i], s[n-1])) {
    i++;
  }
  return i;
}

/* Compute the upper (dir = 1) or lower (dir = -1) hull of the point
 * set s.  The hull is stored in counterclockwise order beginning at
 * s+(return value). 
 */
int heap_upperlower_hull(point *s, int n, int dir)
{
  return heap_compute_hull(s, n, n, 0, dir);
}
