/* File: test1.c
 * Author: Pat Morin
 * Description: A test of the heaphull implementation
 * Date: Sun Aug 19 16:15:58 EDT 2001
 */
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <math.h>

#include "throwaway.h"
#include "heaphull.h"
#include "chanhull.h"

/* Different types of point distributions */
typedef enum { CIRCLE, DISK, SQUARE, HLINE, VLINE, 
	       HVLINE, STDIN } distribution;

/* Various convex hull algorithms */
typedef enum { HEAPHULL, CHANHULL, QHULL, NOHULL } algorithm;
char *alg_names[] = { "HeapHull", "ChanHull", "QHull", "NoHull" };

/* Boolean variables */
typedef enum { false = 0, true } boolean;

/* Global variables corresponding to command line arguments
 */ 
static algorithm alg = HEAPHULL;     /* algorithm to use */
static int n = 1000;                 /* number of points */
static distribution d;               /* type of point distribution */
static boolean verify = false;       /* verify hull after computation? */
static boolean throwaway = false;    /* use throwaway heuristic? */
static char *infile = NULL;          /* file to read input from */
static char *outfile = NULL;         /* file to output hull to */
static char *outifile = NULL;        /* file to output input to */

/* Read points from stdin
 */
static int generate_stdin_points(FILE *fp, point **s, int nalloc)
{
  int n = 0;
  double x, y;
  point *tmp;
  char line[1000];

  while (fgets(line, sizeof(line), fp) != NULL) {
    line[sizeof(line)-1] = '\0';
    if (sscanf(line, "%lf %lf", &x, &y) == 2) {
      if (*s == NULL || n == nalloc - 1) {
	nalloc *= 2;
	if ((tmp = malloc(nalloc*sizeof(point))) == NULL) {
	  fprintf(stderr, "Not enough memory\n");
	  exit(-1);
	}
	memcpy(tmp, *s, n*sizeof(point));
	free(*s);
	*s = tmp;
      }
      (*s)[n].x = x;
      (*s)[n].y = y;
      n++;
    }
  }
  return n;
}

/* Generate i.u.d. points in the unit disk
 */
static void generate_disk_points(point *s, int n)
{
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    do {
      s[i].x = (((double)rand() / RAND_MAX) - 0.5) * 2.0;
      s[i].y = (((double)rand() / RAND_MAX) - 0.5) * 2.0;
    } while ((s[i].x * s[i].x) + (s[i].y * s[i].y) > 1.0);
  }
}

/* Generate i.u.d. points in the unit disk
 */
static void generate_circle_points(point *s, int n)
{
  double theta;
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    theta = ((double)rand() / RAND_MAX) * 2 * M_PI;
    s[i].x = cos(theta);
    s[i].y = sin(theta);
  }
}

/* Generate points on the line (0,0), (1,1)
 */
static void generate_hvline_points(point *s, int n)
{
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    s[i].x = (double)rand() / RAND_MAX;
    s[i].y = s[i].x;
  }
}

/* Generate points on the line (0,0), (1,1)
 */
static void generate_hline_points(point *s, int n)
{
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    s[i].x = (double)rand() / RAND_MAX;
    s[i].y = 0;
  }
}

/* Generate points on the line (0,0), (1,1)
 */
static void generate_vline_points(point *s, int n)
{
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    s[i].x = 0;
    s[i].y = (double)rand() / RAND_MAX;
  }
}

/* Generate i.u.d. points in the x times y rectangle 
 */
static void generate_square_points(point *s, int n)
{
  int i;

  srand(time(NULL));
  for (i = 0; i < n; i++) {
    s[i].x = (double)rand() / RAND_MAX;
    s[i].y = (double)rand() / RAND_MAX;
  }
}

/* Verify that the give convex hull h is convex and that it contains all
 * the points in s.  This is not a very efficient algorithm.
 */
static void verify_hull(point *s, int n, int i)
{
  int j, nh;
  point *h;

  h = s + i;
  nh = n - i;
  if (nh < 2) {
    fprintf(stderr, "verify_hull : can't handle degenerate hulls!\n");
    exit(-1);
  }
  for (i = 0; i < nh; i++) {
    if (!left_turn(h[i], h[(i+1)%nh], h[(i+2)%nh])) {
      fprintf(stderr, "verify_hull : hull is not convex\n");
    } 
  }
  for (j = 0; j < n; j++) {
    for (i = 0; i < nh; i++) {
      if (right_turn(h[i], h[(i+1)%nh], s[j])) {
	fprintf(stderr, "verify_hull : hull does not contain all points"
		" (or it spirals)\n");
      }
    } 
  }  
}

/* Print a usage message
 */
static void usage(char *prog)
{
  fprintf(stderr, "Usage : %s [<args>]\n"
	  " where <args> is any comination of the following:\n"
	  " -n <num>    : generate <num> points\n"
	  " -s          : generate points in the unit square\n"
	  " -d          : generate points in the unit disk\n"
	  " -c          : generate points on the unit circle\n"
	  " -lh         : generate points on horizontal line\n"
	  " -lv         : generate points on vertical line\n"
	  " -lhv        : generate points on 45 degree line\n"
	  " -i <file>   : read points from <file>\n"
	  " -v          : verify correctness (slow)\n"
	  " -throwaway  : preprocess data with throwaway heuristic\n"
          " -heaphull   : use HeapHull2 algorithm (default)\n"
	  " -chanhull   : use Chan's algorithm\n"
	  " -qhull      : use Quickhull algorihtm\n"
	  " -nohull     : don't compute the convex hull\n"
	  " -o <file>   : output hull to <file>\n"
	  " -x <file>   : output input-points to <file>\n"
	  " -h          : print this message and exit\n", prog);
}


/* Read command line arguments
 */
static void read_command_line(int argc, char *argv[])
{
  int i;

  for (i = 1; i < argc; i++) {
    if (strcmp(argv[i], "-s") == 0) {
      /* points are distributed in unit square */
      d = SQUARE;
    } else if (strcmp(argv[i], "-c") == 0) {
      /* points are distributed in the unit circle */
      d = CIRCLE;
    } else if (strcmp(argv[i], "-d") == 0) {
      /* generate points in the unit disk */
      d = DISK;
    } else if (strcmp(argv[i], "-lh") == 0) {
      /* generate points in the unit disk */
      d = HLINE;
    } else if (strcmp(argv[i], "-lv") == 0) {
      /* generate points in the unit disk */
      d = VLINE;
    } else if (strcmp(argv[i], "-lhv") == 0) {
      /* generate points in the unit disk */
      d = HVLINE; 
    } else if (strcmp(argv[i], "-i") == 0) {
      if (i >= argc - 1) {
	usage(argv[0]);
	exit(-1);
      }
      infile = argv[++i];
      /* generate points in the unit disk */
      d = STDIN;
    } else if (strcmp(argv[i], "-v") == 0) {
      /* generate points in the unit disk */
      verify = true;
    } else if (strcmp(argv[i], "-n") == 0) {
      /* specify number of points */
      if (i >= argc - 1) {
	usage(argv[0]);
	exit(-1);
      }
      n = atoi(argv[++i]);
    } else if (strcmp(argv[i], "-h") == 0) {
      /* show help screen */
      usage(argv[0]);
      exit(0);
    } else if (strcmp(argv[i], "-heaphull") == 0) {
      alg = HEAPHULL;
    } else if (strcmp(argv[i], "-chanhull") == 0) {
      alg = CHANHULL;
    } else if (strcmp(argv[i], "-qhull") == 0) {
      alg = QHULL;
    } else if (strcmp(argv[i], "-nohull") == 0) {
      alg = NOHULL;
    } else if (strcmp(argv[i], "-throwaway") == 0) {
      throwaway = true;
    } else if (strcmp(argv[i], "-o") == 0) {
      if (i >= argc - 1) {
	usage(argv[0]);
	exit(-1);
      }
      outfile = argv[++i];
    } else if (strcmp(argv[i], "-x") == 0) {
      if (i >= argc - 1) {
	usage(argv[0]);
	exit(-1);
      }
      outifile = argv[++i];
    } else {
      usage(argv[0]);
      exit(-1);
    }
  }
}

/* Program entry point
 */
int main(int argc, char *argv[])
{
  int i, j, k;
  point *s;
  clock_t stop, start;
  FILE *fp;

  read_command_line(argc, argv);

  /* allocate memory */
  if ((s = malloc(n*sizeof(point))) == NULL) {
    fprintf(stderr, "Not enough memory\n");
    exit(-1);
  }

  /* generate points according to specified distribution */
  if (d == DISK) {
    printf("Generating %d points in the unit disk...", n);
    fflush(stdout);
    generate_disk_points(s, n);
    printf("done.\n");
  } else if (d == SQUARE) {
    printf("Generating %d points in the unit square...", n);
    fflush(stdout);
    generate_square_points(s, n);
    printf("done.\n");
  } else if (d == CIRCLE) {
    printf("Generating %d points on the unit circle...", n);
    fflush(stdout);
    generate_circle_points(s, n);
    printf("done.\n");
  } else if (d == HLINE) {
    printf("Generating %d points on horizontal line...", n);
    fflush(stdout);
    generate_hline_points(s, n);
    printf("done.\n");
  } else if (d == VLINE) {
    printf("Generating %d points on vertical line...", n);
    fflush(stdout);
    generate_vline_points(s, n);
    printf("done.\n");
  } else if (d == HVLINE) {
    printf("Generating %d points on 45 degree line...", n);
    fflush(stdout);
    generate_hvline_points(s, n);
    printf("done.\n");
  } else if (d == STDIN) {
    if (strcmp(infile, "-") == 0) {
      printf("Reading points from stdin");
      fflush(stdout);
      n = generate_stdin_points(stdin, &s, n);
    } else {
      if ((fp = fopen(infile, "rt")) == NULL) {
	fprintf(stderr, "Unable to open input file %s\n", infile);
	exit(-1);
      }
      printf("Reading points from %s...", infile);
      fflush(stdout);
      n = generate_stdin_points(fp, &s, n);
      fclose(fp);
    }
    printf("done, read %d points.\n", n);
  } else {
    fprintf(stderr, "Invalid distribution\n");
    exit(-1);
  }

  /* compute convex hull */
  printf("Computing convex hull using %s%s...", alg_names[alg],
	 throwaway ? " with throwaway heuristic" : "");
  fflush(stdout);
  start = clock();
  if (throwaway) {
    k = throwaway_heuristic(s, n);
    printf("\nThrew away %d points...", k);
    fflush(stdout);
  } else {
    k = 0;
  }
  switch (alg) {
  case HEAPHULL:
    i = heaphull2(s+k, n-k);
    break;
  case CHANHULL:
    i = chanhull(s+k, n-k);
    break;
  case NOHULL:
    i = 0;
    break;
  case QHULL:
  default:
    fprintf(stderr, "Unsupported convex hull algorithm\n");
    exit(-1);
  }
  i += k;
  stop = clock();
  printf("done.\n");
  printf("Computation took %f seconds\n", 
	 (double)(stop-start)/CLOCKS_PER_SEC);
  printf("Hull contains %d points\n", n-i);

  /* verify computation */
  if (verify) {
    printf("Verifying convex hull...");
    fflush(stdout);
    verify_hull(s, n, i);
    printf("done.\n");
  }

  if (outfile != NULL) {
    printf("Writing hull to %s\n", outfile);
    if ((fp = fopen(outfile, "wt")) == NULL) {
      fprintf(stderr, "Unable to open output file %s", outfile);
      exit(-1);
    }
    for (j = i; j < n; j++) {
      fprintf(fp, "%.20f %.20f\n", s[j].x, s[j].y);
    }
    fclose(fp);
  }

  if (outifile != NULL) {
    printf("Writing points to %s\n", outifile);
    if (strcmp(outifile, "-") == 0) {
      fp = stdout;
    } else {
      if ((fp = fopen(outifile, "wt")) == NULL) {
	fprintf(stderr, "Unable to open output file %s", outfile);
	exit(-1);
      }
    }
    for (j = 0; j < n; j++) {
      fprintf(fp, "%.20f %.20f\n", s[j].x, s[j].y);
    }
    if (fp != stdout) {
      fclose(fp);
    }
  }

  free(s);
  return 0;
}
