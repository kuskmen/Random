using System;
using System.Text;
using RabbitMQ.Client.Events;

namespace TestConsumer
{
    using RabbitMQ.Client;
    using System.Threading;

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
                    channel.QueueDeclare(queue: "Hello World", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(
                        queue: "Hello World",
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
                        Thread.Sleep(4500);
                        channel.BasicConsume(queue: "Hello World", noAck: true, consumer: consumer);
                    }
                }
            }
        }
    }
}
