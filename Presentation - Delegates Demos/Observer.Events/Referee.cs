namespace Observer.Events
{
    using System;

    internal class Referee : IObserver
    {
        private readonly string _name;

        public Referee(string name)
        {
            this._name = name;
        }

        public void Update()
        {
            Console.WriteLine($"{_name} is notified.");
        }
    }
}