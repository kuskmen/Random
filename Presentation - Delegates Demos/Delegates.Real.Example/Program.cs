namespace Delegates.Real.Example
{
    class Program
    {

        static void Main(string[] args)
        {
            // Creating instance of our dbController
            DbController dbController = new DbController();

            // Adding our method that hold saving to database logic to our invocation list
            SaveBetHandler saveHandler = new SaveBetHandler(dbController.SaveBet);

            // Creating two dummy bets
            IBet singleBet = new SingleBet();
            IBet singleBet2 = new SingleBet();
            IBet singleBet3 = new SingleBet();
            IBet comboBet = new ComboBet();

            // Creating delegates that point to appropriate placeBet functionality
            PlaceBetHandler singleBetHandler = new PlaceBetHandler(singleBet.PlaceBet);
            singleBetHandler += new PlaceBetHandler(singleBet2.PlaceBet) + new PlaceBetHandler(singleBet3.PlaceBet);
            PlaceBetHandler comBetHandler = new PlaceBetHandler(comboBet.PlaceBet);


            // Invoking them
            singleBetHandler.Invoke(singleBet, saveHandler);
            comBetHandler.Invoke(comboBet, saveHandler);
        }
    }
}
