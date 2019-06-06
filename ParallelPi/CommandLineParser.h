#pragma once
#include <boost/program_options.hpp>

#include "ProgramOptions.h"

class CommandLineParser
{
public:
	CommandLineParser(CommandLineParser const&) = delete;
	void operator=(CommandLineParser const&)	= delete;
	
	void Parse(int argc, const char* argv[], std::unique_ptr<ProgramOptions>&);

	static CommandLineParser* instance();

private:
	CommandLineParser() { }
};

#define sCommandLineParser CommandLineParser::instance()



