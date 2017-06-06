namespace Delegates.Real.Example.Disaster
{
    using System;

    public class SingleBet : IBet
    {
        public void PlaceBet()
        {
            // Place the bet
            Console.WriteLine("Placing single bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{this} is placed!");

            // Save to database
            DbController.SaveBet(this);
        }
    }
}