#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <math.h>
#include "point.h"
#include "GeneratePoints.h"

#define _USE_MATH_DEFINES // for C++
#include <cmath>


/* Generate i.u.d. points in the unit disk
*/
void generate_disk_points(point *s, int n)
{
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		do {
			s[i].x = (((double)rand() / RAND_MAX) - 0.5) * 2.0;
			s[i].y = (((double)rand() / RAND_MAX) - 0.5) * 2.0;
		} while ((s[i].x * s[i].x) + (s[i].y * s[i].y) > 1.0);
	}
}

/* Generate i.u.d. points in the unit disk
*/
void generate_circle_points(point *s, int n)
{
	double theta;
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		theta = ((double)rand() / RAND_MAX) * 2 * M_PI;
		s[i].x = cos(theta);
		s[i].y = sin(theta);
	}
}

/* Generate points on the line (0,0), (1,1)
*/
  void generate_hvline_points(point *s, int n)
{
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		s[i].x = (double)rand() / RAND_MAX;
		s[i].y = s[i].x;
	}
}

/* Generate points on the line (0,0), (1,1)
*/
  void generate_hline_points(point *s, int n)
{
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		s[i].x = (double)rand() / RAND_MAX;
		s[i].y = 0;
	}
}

/* Generate points on the line (0,0), (1,1)
*/
  void generate_vline_points(point *s, int n)
{
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		s[i].x = 0;
		s[i].y = (double)rand() / RAND_MAX;
	}
}

/* Generate i.u.d. points in the x times y rectangle 
*/
  void generate_square_points(point *s, int n)
{
	int i;

	srand((unsigned int)time(NULL));
	for (i = 0; i < n; i++) {
		s[i].x = (double)rand() / RAND_MAX;
		s[i].y = (double)rand() / RAND_MAX;
	}
}


