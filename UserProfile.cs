// File: Models/UserProfile.cs
using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    /// <summary>
    /// Stores user-specific information that the chatbot remembers
    /// Implements memory feature for personalized conversations
    /// </summary>
    public class UserProfile
    {
        // User's name collected during conversation
        public string UserName { get; set; }

        // User's favorite cybersecurity topic they've expressed interest in
        public string FavoriteTopic { get; set; }

        // Stores any other user preferences or remembered information
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();

        // Tracks the current conversation topic for follow-up questions
        public string CurrentTopic { get; set; }

        // Stores the last tip given to allow "tell me more" functionality
        public string LastTipGiven { get; set; }

        // Constructor initializes with default values
        public UserProfile()
        {
            UserName = "User"; // Default until user provides name
            FavoriteTopic = string.Empty;
            CurrentTopic = string.Empty;
            LastTipGiven = string.Empty;
        }
    }
}