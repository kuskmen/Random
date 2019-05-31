#include "SeparationStrategy.h"

std::vector<std::pair<long, long>> EqualSeparationStrategy::Separate(long iterations, short threads)
{
	std::vector<std::pair<long, long>> ranges;
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
