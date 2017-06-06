namespace Delegates.Generic
{
    using System;

    class Program
    {

        // Numeric constrained delegate
        public delegate T FormulaHandler<T>(T arg) where T : struct, IComparable, IComparable<T>,
            IConvertible, IEquatable<T>, IFormattable;

        //public delegate TResult SwagFormulaHandler<in T, out TResult>(T arg)
        //    where T : struct, IComparable, IComparable<T>,
        //        IConvertible, IEquatable<T>, IFormattable; 

        static void Main(string[] args)
        {
            
            FormulaHandler<int> fh = new FormulaHandler<int>(Square);
            FormulaHandler<float> fh1 = new FormulaHandler<float>(Square);

            Console.WriteLine(fh.Invoke(4));
            Console.WriteLine(fh1.Invoke(2.0f));

            Console.WriteLine(FormulaHandler2(2.3, Square));
        }

        public static T Square<T>(T x) where T : struct, IComparable, IComparable<T>,
            IConvertible, IEquatable<T>, IFormattable => (dynamic)x*x;

        public static T FormulaHandler2<T>(T arg, Func<T, T> result) where T : struct, IComparable, IComparable<T>,
            IConvertible, IEquatable<T>, IFormattable => result.Invoke(arg);

    }
}
