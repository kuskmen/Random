namespace Publishers
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    public class BasicPublisher : IDisposable
    {
        private IConnection Connection { get; set; }
        private IModel Channel { get; set; }

        private bool _disposed;

        public BasicPublisher()
        {
            Connection = new ConnectionFactory { HostName = "localhost" }.CreateConnection();
            Channel = Connection?.CreateModel();
            _disposed = false;
        }

        public void DeclareExchange(string name, string type)
        {
            Channel.ExchangeDeclare(exchange: name, type: type);
        }

        public void Publish(string msg, string exchangeName, string routingKey)
        {
            Channel.BasicPublish(
                exchange: exchangeName, 
                routingKey: routingKey, 
                body: Encoding.UTF8.GetBytes(msg));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) { return; }
            if (!disposing) { return;}

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
