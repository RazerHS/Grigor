using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace CardboardCore.Utilities
{
    public static class Log
    {
        private const int MAX_LOGS = 50;

        private static readonly Dictionary<Type, Color> colorDict = new Dictionary<Type, Color>();

        private static readonly List<string> logHistory = new List<string>();

        private static Color GetColor(Type type)
        {
            if (type == null)
            {
                return Color.magenta;
            }

            if (colorDict.TryGetValue(type, out Color color))
            {
                return color;
            }

            // TODO: Generate a seed value based on given type
            // TODO: Create a few predefined color codes to pick randomly from
            color = new Color(Random.value, Random.value, Random.value);

            colorDict.Add(type, color);

            return color;
        }

        private static string GetString(object message)
        {
            Type type = GetCallerType();

            string methodName = GetMethodName();

            Color color = GetColor(type);
            string colorString = ColorUtility.ToHtmlStringRGB(color);

            string typeName = type == null ? "Unknown Source" : type.Name;

            return $"<color=#{colorString}>[{typeName}:{methodName}]</color> - {message}";
        }

        private static Type GetCallerType()
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrame(4)?.GetMethod()?.DeclaringType;
        }

        private static string GetMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrame(4)?.GetMethod()?.Name;
        }

        private static void DoLog(string methodName, object message)
        {
#if UNITY_EDITOR
            string fullMessage = GetString(message);
#else
            string fullMessage = $"{message}";
#endif

            Type debugType = typeof(Debug);

            MethodInfo methodInfo = debugType.GetMethod(methodName, new[] {typeof(string)});

            if (methodInfo == null)
            {
                return;
            }

            methodInfo.Invoke(null, new object[] {fullMessage});

            logHistory.Add(fullMessage);

            if (logHistory.Count > MAX_LOGS)
            {
                logHistory.RemoveAt(0);
            }
        }

        public static void Write(object message)
        {
            DoLog("Log", message);
        }

        public static void Warn(object message)
        {
            DoLog("LogWarning", message);
        }

        public static void Error(object message)
        {
            DoLog("LogError", message);
        }

        public static Exception Exception(object message)
        {
            throw new Exception(GetString(message));
        }
    }
}