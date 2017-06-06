using System;
using System.Security.Cryptography.X509Certificates;

namespace Events.Real.Example
{
    /// <summary>
    /// Interface that provides placing bet logic.
    /// </summary>
    public interface IBet
    {
        /// <summary>
        /// Event that will be raised when bet is placed.
        /// </summary>
        event EventHandler<BetArgs> BetPlaced; 
        
        /// <summary>
        /// Method that implements how bet is placed.
        /// </summary>
        void PlaceBet();
    }
}