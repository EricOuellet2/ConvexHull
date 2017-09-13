using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General
{
	// ****************
	// Never tested
	// ****************

	public class ConcurrentDeque<T>
	{
		// ******************************************************************
		internal class DequeNode<TNode>
		{
			public TNode Item;
			public DequeNode<TNode> Previous;
			public DequeNode<TNode> Next;
		}

		// ******************************************************************
		private DequeNode<T> _first = null;
		private DequeNode<T> _last = null;

		private SpinLock _spinLock = new SpinLock();

		// ******************************************************************
		public bool DequeueFromStart(out T item)
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);

				if (_first != null)
				{
					item = _first.Item;

					_first = _first.Next;
					if (_first != null)
					{
						_first.Previous = null;
					}
					else
					{
						_last = null;
					}

					return true;
				}
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}

			item = default(T);
			return false;
		}

		// ******************************************************************
		public bool DequeueFromEnd(out T item)
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);

				if (_last != null)
				{
					item = _last.Item;

					_last = _last.Previous;
					if (_last != null)
					{
						_last.Next = null;
					}
					else
					{
						_first = null;
					}

					return true;
				}
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}

			item = default(T);
			return false;
		}

		// ******************************************************************
		public void QueueToStart(T item)
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);

				if (_first != null)
				{
					DequeNode<T> newNode = new DequeNode<T>() { Item = item, Next = _first };
					_first.Previous = newNode;
					_first = newNode;
				}
				else
				{
					_first = new DequeNode<T>() { Item = item };
					_last = _first;
				}
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}

		// ******************************************************************
		public void QueueToEnd(T item)
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);

				if (_last != null)
				{
					DequeNode<T> newNode = new DequeNode<T>() { Item = item, Previous = _last };
					_last.Next = newNode;
					_last = newNode;
				}
				else
				{
					_last = new DequeNode<T>() { Item = item };
					_first = _last;
				}
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}

		// ******************************************************************
		public bool IsEmpty()
		{
			return _first == null;
		}

		// ******************************************************************




	}
}
