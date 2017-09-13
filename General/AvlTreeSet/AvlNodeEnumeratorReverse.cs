using System;
using System.Collections;
using System.Collections.Generic;

namespace General.AvlTreeSet
{
	public class AvlNodeEnumeratorReverse<T> : IEnumerator<AvlNode<T>>
	{
		// ******************************************************************
		private AvlNode<T> _current = null;
		private AvlTreeSet<T> _avlTree;

		// ******************************************************************
		public AvlNodeEnumeratorReverse(AvlTreeSet<T> avlTree)
		{
			if (avlTree == null)
			{
				throw new ArgumentNullException("avlTree can't be null");
			}

			_avlTree = avlTree;
		}

		// ******************************************************************
		public AvlNode<T> Current
		{
			get
			{
				if (_current == null)
				{
					throw new InvalidOperationException("Current is invalid");
				}

				return _current;
			}
		}

		// ******************************************************************
		object IEnumerator.Current => Current;

		// ******************************************************************
		public void Dispose()
		{

		}

		// ******************************************************************
		public bool MoveNext()
		{
			if (_current == null)
			{
				_current = _avlTree.GetLastNode();
			}
			else
			{
				_current = _current.GetPreviousNode();
			}

			if (_current == null) // Should check for an empty tree too :-) 
			{
				return false;
			}

			return true;
		}

		// ******************************************************************
		public void Reset()
		{
			_current = null;
		}

		// ******************************************************************

	}
}
