#pragma once

#include <vector>

typedef std::pair<long, long> range_t;
typedef std::vector<std::pair<long, long>> simple_ranges;
typedef std::vector<std::pair<std::pair<long, long>, std::pair<long, long>>> advanced_ranges;

template <typename T>
class SeparationStrategy
{
public:
	virtual T Separate(long, short) = 0;
};

class NaiveSeparationStrategy : public SeparationStrategy<simple_ranges>
{
public:
	simple_ranges Separate(long, short);
};

class OptimizedSeparationStrategy : public SeparationStrategy<advanced_ranges>
{
public:
	advanced_ranges Separate(long, short);
};

