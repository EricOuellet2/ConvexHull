using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuelletConvexHullLinear
{
	public class ThreadJob
	{
		public void ResolveConvexHull(int threadId)
		{
			try
			{
				int pointCount = _listOfPoint.Count;

				for (;;)
				{
					int index = Interlocked.Increment(ref _indexPointCurrent);
					if (index >= pointCount)
					{
						break;
					}

					Point point = _listOfPoint[index];

					if (!IsGoodQuadrantForPoint(point))
					{
						continue;
					}

					if (IsQuickInterpolationCandidate(point)) // Should put in Candidates if candidate
					{
						bool gotLock = false;
						try
						{
							_spinLockCandidates.Enter(ref gotLock);

							Candidates.Push(point);
						}
						finally
						{
							if (gotLock)
							{
								_spinLockCandidates.Exit();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Oops: Ex: {ex}");
				Debug.Print($"Oops: Ex: {ex}");
			}

			Interlocked.Decrement(ref _countOfWorkerThreadRunning);

			//if (Interlocked.Decrement(ref _countOfWorkerThreadRunning) == 0)
			//{
			//	int count = -1;

			//	bool gotLock = false;
			//	try
			//	{
			//		_spinLockCandidates.Enter(ref gotLock);

			//		count = Candidates.Count;
			//	}
			//	finally
			//	{
			//		if (gotLock)
			//		{
			//			_spinLockCandidates.Exit();
			//		}
			//	}

			//	Console.WriteLine($"Candidates left in {this.GetType().Name}: {count}");
			//}
		}

	}
}
