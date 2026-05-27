using System;
using System.Windows;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public class CyberChatMessage
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsUserMessage { get; set; }

        // Computed properties for UI alignment
        public HorizontalAlignment MsgAlignment => IsUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        // Computed properties for message colors
        public Brush MsgBackground => IsUserMessage ?
            new SolidColorBrush(Color.FromRgb(137, 180, 250)) : // Blue for user
            new SolidColorBrush(Color.FromRgb(49, 50, 68));      // Dark gray for bot

        public Brush MsgForeground => IsUserMessage ?
            new SolidColorBrush(Color.FromRgb(30, 30, 46)) : // Dark text for user
            new SolidColorBrush(Color.FromRgb(205, 214, 244)); // Light text for bot
    }
}