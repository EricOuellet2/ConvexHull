#pragma once

#include "Point.h"

/* AVL node */
template <class T>
class AvlNode
{
public:
	T key;
	int balance;
	AvlNode *left, *right, *parent;

	AvlNode(T k, AvlNode *p) : key(k), balance(0), parent(p), left(NULL), right(NULL)
	{
	}

	~AvlNode()
	{
		delete left;
		delete right;
	}
};

