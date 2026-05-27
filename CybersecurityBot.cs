using CybersecurityChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class CybersecurityBot
    {
        private Random random = new Random();
        private UserProfile userProfile;
        private ActivityLogger logger;

        private Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>()
        {
            ["password"] = new List<string>
            {
                "Use strong, unique passwords for each account. Avoid using personal details like your name or birthday!",
                "Create passwords that are at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols.",
                "Never share your passwords with anyone. Consider using a password manager to keep track of them securely.",
                "Enable two-factor authentication (2FA) whenever possible - it adds an extra layer of security to your accounts!"
            },
            ["phish"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations!",
                "Always check the sender's email address carefully - phishers use addresses that look similar to real ones.",
                "Never click on suspicious links. Hover over them first to see where they really lead.",
                "If an email creates urgency ('Your account will be closed!'), it's likely a phishing attempt."
            },
            ["scam"] = new List<string>
            {
                "Scammers often promise huge rewards or threaten consequences. If it sounds too good to be true, it probably is!",
                "Never send money or gift cards to someone you've only met online - this is a common scam tactic.",
                "Verify unexpected calls or messages by contacting the organisation directly using official contact information.",
                "Report scams to the South African Fraud Prevention Service at 0800 222 777."
            },
            ["privacy"] = new List<string>
            {
                "Review your privacy settings on social media regularly. Limit what personal information is visible publicly.",
                "" +
                " Be mindful of what you share online - once something is posted, it's difficult to completely remove it.",
                " Use the principle of least privilege - only share information that is absolutely necessary.",
                " Regularly review which apps have access to your accounts and remove ones you don't use."
            },
            ["safe browsing"] = new List<string>
            {
                "Look for 'https://' and the padlock icon in the address bar before entering any personal information.",
                "Keep your browser and extensions updated to protect against known security vulnerabilities.",
                "Use a reputable ad-blocker to avoid malicious advertisements that can infect your device.",
                "Avoid using public Wi-Fi for banking or sensitive transactions without a VPN."
            },
            ["malware"] = new List<string>
            {
                "Keep your antivirus software updated and run regular scans on your device.",
                "Only download software from official websites - cracked or pirated software often contains malware.",
                "Be careful with USB drives from unknown sources - they can contain infected files.",
                "If your computer is acting strangely (pop-ups, slow performance), run a malware scan immediately."
            },
            ["2fa"] = new List<string>
            {
                "Two-Factor Authentication adds a second verification step using your phone or authenticator app.",
                "Enable 2FA on all accounts that offer it - especially email, banking, and social media.",
                "Authentication apps like Google Authenticator are more secure than SMS-based 2FA.",
                "Save your backup codes in a safe place in case you lose access to your 2FA device."
            }
        };

        private List<string> greetingResponses = new List<string>
        {
            "Hello there! I'm your Cybersecurity Awareness Assistant. How can I help you stay safe online today?",
            "Welcome back! Ready to learn about staying secure in the digital world?",
            "Hi! I'm here to help you navigate online safety. What would you like to learn about today?",
            "Greetings! Remember - cybersecurity is everyone's responsibility. How can I assist you?"
        };

        private Dictionary<string, List<string>> sentimentResponses = new Dictionary<string, List<string>>()
        {
            ["worried"] = new List<string>
            {
                "It's completely understandable to feel that way. Cybersecurity threats can be scary, but knowledge is your best defense. Let me share some practical tips to help you feel more secure.",
                "I hear your concern. Many people feel worried about online threats. The good news is that following basic security practices greatly reduces your risk.",
                "Don't worry - we'll get through this together. Start with small steps like using strong passwords."
            },
            ["curious"] = new List<string>
            {
                "That's great to hear! Curiosity is the first step toward becoming cybersecurity-savvy.",
                "Excellent question! Let me share some interesting information about that.",
                "I love your enthusiasm for learning!"
            },
            ["frustrated"] = new List<string>
            {
                "I understand this can be frustrating. Let me break it down into simple steps for you.",
                "I hear you. What specific part is challenging you?",
                "Let's take a step back. I'll help you find a solution."
            },
            ["confused"] = new List<string>
            {
                "No problem - let me explain that in a simpler way.",
                "I can see where that might be confusing. Let me rephrase that.",
                "Thanks for letting me know. Let's start from the basics."
            }
        };

        public CybersecurityBot(UserProfile profile, ActivityLogger activityLogger)
        {
            userProfile = profile;
            logger = activityLogger;
        }

        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't hear anything. Could you please type your question or concern?";
            }

            string lowerInput = userInput.Trim().ToLower();
            logger.AddLog($"User asked: {userInput}");

            if (IsGreeting(lowerInput))
            {
                return GetGreetingResponse();
            }

            if (lowerInput.Contains("my name is") || lowerInput.Contains("i am called") ||
                lowerInput.Contains("call me") || lowerInput.StartsWith("i'm "))
            {
                string newName = ExtractName(userInput);
                if (!string.IsNullOrEmpty(newName))
                {
                    userProfile.UserName = newName;
                    logger.AddLog($"User name updated to: {newName}");
                    return $"Nice to meet you, {newName}! I'll remember that. How can I help you with cybersecurity today?";
                }
            }

            if (lowerInput.Contains("what do you remember") || lowerInput.Contains("what do you know about me"))
            {
                return GetMemorySummary();
            }

            if (IsFollowUpRequest(lowerInput))
            {
                if (!string.IsNullOrEmpty(userProfile.LastTipGiven))
                {
                    return GetResponseForTopic(userProfile.CurrentTopic, isFollowUp: true);
                }
                else
                {
                    return "I haven't shared any tips yet. What cybersecurity topic would you like to learn about?";
                }
            }

            if (lowerInput.Contains("help") || lowerInput.Contains("what can you do"))
            {
                return GetHelpMessage();
            }

            string detectedSentiment = DetectSentiment(lowerInput);
            if (detectedSentiment != null)
            {
                logger.AddLog($"Sentiment detected: {detectedSentiment}");
                string sentimentResponseText = GetSentimentResponse(detectedSentiment);
                return sentimentResponseText + "\n\n" + GetRandomGeneralTip();
            }

            foreach (var keyword in keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    userProfile.FavoriteTopic = keyword;
                    userProfile.CurrentTopic = keyword;
                    break;
                }
            }

            string responseText = GetResponseForTopic(lowerInput);
            if (!responseText.Contains("didn't quite understand"))
            {
                userProfile.LastTipGiven = responseText;
            }

            return responseText;
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("nervous") || input.Contains("unsure"))
                return "worried";
            if (input.Contains("curious") || input.Contains("interesting") || input.Contains("tell me"))
                return "curious";
            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("confusing"))
                return "frustrated";
            if (input.Contains("confused") || input.Contains("don't understand"))
                return "confused";
            return null;
        }

        private string GetSentimentResponse(string sentiment)
        {
            if (sentimentResponses.ContainsKey(sentiment))
            {
                var responses = sentimentResponses[sentiment];
                return responses[random.Next(responses.Count)];
            }
            return "I appreciate you sharing that with me.";
        }

        private bool IsGreeting(string input)
        {
            string[] greetings = { "hello", "hi", "hey", "good morning", "good afternoon", "good evening", "howdy" };
            foreach (string g in greetings)
            {
                if (input.Contains(g)) return true;
            }
            return false;
        }

        private string GetGreetingResponse()
        {
            string greeting = greetingResponses[random.Next(greetingResponses.Count)];
            if (!string.IsNullOrEmpty(userProfile.UserName) && userProfile.UserName != "User")
            {
                greeting = $"Hi again, {userProfile.UserName}! " + greeting;
            }
            return greeting;
        }

        private bool IsFollowUpRequest(string input)
        {
            string[] phrases = { "tell me more", "more", "another tip", "explain more", "elaborate", "continue", "what else" };
            foreach (string p in phrases)
            {
                if (input.Contains(p)) return true;
            }
            return false;
        }

        private string ExtractName(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("my name is"))
            {
                int nameStart = lowerInput.IndexOf("my name is") + 10;
                if (nameStart < input.Length)
                    return input.Substring(nameStart).Trim().Split(' ')[0];
            }
            else if (lowerInput.Contains("i am called"))
            {
                int nameStart = lowerInput.IndexOf("i am called") + 11;
                if (nameStart < input.Length)
                    return input.Substring(nameStart).Trim().Split(' ')[0];
            }
            else if (lowerInput.Contains("call me"))
            {
                int nameStart = lowerInput.IndexOf("call me") + 7;
                if (nameStart < input.Length)
                    return input.Substring(nameStart).Trim().Split(' ')[0];
            }
            else if (lowerInput.StartsWith("i'm ") || lowerInput.StartsWith("im "))
            {
                return input.Substring(3).Trim().Split(' ')[0];
            }
            return null;
        }

        private string GetMemorySummary()
        {
            string summary = "Here's what I remember about you:\n";
            if (!string.IsNullOrEmpty(userProfile.UserName) && userProfile.UserName != "User")
                summary += $"• Your name is {userProfile.UserName}\n";
            else
                summary += "• I don't know your name yet. Tell me 'My name is [your name]'\n";

            if (!string.IsNullOrEmpty(userProfile.FavoriteTopic))
                summary += $"• You're interested in learning about {userProfile.FavoriteTopic}\n";
            else
                summary += "• You haven't told me your favorite cybersecurity topic yet\n";

            return summary + "\nWhat would you like to learn about today?";
        }

        private string GetHelpMessage()
        {
            return "CYBERSECURITY AWARENESS BOT - HELP \n\n" +
                   "I can help you with these topics:\n" +
                   "• Password safety\n" +
                   "• Phishing detection\n" +
                   "• Scam recognition\n" +
                   "• Privacy protection\n" +
                   "• Safe browsing\n" +
                   "• Malware prevention\n" +
                   "• Two-factor authentication (2FA)\n\n" +
                   "Try asking: 'Tell me about passwords' or 'What is phishing?'\n\n" +
                   "You can also:\n" +
                   "- Say 'tell me more' for additional tips\n" +
                   "- Express feelings like 'I'm worried about scams'\n" +
                   "- Tell me your name: 'My name is Thabo'";
        }

        private string GetResponseForTopic(string input, bool isFollowUp = false)
        {
            string lowerInput = input.ToLower();

            foreach (var kvp in keywordResponses)
            {
                if (lowerInput.Contains(kvp.Key))
                {
                    userProfile.CurrentTopic = kvp.Key;
                    var responses = kvp.Value;
                    string selectedResponse = responses[random.Next(responses.Count)];

                    if (isFollowUp)
                    {
                        selectedResponse = "Here's another important tip: " + selectedResponse;
                    }
                    return selectedResponse;
                }
            }

            if (lowerInput.Contains("cybersecurity") || lowerInput.Contains("online safety"))
            {
                return GetRandomGeneralTip();
            }

            return "I didn't quite understand that. Could you rephrase? Try asking about passwords, phishing, privacy, or scams. Or type 'help' to see what I can do!";
        }

        private string GetRandomGeneralTip()
        {
            List<string> generalTips = new List<string>
            {
                "Did you know? 81% of data breaches are caused by weak or stolen passwords.",
                "Always verify the sender before clicking links in emails.",
                "Back up your important files regularly to an external drive.",
                "Use a different password for every account.",
                "Lock your computer screen when you step away.",
                "Be careful what you post on social media.",
                "South Africa has seen a 300% increase in cybercrime since 2020.",
                "Never use public computers for online banking."
            };
            return generalTips[random.Next(generalTips.Count)];
        }
    }
}