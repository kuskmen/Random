#include <boost/multiprecision/mpfr.hpp>
#include <boost/timer/timer.hpp>
#include <boost/thread/thread.hpp>

#include <iostream>
#include <future>
#include <chrono>
#include <cstdarg>

#include "CommandLineParser.h"
#include "SeparationStrategy.h"
#include "ProgramOptions.h"
#include "Logger.h"
#include "cpu_x86.h"

std::mutex display_mutex;

boost::multiprecision::mpfr_float inc_multiply(long start, long end)
{
	boost::multiprecision::mpfr_float fact = 1;
	for (; start <= end; ++start)
		fact *= start;

	return fact;
}

boost::multiprecision::mpfr_float calculatePi(int count, ...)
{
	using boost::multiprecision::mpfr_float;
	mpfr_float partition;

	std::stringstream ss;
	ss << std::this_thread::get_id();
	std::string thread_id = ss.str();
	
	LOG_VERBOSE("Thread id: " + thread_id + " started.\n");
	auto clock_start = std::chrono::system_clock::now();

	std::va_list ranges;
	va_start(ranges, count);
	
	for (int i = 0; i < count; ++i)
	{
		auto boundary = va_arg(ranges, range_t);
		mpfr_float previous_fac_start = 1.0;
		mpfr_float previous_fac_4xstart = 1.0;

		for (; boundary.first < boundary.second; ++boundary.first)
		{
			mpfr_float fac_start = previous_fac_start * (boundary.first == 0 ? 1 : boundary.first);
			mpfr_float fac_4xstart = previous_fac_4xstart * inc_multiply((4 * boundary.first - 3) < 0 ? 1 : 4 * boundary.first - 3, 4 * boundary.first);

			mpfr_float n = fac_4xstart * mpfr_float(1103 + 26390 * boundary.first);
			mpfr_float d = boost::multiprecision::pow(fac_start, 4) * boost::multiprecision::pow((mpfr_float)396, 4 * boundary.first);

			partition += (n / d);
			previous_fac_start = fac_start;
			previous_fac_4xstart = fac_4xstart;
		}
	}

	va_end(ranges);

	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - clock_start;
	LOG_VERBOSE("Thread id: " + thread_id + " stopped.\n");
	LOG_VERBOSE("Thread " + thread_id + " execution time was " + std::to_string(elapsed.count()) + "ms\n");
	
	return (2 * boost::multiprecision::sqrt((mpfr_float)2) / 9801) * partition;
}

boost::multiprecision::mpfr_float calculate(ProgramOptions* options)
{
	std::vector<std::future<boost::multiprecision::mpfr_float>> futures;

	// Choose thread work sepration strategy
	// TODO: Fix duplication, think of dynamic creation of strategies.
	if (options->IsOptimized())
	{
		auto strategy = new OptimizedSeparationStrategy();
		auto partitions = strategy->Separate(options->GetIterations(), options->GetThreadsCount());

		for (auto partition : partitions)
			futures.emplace_back(std::async(calculatePi, 2, partition.first, partition.second));
	}
	else
	{
		auto strategy = new NaiveSeparationStrategy();
		auto partitions = strategy->Separate(options->GetIterations(), options->GetThreadsCount());

		for (auto partition : partitions)
			futures.emplace_back(std::async(calculatePi, 1, partition));
	}

	boost::multiprecision::mpfr_float pi;
	for (auto threadId = 0; threadId < futures.size(); ++threadId)
		pi += futures[threadId].get();

	return pi;
}

int main(int argc, const char *argv[])
{
	// Parse arguments
	std::unique_ptr<ProgramOptions> programOptions(new ProgramOptions());
	sCommandLineParser->Parse(argc, argv, programOptions);
	
	// Pass options to initialize Logger
	Logger::options = programOptions.get();
	LOG_QUIET(
		"Starting execution with following setup: \
			   \nprecision: " + std::to_string(programOptions->GetIterations() * 8)
			+ "\titerations: " + std::to_string(programOptions->GetIterations())
			+ "\tlog level: " + pp::to_string(programOptions->GetLogLevel())
			+ "\tthreads: " + std::to_string(programOptions->GetThreadsCount())
			+ "\toptimized: " + std::string(programOptions->IsOptimized() ? "Yes" : "No")
			+ "\n\n"
		);

	// Print CPU features
	FeatureDetector::cpu_x86::print_host();

	// Set precision for the output and data type
	// Ramanujan formula calculates 8 digits each iteration
	boost::multiprecision::mpfr_float::default_precision(programOptions->GetIterations() * 8);
	
	boost::timer::cpu_timer timer;
	auto start = std::chrono::system_clock::now();
	auto pi = calculate(programOptions.get());
	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - start;

	LOG_QUIET("1/pi: " + pi.str() + "\n");
	LOG_VERBOSE("Threads used in current run: " + std::to_string(programOptions->GetThreadsCount()) + ".\n");
	LOG_QUIET("Total CPU execution time: " + timer.format());
	LOG_QUIET("Total execution time in miliseconds: " + std::to_string(elapsed.count()) + "ms.");

	return 0;
}