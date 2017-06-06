using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Presentation
{
    /// <summary>
    /// 1. It knows its observers.
    /// 2. Provides an interface for attaching and detaching them.
    /// </summary>
    internal abstract class Subject
    {
        private ICollection<IObserver> _observers = new Collection<IObserver>();

        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in _observers)
            {
                observer.Update();
            }
        }
    }
}