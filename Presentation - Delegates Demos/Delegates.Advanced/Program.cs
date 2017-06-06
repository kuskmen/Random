using System;
using System.Runtime.Remoting.Messaging;

namespace Delegates.Advanced
{
    class Program
    {

        public delegate string AsyncMethodHandler(int callDuration, out int threadId);


        public static string AsyncMethod(int callDuration, out int threadId)
        {
            // Log
            Console.WriteLine("AsyncMethod call begin.");

            // Simulate work
            System.Threading.Thread.Sleep(callDuration);

            // Finish
            threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            return $"My call time was {callDuration}";
        }
        
        static void Main(string[] args)
        {

            int threadId;

            AsyncMethodHandler ah = new AsyncMethodHandler(Program.AsyncMethod);

            ah.BeginInvoke(9000, 
                out threadId, 
                new AsyncCallback(CallbackMethod), 
                "The call executed on thread {0}, with return value: \n\"{1}\".");

            Console.WriteLine($"The main thread {System.Threading.Thread.CurrentThread.ManagedThreadId} continues to execute...");
            System.Threading.Thread.Sleep(4000);

            Console.WriteLine($"Main thread {System.Threading.Thread.CurrentThread.ManagedThreadId} ends.");
        }

        static void CallbackMethod(IAsyncResult ar)
        {
            // ar -> delegate which comes from BeginInvoke method
            // so we retrieve it
            AsyncResult async = (AsyncResult)ar;
            AsyncMethodHandler asyncCaller = (AsyncMethodHandler) async.AsyncDelegate;

            // these are parameters that we passed to our async method
            string formatString = (string) ar.AsyncState;
            int threadId = 0;

            Console.WriteLine(formatString, threadId, asyncCaller.EndInvoke(out threadId, ar));

        }
    }
}
