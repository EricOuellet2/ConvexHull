using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace General.AvlTreeSet
{
	/// <summary>
	/// 2016-12-08, Eric Ouellet
	/// The code is an adapted version of BitLush AvlTree: https://bitlush.com/blog/efficient-avl-tree-in-c-sharp
	/// 
	/// Rule for property "balance":
	///		The balance is the difference of the sum of childs count on each side of the node. 
	///			- It is -1 if the right side has +1 node than left one.
	///			- It is +1 if the left side has +1 node then right side.
	///			- A diff of 2 require a rebalance, there should'nt be any abs(balance) >= 2 
	/// </summary>
	public class AvlTreeSet<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection, IReadOnlyCollection<T> // ISet<T>
	{
		// ******************************************************************
		private IComparer<T> _comparer;
		private AvlNode<T> _root;
		protected int _count = 0;

		public string Name { get; protected set; }

		// ******************************************************************
		public AvlTreeSet(IComparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException($"{nameof(comparer)} can't be null");
			}
			
			_comparer = comparer;
			_enumerableWrapper = new EnumerableWrapper<AvlNode<T>>(() => new AvlNodeEnumerator<T>(this));
			_enumerableWrapperReverse = new EnumerableWrapper<AvlNode<T>>(() => new AvlNodeEnumeratorReverse<T>(this));
		}

		// ******************************************************************
		public AvlTreeSet() : this(Comparer<T>.Default)
		{

		}

		// ******************************************************************
		public AvlNode<T> Root
		{
			get { return _root; }
		}

		// ******************************************************************
		public int Count
		{
			get { return _count; }
		}

		// ******************************************************************
		private object _syncRoot;

		public object SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
					Interlocked.CompareExchange(ref this._syncRoot, new object(), (object)null);
				return this._syncRoot;
			}
		}

		// ******************************************************************
		public bool IsSynchronized
		{
			get { return true; }
		}

		// ******************************************************************
		public bool IsReadOnly
		{
			get { return false; }
		}

		// ******************************************************************
		public IEnumerator<T> GetEnumerator()
		{
			return new AvlNodeItemEnumerator<T>(this);
		}

		// ******************************************************************
		private readonly EnumerableWrapper<AvlNode<T>> _enumerableWrapper = null;

		public IEnumerable<AvlNode<T>> Nodes()
		{
			return _enumerableWrapper;
		}

		// ******************************************************************
		private readonly EnumerableWrapper<AvlNode<T>> _enumerableWrapperReverse = null;

		public IEnumerable<AvlNode<T>> NodesReverse()
		{
			return _enumerableWrapperReverse;
		}

		// ******************************************************************
		protected AvlNode<T> GetNode(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				int compareResult = _comparer.Compare(item, node.Item);
				if (compareResult < 0)
				{
					node = node.Left;
				}
				else if (compareResult > 0)
				{
					node = node.Right;
				}
				else
				{
					return node;
				}
			}

			return null;
		}

		// ******************************************************************
		public bool Contains(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				int compareResult = _comparer.Compare(item, node.Item);
				if (compareResult < 0)
				{
					node = node.Left;
				}
				else if (compareResult > 0)
				{
					node = node.Right;
				}
				else
				{
					return true;
				}
			}

			return false;
		}

		// ******************************************************************
		public virtual AvlNode<T> Add(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				int compare = _comparer.Compare(item, node.Item);

				if (compare < 0)
				{
					AvlNode<T> left = node.Left;

					if (left == null)
					{
						AvlNode<T> newNode = new AvlNode<T> { Item = item, Parent = node };
						node.Left = newNode;
						AddBalance(node, 1);
						return newNode;
					}
					else
					{
						node = left;
					}
				}
				else if (compare > 0)
				{
					AvlNode<T> right = node.Right;

					if (right == null)
					{
						AvlNode<T> newNode = new AvlNode<T> { Item = item, Parent = node };
						node.Right = newNode;
						AddBalance(node, -1);
						return newNode;
					}
					else
					{
						node = right;
					}
				}
				else
				{
					return null;
				}
			}

			_root = new AvlNode<T> { Item = item };
			_count++;

			return _root;
		}

		// ******************************************************************
		public virtual AvlNode<T> AddOrUpdate(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				int compare = _comparer.Compare(item, node.Item);

				if (compare < 0)
				{
					AvlNode<T> left = node.Left;

					if (left == null)
					{
						AvlNode<T> newNode = new AvlNode<T> { Item = item, Parent = node };
						node.Left = newNode;
						AddBalance(node, 1);
						return newNode;
					}
					else
					{
						node = left;
					}
				}
				else if (compare > 0)
				{
					AvlNode<T> right = node.Right;

					if (right == null)
					{
						AvlNode<T> newNode = new AvlNode<T> { Item = item, Parent = node };
						node.Right = newNode;
						AddBalance(node, -1);
						return newNode;
					}
					else
					{
						node = right;
					}
				}
				else
				{
					node.Item = item;
					return node;
				}
			}

			_root = new AvlNode<T> { Item = item };
			_count++;

			return _root;
		}

		// ******************************************************************
		/// <summary>
		/// Should always be called for any inserted node
		/// </summary>
		/// <param name="node"></param>
		/// <param name="balance"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void AddBalance(AvlNode<T> node, int balance)
		{
			_count++;

			while (node != null)
			{
				balance = (node.Balance += balance);

				if (balance == 0)
				{
					break;
				}

				if (balance == 2)
				{
					if (node.Left.Balance == 1)
					{
						RotateRight(node);
					}
					else
					{
						RotateLeftRight(node);
					}

					break;
				}

				if (balance == -2)
				{
					if (node.Right.Balance == -1)
					{
						RotateLeft(node);
					}
					else
					{
						RotateRightLeft(node);
					}

					break;
				}

				AvlNode<T> parent = node.Parent;

				if (parent != null)
				{
					balance = parent.Left == node ? 1 : -1;
				}

				node = parent;
			}
		}

		// ******************************************************************
		protected AvlNode<T> RotateLeft(AvlNode<T> node)
		{
			AvlNode<T> right = node.Right;
			AvlNode<T> rightLeft = right.Left;
			AvlNode<T> parent = node.Parent;

			right.Parent = parent;
			right.Left = node;
			node.Right = rightLeft;
			node.Parent = right;

			if (rightLeft != null)
			{
				rightLeft.Parent = node;
			}

			if (node == _root)
			{
				_root = right;
			}
			else if (parent.Right == node)
			{
				parent.Right = right;
			}
			else
			{
				parent.Left = right;
			}

			right.Balance++;
			node.Balance = -right.Balance;

			return right;
		}

		// ******************************************************************
		protected AvlNode<T> RotateRight(AvlNode<T> node)
		{
			AvlNode<T> left = node.Left;
			AvlNode<T> leftRight = left.Right;
			AvlNode<T> parent = node.Parent;

			left.Parent = parent;
			left.Right = node;
			node.Left = leftRight;
			node.Parent = left;

			if (leftRight != null)
			{
				leftRight.Parent = node;
			}

			if (node == _root)
			{
				_root = left;
			}
			else if (parent.Left == node)
			{
				parent.Left = left;
			}
			else
			{
				parent.Right = left;
			}

			left.Balance--;
			node.Balance = -left.Balance;

			return left;
		}

		// ******************************************************************
		protected AvlNode<T> RotateLeftRight(AvlNode<T> node)
		{
			AvlNode<T> left = node.Left;
			AvlNode<T> leftRight = left.Right;
			AvlNode<T> parent = node.Parent;
			AvlNode<T> leftRightRight = leftRight.Right;
			AvlNode<T> leftRightLeft = leftRight.Left;

			leftRight.Parent = parent;
			node.Left = leftRightRight;
			left.Right = leftRightLeft;
			leftRight.Left = left;
			leftRight.Right = node;
			left.Parent = leftRight;
			node.Parent = leftRight;

			if (leftRightRight != null)
			{
				leftRightRight.Parent = node;
			}

			if (leftRightLeft != null)
			{
				leftRightLeft.Parent = left;
			}

			if (node == _root)
			{
				_root = leftRight;
			}
			else if (parent.Left == node)
			{
				parent.Left = leftRight;
			}
			else
			{
				parent.Right = leftRight;
			}

			if (leftRight.Balance == -1)
			{
				node.Balance = 0;
				left.Balance = 1;
			}
			else if (leftRight.Balance == 0)
			{
				node.Balance = 0;
				left.Balance = 0;
			}
			else
			{
				node.Balance = -1;
				left.Balance = 0;
			}

			leftRight.Balance = 0;

			return leftRight;
		}

		// ******************************************************************
		protected AvlNode<T> RotateRightLeft(AvlNode<T> node)
		{
			AvlNode<T> right = node.Right;
			AvlNode<T> rightLeft = right.Left;
			AvlNode<T> parent = node.Parent;
			AvlNode<T> rightLeftLeft = rightLeft.Left;
			AvlNode<T> rightLeftRight = rightLeft.Right;

			rightLeft.Parent = parent;
			node.Right = rightLeftLeft;
			right.Left = rightLeftRight;
			rightLeft.Right = right;
			rightLeft.Left = node;
			right.Parent = rightLeft;
			node.Parent = rightLeft;

			if (rightLeftLeft != null)
			{
				rightLeftLeft.Parent = node;
			}

			if (rightLeftRight != null)
			{
				rightLeftRight.Parent = right;
			}

			if (node == _root)
			{
				_root = rightLeft;
			}
			else if (parent.Right == node)
			{
				parent.Right = rightLeft;
			}
			else
			{
				parent.Left = rightLeft;
			}

			if (rightLeft.Balance == 1)
			{
				node.Balance = 0;
				right.Balance = -1;
			}
			else if (rightLeft.Balance == 0)
			{
				node.Balance = 0;
				right.Balance = 0;
			}
			else
			{
				node.Balance = 1;
				right.Balance = 0;
			}

			rightLeft.Balance = 0;

			return rightLeft;
		}

		// ******************************************************************
		public virtual bool Remove(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				if (_comparer.Compare(item, node.Item) < 0)
				{
					node = node.Left;
				}
				else if (_comparer.Compare(item, node.Item) > 0)
				{
					node = node.Right;
				}
				else
				{
					RemoveNode(node);
					return true;
				}
			}

			DebugEnsureTreeIsValid();

			return false;
		}

		// ******************************************************************
		public void RemoveNode(AvlNode<T> node)
		{
			_count--;

			AvlNode<T> left = node.Left;
			AvlNode<T> right = node.Right;

			if (left == null)
			{
				if (right == null)
				{
					if (node == _root)
					{
						_root = null;
					}
					else
					{
						if (node.Parent.Left == node)
						{
							node.Parent.Left = null;

							RemoveBalance(node.Parent, -1);
						}
						else if (node.Parent.Right == node)
						{
							node.Parent.Right = null;

							RemoveBalance(node.Parent, 1);
						}
						else
						{
							DumpVisual2();
							Debug.Assert(false); // Duplicate values ???
						}
					}
				}
				else
				{
					Replace(node, right);

					RemoveBalance(node, 0);
				}
			}
			else if (right == null)
			{
				Replace(node, left);

				RemoveBalance(node, 0);
			}
			else
			{
				AvlNode<T> successor = right;

				if (successor.Left == null)
				{
					AvlNode<T> parent = node.Parent;

					successor.Parent = parent;
					successor.Left = left;
					successor.Balance = node.Balance;

					left.Parent = successor;

					if (node == _root)
					{
						_root = successor;
					}
					else
					{
						if (parent.Left == node)
						{
							parent.Left = successor;
						}
						else
						{
							parent.Right = successor;
						}
					}

					RemoveBalance(successor, 1);
				}
				else
				{
					while (successor.Left != null)
					{
						successor = successor.Left;
					}

					AvlNode<T> parent = node.Parent;
					AvlNode<T> successorParent = successor.Parent;
					AvlNode<T> successorRight = successor.Right;

					if (successorParent.Left == successor)
					{
						successorParent.Left = successorRight;
					}
					else
					{
						successorParent.Right = successorRight;
					}

					if (successorRight != null)
					{
						successorRight.Parent = successorParent;
					}

					successor.Parent = parent;
					successor.Left = left;
					successor.Balance = node.Balance;
					successor.Right = right;
					right.Parent = successor;

					left.Parent = successor;

					if (node == _root)
					{
						_root = successor;
					}
					else
					{
						if (parent.Left == node)
						{
							parent.Left = successor;
						}
						else
						{
							parent.Right = successor;
						}
					}

					RemoveBalance(successorParent, -1);
				}
			}

			DebugEnsureTreeIsValid();
		}

		// ******************************************************************
		/// <summary>
		/// Shoould always be called for any removed node
		/// </summary>
		/// <param name="node"></param>
		/// <param name="balance"></param>
		protected void RemoveBalance(AvlNode<T> node, int balance)
		{
			while (node != null)
			{
				balance = (node.Balance += balance);

				if (balance == 2)
				{
					if (node.Left.Balance >= 0)
					{
						node = RotateRight(node);

						if (node.Balance == -1)
						{
							return;
						}
					}
					else
					{
						node = RotateLeftRight(node);
					}
				}
				else if (balance == -2)
				{
					if (node.Right.Balance <= 0)
					{
						node = RotateLeft(node);

						if (node.Balance == 1)
						{
							return;
						}
					}
					else
					{
						node = RotateRightLeft(node);
					}
				}
				else if (balance != 0)
				{
					return;
				}

				AvlNode<T> parent = node.Parent;

				if (parent != null)
				{
					balance = parent.Left == node ? -1 : 1;
				}

				node = parent;
			}
		}

		// ******************************************************************
		/// <summary>
		/// WARNING THIS METHOD is modifying the node content. 
		/// This weird behavior cause side effect which could invalid any node previously referenced.
		/// This method is used to remove node.
		/// 
		/// - THE BIG QUESTION is: Should I be able to obtain a ref on the node or not (like I added for remove nmode)??? 
		/// - My answer is "NO" theoritically but in real life I say "YES" I want a ref in order to be more efficient in my context.
		/// - I know that this could affect the Tree implementtion abstraction which could be prevented to change in the future 
		/// because I play with inner stuff but honestly, for performance here, I  don't give a shit.
		/// 
		/// But could I replace node without touching the item? I should be able. It cost a little more thought :-)
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>

		private static void Replace(AvlNode<T> target, AvlNode<T> source)
		{
			AvlNode<T> left = source.Left;
			AvlNode<T> right = source.Right;

			target.Balance = source.Balance;
			target.Item = source.Item;
			target.Left = left;
			target.Right = right;

			if (left != null)
			{
				left.Parent = target;
			}

			if (right != null)
			{
				right.Parent = target;
			}
		}

		// ******************************************************************
		// No side effect on any node previously referenced, other than the deleted one and/or root if deleted node is the root
		public virtual bool RemoveSafe(T item)
		{
			AvlNode<T> node = _root;

			while (node != null)
			{
				if (_comparer.Compare(item, node.Item) < 0)
				{
					node = node.Left;
				}
				else if (_comparer.Compare(item, node.Item) > 0)
				{
					node = node.Right;
				}
				else
				{
					RemoveNodeSafe(node);
					return true;
				}
			}

			return false;
		}

		// ******************************************************************
		// No side effect on any node previously referenced, other than the deleted one and/or root if deleted node is the root
		public void RemoveNodeSafe(AvlNode<T> node)
		{
			_count--;

			AvlNode<T> left = node.Left;
			AvlNode<T> right = node.Right;

			if (left == null)
			{
				if (right == null)
				{
					if (node == _root)
					{
						_root = null;
					}
					else
					{
						if (node.Parent.Left == node)
						{
							node.Parent.Left = null;

							RemoveBalance(node.Parent, -1);
						}
						else if (node.Parent.Right == node)
						{
							node.Parent.Right = null;

							RemoveBalance(node.Parent, 1);
						}
						else
						{
							DumpVisual2();
							Debug.Assert(false); // Duplicate values ???
						}
					}
				}
				else
				{
					ReplaceSafe(node, right);
					RemoveBalance(right, 0);

				}
			}
			else if (right == null)
			{
				ReplaceSafe(node, left);
				RemoveBalance(left, 0);
			}
			else
			{
				AvlNode<T> successor = right;

				if (successor.Left == null)
				{
					AvlNode<T> parent = node.Parent;

					successor.Parent = parent;
					successor.Left = left;
					successor.Balance = node.Balance;

					left.Parent = successor;

					if (node == _root)
					{
						_root = successor;
					}
					else
					{
						if (parent.Left == node)
						{
							parent.Left = successor;
						}
						else
						{
							parent.Right = successor;
						}
					}

					RemoveBalance(successor, 1);
				}
				else
				{
					while (successor.Left != null)
					{
						successor = successor.Left;
					}

					AvlNode<T> parent = node.Parent;
					AvlNode<T> successorParent = successor.Parent;
					AvlNode<T> successorRight = successor.Right;

					if (successorParent.Left == successor)
					{
						successorParent.Left = successorRight;
					}
					else
					{
						successorParent.Right = successorRight;
					}

					if (successorRight != null)
					{
						successorRight.Parent = successorParent;
					}

					successor.Parent = parent;
					successor.Left = left;
					successor.Balance = node.Balance;
					successor.Right = right;
					right.Parent = successor;

					left.Parent = successor;

					if (node == _root)
					{
						_root = successor;
					}
					else
					{
						if (parent.Left == node)
						{
							parent.Left = successor;
						}
						else
						{
							parent.Right = successor;
						}
					}

					RemoveBalance(successorParent, -1);
				}
			}
		}

		// ******************************************************************
		/// <summary>
		/// EO: New
		/// </summary>
		/// <param name="itemToDelete"></param>
		/// <param name="child"></param>

		private void ReplaceSafe(AvlNode<T> itemToDelete, AvlNode<T> child)
		{
			child.Parent = itemToDelete.Parent;
			if (itemToDelete.Parent != null)
			{
				if (itemToDelete.Parent.Left == itemToDelete)
				{
					itemToDelete.Parent.Left = child;
				}
				else if (itemToDelete.Parent.Right == itemToDelete)
				{
					itemToDelete.Parent.Right = child;
				}
			}
			else
			{
				_root = child;
			}
		}

		// ******************************************************************
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// ******************************************************************
		public T GetFirstItem()
		{
			AvlNode<T> node = GetFirstNode();
			if (node != null)
			{
				return node.Item;
			}

			return default(T);
		}

		// ******************************************************************
		public AvlNode<T> GetFirstNode()
		{
			if (Root != null)
			{
				AvlNode<T> current = Root;
				while (current.Left != null)
				{
					current = current.Left;
				}

				return current;
			}

			return null;
		}

		// ******************************************************************
		public T GetLastItem()
		{
			AvlNode<T> node = GetLastNode();
			if (node != null)
			{
				return node.Item;
			}

			return default(T);
		}

		// ******************************************************************
		public AvlNode<T> GetLastNode()
		{
			if (Root != null)
			{
				AvlNode<T> current = Root;
				while (current.Right != null)
				{
					current = current.Right;
				}

				return current;
			}

			return null;
		}

		// ******************************************************************
		/// <summary>
		/// Simple dump of each node, one by line
		/// </summary>
		public virtual void Dump()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"AvlTree dump of '{Name}', First: {GetFirstNode()}, Last: {GetLastNode()}");

			bool isFirst = true;
			foreach (var pt in this)
			{
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					sb.Append(", ");
				}

				sb.Append("[");
				sb.Append(pt);
				sb.Append("]");
			}

			Debug.Print(sb.ToString());
		}

		// ******************************************************************
		/// <summary>
		/// This dump the tree in a more visual and easy way to see. Where each item is tabbed according to its depth.
		/// Each line is an item. The first line is the first item and so on. 
		/// If you look on the side, the tree will look like reverse. I prefer using DumpVisual.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="title"></param>
		public virtual void DumpVisual2(string prefix = "", string title = "")
		{
			VerifyIntegrity();

			Debug.Print($"{prefix}---------------------------- AVL Tree Dump {title} -------------------------------");
			Debug.Print($"Count: {Count}, First: {GetFirstItem()}, Last: {GetLastItem()}, DebugCount: {DebugCount()}");

			StringBuilder sb = new StringBuilder();
			foreach (var node in Nodes())
			{
				sb.Clear();
				sb.Append(prefix);
				for (int n = 0; n < node.GetHeight(); n++)
				{
					sb.Append("  ");
				}
				sb.Append(node);
				Debug.Print(sb.ToString());
			}
		}

		// ******************************************************************
		[DebuggerHidden]
		public void VerifyIntegrity()
		{
			if (this.Root != null)
			{
				Debug.Assert(Root.Parent == null);
				VerifyNodeIntegrityRecursive(Root);
			}
		}

		// ******************************************************************
		[DebuggerHidden]
		private void VerifyNodeIntegrityRecursive(AvlNode<T> node)
		{
			if (node.Left != null)
			{
				Debug.Assert(node.Left.Parent == node);
				VerifyNodeIntegrityRecursive(node.Left);
			}

			if (node.Right != null)
			{
				Debug.Assert(node.Right.Parent == node);
				VerifyNodeIntegrityRecursive(node.Right);
			}
		}

		// ******************************************************************
		public int DebugCount()
		{
			return RecursiveCount(Root);
		}

		// ******************************************************************
		private int RecursiveCount(AvlNode<T> node)
		{
			if (node == null)
			{
				return 0;
			}

			return 1 + RecursiveCount(node.Left) + RecursiveCount(node.Right);
		}

		// ******************************************************************
		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		// ******************************************************************
		public void Clear()
		{
			_root = null;
			_count = 0;
		}

		// ******************************************************************
		public void CopyTo(T[] array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("'array' can't be null");
			}

			if (index < 0)
			{
				throw new ArgumentException("'index' can't be null");
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("'count' should be greater or equal to 0");
			}

			if (index > array.Length || count > array.Length - index)
			{
				throw new ArgumentException("The array size is not big enough to get all items");
			}

			if (count == 0)
			{
				return;
			}

			int indexIter = 0;
			int indexArray = 0;
			foreach (AvlNode<T> node in this.Nodes())
			{
				if (indexIter >= index)
				{
					array[indexArray] = node.Item;
					indexArray++;
					count--;
					if (count == 0)
					{
						return;
					}
				}
				indexIter++;
			}
		}

		// ******************************************************************
		public void CopyTo(T[] array, int arrayIndex)
		{
			SortedSet<double> t = new SortedSet<double>();
			CopyTo(array, arrayIndex, Count);
		}

		// ******************************************************************
		public void CopyTo(Array array, int index)
		{
			CopyTo(array as T[], index, Count);
		}

		// ******************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		public void DebugEnsureTreeIsValid()
		{
			Debug.Assert(Count == DebugCount());
			DebugEnsureProperBalance();
			DebugEnsureTreeIsSorted();
			DebugEnsureProperParentRecursive(Root);
		}

		// ******************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		private void DebugEnsureProperParentRecursive(AvlNode<T> node)
		{
			if (node.Left != null)
			{
				Debug.Assert(node.Left.Parent == node);
				DebugEnsureProperParentRecursive(node.Left);
			}

			if (node.Right != null)
			{
				Debug.Assert(node.Right.Parent == node);
				DebugEnsureProperParentRecursive(node.Right);
			}
		}

		// ******************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		public void DebugEnsureProperBalance()
		{
			RecursiveEnsureNodeBalanceIsValid(Root);
		}

		// ******************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		private void RecursiveEnsureNodeBalanceIsValid(AvlNode<T> node)
		{
			int leftHeight = GetMaxHeightRecursive(node.Left);
			int rightHeight = GetMaxHeightRecursive(node.Right);

			if (leftHeight == 0)
			{
				if (rightHeight == 0)
				{
					DumpIfNotTrue(node.Balance == 0);
				}
				else
				{
					DumpIfNotTrue(node.Balance == -1);
				}
			}
			else
			{
				if (rightHeight == 0)
				{
					DumpIfNotTrue(node.Balance == 1);
				}
				else
				{
					DumpIfNotTrue(node.Balance == leftHeight - rightHeight);
				}
			}

			if (node.Left != null)
			{
				RecursiveEnsureNodeBalanceIsValid(node.Left);
			}

			if (node.Right != null)
			{
				RecursiveEnsureNodeBalanceIsValid(node.Right);
			}
		}

		// ************************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		private void DumpIfNotTrue(bool assertion)
		{
			if (!assertion)
			{
				DumpVisual2();
				DumpVisual();
				Debug.Assert(false);
			}
		}

		// ******************************************************************
		[System.Diagnostics.Conditional("DEBUG")]
		[DebuggerHidden]
		public void DebugEnsureTreeIsSorted()
		{
			var enumerator = this.GetEnumerator();
			enumerator.Reset();
			if (enumerator.MoveNext())
			{
				T previous = enumerator.Current;

				while (enumerator.MoveNext())
				{
					if (this._comparer.Compare(previous, enumerator.Current) != -1)
					{
						Debug.Assert(false);
					}

					previous = enumerator.Current;
				}
			}
		}

		// ************************************************************************
		private int GetMaxHeightRecursive(AvlNode<T> node)
		{
			if (node == null)
			{
				return 0;
			}

			int leftHeight = 0;
			if (node.Left != null)
			{
				leftHeight = GetMaxHeightRecursive(node.Left);
			}

			int rightHeight = 0;
			if (node.Right != null)
			{
				rightHeight = GetMaxHeightRecursive(node.Right);
			}

			return 1 + Math.Max(leftHeight, rightHeight);
		}

		// ******************************************************************
		public void CopyTo(AvlTreeSet<T> avlTree)
		{
			avlTree.Name = this.Name;
			avlTree._comparer = this._comparer;
			avlTree._count = this._count;
			avlTree._root = CopyTreeRecursive(_root, null);
		}

		// ******************************************************************
		private AvlNode<T> CopyTreeRecursive(AvlNode<T> source, AvlNode<T> copyParent)
		{
			if (source == null)
			{
				return null;
			}

			AvlNode<T> copy = new AvlNode<T>();
			copy.Parent = copyParent;
			copy.Item = source.Item;
			copy.Balance = source.Balance;
			copy.Left = CopyTreeRecursive(source.Left, copy);
			copy.Right = CopyTreeRecursive(source.Right, copy);

			return copy;
		}

		// ******************************************************************
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var avlTree = obj as AvlTreeSet<T>;

			if (avlTree == null)
			{
				return false;
			}

			if (this.Count != avlTree.Count || this.Name != avlTree.Name)
			{
				return false;
			}

			using (var enumThis = this.GetEnumerator())
			{
				using (var enumObj = avlTree.GetEnumerator())
				{
					enumThis.Reset();
					enumObj.Reset();

					for (; ; )
					{
						bool thisMoveNextResult = enumThis.MoveNext();
						bool objMoveNextResult = enumObj.MoveNext();
						if (thisMoveNextResult != objMoveNextResult)
						{
							return false;
						}

						if (thisMoveNextResult == false)
						{
							break;
						}

						if (! object.Equals(enumThis.Current, enumObj.Current))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		// ************************************************************************
		private int GetMaxLevel()
		{
			return GetMaxLevelRecursive(Root);
		}

		// ************************************************************************
		private int GetMaxLevelRecursive(AvlNode<T> node)
		{
			if (node == null)
			{
				return 0;
			}

			return 1 + Math.Max(GetMaxLevelRecursive(node.Left), GetMaxLevelRecursive(node.Right));
		}

		// ************************************************************************
		/// This dump the tree in a more visual and easy way to see. Where each item is tabbed according to its depth.
		/// Each line is an item. The first line is the last item and so on. 
		/// If you look on the side, the tree will look like normal. I prefer using this one instead of DumpVisual2.
		public void DumpVisual(string title = null)
		{
			if (title == null)
			{
				title = Name;
			}

			// int maxLevel = GetMaxLevel();
			int itemWidth = 5;

			AvlNode<T> node = GetLastNode();

			Debug.Print($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Tree Visual Dump Start '{title}' (First node is at the bottom and last node just below this line)");

			if (Count > 256)
			{
				Debug.Print("Too much items (>256)...");
			}
			else
			{
				while (node != null)
				{
					int nodeHeight = node.GetHeight() * itemWidth;
					Debug.Print($"{new string(' ', nodeHeight)}{node.Item}({node.Balance})");
					node = node.GetPreviousNode();
				}
			}
			Debug.Print($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Tree Visual Dump End '{title}'");

		}

		// ************************************************************************
		public override string ToString()
		{
			return $"AvlTreeSet of {typeof(T).Name}. Count = {Count}";
		}

		// ************************************************************************

	}
}

