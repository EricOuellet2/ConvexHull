#pragma once

#include <stdio.h>
#include "Node.h"

// source: https://rosettacode.org/wiki/AVL_tree#C.2B.2B

/* AVL tree */
template <class T>
class AvlTree 
{
public:
	AvlTree(void);
	~AvlTree(void);
	virtual bool insert(const T key);
	void deleteKey(const T key);
	void printBalance();

public:
	//AvlNode<T> GetNextNode(AvlNode<T>* avlNode);
	//AvlNode<T> GetPreviousNode(AvlNode<T>* avlNode);

protected:
	AvlNode<T> *root;
	AvlNode<T>* rotateLeft(AvlNode<T> *a);
	AvlNode<T>* rotateRight(AvlNode<T> *a);
	AvlNode<T>* rotateLeftThenRight(AvlNode<T> *n);
	AvlNode<T>* rotateRightThenLeft(AvlNode<T> *n);
	void rebalance(AvlNode<T> *n);
	int height(AvlNode<T> *n);
	void setBalance(AvlNode<T> *n);
	void printBalance(AvlNode<T> *n);
	void clearNode(AvlNode<T> *n);
};

