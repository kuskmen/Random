#pragma once
#include <future>
#include <string>

class ProgramOptions;

enum LogLevel : int8_t { LOG_LEVEL_VERBOSE, LOG_LEVEL_QUIET };

class Logger
{
public:
	static ProgramOptions* options;
	
	Logger(Logger const&) = delete;
	void operator=(Logger const&) = delete;

	void Log(std::string, LogLevel);
	//bool ShouldLog(LogLevel) const;

	static Logger* instance();
private:
	Logger(std::string fileName);

	std::mutex display_mutex;
};

#define sLogger Logger::instance()

#define LOG_VERBOSE(msg) \
	sLogger->Log(msg, LOG_LEVEL_VERBOSE)

#define LOG_QUIET(msg__) \
	sLogger->Log(msg__, LOG_LEVEL_QUIET)
