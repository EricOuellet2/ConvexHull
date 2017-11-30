#include "stdafx.h"
#include "AvlTree.h"

/* AVL class definition */

// **************************************************************************
template <class T>
void AvlTree<T>::rebalance(AvlNode<T> *n)
{
	setBalance(n);

	if (n->balance == -2) {
		if (height(n->left->left) >= height(n->left->right))
			n = rotateRight(n);
		else
			n = rotateLeftThenRight(n);
	}
	else if (n->balance == 2) {
		if (height(n->right->right) >= height(n->right->left))
			n = rotateLeft(n);
		else
			n = rotateRightThenLeft(n);
	}

	if (n->parent != NULL) {
		rebalance(n->parent);
	}
	else {
		root = n;
	}
}

// **************************************************************************
template <class T>
AvlNode<T>* AvlTree<T>::rotateLeft(AvlNode<T> *a)
{
	AvlNode<T> *b = a->right;
	b->parent = a->parent;
	a->right = b->left;

	if (a->right != NULL)
		a->right->parent = a;

	b->left = a;
	a->parent = b;

	if (b->parent != NULL) {
		if (b->parent->right == a) {
			b->parent->right = b;
		}
		else {
			b->parent->left = b;
		}
	}

	setBalance(a);
	setBalance(b);
	return b;
}

// **************************************************************************
template <class T>
AvlNode<T>* AvlTree<T>::rotateRight(AvlNode<T> *a)
{
	AvlNode<T> *b = a->left;
	b->parent = a->parent;
	a->left = b->right;

	if (a->left != NULL)
		a->left->parent = a;

	b->right = a;
	a->parent = b;

	if (b->parent != NULL) {
		if (b->parent->right == a) {
			b->parent->right = b;
		}
		else {
			b->parent->left = b;
		}
	}

	setBalance(a);
	setBalance(b);
	return b;
}

// **************************************************************************
template <class T>
AvlNode<T>* AvlTree<T>::rotateLeftThenRight(AvlNode<T> *n)
{
	n->left = rotateLeft(n->left);
	return rotateRight(n);
}

// **************************************************************************
template <class T>
AvlNode<T>* AvlTree<T>::rotateRightThenLeft(AvlNode<T> *n)
{
	n->right = rotateRight(n->right);
	return rotateLeft(n);
}

// **************************************************************************
template <class T>
int AvlTree<T>::height(AvlNode<T> *n)
{
	if (n == NULL)
		return -1;
	return 1 + std::max(height(n->left), height(n->right));
}

// **************************************************************************
template <class T>
void AvlTree<T>::setBalance(AvlNode<T> *n)
{
	n->balance = height(n->right) - height(n->left);
}

// **************************************************************************
template <class T>
void AvlTree<T>::printBalance(AvlNode<T> *n)
{
	if (n != NULL) {
		printBalance(n->left);
		std::cout << n->balance << " ";
		printBalance(n->right);
	}
}

// **************************************************************************
template <class T>
AvlTree<T>::AvlTree(void) : root(NULL)
{
}

// **************************************************************************
template <class T>
AvlTree<T>::~AvlTree(void)
{
	delete root;
}

// **************************************************************************
template <class T>
bool AvlTree<T>::insert(T key)
{
	if (root == NULL) {
		root = new AvlNode<T>(key, NULL);
	}
	else {
		AvlNode<T>
			*n = root,
			*parent;

		while (true) {
			if (n->key == key)
				return false;

			parent = n;

			bool goLeft = n->key > key;
			n = goLeft ? n->left : n->right;

			if (n == NULL) {
				if (goLeft) {
					parent->left = new AvlNode<T>(key, parent);
				}
				else {
					parent->right = new AvlNode<T>(key, parent);
				}

				rebalance(parent);
				break;
			}
		}
	}

	return true;
}

// **************************************************************************
template <class T>
void AvlTree<T>::deleteKey(const T delKey)
{
	if (root == NULL)
		return;

	AvlNode<T>
		*n = root,
		*parent = root,
		*delNode = NULL,
		*child = root;

	while (child != NULL) {
		parent = n;
		n = child;
		child = delKey >= n->key ? n->right : n->left;
		if (delKey == n->key)
			delNode = n;
	}

	if (delNode != NULL) {
		delNode->key = n->key;

		child = n->left != NULL ? n->left : n->right;

		if (root->key == delKey) {
			root = child;
		}
		else {
			if (parent->left == n) {
				parent->left = child;
			}
			else {
				parent->right = child;
			}

			rebalance(parent);
		}
	}
}

// **************************************************************************
template <class T>
void AvlTree<T>::printBalance()
{
	printBalance(root);
	std::cout << std::endl;
}

// **************************************************************************
