#include <boost/multiprecision/mpfr.hpp>
#include <boost/timer/timer.hpp>
#include <boost/thread/thread.hpp>

#include <iostream>
#include <future>
#include <chrono>

#include "CommandLineParser.h"
#include "SeparationStrategy.h"

boost::multiprecision::mpfr_float factorial(int start, int end)
{
	boost::multiprecision::mpfr_float fact = 1;
	for (; start <= end; ++start)
		fact *= start;

	return fact;
}

boost::multiprecision::mpfr_float calculatePi(int start, int end)
{
	using boost::multiprecision::mpfr_float;

	mpfr_float partition = 0;
	
	for (; start < end; ++start)
	{
		mpfr_float fac_1_start = factorial(1, start);
		mpfr_float fac_1_4xStart = fac_1_start * factorial(start + 1, 4 * start);

		mpfr_float n = fac_1_4xStart * mpfr_float(1103 + 26390 * start);
		mpfr_float d = boost::multiprecision::pow(fac_1_start, 4) * boost::multiprecision::pow((mpfr_float)396, 4 * start);
		
		partition += (n / d);
	}
	
	return (2 * boost::multiprecision::sqrt((mpfr_float)2) / 9801) * partition;
}

int main(int argc, const char *argv[])
{
	// Parse arguments
	ProgramOptions programOptions;
	CommandLineParser::GetInstance().Parse(argc, argv, programOptions);

	// Set precision for the output and data type
	boost::multiprecision::mpfr_float::default_precision(programOptions.GetPrecision());
	std::cout.precision(boost::multiprecision::mpfr_float::default_precision());
	
	// Choose thread work sepration strategy
	EqualSeparationStrategy strategy;
	auto partitionBounds = strategy.Separate(programOptions.GetIterations(), programOptions.GetThreadsCount());

	
	
	std::vector<std::future<boost::multiprecision::mpfr_float>> threads;

	boost::timer::cpu_timer timer;
	auto start = std::chrono::system_clock::now();
	for (auto partitionBound : partitionBounds)
	{
		threads.emplace_back(
			std::async(calculatePi, partitionBound.first, partitionBound.second));
	}
	
	boost::multiprecision::mpfr_float pi;
	for (auto threadId = 0; threadId < threads.size(); ++threadId)
		pi += threads[threadId].get();

	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - start;

	std::cout << pi << std::endl;
	std::cout << elapsed.count() << std::endl;
	std::cout << timer.format() << std::endl;

	return 0;
}