namespace Events.Real.Example
{
    using System;

    public class ComboBet : Bet
    {
        public override void PlaceBet()
        {
            // Place the bet
            Console.WriteLine("Placing combo bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{this}, placed!");

            // Raise an event
            OnBetPlaced(this, new BetArgs());
        }
    }
}