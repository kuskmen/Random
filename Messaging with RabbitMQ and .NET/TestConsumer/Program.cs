using System;
using System.Text;
using RabbitMQ.Client.Events;

namespace TestConsumer
{
    using RabbitMQ.Client;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "Hello World", type: "fanout");

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(
                        queue: queueName,
                        exchange: "Hello World",
                        routingKey: ""
                        );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, eventArgs) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(eventArgs.Body));
                    };
                    while (true)
                    {
                        channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                    }
                }
            }
        }
    }
}
