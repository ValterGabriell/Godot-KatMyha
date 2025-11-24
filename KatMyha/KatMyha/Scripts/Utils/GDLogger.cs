using Godot;
using System;
using System.Runtime.CompilerServices;

namespace PrototipoMyha.Utilidades
{
    public static class GDLogger
    {
        private static readonly object _lockObject = new object();
        public static void LogBlue(object message,
        bool isVerbose = false,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("🔵", message, isVerbose, memberName, filePath, lineNumber);
        }

        public static void LogGreen(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("🟢", message, isVerbose, memberName, filePath, lineNumber);
        }


        public static void Log(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("LOG: ", message, isVerbose, memberName, filePath, lineNumber);
        }


        public static void Print(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("P", message, isVerbose, memberName, filePath, lineNumber);
        }

        public static void LogRed(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("🔴", message, isVerbose, memberName, filePath, lineNumber);
        }

        public static void LogYellow(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("🟡", message, isVerbose, memberName, filePath, lineNumber);
        }

        private static void PrintWithEmoji(string emoji, object message, bool isVerbose,
            string memberName, string filePath, int lineNumber)
        {
            var className = GetClassNameFromFilePath(filePath);
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            lock (_lockObject)
            {
                if (isVerbose)
                {
                    GD.Print($"{emoji} ╔══ VERBOSE LOG ══════════════════════════════════");
                    GD.Print($"{emoji} ║ Timestamp: {timestamp}");
                    GD.Print($"{emoji} ║ Thread: {threadId}");
                    GD.Print($"{emoji} ║ Class: {className}");
                    GD.Print($"{emoji} ║ Method: {memberName}");
                    GD.Print($"{emoji} ║ Line: {lineNumber}");
                    GD.Print($"{emoji} ║ File: {System.IO.Path.GetFileName(filePath)}");
                    GD.Print($"{emoji} ║ Message: {message}");
                    GD.Print($"{emoji} ╚═════════════════════════════════════════════════");
                }
                else
                {
                    GD.Print($"[{className}/{memberName}] - {emoji} {message}");
                }
            }
        }

        private static string GetClassNameFromFilePath(string filePath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(filePath) ?? "UnknownClass";
        }

        internal static void LogError(object message,
            bool isVerbose = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            PrintWithEmoji("ERR - ", message, isVerbose, memberName, filePath, lineNumber);
        }

    }
}
