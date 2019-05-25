#pragma once

#include <vector>

class SeparationStrategy
{
public:
	SeparationStrategy();
	~SeparationStrategy();

	virtual std::vector<std::pair<long, long>> Separate(long, short) = 0;
};

class EqualSeparationStrategy : public SeparationStrategy
{
public:
	EqualSeparationStrategy();
	~EqualSeparationStrategy();
	std::vector<std::pair<long, long>> Separate(long iterations, short threads);
};

