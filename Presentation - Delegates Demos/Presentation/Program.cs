namespace Presentation

{
    internal class Program
    {
        static void Main(string[] args)
        {
            Admin admin = new Admin();

            admin.Subscribe(new Referee("Yanislav"));
            admin.Subscribe(new Referee("Shtoino"));

            admin.SendNotification();

        }
    }
}
