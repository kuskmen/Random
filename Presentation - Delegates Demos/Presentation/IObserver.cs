namespace Presentation
{
    /// <summary>
    /// Provides interface about Updating for Objects that should be notified.
    /// </summary>
    internal interface IObserver
    {
        /// <summary>
        /// Logic that needs to be done once this Observer is notified
        /// from a subject.
        /// </summary>
        void Update();
    }
}