namespace Delegates.MemoryLeaks
{
    using System;

    public class A
    {
        public Action X { get; set; }

        public A(Action action)
        {
            this.X = action;
        }
    }

    class B
    {
        public B(A a)
        {
            Console.WriteLine("b created");
            a.X = z;
        }

        ~B()
        {
            Console.WriteLine("b finalized");
        }

        private void z()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Action ac = () => { };

            //StrongReference(ac);
            //WeakReference(ac);        
        }

        public static void StrongReference(Action ac)
        {
            Console.WriteLine("Using strong reference!\n");
            A a = new A(ac);
            new B(a);
           
            Console.WriteLine("First try(strong reference): Delegate reference is still alive,\n " +
                              "but we call garbage collector anyway.\n");
            GC.Collect();
            Console.ReadLine();

            Console.WriteLine("Second try(strong reference): Delegate reference is null \n" +
                              "and we call garbage collector again.\n");
            a.X = null;
            GC.Collect();
            Console.ReadLine();
        }

        public static void WeakReference(Action ac)
        {
            // weak reference
            var weak = new WeakReference(new A(ac), false);
            new B((A)weak.Target);
            Console.WriteLine("First try(WeakReference): Delegate reference is still alive,\n " +
                  "but we call garbage collector anyway.\n");
            GC.Collect();
            Console.ReadLine();

            (weak.Target as A).X = null;
            Console.WriteLine("Second try(WeakReference): Delegate reference is null \n" +
                              "and we call garbage collector again.\n");
            GC.Collect();
            Console.ReadLine();
        }
    }
}
