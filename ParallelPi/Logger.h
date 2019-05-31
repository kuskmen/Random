#pragma once
#include <future>

#include "ProgramOptions.h"

class Logger
{
public:
	Logger(ProgramOptions&);
	Logger(Logger const&) = delete;
	void operator=(Logger const&) = delete;

	void Log(std::string, LogLevel);
	//bool ShouldLog(LogLevel) const;

private:
	ProgramOptions _options;
	std::mutex display_mutex;
};

#define LOG_VERBOSE(msg) \
	Logger::Log(msg, LOG_LEVEL_VERBOSE)

#define LOG_QUIET(msg) \
	Logger::Log(msg, LOG_LEVEL_QUIET)
