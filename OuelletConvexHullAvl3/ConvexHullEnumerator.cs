using System.Collections;
using System.Collections.Generic;
using System.Windows;
using General.AvlTreeSet;
using OuelletConvexHullAvl3.AvlTreeSet;

namespace OuelletConvexHullAvl3
{
	/// <summary>
	/// Does not support multihtread
	/// </summary>
	public class ConvexHullEnumerator : IEnumerator<Point>
	{
		// ******************************************************************
		private int _count = 0;
		private ConvexHull _convexHull;

		private Quadrant _currentQuadrant = null;
		private AvlNode<Point> _currentNode = null;

		// ******************************************************************
		public ConvexHullEnumerator(ConvexHull convexHull)
		{
			_convexHull = convexHull;
			_count = 0;
		}

		// ******************************************************************
		public Point Current => _currentNode.Item;

		object IEnumerator.Current => _currentNode.Item;

		// ******************************************************************
		public void Dispose()
		{

		}

		// ******************************************************************
		public bool MoveNext()
		{
			if (! _convexHull.IsInitDone)
			{
				return false;
			}

			if (_currentQuadrant == null)
			{
				_currentQuadrant = _convexHull._q1;
				_currentNode = _currentQuadrant.GetFirstNode();
			}
			else
			{
				for (;;)
				{
					AvlNode<Point> nextNode = _currentNode.GetNextNode();
					if (nextNode == null)
					{
						if (_currentQuadrant == _convexHull._q4)
						{
							return false;
						}

						_currentQuadrant = _currentQuadrant.GetNextQuadrant();

						nextNode = _currentQuadrant.GetFirstNode();

						if (nextNode.Item == _currentNode.Item)
						{
							_currentNode = nextNode;
							return MoveNext();
						}
					}
					else
					{
						_currentNode = nextNode;
						break;
					}
				}
			}

			return true;
		}

		// ******************************************************************
		public void Reset()
		{
			_currentQuadrant = null;
			_currentNode = null;
		}

		// ******************************************************************

	}
}
