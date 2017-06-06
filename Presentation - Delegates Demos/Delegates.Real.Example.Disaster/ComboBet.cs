namespace Delegates.Real.Example.Disaster
{
    using System;
    public class ComboBet : IBet
    {
        public void PlaceBet()
        {
            // Place the bet
            Console.WriteLine("Placing combo bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{this}, placed!");

            // Save to database
            DbController.SaveBet(this);
        }
    }
}