namespace Events.Real.Example
{
    using System;
    public class DbController
    {
        /// <summary>
        /// Method that is called upon bet that is placed.
        /// </summary>
        /// <param name="source"> The publisher sending the event.</param>
        /// <param name="args"> Additional data that is passed.</param>
        public void OnBetPlaced(object source, BetArgs args)
        {
            Console.WriteLine($"Saving {source} in database...");
            System.Threading.Thread.Sleep(2000);
        }
    }
}