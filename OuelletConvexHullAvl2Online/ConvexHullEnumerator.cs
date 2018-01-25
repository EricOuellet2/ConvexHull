using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using General.AvlTreeSet;

namespace OuelletConvexHullAvl2Online
{
	/// <summary>
	/// Does not support multihtread
	/// </summary>
	public class ConvexHullEnumerator : IEnumerator<Point>
	{
		// ******************************************************************
		private int _count = 0;
		private ConvexHullOnline _convexHullOnline;

		private Quadrant _currentQuadrant = null;
		private AvlNode<Point> _currentNode = null;

		// ******************************************************************
		public ConvexHullEnumerator(ConvexHullOnline convexHullOnline)
		{
			_convexHullOnline = convexHullOnline;
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
			if (! _convexHullOnline.IsInitDone)
			{
				return false;
			}

			if (_currentQuadrant == null)
			{
				_currentQuadrant = _convexHullOnline._q1;
				_currentNode = _currentQuadrant.GetFirstNode();
			}
			else
			{
				for (;;)
				{
					AvlNode<Point> nextNode = _currentNode.GetNextNode();
					if (nextNode == null)
					{
						if (_currentQuadrant == _convexHullOnline._q1)
						{
							_currentQuadrant = _convexHullOnline._q2;
						}
						else if (_currentQuadrant == _convexHullOnline._q2)
						{
							_currentQuadrant = _convexHullOnline._q3;
						}

						else if (_currentQuadrant == _convexHullOnline._q3)
						{
							_currentQuadrant = _convexHullOnline._q4;
						}
						else
						{
							return false;
						}

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
