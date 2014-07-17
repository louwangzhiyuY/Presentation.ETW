using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CounterFast
{
    class Program
    {
        [DllImport("kernel32.dll")]
        extern static int GetCurrentThreadId();

        private static readonly CancellationTokenSource Source = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Source.Cancel();
            };

            ThreadSyncTester tester = new ThreadSyncTester(Source.Token);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ExecuteTest(tester);
            stopwatch.Stop();

            Console.WriteLine("Execution time (fast): " + stopwatch.Elapsed.ToString());
            tester.PrintResult(Console.Out);
        }

        private static void ExecuteTest(ThreadSyncTester tester)
        {
            Task taskA = Task.Run(() =>
            {
                SetThreadAffinity();
                tester.ThreadA();
            });

            Task taskB = Task.Run(() =>
            {
                SetThreadAffinity();
                tester.ThreadB();
            });

            Thread.Sleep(TimeSpan.FromSeconds(5));
            Source.Cancel();

            Task.WaitAll(taskA, taskB);
        }

        private static void SetThreadAffinity()
        {
            //See: http://geekswithblogs.net/akraus1/archive/2014/04/30/156156.aspx
            Process proc = Process.GetCurrentProcess();
            var currentThread = proc.Threads.Cast<ProcessThread>().Single(t => t.Id == GetCurrentThreadId());
            currentThread.ProcessorAffinity = (IntPtr)0x03; // Force thread affinity on core 2
        }
    }
}
