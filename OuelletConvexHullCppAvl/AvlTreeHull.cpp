#include "stdafx.h"
#include "AvlTreeHull.h"

// **************************************************************************
bool AvlTreeHull::insert(const point key)
{
	if (root == NULL)
	{
		root = new AvlNode<point>(key, NULL);
	}
	else
	{
		AvlNode<point> *n = root, *parent;

		bool goLeft;

		while (true)
		{
			if (compare_points(n->key, key))
				return false;

			parent = n;

			if (n->key.x > key.x)
			{
				goLeft = true;
			}
			else if (n->key.x < key.x)
			{
				goLeft = false;
			}
			else
			{ //x equality case
				if (n->key.x == n->key.x)
				{
					return false; // this point already exists. It is a duplicate one

					// Important here to check if C# Avl v2 suport this case
				}

				//if ()
				//{

				//}

				return true;
			}

			n = goLeft ? n->left : n->right;

			if (n == NULL)
			{
				if (goLeft)
				{
					parent->left = new AvlNode<point>(key, parent);
				}
				else
				{
					parent->right = new AvlNode<point>(key, parent);
				}

				rebalance(parent);
				break;
			}
		}

	}

	return true;
}