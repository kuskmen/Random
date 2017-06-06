namespace Observer.MSLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            Admin<Referee> admin = new Admin<Referee>();
            RefereeReporter refereeReporter = new RefereeReporter("Yanislav");
            refereeReporter.Subscribe(admin);
            RefereeReporter refereeReporter2 = new RefereeReporter("Borko-poborko");
            refereeReporter2.Subscribe(admin);

            admin.SendNotification();
        }
    }
}
