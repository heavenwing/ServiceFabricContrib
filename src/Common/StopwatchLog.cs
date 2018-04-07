using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    //http://lancelarsen.com/stopwatchlog-easy-method-timing-in-c/

    /// <summary>
    ///  Collects information about executing method and logs metrics 
    ///     when the stopwatch log is disposed.
    /// </summary>
    /// <example>
    ///   using (StopwatchLog.Track())
    ///   {
    ///     // Code here...
    ///   }
    /// </example>
    public sealed class StopwatchLog : IDisposable
    {
        public string MethodName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public DateTime StopwatchStart { get; set; }
        public DateTime StopwatchStop { get; set; }

        private StopwatchLog(string methodName, string filePath, int lineNumber)
        {
            MethodName = methodName;
            FilePath = filePath;
            LineNumber = lineNumber;
            StopwatchStart = DateTime.UtcNow;
            Stopwatch = Stopwatch.StartNew();
        }

        void IDisposable.Dispose()
        {
            Stopwatch.Stop();
            StopwatchStop = DateTime.UtcNow;
            LogMetrics();
        }

        private void LogMetrics()
        {
            Trace.TraceInformation("[Stopwatch] Method '{0}' | File '{1}' | Line Number: {2}  | Started: {3} | Ended {4} | Elapsed {5} ms",
                    MethodName,
                    FilePath,
                    LineNumber,
                    StopwatchStart.ToDateTimeHighPrecision(),
                    StopwatchStop.ToDateTimeHighPrecision(),
                    Stopwatch.ElapsedMilliseconds);
        }

        public static StopwatchLog Track(
            [CallerMemberName] string callingMethodName = "",
            [CallerFilePath] string callingFilePath = "",
            [CallerLineNumber] int callingLineNumber = 0)
        {
            return new StopwatchLog(callingMethodName, callingFilePath, callingLineNumber);
        }

        public static void TrackFunc(Action func,
            [CallerMemberName] string callingMethodName = "",
            [CallerFilePath] string callingFilePath = "",
            [CallerLineNumber] int callingLineNumber = 0)
        {
            using (new StopwatchLog(callingMethodName, callingFilePath, callingLineNumber))
            {
                func();
            }
        }

        public static async Task TrackFuncAsync(Func<Task> func,
            [CallerMemberName] string callingMethodName = "",
            [CallerFilePath] string callingFilePath = "",
            [CallerLineNumber] int callingLineNumber = 0)
        {
            using (new StopwatchLog(callingMethodName, callingFilePath, callingLineNumber))
            {
                await func();
            }
        }
    }

    public static class DateTimeExtentions
    {
        public static string ToDateTimeHighPrecision(this DateTime source)
        {
            return source.ToString("MM/dd/yyyy h:mm:ss.fff tt");
        }
    }
}
