#include <iostream>

#include "ProgramOptions.h"

ProgramOptions::ProgramOptions() : ProgramOptions(LOG_LEVEL_QUIET, 1, 1024, 1024) { }

ProgramOptions::ProgramOptions(LogLevel logLevel, short threadsCount, long precision, long iterations)
	: _logLevel(logLevel), _threadsCount(threadsCount), _precision(precision), _iterations(iterations) { }
