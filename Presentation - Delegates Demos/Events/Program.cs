namespace Events
{
    using System;

    class Program
    {
        public delegate void ApplicationStartHandler(string str);
        public static event ApplicationStartHandler ApplicationStart;

        static void Main(string[] args)
        {
            // Subscribe to an event by attaching event handler
            Program.ApplicationStart += Program.HelloWorld;

            // Event fires
            OnApplicationStart(Environment.MachineName);

            // Unsubscribe
            Program.ApplicationStart -= Program.HelloWorld;
        }

        public static void OnApplicationStart(string str)
        {
            ApplicationStartHandler del = ApplicationStart as ApplicationStartHandler;
            del?.Invoke(str);           
        }

        public static void HelloWorld(string str)
        {
            Console.WriteLine($"{str} machine just stated the application!");
        }
    }
}
