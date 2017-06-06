using System;

namespace Delegates.Real.Example
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bet">Bet that is being saved in database.</param>
    public delegate void SaveBetHandler(IBet bet);
    
    /// <summary>
    /// Class that is responsible for saving data to the database.
    /// It doesn't need to know about the logic.
    /// </summary>
    public sealed class DbController
    {
        /// <summary>
        /// Saves bet in the database without caring what type it is 
        /// and what is the logic of placing it.
        /// </summary>
        /// <param name="bet">Generic type of bet.</param>
        public void SaveBet(IBet bet)
        {
            Console.WriteLine($"Saving {bet} in database...");
            System.Threading.Thread.Sleep(3000); 
        }
    }
}