/* File: chanhull.c
 * Author: Pat Morin
 * Description: Implementation of Chan's second output sensitive convex hull
 *              algorithm
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#include <assert.h>

#include "heaphull.h"
#include "chanhull.h"

#include <omp.h>

/* Boolean values */
//typedef enum { false = 0, true } boolean;

/* Utility for swapping two values */
#define swap(a, b, c) { c = (a); (a) = (b); (b) = c; }


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
		}
		else {
			n--;
			swap(s[i], s[n], tmp);
		}
	}
	return n;
}

/* Compute the projection of c onto the line through a and b */
#define projection(a, b, c) { (c).x = 0; (c).y = 0; }

/* Compute a point on the line orthogonal to the line through a and b
 * that contains the point c */
#define orthogonal(a, b, c, d) { \
   (d).x = ((a).y - (b).y) + (c).x; \
   (d).y = -((a).x - (b).x) + (c).y;  }

/* Compare the slopes between the two lines */
#define slope_cmp(a, b, c, d) (-1)

/* Place the point p at location i in s, if necessary, add two elements
 * to the stack
 */
static int place(point p, point *s, int i, int *r, int *eof,
	point *stack, int m)
{
	int j;

	for (j = 0; j < 3; j++) {
		assert(r[j] % 2 == 0);
		if (i == r[j] && r[j] < eof[j]) {
			stack[2 * m] = s[r[j]++];
			stack[2 * m + 1] = s[r[j]++];
			m++;
		}
	}
	s[i] = p;
	return m;
}

#ifdef EXACT_SELECTION
/* a pair of points */
typedef struct {
	point a, b;
} point_pair;

#define slope_cmp(x, y) sign(area(x.a, x.b, x.

static int select_slope(point_pair *s, int n, int r)
{
	int i, j, ret;
	double dx, dy;
	point tmp;
	point_pair pivot;

	i = rand() % n;
	pivot = s[i];
	for (i = 0, j = n; i < n; i++) {
		dx = s[i].a.x - pivot.a.x;
		dy = s[i].a.y - pivot.a.y;
		tmp.x = s[i].b.x - dx;
		tmp.y = s[i].b.y - dy;
		ret = sign(area(pivot.a, pivot.b, tmp));
	}
	return 0;
}
#endif /*EXACT_SELECTION*/

/* Compute the upper hull of the point set s.
 */
static int chan_compute_hull(point *s, int n, int dir)
{
	point a, b, c, max, tmp;
	double dx, dy, ar;
	int maxi = 0, m, ri, g, i, j, k, l, im, jm, km, ret, p1, p2,
		x, y, z, r[3], eof[3];
	point stack[50];


	/* for small cases, use heaphull algorithm */
	if (n < 10) {
		return heap_upperlower_hull(s, n, dir);
	}

	/* arrange points into pairs with their left endpoint first */
	for (i = 0; i < n / 2; i++) {
		if (dir*cmp(s[2 * i], s[2 * i + 1]) > 0) {
			swap(s[2 * i], s[2 * i + 1], tmp);
		}
	}

	/* choose a random slope and find the extreme point in the direction
	 * orthogonal to this slope */
	ri = (rand() % (n / 2)) * 2;
	dx = s[ri + 1].x - s[ri].x;
	dy = s[ri + 1].y - s[ri].y;
	maxi = 0;
	c.x = s[maxi].x + dx;
	c.y = s[maxi].y + dy;
	for (i = 1; i < n; i++) {
		ar = area(s[maxi], c, s[i]);
		if (ar > 0 || (ar == 0 && dir * cmp(s[i], s[maxi]) >= 0)) {
			maxi = i;
			c.x = s[maxi].x + dx;
			c.y = s[maxi].y + dy;
		}
	}
	max = s[maxi];

	/* compute subproblem sizes */
	p1 = p2 = 0;
	/* check left endpoints of pairs */
	for (i = 0; i < n; i += 2) {
		ret = dir * cmp(s[i], s[maxi]);
		if (ret == 0) {
			p1++;
		}
		else if (ret < 0) {
			p2++;
		}
		else if (i == n - 1 || left_turn(s[maxi], s[i + 1], s[i])) {
			p1++;
		}
	}
	/* check right endpoints of pairs */
	for (i = 1; i < n; i += 2) {
		ret = dir * cmp(s[i], s[maxi]);
		if (ret >= 0) {
			p1++;
		}
		else if (left_turn(s[i - 1], s[maxi], s[i])) {
			p2++;
		}
	}
	/* sanity check */
	assert(p1 != 0);

	/* Start shuffling into three subproblems */
	if (n % 2 == 1) {
		tmp = s[n - 1];     /* save this for later */
	}
	i = 0;
	r[0] = i;
	j = n - (p1 + p2);
	im = j;
	x = j / 2 * 2;
	r[1] = x;
	eof[0] = x;
	k = (n - p2);
	jm = k;
	y = k / 2 * 2;
	r[2] = y;
	eof[1] = y;
	z = n / 2 * 2;
	km = n;
	eof[2] = z;

	p1 = 0;
	p2 = 0;
	m = 0;
	/* set up the stack */
	for (l = 0; l < 3; l++) {
		assert(r[l] % 2 == 0);
		if (r[l] != eof[l]) {
			stack[2 * m] = s[r[l]++];
			stack[2 * m + 1] = s[r[l]++];
			assert(dir * cmp(stack[2 * m], stack[2 * m + 1]) <= 0);
			m++;
		}
	}
	while (m > 0 || r[0] != eof[0] || r[1] != eof[1] || r[2] != eof[2]) {
		if (m == 0) {
			/* stack has become empty, read from one of our streams */
			for (l = 0; l < 3; l++) {
				assert(r[l] % 2 == 0);
				if (r[l] != eof[l]) {
					stack[2 * m] = s[r[l]++];
					stack[2 * m + 1] = s[r[l]++];
					assert(dir * cmp(stack[2 * m], stack[2 * m + 1]) <= 0);
					break;
				}
			}
			m++;
		}
		/* pop a pair off the stack */
		m--;
		a = stack[2 * m];
		b = stack[2 * m + 1];
		assert(dir * cmp(a, b) <= 0);
		/* place left endpoint */
		ret = dir * cmp(a, max);
		if (ret == 0) {
			assert(j < jm);
			m = place(a, s, j, r, eof, stack, m);
			j++;
			p1++;
		}
		else if (ret < 0) {
			assert(k < km);
			m = place(a, s, k, r, eof, stack, m);
			k++;
			p2++;
		}
		else if (left_turn(max, b, a)) {
			assert(j < jm);
			m = place(a, s, j, r, eof, stack, m);
			j++;
			p1++;
		}
		else {
			assert(i < im);
			m = place(a, s, i, r, eof, stack, m);
			i++;
		}
		/* place right endpoint */
		ret = dir * cmp(b, max);
		if (ret >= 0) {
			assert(j < jm);
			m = place(b, s, j, r, eof, stack, m);
			j++;
			p1++;
		}
		else if (left_turn(a, max, b)) {
			assert(k < km);
			m = place(b, s, k, r, eof, stack, m);
			k++;
			p2++;
		}
		else {
			assert(i < im);
			m = place(b, s, i, r, eof, stack, m);
			i++;
		}
		assert(m < 4);
	}

	/* handle unmatched point */
	if (n % 2 == 1) {
		if (dir * cmp(tmp, max) < 0) {
			assert(k < km);
			s[k++] = tmp;
			p2++;
		}
		else {
			assert(j < jm);
			s[j++] = tmp;
			p1++;
		}
	}

#ifdef DEBUG
	/* sanity check */
	assert(i == im);
	assert(j == jm);
	assert(k == km);
	for (l = i; l < j; l++) {
		assert(dir * cmp(s[l], max) >= 0);
	}
	for (l = j; l < k; l++) {
		assert(dir * cmp(s[l], max) < 0);
	}
#endif /*DEBUG*/

	/* recurse */
	x = chan_compute_hull(s + i, j - i, dir);
	m = j - i - x;
	m -= 1;
	x += i;

#ifdef DEBUG
	/* verify result */
	assert(cmp(s[x+m], max) == 0);
	for (l = 0; l < m-1; l++) {
		assert(dir * cmp(s[x+l], s[x+l+1]) >= 0);
	}
#endif /*DEBUG*/

	y = chan_compute_hull(s + j - 1, k - j + 1, dir);
	g = y;
	y += j - 1;

#ifdef DEBUG
	/* verify result */
	assert(cmp(s[y], max) == 0);
	for (l = y; l < n-1; l++) {
		assert(dir * cmp(s[l], s[l+1]) >= 0);
	}
#endif /*DEBUG*/

	while (m > 0) {
		swap(s[x + m - 1], s[x + m + g - 1], tmp);
		m--;
	}

	return x + g;
}

/* Compute the convex hull of the point set s.  The hull is stored at
 * location s+(return value) sorted in counterclockwise order
 */
int chanhull(point *s, int n)
{
	point tmp;
	int i, j, k, g;

	i = partition(s, n);

	k = chan_compute_hull(s + i, n - i, 1);
	k += i;

	/* bring leftmost endpoint to location k */
	for (j = n - 1; j > k; j--) {
		swap(s[j], s[j - 1], tmp);
	}

	/* move discarded points to front of array */
	g = k - i;
	for (j = i - 1; j > 0; j--) {
		swap(s[j], s[j + g], tmp);
	}

	j = chan_compute_hull(s + g, i + 2, -1);
	j += g;

	return j;
}


/* Compute the convex hull of the point set s.  The hull is stored at
* location s+(return value) sorted in counterclockwise order
*/
int chanhullWithElapsedTime(point *s, int n, double* elapsedTime)
{
	double startTime = omp_get_wtime();

	int count = chanhull(s, n);

	*elapsedTime = omp_get_wtime() - startTime;

	return count;
}
