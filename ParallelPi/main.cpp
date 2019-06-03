#include <boost/multiprecision/mpfr.hpp>
#include <boost/timer/timer.hpp>
#include <boost/thread/thread.hpp>

#include <iostream>
#include <future>
#include <chrono>
#include <cstdarg>

#include "CommandLineParser.h"
#include "SeparationStrategy.h"
#include "Logger.h"
#include "ProgramOptions.h"

std::mutex display_mutex;

mpfr_ptr built_in_factorial(int end)
{
	mpfr_t fact;
	mpfr_init2(fact, boost::multiprecision::mpfr_float::default_precision());
	mpfr_fac_ui(fact, end, MPFR_RNDN);

	return fact;
}

mpfr_ptr calculatePi(int count, ...)
{
	// using boost::multiprecision::mpfr_float;
	mpfr_t partition;
	mpfr_init(partition);
	mpfr_set_d(partition, 0.0, MPFR_RNDN);

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
		
		for (; boundary.first < boundary.second; ++boundary.first)
		{
			// mpfr_float fac_1_start = built_in_factorial(boundary.first);
			mpfr_t fac_1_start;
			mpfr_init(fac_1_start);
			mpfr_fac_ui(fac_1_start, boundary.first, MPFR_RNDN);

			// mpfr_float fac_1_4xStart = built_in_factorial(4 * boundary.first);
			mpfr_t fac_1_4xStart;
			mpfr_init(fac_1_4xStart);
			mpfr_fac_ui(fac_1_4xStart, 4 * boundary.first, MPFR_RNDN);

			// mpfr_float n = fac_1_4xStart * mpfr_float(1103 + 26390 * boundary.first);
			mpfr_t n;
			mpfr_init(n);
			mpfr_mul_ui(n, fac_1_4xStart, (1103 + 26390 * boundary.first), MPFR_RNDN);

			// mpfr_float d = boost::multiprecision::pow(fac_1_start, 4) * boost::multiprecision::pow((mpfr_float)396, 4 * boundary.first);
			mpfr_t d;
			mpfr_t power;
			mpfr_init(d);
			mpfr_init(power);
			mpfr_pow_si(d, fac_1_start, 4, MPFR_RNDN);
			mpfr_ui_pow_ui(power, 396, 4 * boundary.first, MPFR_RNDN);
			mpfr_mul(d, d, power, MPFR_RNDN);

			// partition += (n / d);
			mpfr_t n_div_d;
			mpfr_init(n_div_d);
			mpfr_div(n_div_d, n, d, MPFR_RNDN);
			mpfr_add(partition, partition, n_div_d, MPFR_RNDN);
		}
	}

	va_end(ranges);

	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - clock_start;
	LOG_VERBOSE("Thread id: " + thread_id + " stopped.\n");
	LOG_VERBOSE("Thread " + thread_id + " execution time was " + std::to_string(elapsed.count()) + "ms\n");
	
	//return (2 * boost::multiprecision::sqrt((mpfr_float)2) / 9801) * partition;
	mpfr_t result;
	mpfr_init(result);
	mpfr_sqrt_ui(result, 2, MPFR_RNDN);

	mpfr_mul_ui(result, result, 2, MPFR_RNDN);
	mpfr_div_ui(result, result, 9801, MPFR_RNDN);
	mpfr_mul(result, result, partition, MPFR_RNDN);

	return result;
}

int main(int argc, const char *argv[])
{
	// Parse arguments
	std::unique_ptr<ProgramOptions> programOptions(new ProgramOptions());
	sCommandLineParser->Parse(argc, argv, programOptions);

	// Set precision for the output and data type
	mpfr_set_default_prec(programOptions->GetPrecision());
	
	// Choose thread work sepration strategy
	SeparationStrategy<advanced_ranges>* strategy = new OptimizedSeparationStrategy();
	auto partitionBounds = strategy->Separate(programOptions->GetIterations(), programOptions->GetThreadsCount());
	
	// Initialize Logger options
	Logger::options = programOptions.get();

	std::vector<std::future<mpfr_ptr>> futures;

	boost::timer::cpu_timer timer;
	auto start = std::chrono::system_clock::now();
	for (auto partitionBound : partitionBounds)
	{
		if (dynamic_cast<OptimizedSeparationStrategy*>(strategy) != nullptr)
			futures.emplace_back(std::async(calculatePi, 2, partitionBound.first, partitionBound.second));
		else
			futures.emplace_back(std::async(calculatePi, 1, partitionBound));
	}
	
	mpfr_t pi;
	mpfr_init(pi);
	mpfr_set_d(pi, 0.0, MPFR_RNDN);
	for (auto threadId = 0; threadId < futures.size(); ++threadId)
		mpfr_add(pi, pi, futures[threadId].get(), MPFR_RNDN);

	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - start;

	mpfr_exp_t exp;
	auto pi_str = std::string(mpfr_get_str(NULL, &exp, 10, programOptions->GetPrecision(), pi, MPFR_RNDN));
	LOG_QUIET("1/pi: " + std::to_string(exp) + "." + pi_str + "\n");
	LOG_VERBOSE("Threads used in current run: " + std::to_string(programOptions->GetThreadsCount()) + "\n.");
	LOG_QUIET("Total CPU execution time: " + timer.format());
	LOG_QUIET("Total execution time in miliseconds: " + std::to_string(elapsed.count()) + "ms.");

	return 0;
}