#pragma once

enum LogLevel : int8_t { VERBOSE, QUIET };

class ProgramOptions
{
public:
	ProgramOptions();
	ProgramOptions(LogLevel, short, long, long);

	inline void SetLogLevel(LogLevel const logLevel) { _logLevel = logLevel; }
	inline void SetThreadsCount(short const threadsCount) { _threadsCount = threadsCount; }
	inline void SetPrecision(long const precision) { _precision = precision; }
	inline void SetIterations(long iterations) { _iterations = iterations; }

	inline LogLevel const GetLogLevel() { return _logLevel; }
	inline short const GetThreadsCount() { return _threadsCount; }
	inline long const GetPrecision() { return _precision; }
	inline long const GetIterations() { return _iterations; }

private:
	LogLevel _logLevel;
	short _threadsCount;
	long _precision;
	long _iterations;
};

