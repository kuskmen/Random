#pragma once
#include <string>

#include "Logger.h"

class ProgramOptions
{
public:
	ProgramOptions();
	ProgramOptions(LogLevel, short, long, long);

	inline void SetLogLevel(LogLevel const logLevel) { _logLevel = logLevel; }
	inline void SetThreadsCount(short const threadsCount) { _threadsCount = threadsCount; }
	inline void SetPrecision(long const precision) { _precision = precision; }
	inline void SetIterations(long iterations) { _iterations = iterations; }
	inline void SetOutputFileName(std::string name) { _outputFileName = name; }

	inline LogLevel GetLogLevel() { return _logLevel; }
	inline short const GetThreadsCount() { return _threadsCount; }
	inline long const GetPrecision() { return _precision; }
	inline long const GetIterations() { return _iterations; }
	inline std::string const GetFileName() { return _outputFileName; }

private:
	LogLevel _logLevel;
	short _threadsCount;
	long _precision;
	long _iterations;
	std::string _outputFileName;
};

