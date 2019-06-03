#include <iostream>

#include "ProgramOptions.h"

ProgramOptions* Logger::options;

void Logger::Log(std::string msg, LogLevel level)
{
	if (options->GetLogLevel() <= level)
	{
		// TODO different streams
		display_mutex.lock();
		std::cout << msg << std::endl;
		display_mutex.unlock();
	}
}

Logger* Logger::instance()
{
	static Logger instance;
	return &instance;
}