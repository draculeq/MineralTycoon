using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Plugins.Utilities
{
    public enum LogPriority { Low, Medium, High, Critical }
    public enum LogCategory { General, Database, Buildings, Production, Analytics }

    public static class DeadbitLog
    {
        private static Dictionary<LogCategory, LogPriority> levels;

        static DeadbitLog()
        {
            levels = new Dictionary<LogCategory, LogPriority>();

            //Default Values
            foreach (var productType in Enum.GetNames(typeof(LogCategory)))
                levels.Add((LogCategory)Enum.Parse(typeof(LogCategory), productType), LogPriority.Low);

            //Custom Values
            levels[LogCategory.General] = LogPriority.High;
            levels[LogCategory.Database] = LogPriority.High;
            levels[LogCategory.Buildings] = LogPriority.High;
            levels[LogCategory.Production] = LogPriority.High;
        }
        public static void Log(object message, LogCategory category = LogCategory.General, LogPriority level = LogPriority.Medium, UnityEngine.Object context = null)
        {
            if (!MeetsRequirements(category, level))
                return;
            message += "\n";
            switch (level)
            {
                case LogPriority.Low:
                case LogPriority.Medium:
                    UnityEngine.Debug.Log("[" + category + "] " + message, context);
                    break;
                case LogPriority.High:
                    UnityEngine.Debug.LogWarning("[" + category + "] " + message, context);
                    break;
                case LogPriority.Critical:
                    UnityEngine.Debug.LogError("[" + category + "] " + message, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level", level, null);
            }
        }

        private static bool MeetsRequirements(LogCategory category, LogPriority level)
        {
            return level >= levels[category];
        }
    }
}
