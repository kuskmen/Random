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

std::mutex display_mutex;
std::mutex addition_mutex;

void inc_multiply(mpfr_t fact, long start, long end)
{
	for (; start <= end; ++start)
		mpfr_mul_ui(fact, fact, start, MPFR_RNDN);
}

void calculatePi(mpfr_t pi, int count, ...)
{
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
		mpfr_t previous_fac_start;
		mpfr_init(previous_fac_start);
		mpfr_set_d(previous_fac_start, 1.0, MPFR_RNDN);
		mpfr_t previous_fac_4xstart;
		mpfr_init(previous_fac_4xstart);
		mpfr_set_d(previous_fac_4xstart, 1.0, MPFR_RNDN);

		for (; boundary.first < boundary.second; ++boundary.first)
		{
			mpfr_t fac_start;
			mpfr_init(fac_start);
			mpfr_mul_ui(fac_start, previous_fac_start, (boundary.first == 0 ? 1 : boundary.first), MPFR_RNDN);

			mpfr_t fac_4xstart;
			mpfr_init(fac_4xstart);
			mpfr_t fact;
			mpfr_init(fact);
			mpfr_set_d(fact, 1.0, MPFR_RNDN);
			inc_multiply(fact, (4 * boundary.first - 3) < 0 ? 1 : 4 * boundary.first - 3, 4 * boundary.first);
			mpfr_mul(fac_4xstart, previous_fac_4xstart, fact, MPFR_RNDN);

			mpfr_t n;
			mpfr_init(n);
			mpfr_mul_ui(n, fac_4xstart, 1103 + 26390 * boundary.first, MPFR_RNDN);

			mpfr_t d;
			mpfr_t power;
			mpfr_init(d);
			mpfr_init(power);
			mpfr_pow_si(d, fac_start, 4, MPFR_RNDN);
			mpfr_ui_pow_ui(power, 396, 4 * boundary.first, MPFR_RNDN);
			mpfr_mul(d, d, power, MPFR_RNDN);

			// partition += (n / d);
			mpfr_t n_div_d;
			mpfr_init(n_div_d);
			mpfr_div(n_div_d, n, d, MPFR_RNDN);
			mpfr_add(partition, partition, n_div_d, MPFR_RNDN);

			mpfr_set(previous_fac_start, fac_start, MPFR_RNDN);
			mpfr_set(previous_fac_4xstart, fac_4xstart, MPFR_RNDN);

			mpfr_clear(d);
			mpfr_clear(power);
			mpfr_clear(n);
			mpfr_clear(fact);
			mpfr_clear(fac_start);
			mpfr_clear(fac_4xstart);
		}

		mpfr_clear(previous_fac_start);
		mpfr_clear(previous_fac_4xstart);
	}

	va_end(ranges);

	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - clock_start;
	LOG_VERBOSE("Thread id: " + thread_id + " stopped.\n");
	LOG_VERBOSE("Thread " + thread_id + " execution time was " + std::to_string(elapsed.count()) + "ms\n");

	mpfr_t result;
	mpfr_init(result);
	mpfr_sqrt_ui(result, 2, MPFR_RNDN);

	mpfr_mul_ui(result, result, 2, MPFR_RNDN);
	mpfr_div_ui(result, result, 9801, MPFR_RNDN);
	mpfr_mul(result, result, partition, MPFR_RNDN);

	addition_mutex.lock();
	mpfr_add(pi, pi, result, MPFR_RNDN);
	addition_mutex.unlock();
	mpfr_clear(result);
}

void calculate(ProgramOptions * options, mpfr_t pi)
{
	std::vector<std::thread> threads;

	// Choose thread work sepration strategy
	// TODO: Fix duplication, think of dynamic creation of strategies.
	if (options->IsOptimized())
	{
		auto strategy = new OptimizedSeparationStrategy();
		auto partitions = strategy->Separate(options->GetIterations(), options->GetThreadsCount());

		for (auto partition : partitions)
			threads.emplace_back(calculatePi, std::ref(pi), 2, partition.first, partition.second);
	}
	else
	{
		auto strategy = new NaiveSeparationStrategy();
		auto partitions = strategy->Separate(options->GetIterations(), options->GetThreadsCount());

		for (auto partition : partitions)
			threads.emplace_back(calculatePi, std::ref(pi), 1, partition);
	}

	
	for (auto threadId = 0; threadId < threads.size(); ++threadId)
		threads[threadId].join();
}

int main(int argc, const char* argv[])
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

	// Set precision for the output and data type
	// Ramanujan formula calculates 8 digits each iteration
	mpfr_set_default_prec(programOptions->GetIterations() * 8);

	mpfr_t pi;
	mpfr_init(pi);
	mpfr_set_d(pi, 0.0, MPFR_RNDN);

	boost::timer::cpu_timer timer;
	auto start = std::chrono::system_clock::now();
	calculate(programOptions.get(), pi);
	std::chrono::duration<double> elapsed = std::chrono::system_clock::now() - start;

	mpfr_exp_t exp;
	auto pi_str = std::string(mpfr_get_str(NULL, &exp, 10, programOptions->GetIterations() * 8, pi, MPFR_RNDN));
	LOG_QUIET("1/pi: " + std::to_string(exp) + "." + pi_str + "\n");
	LOG_VERBOSE("Threads used in current run: " + std::to_string(programOptions->GetThreadsCount()) + "\n.");
	LOG_QUIET("Total CPU execution time: " + timer.format());
	LOG_QUIET("Total execution time in miliseconds: " + std::to_string(elapsed.count()) + "ms.");

	return 0;
}
