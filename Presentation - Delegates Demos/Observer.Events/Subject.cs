namespace Observer.Events
{
    internal delegate void Notify();

    internal abstract class Subject
    {
        public event Notify NotifyEvent;

        public void Subscribe(Notify ob)
        {
            NotifyEvent += ob;
        }

        public void Unsubscribe(Notify ob)
        {
            NotifyEvent -= ob;
        }

        public void Notify()
        {
            NotifyEvent?.Invoke();
        }
    }
}