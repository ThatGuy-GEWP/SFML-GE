using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// A Simple way of logging colored and formatted debug information to the console.
    /// </summary>
    public static class DebugLogger
    {
        static bool do_logging = false;
        static string log_path = "";

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

        /// <summary> The line color for <see cref="LogDebug(string)"/> </summary>
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
        static void Log(string message)
        {
            if (log_events == 0)
            {
                File.WriteAllText(log_path, "");
            }
            log_events++;
            File.AppendAllLines(log_path, new string[] { message });
        }

        /// <summary>
        /// Logs a message to the console with "Info" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogInfo(string message)
        {
            if (!Enabled) { return; }
            Console.ForegroundColor = InfoColor;
            string log_string = "[Info] " + message;
            Console.WriteLine(log_string);
            Log(log_string);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Logs a message to the console with "Priority Info" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogInfoPriority(string message)
        {
            if (!Enabled) { return; }
            Console.ForegroundColor = PriorityInfoColor;
            string log_string = "[Info] " + message;
            Console.WriteLine(log_string);
            Log(log_string);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Logs a message to the console with "Warning" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogWarning(string message)
        {
            if (!Enabled) { return; }
            Console.ForegroundColor = WarningColor;
            string log_string = "[Warning] " + message;
            Console.WriteLine(log_string);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Logs a message to the console with "Error" formatting.
        /// </summary>
        /// <param name="message">the message to log.</param>
        public static void LogError(string message)
        {
            if (!Enabled) { return; }
            Console.ForegroundColor = ErrorColor;
            string log_string = "[Error] " + message;
            Console.WriteLine(log_string);
            Log(log_string);
            Console.ForegroundColor = ConsoleColor.White;
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
        internal static void LogDebug(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            if (!Enabled) { return; }


            string fileName = Path.GetFileName(sourceFilePath).Replace(".cs", "");

            Console.ForegroundColor = DebugColor;
            string log_string = $"[{lineNumber}: {fileName}.{memberName}()] {message}";
            Console.WriteLine(log_string);
            Log(log_string);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <inheritdoc cref="LogDebug(string)"/>
        public static void LogDebug(object message)
        {
            string output = message.ToString() ?? "null";
            LogDebug(output);
        }
    }
}
