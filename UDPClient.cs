using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Serilog;
using Serilog.Sinks.Network;

class Program
{
    private static DateTimeOffset startTime;
    private static Thread[] threads;
    private static int numberOfThreads = 10;
    private static Stopwatch stopwatch = new Stopwatch();

    static void Main(string[] args)
    {
        startTime = DateTimeOffset.UtcNow;

        Log.Logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .WriteTo.UDPSink("127.0.0.1", 50000, new Serilog.Formatting.Display.MessageTemplateTextFormatter("{Message}"))
         .CreateLogger();

        threads = new Thread[numberOfThreads];

        for (int i = 0; i < 10; i++)
        {
            int threadId = i + 1;
            threads[i] = new Thread(Metod);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine($"Total execution time: {(DateTimeOffset.UtcNow - startTime).TotalSeconds} seconds");

        Log.CloseAndFlush();
    }

    private static void Metod()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 100000; i++)
        {
            var sc = (DateTimeOffset.UtcNow - startTime).TotalSeconds;
            string message = $" {sc} | Thread {Thread.CurrentThread.ManagedThreadId} | Logging message {i}";
            sb.AppendLine(message);

            if (i % 1000 == 0)
            {
                Log.Information(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0)
        {
            Log.Information(sb.ToString());
        }
    }
}
