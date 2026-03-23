using System.Diagnostics;

namespace SFML_GE.System
{
    /// <summary>
    /// Keeps track of timings between Log watch calls
    /// </summary>
    class LogWatch
    {
        private readonly Stopwatch watch;

        public string name;

        public readonly int max_timings;

        private readonly TimeSpan[] timings;

        // used to wrap around the timings array
        // i know there is SOME like actual term for this but i cannot remember
        public int write_pos = 0;
        public int read_pos = 0;

        public LogWatch(string name, int max_timings = 10_000)
        {
            this.max_timings = max_timings;

            this.name = name;
            timings = new TimeSpan[max_timings];

            for (int i = 0; i < timings.Length; i++)
            {
                timings[i] = TimeSpan.Zero;
            }

            watch = new Stopwatch();
        }

        public static LogWatch StartNew(string name, int max_timings = 10_000)
        {
            var watch = new LogWatch(name, max_timings);
            watch.Start();
            return watch;
        }
        public void AddTiming(TimeSpan span)
        {
            timings[write_pos] = span;
            read_pos = write_pos;

            write_pos++;
            write_pos = (int)MathGE.Mod(write_pos, max_timings);
        }
        public void Start()
        {
            watch.Restart();
        }

        public void Stop()
        {
            watch.Stop();
            AddTiming(watch.Elapsed);
        }

        public TimeSpan LatestTiming()
        {
            return timings[read_pos];
        }

        public TimeSpan[] AllTimings()
        {
            TimeSpan[] sorted = new TimeSpan[max_timings];

            for (int i = 0; i < max_timings; i++)
            {
                int off = (int)MathGE.Mod(read_pos - i, max_timings);

                sorted[i] = timings[(max_timings - 1) - off];

            }

            return sorted;
        }
    }


    /// <summary>
    /// A Simple way of logging information, and timing things.
    /// </summary>
    public static class DebugLogger
    {
        static bool do_logging = false;
        static string log_path = "";
        static readonly Dictionary<string, LogWatch> watches = new Dictionary<string, LogWatch>();

        /// <summary>
        /// Enables or disables the DebugLogger from writing to a log file
        /// </summary>
        /// <param name="do_log">Enables or disables logging to an external file</param>
        /// <param name="logpath">The path to the file including extension to log to, this will clear the file!</param>
        public static void SetLogging(bool do_log, string logpath)
        {
            do_logging = do_log;
            log_path = logpath;
        }

        /// <summary> The line color for <see cref="LogInfo(string)"/> </summary>
        public static ConsoleColor InfoColor { get; set; } = ConsoleColor.Gray;

        /// <summary> The line color for <see cref="LogInfoPriority(string)"/> </summary>
        public static ConsoleColor PriorityInfoColor { get; set; } = ConsoleColor.White;

        /// <summary> The line color for <see cref="LogWarning(string)"/> </summary>
        public static ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;

        /// <summary> The line color for <see cref="LogError(string)"/> </summary>
        public static ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;

        /// <summary> The line color for <see cref="LogDebug(string, bool)"/> </summary>
        public static ConsoleColor DebugColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// If false, nothing will be written to console by this class.
        /// </summary>
        public static bool Enabled
        {
            get;
            set;
        } = true;

        static uint log_events = 0;

        static readonly object file_locker = new object();

        static void Log(string message, ConsoleColor col)
        {
            if (Enabled == false) { return; }

            // prob not the best way of doing it, but it works!
            // also locks the output!
            lock (file_locker)
            {
                if (do_logging)
                {
                    if (log_events == 0) // just to clear the file if we about to log to it
                    {
                        File.WriteAllBytes(log_path, Array.Empty<byte>());
                    }
                    log_events++;
                    File.AppendAllLines(log_path, new string[] { message });
                }



                Console.ForegroundColor = col;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Logs a message to the console with "Info" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogInfo(string message)
        {
            if (!Enabled) { return; }
            string log_string = "[Info] " + message;
            Log(log_string, InfoColor);
        }

        /// <summary>
        /// Logs a message to the console with "Priority Info" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogInfoPriority(string message)
        {
            if (!Enabled) { return; }
            string log_string = "[Info] " + message;
            Log(log_string, PriorityInfoColor);
        }

        /// <summary>
        /// Logs a message to the console with "Warning" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogWarning(string message)
        {
            if (!Enabled) { return; }
            string log_string = "[Warning] " + message;
            Log(log_string, WarningColor);
        }

        /// <summary>
        /// Logs a message to the console with "Error" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogError(string message)
        {
            if (!Enabled) { return; }
            string log_string = "[Error] " + message;
            Log(log_string, ErrorColor);
        }

        /// <inheritdoc cref="LogInfo(string)"/>
        public static void LogInfo(object message)
        {
            string output = message.ToString() ?? "null";
            LogInfo(output);
        }

        /// <inheritdoc cref="LogInfoPriority(string)"/>
        public static void LogInfoPriority(object message)
        {
            string output = message.ToString() ?? "null";
            LogInfoPriority(output);
        }

        /// <inheritdoc cref="LogWarning(string)"/>
        public static void LogWarning(object message)
        {
            string output = message.ToString() ?? "null";
            LogWarning(output);
        }

        /// <inheritdoc cref="LogError(string)"/>
        public static void LogError(object message)
        {
            string output = message.ToString() ?? "null";
            LogError(output);
        }

        /// <summary>
        /// Logs a debug message.
        /// Debug messages automatically grab the line number of the caller,
        /// as well as the memeber name, and file path, and adds it to the log.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="show_stacktrace">shows the stack trace taken to get to the function, may not be the full trace in release mode.</param>
        internal static void LogDebug(string message, bool show_stacktrace = true)
        {
            if (!Enabled) { return; }

            string log_string = "[Debug] \"" + message;

            if (show_stacktrace)
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame[] frames = stackTrace.GetFrames();

                log_string += "\"\n Stack Trace :";

                foreach (StackFrame frame in frames)
                {
                    if (frame.GetMethod() == null) { break; }


                    string class_name = frame.GetMethod()!.DeclaringType!.Name;

                    if (class_name == typeof(DebugLogger).Name) { continue; }

                    string method_name = frame.GetMethod()!.Name;

                    log_string += $"\n\t-> {class_name}.{method_name}()";
                }
            }

            Log(log_string, DebugColor);
        }

        /// <inheritdoc cref="LogDebug(string, bool)"/>
        public static void LogDebug(object? message, bool show_stacktrace = true)
        {
            string output = message?.ToString() ?? "null";
            LogDebug(output, show_stacktrace);
        }


        /// <summary>
        /// Starts a timer to track how long something takes
        /// To stop the timer use <see cref="StopTimer(string)"/>
        /// If a timer with the name is already running or created, it will restart it and store its timings internally
        /// You can then get these timings by using <see cref="GetTimerTimings(string)"/> or <see cref="GetLatestTiming(string)"/>
        /// </summary>
        /// <param name="name">the name of the timer to start</param>
        public static void StartTimer(string name)
        {
            if (watches.TryGetValue(name, out LogWatch? watch))
            {
                watch.Start();
                return;
            }
            watches[name] = LogWatch.StartNew(name);
        }

        /// <summary>
        /// Gets all the logged timings of a timer
        /// a timer will save the time from StartTimer to StopTimer internally to a ring buffer
        /// values not yet reached in the buffer default to <see cref="TimeSpan.Zero"/>
        /// If the timer does not exist, an empty array will be returned
        /// </summary>
        /// <param name="name">the name of time timer to get the timings of</param>
        /// <returns></returns>
        public static TimeSpan[] GetTimerTimings(string name) // what a name :sob:
        {
            if (watches.TryGetValue(name, out LogWatch? watch))
            {
                return watch.AllTimings();
            }
            return [];
        }

        /// <summary>
        /// Gets the latest timing of a timer
        /// If the timer does not exist or hasnt been stopped once, then <see cref="TimeSpan.Zero"/> will be returned
        /// Does not return the currently running timing, just the last one finished when <see cref="StopTimer(string)"/> was called on this string
        /// </summary>
        /// <param name="name">the name of time timer to get the timing of</param>
        /// <returns></returns>
        public static TimeSpan GetLatestTiming(string name)
        {
            if (watches.TryGetValue(name, out LogWatch? watch))
            {
                return watch.LatestTiming();
            }
            return TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the latest timing of a timer in miliseconds
        /// If the timer does not exist or hasnt been stopped once, then 0 will be returned
        /// Does not return the currently running timing, just the last one finished when <see cref="StopTimer(string)"/> was called on this string
        /// </summary>
        /// <param name="name">the name of time timer to get the timing of</param>
        /// <returns></returns>
        public static int GetLatestTimingMili(string name)
        {
            if (watches.TryGetValue(name, out LogWatch? watch))
            {
                return watch.LatestTiming().Milliseconds;
            }
            return 0;
        }

        /// <summary>
        /// Stops a timer of the given name, then stores its timing internally.
        /// You can then get these timings by using <see cref="GetTimerTimings(string)"/> or <see cref="GetLatestTiming(string)"/>
        /// </summary>
        /// <param name="name">the name of time timer to stop</param>
        public static void StopTimer(string name)
        {
            if (watches.TryGetValue(name, out LogWatch? watch))
            {
                watch.Stop();
            }
        }
    }
}
