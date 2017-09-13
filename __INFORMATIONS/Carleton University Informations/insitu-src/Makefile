CC=gcc
#CFLAGS=-O2 -Wall
CFLAGS=-g -Wall -DDEBUG
LD=gcc
LDFLAGS=-O2 -Wall
LDFLAGS=-g -Wall
LIBS=-lm

ALL = test1 2dch
OBJECTS = heaphull.o chanhull.o throwaway.o
HEADERS = heaphull.h chanhull.h

all : $(ALL)

test1 : test1.o $(OBJECTS)
	$(LD) $(LDFLAGS) -o test1 test1.o $(OBJECTS)  $(LIBS)

test1.o : test1.c  $(HEADERS)
	$(CC) $(CFLAGS) -c test1.c

heaphull.o : heaphull.c heaphull.h
	$(CC) $(CFLAGS) -c heaphull.c

chanhull.o : chanhull.c chanhull.h
	$(CC) $(CFLAGS) -c chanhull.c

throwaway.o : throwaway.c throwaway.h
	$(CC) $(CFLAGS) -c throwaway.c

2dch : 2dch.c
	$(CC) $(LDFLAGS) $(CFLAGS) -o 2dch 2dch.c $(LIBS)

clean :
	rm -f *~ *.o $(ALL)
