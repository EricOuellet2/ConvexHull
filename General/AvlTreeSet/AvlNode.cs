using System.Diagnostics;

namespace General.AvlTreeSet
{
	public sealed class AvlNode<T>
	{
		// ******************************************************************
		public AvlNode<T> Parent;
		public AvlNode<T> Left;
		public AvlNode<T> Right;
		public T Item;
		public int Balance;

		// ******************************************************************
		/// <summary>
		/// Non recursive function that calc node height (mainly for debugging)
		/// </summary>
		/// <returns></returns>
		public int GetHeight() // Mainly for Debug purpose
		{
			int height = 1;
			AvlNode<T> node = this;
			while (node.Parent != null)
			{
				height++;
				node = node.Parent;
			}

			return height;
		}

		// ******************************************************************
		/// <summary>
		/// Non recursive function that return the next ordered node
		/// </summary>
		/// <returns></returns>
		public AvlNode<T> GetNextNode()
		{
			AvlNode<T> current;

			if (Right != null)
			{
				current = Right;
				while (current.Left != null)
				{
					current = current.Left;
				}
				return current;
			}

			current = this;
			while (current.Parent != null)
			{
				if (current.Parent.Left == current)
				{
					return current.Parent;
				}

				current = current.Parent;
			}

			return null;
		}

		// ******************************************************************
		/// <summary>
		/// Non recursive function that return the previous ordered node
		/// </summary>
		/// <returns></returns>
		public AvlNode<T> GetPreviousNode()
		{
			AvlNode<T> current;

			if (Left != null)
			{
				current = Left;
				while (current.Right != null)
				{
					current = current.Right;
				}
				return current;
			}

			current = this;
			while (current.Parent != null)
			{
				if (current.Parent.Right == current)
				{
					return current.Parent;
				}

				current = current.Parent;
			}

			return null;
		}

		// ******************************************************************
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var node = obj as AvlNode<T>;
			if (node == null)
			{
				return false;
			}

			if (this.Balance != node.Balance || !object.Equals(this.Item, node.Item))
			{
				return false;
			}

			if (this.Left != null)
			{
				if (node.Left == null)
				{
					return false;
				}

				if (!object.Equals(this.Left.Item, node.Left.Item))
				{
					return false;
				}
			}
			else
			{
				if (node.Left != null)
				{
					return false;
				}
			}

			if (this.Right != null)
			{
				if (node.Right == null)
				{
					return false;
				}

				if (!object.Equals(this.Right.Item, node.Right.Item))
				{
					return false;
				}
			}
			else
			{
				if (node.Right != null)
				{
					return false;
				}
			}

			return true;
		}

		// ******************************************************************
		public override string ToString()
		{
			return $"AvlNode [{Item}], balance: {Balance}, Parent: {Parent?.Item.ToString()}, Left: {Left?.Item.ToString()}, Right: {Right?.Item.ToString()},";
		}

		// ******************************************************************
	}
}
