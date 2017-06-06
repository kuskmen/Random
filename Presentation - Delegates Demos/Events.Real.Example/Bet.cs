using System;

namespace Events.Real.Example
{
    public abstract class Bet : IBet
    {
        public virtual event EventHandler<BetArgs> BetPlaced;
        public abstract void PlaceBet();
        public virtual void OnBetPlaced(object source, BetArgs betArgs)
        {
            EventHandler<BetArgs> del = BetPlaced as EventHandler<BetArgs>;
            del?.Invoke(this, betArgs);
        }
    }
}