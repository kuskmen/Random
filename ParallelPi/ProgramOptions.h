#pragma once
#include <string>

#include "Logger.h"

class ProgramOptions
{
public:
	friend class CommandLineParser;

	ProgramOptions();
	ProgramOptions(LogLevel, short, long, bool);

	inline LogLevel GetLogLevel() { return _logLevel; }
	inline short const GetThreadsCount() { return _threadsCount; }
	inline long const GetIterations() { return _iterations; }
	inline std::string const GetFileName() { return _outputFileName; }
	inline bool const IsOptimized() { return _optimized; }

protected:
	inline void SetLogLevel(LogLevel const logLevel) { _logLevel = logLevel; }
	inline void SetThreadsCount(short const threadsCount) { _threadsCount = threadsCount; }
	inline void SetIterations(long iterations) { _iterations = iterations; }
	inline void SetOutputFileName(std::string name) { _outputFileName = name; }
	inline void SetOptimized(bool value) { _optimized = value; }

private:
	LogLevel _logLevel;
	short _threadsCount;
	long _iterations;
	std::string _outputFileName;
	bool _optimized;
};

