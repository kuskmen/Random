namespace Events.Real.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            DbController dc = new DbController();
            
            SingleBet sb = new SingleBet();
            ComboBet cb = new ComboBet();

            // subscribe
            sb.BetPlaced += dc.OnBetPlaced;
            cb.BetPlaced += dc.OnBetPlaced;

            sb.PlaceBet();
            cb.PlaceBet();

            // unsubscribe
            sb.BetPlaced -= dc.OnBetPlaced;
            cb.BetPlaced -= dc.OnBetPlaced;
        }
    }
}

