#include <iostream>

#include "Logger.h"

Logger::Logger(ProgramOptions& options) : _options(options) { }

void Logger::Log(std::string msg, LogLevel level)
{
	if (_options.GetLogLevel() <= level)
	{
		// TODO different streams
		display_mutex.lock();
		std::cout << msg << std::endl;
		display_mutex.unlock();
	}
}