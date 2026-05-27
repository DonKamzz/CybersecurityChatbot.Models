using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class ActivityLogger
    {
        private List<string> Logs { get; set; } = new List<string>();
        private const int MaxLogs = 100;

        public void AddLog(string action)
        {
            string timestampedLog = $"{DateTime.Now:HH:mm:ss} - {action}";
            Logs.Add(timestampedLog);
            if (Logs.Count > MaxLogs)
            {
                Logs.RemoveAt(0);
            }
        }

        public string ShowLogs()
        {
            if (Logs.Count == 0)
            {
                return "No activity recorded yet.";
            }
            return string.Join("\n", Logs);
        }

        public List<string> GetRecentLogs(int count = 10)
        {
            int startIndex = Math.Max(0, Logs.Count - count);
            return Logs.GetRange(startIndex, Logs.Count - startIndex);
        }

        public void ClearLogs()
        {
            Logs.Clear();
            AddLog("Activity log cleared by user");
        }
    }
}