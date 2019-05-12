using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelFor
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, 10, Factorial);

            Console.ReadLine();
        }
        public static void Factorial(int x)
        {
            int result = 1;

            for (int i = 1; i <= x; i++)
            {
                result *= i;
            }
            Console.WriteLine("Task: {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Factorial number {0} is: {1}", x, result);
            Thread.Sleep(3000);
        }
    }
    /// <summary>
    /// Provides support for parallel loops.
    /// </summary>
    public class Parallel
    {
        /// <summary>
        /// Parallel for loop. Invokes given action.Returns when loop finished.
        /// </summary>
        /// <param name="fromInclusive">Is the start index(inclusive)</param>
        /// <param name="toExclusive">Is the end index(exclusive)</param>
        /// <param name="body">Is the delegate that is invoked once per iteration.</param>
        public static void For(int fromInclusive, int toExclusive, Action<int> body)
        {
            int chunkSize = 4;

            int threadCount = Environment.ProcessorCount;
            int index = fromInclusive - chunkSize;
            var locker = new object();
            Action process = delegate ()
            {
                while (true)
                {
                    int chunkStart = 0;
                    lock (locker)
                    {
                        index += chunkSize;
                        chunkStart = index;
                    }
                    for (int i = chunkStart; i < chunkStart + chunkSize; i++)
                    {
                        if (i >= toExclusive) return;
                        body(i);
                    }
                }
            };
            IAsyncResult[] asyncResults = new IAsyncResult[threadCount];
            for (int i = 0; i < threadCount; ++i)
            {
                asyncResults[i] = process.BeginInvoke(null, null);
            }
            for (int i = 0; i < threadCount; ++i)
            {
                process.EndInvoke(asyncResults[i]);
            }
        }
    }
}
