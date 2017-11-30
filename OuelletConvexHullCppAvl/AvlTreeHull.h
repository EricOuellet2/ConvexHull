#pragma once

#include "Point.h"
#include "AvlTree.h"

class AvlTreeHull : public AvlTree<point>
{
public:
	bool insert(const point key) override;
	// bool insert(const point key);
};
