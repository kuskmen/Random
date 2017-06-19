namespace TestConsumer
{
    using System;
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class BasicConsumer : IDisposable
    {
        private IConnection Connection { get; set; }
        private IModel Channel { get; set; }

        private bool _disposed;

        public BasicConsumer()
        {
            Connection = new ConnectionFactory {HostName = "localhost"}.CreateConnection();
            Channel = Connection?.CreateModel();
            _disposed = false;
        } 

        public void DeclareExchange(string name, string type)
        {
            Channel?.ExchangeDeclare(exchange: name, type: type);
        }

        public void DeclareQueue(string exchangeName, string routingKey)
        {
            Channel?.QueueBind(
                queue: "Hello World",
                exchange: "Hello World",
                routingKey: "");
        }

        public void DeclareConsumer()
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, args) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(args.Body));
            };
        }

        public void Consume(string queueName, IBasicConsumer consumer)
        {
            while (true)
            {
                Channel?.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) { return;}
            if (disposing)
            {
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
                if (Channel != null)
                {
                    Channel.Dispose();
                    Channel = null;
                }
                _disposed = true;
            }
        }
    }
}
