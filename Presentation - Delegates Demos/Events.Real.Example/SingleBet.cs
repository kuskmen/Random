namespace Events.Real.Example
{
    using System;

    public class SingleBet : Bet
    {
        public override void PlaceBet()
        {
            // Place the bet
            Console.WriteLine("Placing single bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{this} is placed!");

            // Raise an event
            OnBetPlaced(this, new BetArgs());
        }
    }
}