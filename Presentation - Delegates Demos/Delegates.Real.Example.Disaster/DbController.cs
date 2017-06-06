namespace Delegates.Real.Example.Disaster
{
    using System;

    public static class DbController
    {
        public static void SaveBet(IBet bet)
        {
            Console.WriteLine($"Saving {bet} in database...");
            System.Threading.Thread.Sleep(3000);
        }
    }
}