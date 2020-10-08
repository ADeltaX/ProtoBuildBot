using ProtoBuildBot.DataStore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProtoBuildBot.Logger
{
    public static class BotLogger
    {
        private static readonly object objLock = new object();

        public static void LogVerbose(string message, string competenceBy, DateTime? timestamp = null)
        {
            timestamp ??= DateTime.UtcNow;
            InternalLog($"[VERB][{competenceBy}] {message}", timestamp.Value, ConsoleColor.Cyan, true);
        }

        public static void LogInfo(string message, string competenceBy, DateTime? timestamp = null)
        {
            timestamp ??= DateTime.UtcNow;
            InternalLog($"[INFO][{competenceBy}] {message}", timestamp.Value, ConsoleColor.White, false);
        }

        public static void LogWarning(string message, string competenceBy, DateTime? timestamp = null)
        {
            timestamp ??= DateTime.UtcNow;
            InternalLog($"[WARN][{competenceBy}] {message}", timestamp.Value, ConsoleColor.Yellow, true);
        }

        public static void LogError(string message, string competenceBy, DateTime? timestamp = null)
        {
            timestamp ??= DateTime.UtcNow;
            InternalLog($"[ERRO][{competenceBy}] {message}", timestamp.Value, ConsoleColor.Red, true);
        }

        public static void LogFatal(string message, string competenceBy, DateTime? timestamp = null)
        {
            timestamp ??= DateTime.UtcNow;
            InternalLog($"[FATA][{competenceBy}] {message}", timestamp.Value, ConsoleColor.DarkRed, true);
        }

        private static void InternalLog(string message, DateTime timestamp, ConsoleColor consoleColor, bool logToDatabase)
        {
            try
            {
                lock (objLock)
                {
                    Console.ForegroundColor = consoleColor;
                    Console.WriteLine($"[{timestamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}]" + message);
                    Console.ResetColor();

                    if (logToDatabase)
                        SharedDBcmd.TraceError(-1, message, timestamp);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{timestamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}][BOTLOGGER]" + ex.Message);
                Console.ResetColor();
            }
        }
    }
}
