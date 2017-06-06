namespace Delegates.Real.Example.Disaster
{
    class Program
    {
        static void Main(string[] args)
        {
            SingleBet singleBet = new SingleBet();
            ComboBet comboBet = new ComboBet();

            singleBet.PlaceBet();
            comboBet.PlaceBet();
        }
    }
}
