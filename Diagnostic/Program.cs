using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Session;

namespace Diagnostic
{
    class Program
    {
        private static readonly CancellationTokenSource Source = new CancellationTokenSource();
        private const string MyEtwProvider = "ETWPresentation-Product-Component";

        static void Main(string[] args)
        {
            Task eventReader = new Task(ReadEvents);
            eventReader.Start();

            Console.WriteLine("0 - exit, 1 - send event, 2 - start, 3 - stop, 4 - exception, 5 - GC collect");
            while (!Source.Token.IsCancellationRequested)
            {                
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.KeyChar)
                {
                    case '0':
                        Source.Cancel();
                        break;
                    case '1':
                        SimpleLogger.Log.UserInput("1");
                        break;
                    case '2':
                        SimpleLogger.Log.Start();
                        break;
                    case '3':
                        SimpleLogger.Log.Stop();
                        break;
                    case '4':
                        try
                        {
                            throw new Exception("My exception");
                        }
                        catch
                        {
                            
                        }
                        break;
                    case '5':
                        GC.Collect(2);
                        break;
                }
            }

            eventReader.Wait();
        }

        private static void ReadEvents()
        {
            using (TraceEventSession session = new TraceEventSession("MySession"))
            {
                session.StopOnDispose = true;
                session.EnableProvider(MyEtwProvider);
                session.EnableProvider(
                    ClrTraceEventParser.ProviderGuid,
                    TraceEventLevel.Verbose,
                    (ulong)(ClrTraceEventParser.Keywords.GC | ClrTraceEventParser.Keywords.Exception));

                session.Source.Clr.GCHeapStats += (GCHeapStatsTraceData data) => 
                    Console.WriteLine("GCHeapStats {0}", data.GenerationSize0.ToString("N1"));
                session.Source.Clr.ExceptionStart += Print;
                session.Source.Dynamic.All += Print;

                Task.Run(() => { session.Source.Process(); });

                Source.Token.WaitHandle.WaitOne();
                session.Stop(true);
            }
        }

        private static void Print(TraceEvent data)
        {
            if (string.IsNullOrEmpty(data.FormattedMessage))
            {
                String message = data.EventName + ":";
                foreach (String name in data.PayloadNames)
                {
                    message += " " + name + " - " + data.PayloadByName(name);
                }
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine(data.FormattedMessage);
            }
        }
    }
}
