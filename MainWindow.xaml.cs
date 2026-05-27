using CybersecurityChatbot.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private CybersecurityBot chatbot;
        private UserProfile userProfile;
        private ActivityLogger logger;
        private ObservableCollection<CyberChatMessage> messages = new ObservableCollection<CyberChatMessage>();
        private SoundPlayer voiceGreeting;

        // Track if we're waiting for user's name
        private bool waitingForName = true;

        public MainWindow()
        {
            InitializeComponent();

            // Load image and play sound
            LoadLogoImage();
            PlayVoiceGreeting();

            InitializeChatbot();
            lstConversation.ItemsSource = messages;

            // Ask for name first
            AskForUserName();

            txtStatus.Text = "Online • Waiting for your name";
        }

        private void AskForUserName()
        {
            AddBotMessage("Welcome to the Cybersecurity Awareness Assistant!");
            AddBotMessage("I'm here to help you stay safe online.");
            
            AddBotMessage("Before we begin, what is your name?");
        }

        private void LoadLogoImage()
        {
            try
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string[] possiblePaths = {
                    Path.Combine(exeDirectory, "lock.png"),
                    Path.Combine(exeDirectory, "lock.jpg"),
                    Path.Combine(exeDirectory, "logo.png"),
                    Path.Combine(exeDirectory, "Assets", "lock.png"),
                    Path.Combine(Directory.GetParent(exeDirectory).Parent.Parent.FullName, "lock.png"),
                    Path.Combine(Directory.GetParent(exeDirectory).Parent.Parent.FullName, "Assets", "lock.png")
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(foundPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgLogo.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string[] possiblePaths = {
                    Path.Combine(exeDirectory, "sound.wav"),
                    Path.Combine(exeDirectory, "sound"),
                    Path.Combine(exeDirectory, "greeting.wav"),
                    Path.Combine(exeDirectory, "Assets", "sound.wav"),
                    Path.Combine(Directory.GetParent(exeDirectory).Parent.Parent.FullName, "sound.wav"),
                    Path.Combine(Directory.GetParent(exeDirectory).Parent.Parent.FullName, "Assets", "sound.wav")
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    voiceGreeting = new SoundPlayer(foundPath);
                    voiceGreeting.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private void InitializeChatbot()
        {
            logger = new ActivityLogger();
            userProfile = new UserProfile();
            chatbot = new CybersecurityBot(userProfile, logger);
            logger.AddLog("Chatbot initialized");
        }

        private void AddUserMessage(string message)
        {
            messages.Add(new CyberChatMessage
            {
                Message = message,
                Timestamp = DateTime.Now,
                IsUserMessage = true
            });
            ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            messages.Add(new CyberChatMessage
            {
                Message = message,
                Timestamp = DateTime.Now,
                IsUserMessage = false
            });
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (lstConversation != null && lstConversation.Items.Count > 0)
            {
                lstConversation.ScrollIntoView(lstConversation.Items[lstConversation.Items.Count - 1]);
            }
        }

        private void ProcessUserName(string userInput)
        {
            // Extract name from various inputs
            string name = null;
            string lowerInput = userInput.ToLower();

            if (lowerInput.Contains("my name is"))
            {
                int nameStart = lowerInput.IndexOf("my name is") + 10;
                if (nameStart < userInput.Length)
                {
                    name = userInput.Substring(nameStart).Trim();
                    // Take only first word or until space/punctuation
                    int spaceIndex = name.IndexOfAny(new char[] { ' ', '.', ',', '!' });
                    if (spaceIndex > 0)
                        name = name.Substring(0, spaceIndex);
                }
            }
            else if (lowerInput.Contains("i am") && !lowerInput.Contains("i am interested"))
            {
                int nameStart = lowerInput.IndexOf("i am") + 4;
                if (nameStart < userInput.Length)
                {
                    name = userInput.Substring(nameStart).Trim();
                    int spaceIndex = name.IndexOfAny(new char[] { ' ', '.', ',', '!' });
                    if (spaceIndex > 0)
                        name = name.Substring(0, spaceIndex);
                }
            }
            else if (lowerInput.StartsWith("i'm") || lowerInput.StartsWith("im "))
            {
                int nameStart = userInput.ToLower().StartsWith("i'm") ? 3 : 2;
                if (nameStart < userInput.Length)
                {
                    name = userInput.Substring(nameStart).Trim();
                    int spaceIndex = name.IndexOfAny(new char[] { ' ', '.', ',', '!' });
                    if (spaceIndex > 0)
                        name = name.Substring(0, spaceIndex);
                }
            }
            else
            {
                // If user just types a name without any prefix
                string[] words = userInput.Trim().Split(' ');
                if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                {
                    name = words[0];
                }
            }

            // Capitalize first letter
            if (!string.IsNullOrEmpty(name))
            {
                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                userProfile.UserName = name;
                waitingForName = false;
                logger.AddLog($"User name set to: {name}");

                
                AddBotMessage($"Nice to meet you, {name}!");
                AddBotMessage($"I'll remember your name throughout our conversation.");
                
                AddBotMessage($"Now, let's talk about cybersecurity!");
                AddBotMessage($"");
                AddBotMessage($"What would you like to learn about today?");
                AddBotMessage($"");
                AddBotMessage($" Try asking me about:");
                AddBotMessage($"   • Passwords and how to create strong ones");
                AddBotMessage($"   • Phishing scams and how to spot them");
                AddBotMessage($"   • Online privacy and protection");
                AddBotMessage($"   • Common scams in South Africa");
                AddBotMessage($"");
                AddBotMessage($"Or type 'help' to see all topics I can help with.");

                txtStatus.Text = $"Online • Chatting with {name}";
            }
            else
            {
                AddBotMessage("I didn't catch your name. Could you please tell me your name?");
                AddBotMessage("You can say: 'My name is John' or 'I am Sarah'");
            }
        }

        private void SendMessage()
        {
            string userInput = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            AddUserMessage(userInput);

            // If waiting for name, process name input
            if (waitingForName)
            {
                ProcessUserName(userInput);
            }
            else
            {
                // Normal conversation - get bot response
                string botResponse = chatbot.GetResponse(userInput);
                AddBotMessage(botResponse);
            }

            txtUserInput.Clear();
            txtUserInput.Focus();
            txtStatus.Text = $"Online • Last message: {DateTime.Now:HH:mm:ss}";
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            messages.Clear();
            waitingForName = true;
            AskForUserName();
            logger.AddLog("Conversation cleared by user");
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (waitingForName)
            {
                AddBotMessage("Please tell me your name first!");
                AddBotMessage("You can say: 'My name is John' or 'I am Sarah'");
            }
            else
            {
                string helpMessage = chatbot.GetResponse("help");
                AddBotMessage(helpMessage);
            }
        }

        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }
    }
}