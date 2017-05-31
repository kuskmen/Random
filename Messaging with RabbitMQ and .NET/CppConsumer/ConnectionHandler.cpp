#include "stdafx.h"

class ConnectionHandler : public AMQP::ConnectionHandler {
	
	/**
	 *  Method that is called by the AMQP library every time it has data
	 *  available that should be sent to RabbitMQ.
	 *  @param  connection  pointer to the main connection object
	 *  @param  data        memory buffer with the data that should be sent to RabbitMQ
	 *  @param  size        size of the buffer
	 */
	virtual void onData(AMQP::Connection *connection, const char *data, size_t size)
	{
		// @todo
		//  Add your own implementation, for example by doing a call to the
		//  send() system call. But be aware that the send() call may not
		//  send all data at once, so you also need to take care of buffering
		//  the bytes that could not immediately be sent, and try to send
		//  them again when the socket becomes writable again
		
	}

	/**
	*  Method that is called by the AMQP library when the login attempt
	*  succeeded. After this method has been called, the connection is ready
	*  to use.
	*  @param  connection      The connection that can now be used
	*/
	virtual void onConnected(AMQP::Connection *connection)
	{
		// @todo
		//  add your own implementation, for example by creating a channel
		//  instance, and start publishing or consuming
		AMQP::Channel channel(connection);

		channel.declareExchange("Hello World", AMQP::fanout)
			.onSuccess([]() {
				std::printf("Exchange was created sucessfully!\n");
			})
			.onError([](const char* message) {
				std::printf("Something went wrong creating exchange! %s\n", message);
			});
		
		channel.declareQueue()
			.onSuccess([]() {
				std::printf("Queue created successfully!\n");
			})
			.onError([](const char* message) {
				std::printf("Something went wrong creating queue! %s\n", message);
			});
		
			auto startCb = []() {
				std::printf("Consuming started...");
			};

			auto errorCb = [](const char* message) {
				std::printf("Something went wrong while consuming... %s\n", message);
			};

			auto msgCb = [&channel](const AMQP::Message &message, std::uint64_t deliveryTag, bool redelivered) {
				std::printf("Message recieved: %s\n", message.message);
			};

			channel.consume("Hello World")
				.onReceived(msgCb)
				.onError(errorCb)
				.onSuccess(startCb);

	}

	/**
	*  Method that is called by the AMQP library when a fatal error occurs
	*  on the connection, for example because data received from RabbitMQ
	*  could not be recognized.
	*  @param  connection      The connection on which the error occured
	*  @param  message         A human readable error message
	*/
	virtual void onError(AMQP::Connection *connection, const char *message)
	{
		// @todo
		//  add your own implementation, for example by reporting the error
		//  to the user of your program, log the error, and destruct the
		//  connection object because it is no longer in a usable state
		std::printf("Something went wrong while making the connection... %s\n", message);
		connection->close();
		delete connection;
	}

	/**
	*  Method that is called when the connection was closed. This is the
	*  counter part of a call to Connection::close() and it confirms that the
	*  connection was correctly closed.
	*
	*  @param  connection      The connection that was closed and that is now unusable
	*/
	virtual void onClosed(AMQP::Connection *connection) 
	{
		std::printf("Connection was closed!\n");
	}

};