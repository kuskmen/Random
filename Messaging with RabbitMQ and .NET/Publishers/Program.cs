namespace Publishers
{
    using System;
    using System.Text;
    using System.Threading;

    using RabbitMQ.Client;

    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "Hello World", type: "fanout");
                    while (true)
                    {
                        channel.BasicPublish(exchange: "Hello World",
                            routingKey: "",
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes("Hello World!"));

                        Console.WriteLine(" [x] Sent Hello World!");
                        Thread.Sleep(5000);
                    }
                }
            }
        }
    }
}
