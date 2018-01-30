using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvexHullHelper
{
	internal class ConvexHullComparer
	{
		// ******************************************************************
		private int _countOfPtsRef = 0;
		private int _countOfPtsToCompare = 0;
		private string _name;

		internal ConvexHullComparer(string name)
		{
			_name = name;
		}

		// ******************************************************************
		/// <summary>
		/// This check will try to identify any differences between both path. It will stop at the first problem found.
		/// Problems found are not exhaustive. Sometimes it could contains all problems, sometimes not.
		/// But it will fails with at least an error if there is any differences in paths. 
		/// It validate the sequence also, not only that same points are present. Order could be the same or reverse.
		/// </summary>
		/// <param name="ptsReference"></param>
		/// <param name="pts"></param>
		/// <returns></returns>
		public DifferencesInPath GetPathDifferences(IReadOnlyList<Point> ptsSource, IReadOnlyList<Point> ptsReference, IReadOnlyList<Point> pts)
		{
			if (ptsReference == null)
			{
				throw new ArgumentNullException("ptsReference can't be null");
			}

			if (pts == null)
			{
				throw new ArgumentNullException("pts can't be null");
			}

			DifferencesInPath diffs = new DifferencesInPath(_name, ptsSource, ptsReference, pts);

			if (ptsReference.Count == 1 || ptsSource.Count == 1)
			{
				if (ptsReference.Count != ptsSource.Count)
				{
					diffs.CountOfPointsIsDifferent = true;
					return diffs;
				}

				if (ptsReference[0] != ptsSource[0])
				{
					diffs.FirstSequenceErrorDetectedNearPoint = ptsReference[0];
					return diffs;
				}
			}

			if (ptsReference.Count > 0 && ptsReference[0] == ptsReference[ptsReference.Count - 1])
			{
				_countOfPtsRef = ptsReference.Count - 1;
			}
			else
			{
				_countOfPtsRef = ptsReference.Count;
			}

			if (pts.Count > 0 && pts[0] == pts[pts.Count - 1])
			{
				_countOfPtsToCompare = pts.Count - 1;
				diffs.HasClosedPath = true;
			}
			else
			{
				_countOfPtsToCompare = pts.Count;
				diffs.HasClosedPath = false;
			}

			if (_countOfPtsRef != _countOfPtsToCompare)
			{
				diffs.CountOfPointsIsDifferent = true;
				FillDifferentPointCollections(ptsReference, pts, diffs);
				return diffs;
			}

			if (_countOfPtsRef == 0)
			{
				// Nothing to compare
				return diffs;
			}

			int indexStartPts;
			
			// Try to find a common start point to start iteration
			for (indexStartPts = 0; indexStartPts < _countOfPtsToCompare; indexStartPts++)
			{
				if (pts[indexStartPts] == ptsReference[0])
				{
					break;
				}
			}

			if (indexStartPts == -1 || indexStartPts >= _countOfPtsToCompare)
			{
				diffs.FirstSequenceErrorDetectedNearPoint = ptsReference[0];
				FillDifferentPointCollections(ptsReference, pts, diffs);
				return diffs;
			}

			int indexPts = indexStartPts;
			int indexRef;

			int errorAtRefIndex = -1;
			for (indexRef = 0; indexRef < _countOfPtsRef; indexRef++)
			{
				if (pts[indexPts] != ptsReference[indexRef])
				{
					errorAtRefIndex = indexRef;
					break;
				}

				indexPts++;
				if (indexPts >= _countOfPtsToCompare)
				{
					indexPts = 0;
				}
			}

			if (errorAtRefIndex == -1)
			{
				// No error found, return
				return diffs;
			}

			// Error were found but should we need to try in reverse order before ?
			if (indexRef == 1) // We should try reverse order
			{
				// try reverse order ???
				indexPts = indexStartPts;
				errorAtRefIndex = -1;
			}
			else
			{
				diffs.FirstSequenceErrorDetectedNearPoint = ptsReference[errorAtRefIndex];
				FillDifferentPointCollections(ptsReference, pts, diffs);
				return diffs;
			}

			for (indexRef = 0; indexRef < _countOfPtsRef; indexRef++)
			{
				if (pts[indexPts] != ptsReference[indexRef])
				{
					errorAtRefIndex = indexRef - 1;
					break;
				}

				if (indexPts > 0)
				{
					indexPts--;
				}
				else
				{
					indexPts = _countOfPtsToCompare - 1;
				}
			}

			if (errorAtRefIndex != -1)
			{
				diffs.FirstSequenceErrorDetectedNearPoint = ptsReference[errorAtRefIndex];
				FillDifferentPointCollections(ptsReference, pts, diffs);
			}

			return diffs;
		}

		// ******************************************************************
		/// <summary>
		/// Take very long when array is large: O(n x n)
		/// </summary>
		/// <param name="ptsReference"></param>
		/// <param name="pts"></param>
		/// <param name="diffs"></param>
		private void FillDifferentPointCollections(IReadOnlyList<Point> ptsReference, IReadOnlyList<Point> pts, DifferencesInPath diffs)
		{
			diffs.UnwantedPoints.AddRange(pts.Where(pt => !ptsReference.Contains(pt)));
			diffs.MissingPoints.AddRange(ptsReference.Where(pt => !pts.Contains(pt)));
		}

		// ******************************************************************
	}
}
