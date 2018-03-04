# ConvexHull
Code of OuelletConvexHull and its workbench for CodeProject article : Fast and improved 2D Convex Hull algorithm and its implementation in O(n log h). Link: https://www.codeproject.com/Articles/1210225/Fast-and-improved-D-Convex-Hull-algorithm-and-its

It contains:
 - Many ConvexHull algorithm implementations
 - A workbench to test implementations working
 - A workbench to test and compare implementations performance

IMPORTANT: THe code is made to RUN on "Release" - "x64". It can run in "Debug" - "x64". But you should avoid any other configurations, otherwise you will have problems with any C++ code.

IMPORTANT: If it crash or does not works. It could be related to C++ files. Try to test without selecting "Chan", "Heap" or "Ouellet CPP".

2018-02-06, Update: The code should be close to the one that will be available in the next article about Online Convex Hull (dynamic add).
The article should be available in a month or so.
2018-03-03, Update: Add code to remove all warning (mainly GetHashCode). Fixed compilation of C++ dlls folders. Added MSVCRT dlls to make it easier to compile and run on any environment.
