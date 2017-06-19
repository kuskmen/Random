#pragma once
#include <functional>
#include <io.h>
#include <memory>
#include <event2\event.h>
#include <amqpcpp\libevent.h>

#define STDIN_FILENO 0

class ErrorHandler : public AMQP::LibEventHandler 
{
private:
	struct event_base* evbase_{ nullptr };

public:
	ErrorHandler(struct event_base* evbase) : LibEventHandler(evbase), evbase_(evbase_) {}
	void onError(AMQP::TcpConnection *connection, const char * message) override {
		std::printf("Error: %s\n", message);
		event_base_loopbreak(evbase_);
	}
};

class ConnectionHander {
public:
	using EventBasePtrT = std::unique_ptr< struct event_base, std::function<void(struct event_base*)> >;
	using EventPtrT = std::unique_ptr< struct event, std::function< void(struct event*)> >;

	ConnectionHander() : evbase_(event_base_new(), event_base_free),
		stdin_event_(event_new(evbase_.get(), STDIN_FILENO, EV_READ, stop, evbase_.get()), event_free),
		evHandler_(evbase_.get())
	{
		event_add(stdin_event_.get(), nullptr);
	}

	void Consume() {
		std::printf("Waiting for messages. Press enter to exit.\n");
		event_base_dispatch(evbase_.get());
	}

	void Terminate() {
		event_base_loopbreak(evbase_.get());
	}

	operator AMQP::TcpHandler*() {
		return &evHandler_;
	}

private:
	static void stop(evutil_socket_t fd, short what, void *evbase) {
		std::printf("Safely braking event loop.\n");
		event_base_loopbreak(reinterpret_cast<event_base*>(evbase));
	};

	EventBasePtrT evbase_;
	EventPtrT stdin_event_;
	ErrorHandler evHandler_;
};