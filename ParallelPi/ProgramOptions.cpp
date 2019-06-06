#include <iostream>

#include "ProgramOptions.h"

ProgramOptions::ProgramOptions() : ProgramOptions(LOG_LEVEL_QUIET, 1, 1024, false) { }

ProgramOptions::ProgramOptions(LogLevel logLevel, short threadsCount, long iterations, bool optimized)
	: _logLevel(logLevel), _threadsCount(threadsCount), _iterations(iterations), _optimized(optimized) { }
