namespace Observer.Events
{
    using System;
    using System.Threading;

    internal class Admin : Subject
    {
        public void SendNotification()
        {
            Console.WriteLine("Sending emails to all subscribers...");
            Thread.Sleep(2000);
            this.Notify();
        }
    }
}