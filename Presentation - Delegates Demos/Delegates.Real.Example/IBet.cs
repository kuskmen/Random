namespace Delegates.Real.Example
{
    /// <summary>
    /// Interface that provides placing bet logic.
    /// </summary>
    public interface IBet
    {
        /// <summary>
        /// Method that implements how bet is placed.
        /// </summary>
        /// <param name="bet">Bet that will be placed.</param>
        /// <param name="handler"></param>
        void PlaceBet(IBet bet, SaveBetHandler handler);
    }
}