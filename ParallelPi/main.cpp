#include <boost/multiprecision/mpfr.hpp>
#include <boost/timer/timer.hpp>
#include <boost/thread/thread.hpp>

#include <iostream>
#include <future>
#include <chrono>

#include "CommandLineParser.h"
#include "SeparationStrategy.h"
#include "Logger.h"

std::mutex display_mutex;

boost::multiprecision::mpfr_float built_in_factorial(int end)
{
	mpfr_t fact;
	mpfr_init2(fact, boost::multiprecision::mpfr_float::default_precision());
	mpfr_fac_ui(fact, end, MPFR_RNDN);

	return fact;
}

boost::multiprecision::mpfr_float calculatePi(int start, int end)
{
	using boost::multiprecision::mpfr_float;
	mpfr_float partition = 0;
	LOG_VERBOSE("Thread id: " + std::this_thread::get_id() + " started.\n");
	for (; start < end; ++start)
	{
		mpfr_float fac_1_start = built_in_factorial(start);
		mpfr_float fac_1_4xStart = built_in_factorial(4 * start);

		mpfr_float n = fac_1_4xStart * mpfr_float(1103 + 26390 * start);
		mpfr_float d = boost::multiprecision::pow(fac_1_start, 4) * boost::multiprecision::pow((mpfr_float)396, 4 * start);
		
		partition += (n / d);
	}
	LOG_VERBOSE("Thread id: " + std::this_thread::get_id() + " stopped.\n");
	display_mutex.unlock();
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
	
	// Initialize Logger
	Logger logger(programOptions);

	std::vector<std::future<boost::multiprecision::mpfr_float>> futures;

	boost::timer::cpu_timer timer;
	auto start = std::chrono::system_clock::now();
	for (auto partitionBound : partitionBounds)
	{
		futures.emplace_back(std::async(calculatePi, partitionBound.first, partitionBound.second));
	}
	
	boost::multiprecision::mpfr_float pi;
	for (auto threadId = 0; threadId < futures.size(); ++threadId)
		pi += futures[threadId].get();
	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - start;

	std::cout << pi << std::endl;
	std::cout << elapsed.count() << std::endl;
	std::cout << timer.format() << std::endl;

	return 0;
}