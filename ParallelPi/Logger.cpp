#include <iostream>
#include <fstream>

#include "Logger.h"
#include "ProgramOptions.h"

ProgramOptions* Logger::options;

Logger::Logger(std::string fileName)
{
	std::ofstream file{ fileName };
}

void Logger::Log(std::string msg, LogLevel level)
{
	if (options->GetLogLevel() <= level)
	{
		display_mutex.lock();
		std::ofstream logFile;
		logFile.open(options->GetFileName(), std::ios::in | std::ios::app);
		logFile << msg;
		logFile.close();
		display_mutex.unlock();
	}
}

Logger* Logger::instance()
{
	static Logger instance(options->GetFileName());
	return &instance;
}

std::string pp::to_string(LogLevel level)
{
	switch (level)
	{
		case LOG_LEVEL_VERBOSE:
			return "Verbose";
		case LOG_LEVEL_QUIET:
			return "Quiet";
		default:
			return "Unspecified";
	}
}