namespace Delegates.Real.Example
{
    using System;
    /// <summary>
    /// Delegate that points to a method where placing bet logic is.
    /// </summary>
    /// <param name="bet">Bet that is beign placed.</param>
    /// <param name="handler">Delegate that points to a method where saving it to dabatase logic is.</param>
    public delegate void PlaceBetHandler(IBet bet, SaveBetHandler handler);

    /// <summary>
    /// Base single bet class implementing IBet logic.
    /// </summary>
    public class SingleBet : IBet
    {
        /// <summary>
        /// Logic for placing single bet.
        /// </summary>
        /// <param name="bet">Bet that is going to be placed.</param>
        /// <param name="handler">Delegate that points to a method where saving it to dabatase logic is.</param>
        public void PlaceBet(IBet bet, SaveBetHandler handler)
        {
            // Place the bet
            Console.WriteLine("Placing single bet logic method invoked...");
            System.Threading.Thread.Sleep(2000);

            // Eventual log
            Console.WriteLine($"{bet} is placed!");

            // Save to database
            handler.Invoke(bet);
        }
    }
}