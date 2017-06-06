namespace Events.Advances
{
    using System;
    using System.Runtime.CompilerServices;

    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public delegate void VideoEncodedHandler(object source, EventArgs args);

    public class VideoEncoder
    {
        // Control flow of event subscribing/unsubscribing
        private VideoEncodedHandler _VideoEncodedHandler;
        public event VideoEncodedHandler VideoEncoded
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _VideoEncodedHandler = (VideoEncodedHandler) Delegate.Combine(_VideoEncodedHandler, value); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _VideoEncodedHandler = (VideoEncodedHandler) Delegate.Remove(_VideoEncodedHandler, value); }
        }
    }
}
