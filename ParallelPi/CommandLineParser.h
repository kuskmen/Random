#pragma once
#include <boost/program_options.hpp>

#include "ProgramOptions.h"

class CommandLineParser
{
public:

	CommandLineParser(CommandLineParser const&) = delete;
	void operator=(CommandLineParser const&)	= delete;
	
	void Parse(int argc, const char* argv[], ProgramOptions&);

	static CommandLineParser& GetInstance();

private:
	CommandLineParser() { }
};



