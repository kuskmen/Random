#pragma once

#include <vector>

class SeparationStrategy
{
public:
	virtual std::vector<std::pair<long, long>> Separate(long, short) = 0;
};

class EqualSeparationStrategy : public SeparationStrategy
{
public:
	std::vector<std::pair<long, long>> Separate(long iterations, short threads);
};

