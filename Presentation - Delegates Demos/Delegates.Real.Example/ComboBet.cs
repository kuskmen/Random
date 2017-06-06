namespace Delegates.Real.Example
{
    using System;
    /// <summary>
    /// Base combo bet class implementing IBet logic
    /// </summary>
    public class ComboBet : IBet
    {
        /// <summary>
        /// Logic for placing combo bet.
        /// </summary>
        /// <param name="bet">Bet that is going to be placed.</param>
        /// <param name="handler">Delegate that points to a method where saving it to dabatase logic is.</param>
        public void PlaceBet(IBet bet, SaveBetHandler handler)
        {
            // Place the bet
            Console.WriteLine("Placing combo bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{bet}, placed!");

            // Save to database
            handler.Invoke(bet);
        }
    }
}