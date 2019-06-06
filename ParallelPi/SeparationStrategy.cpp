#include "SeparationStrategy.h"

simple_ranges NaiveSeparationStrategy::Separate(long iterations, short threads)
{
	simple_ranges ranges;
	if (threads == 0)
		return ranges;

	ranges.reserve(threads);

	auto chunk = iterations / threads;
	auto remainder = iterations % threads;

	auto loopIterations = 0;
	for (auto partition = 0; partition <= iterations - chunk + loopIterations; partition += chunk + 1)
	{
		ranges.emplace_back(partition, partition + chunk > iterations ? iterations : partition + chunk);
		++loopIterations;
	}

	return ranges;
}

advanced_ranges OptimizedSeparationStrategy::Separate(long iterations, short threads)
{
	advanced_ranges ranges;
	if (threads == 0)
		return ranges;

	ranges.reserve(threads);

	auto leftChunk = 0, rightChunk = 0;
	
	if ((iterations / threads) % 2 != 0)
	{
		leftChunk = (iterations / threads) / 2;
		rightChunk = (iterations / threads) / 2 + 1;
	}
	else
	{
		leftChunk = rightChunk = (iterations / threads) / 2;
	}

	long l = 0, r = iterations, previous_l = l, previous_r = r;

	while (l < r)
	{
		ranges.emplace_back(std::make_pair(previous_l, l + leftChunk), std::make_pair(r - rightChunk, previous_r));

		previous_l = l + leftChunk + 1;
		previous_r = r - rightChunk - 1;

		l += leftChunk;
		r -= rightChunk;
	}

	// make last range excluding last number
	// as it will be calculated from the second last range.
	ranges.back().first.second--;

	return ranges;
}