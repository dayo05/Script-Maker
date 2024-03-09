using System;
using System.Reflection;
using UnityEngine;
#if !UNITY_EDITOR
using System.IO;
#endif

namespace ScriptMaker.Util
{
    public static class Log
    {
#if !UNITY_EDITOR
        static Log()
        {
            try
            {
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);
                logStream = new StreamWriter(Path.Combine(logPath,
                    $"maker-{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.log"));
            }
            catch (IOException)
            {
                Application.Quit();
            }
        }
        private static readonly StreamWriter logStream;
#endif

        public static void Info(string message, string assemblyName = null)
        {
#if !UNITY_EDITOR
            assemblyName ??= Assembly.GetCallingAssembly().GetName().Name;
            logStream.WriteLine($"{assemblyName} [INFO]: {message}");
            logStream.Flush();
#else
            Debug.Log(message);
#endif
        }

        public static void Info(Exception e, string assemblyName = null)
        {
            Info(e.ToString(), assemblyName ?? Assembly.GetCallingAssembly().GetName().Name);
        }

        public static void Warn(string message, string assemblyName = null)
        {
#if !UNITY_EDITOR
            assemblyName ??= Assembly.GetCallingAssembly().GetName().Name;
            logStream.WriteLine($"{assemblyName} [WARN]: {message}");
            logStream.Flush();
#else
            Debug.LogWarning(message);
#endif
        }

        public static void Warn(Exception e, string assemblyName = null)
        {
            Warn(e.ToString(), assemblyName ?? Assembly.GetCallingAssembly().GetName().Name);
        }

        public static void Error(string message, string assemblyName = null)
        {
#if !UNITY_EDITOR
            assemblyName ??= Assembly.GetCallingAssembly().GetName().Name;
            logStream.WriteLine($"{assemblyName} [ERROR]: {message}");
            logStream.Flush();
#else
            Debug.LogError(message);
#endif
        }

        public static void Error(Exception e, string assemblyName = null)
        {
            Error(e.ToString(), assemblyName ?? Assembly.GetCallingAssembly().GetName().Name);
        }
    }
}