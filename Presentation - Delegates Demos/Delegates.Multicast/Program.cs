namespace Delegates.Multicast
{
    using System;
    using System.Reflection;

    class Program
    {

        public delegate void ApplicationStartHandler(string str);

        public delegate string OopsApplicationStartHandler();

        static void Main(string[] args)
        {
            ApplicationStartHandler ah = new ApplicationStartHandler(ApplicationStarted);
            ApplicationStartHandler ah1 = new ApplicationStartHandler(DoStuff);
            ApplicationStartHandler ah2 = new ApplicationStartHandler(HelloWorld);

            // smart 
            ah += ah1 + ah2;

            ah.Invoke(Environment.MachineName);


            OopsApplicationStartHandler oah = new OopsApplicationStartHandler(OopsMethod);
            OopsApplicationStartHandler oah1 = new OopsApplicationStartHandler(OopsMethod2);
            OopsApplicationStartHandler oah2 = new OopsApplicationStartHandler(OopsMethod3);

            // Not so smart...
            oah += oah1 + oah2;
            Console.WriteLine(oah.Invoke());


            // Wut did I just did ? o.O
            //if (oah.Target == null)
            //{
            //    Console.WriteLine(oah.Target);
            //}

            //Console.WriteLine(null);
        }

        public static void ApplicationStarted(string str)
        {
            Console.WriteLine($"Application started from {str}");
        }
        public static void DoStuff(string str)
        {
            Console.WriteLine($"{str} is amazing and I love it!");
        }
        public static void HelloWorld(string str)
        {
            Console.WriteLine("SWAG Hello World program!");
        }

        public static string OopsMethod()
        {
            return $"{MethodBase.GetCurrentMethod().Name}";
        }
        public static string OopsMethod2()
        {
            return $"{MethodBase.GetCurrentMethod().Name}";
        }
        public static string OopsMethod3()
        {
            return $"{MethodBase.GetCurrentMethod().Name}";
        }
    }
}
