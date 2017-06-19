namespace Publishers
{
    using System;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var publisher = new BasicPublisher())
            {
                publisher.DeclareExchange(name: "Hello World", type: "fanout");
                while (true)
                {
                    publisher.Publish(msg: $"Hello World! {DateTime.Now}", exchangeName: "Hello World", routingKey: "");
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
