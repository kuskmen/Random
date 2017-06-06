namespace Observer.MSLibrary
{
    using System;
    using System.Threading;

    internal class RefereeReporter : IObserver<Referee>
    {
        private IDisposable _unsubscriber;

        public RefereeReporter(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public virtual void Subscribe(IObservable<Referee> provider)
        {
            if (provider != null)
                _unsubscriber = provider.Subscribe(this);
        }

        public void OnNext(Referee referee)
        {
            Console.WriteLine($"OnNext method. {referee.Name}");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Exception was thrown while trying to send report to {this.Name}!");
        }

        public void OnCompleted()
        {
            Console.WriteLine($"Sending email to {this.Name}...");
            Thread.Sleep(1000);
            this.Unsubscribe();
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

    }
}