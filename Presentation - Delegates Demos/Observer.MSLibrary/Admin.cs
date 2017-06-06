namespace Observer.MSLibrary
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class Admin<T> : IObservable<T> where T : Referee
    {
        private readonly ICollection<IObserver<T>> _observers = new Collection<IObserver<T>>();

        public void SendNotification()
        {
            foreach (var observer in _observers.ToArray())
            {
                if(_observers.Contains(observer))
                    observer.OnCompleted();
            }

            _observers.Clear();
        }

        internal class Unsubscriber : IDisposable
        {
            private readonly ICollection<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers,observer);
        }
    }
}