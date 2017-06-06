namespace Observer.Events
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Admin ad = new Admin();

            ad.Subscribe(new Referee("Yanislav").Update);
            ad.Subscribe(new Referee("Shtoino").Update);

            ad.SendNotification();
        }
    }
}
