namespace Delegates
{
    using System;

    class Program
    {

        public delegate void ApplicationStartHandler(string str);

        static void Main(string[] args)
        {
           ApplicationStartHandler ap = new ApplicationStartHandler(ApplicationStarted);

           ap.Invoke(Environment.MachineName);
            
        }

        static void ApplicationStarted(string str)
        {
            Console.WriteLine($"You've just started the application on {str}");
        }
    }
}
