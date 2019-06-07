#include <iostream>
#include <ctime>

#include "CommandLineParser.h"

CommandLineParser* CommandLineParser::instance()
{
	static CommandLineParser instance;

	return &instance;
}

void CommandLineParser::Parse(int argc, const char* argv[], std::unique_ptr<ProgramOptions>& programOptions)
{
	boost::program_options::variables_map arguments;
	LogLevel logLevel;

	try
	{
		boost::program_options::options_description options{ "Options" };
		options.add_options()
			("help,h", "Prints this page.")
			
			("iterations,i", 
				boost::program_options::value<long>()->value_name("(2378542374, 87235487325, ...)")->required(),
				"Use this option to denote how many terms you want to be calculated from Ramanujan's formula. This option is required.")
			
			("threads,t", 
				boost::program_options::value<short>()->default_value(1)->value_name("(1, 2, 3, ...)"),			
				"Use this option to denote how many threads do you want to use during computation. Default value is: 1.")
			
			("verbosity,v", 
				boost::program_options::value(&logLevel)->default_value(LOG_LEVEL_QUIET)->value_name("(quiet, verbose)"), 
				"Use this option to configure level of loging during the computation. Possible values are verbose and quiet, default is quiet.")

			("output,o",
				boost::program_options::value<std::string>()->value_name("(name of the output file, can be any valid file name)"),
				"Use this option to denote the name of the output file.")
		    ("optimized",
				boost::program_options::bool_switch()->default_value(false),
				"Use this option to configure optimized strategy for work separation.");

		boost::program_options::store(boost::program_options::command_line_parser(argc, argv).options(options).allow_unregistered().run(), arguments);
		boost::program_options::notify(arguments);

		time_t rawtime;
		struct tm timeinfo;

		time(&rawtime);
		localtime_s(&timeinfo, &rawtime);

		if (arguments.count("help"))
		{
			std::cout << options << std::endl;

			// if help is requested failfast
			exit(0);
		}
		if (arguments.count("verbosity"))
			programOptions->SetLogLevel(arguments["verbosity"].as<LogLevel>());
		if (arguments.count("threads"))
			programOptions->SetThreadsCount(arguments["threads"].as<short>());
		if (arguments.count("iterations"))
			programOptions->SetIterations(arguments["iterations"].as<long>());
		if (arguments.count("output"))
			programOptions->SetOutputFileName(arguments["output"].as<std::string>());
		else
			programOptions->SetOutputFileName("output-" 
				+ std::to_string(timeinfo.tm_year + 1900)
				+ std::to_string(timeinfo.tm_mday)
				+ std::to_string(timeinfo.tm_mon + 1)
				+ std::to_string(timeinfo.tm_hour)
				+ std::to_string(timeinfo.tm_min)
				+ std::to_string(timeinfo.tm_sec)
				+ ".txt");
		if (arguments.count("optimized"))
			programOptions->SetOptimized(arguments["optimized"].as<bool>());
	}
	catch (boost::program_options::error & e)
	{
		std::cerr << "ERROR: " << e.what() << std::endl << std::endl;
		exit(1);
	}
}

std::istream& operator>>(std::istream& in, LogLevel& logLevel)
{
	std::string token;
	in >> token;
	std::transform(token.begin(), token.end(), token.begin(), ::tolower);

	if (token == "verbose")
		logLevel = LOG_LEVEL_VERBOSE;
	else if (token == "quiet")
		logLevel = LOG_LEVEL_QUIET;
	else
		in.setstate(std::ios_base::failbit);
	return in;
}

std::ostream & operator<<(std::ostream & out, const LogLevel & logLevel)
{
	switch (logLevel)
	{
		case LOG_LEVEL_VERBOSE:
			return out << "verbose";
		case LOG_LEVEL_QUIET:
			return out << "quiet";
		default:
			return out;
	}

	return out;
}