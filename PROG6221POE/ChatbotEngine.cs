using PROG6221POE;
using System;
using System.Collections.Generic;

namespace PROG6221POE
{
    // Delegate used for one part of the response process
    public delegate string BotResponseDelegate(string input);

    public class ChatbotEngine
    {
        private string userName;
        private string favouriteTopic = "";
        private string currentTopic = "";

        private readonly Random random = new Random();

        private readonly Dictionary<string, string> directResponses;
        private readonly Dictionary<string, List<string>> topicResponses;
        private readonly Dictionary<string, List<string>> emotionResponses;

        public ChatbotEngine(string userName)
        {
            this.userName = string.IsNullOrWhiteSpace(userName) ? "Friend" : userName;

            // Direct responses for common chatbot questions
            directResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how are you", $"I am running properly, {this.userName}. No system crashes yet, so we are winning." },
                { "what is your purpose", "I help users understand cybersecurity threats and safer online behaviour." },
                { "what can i ask you about", "You can ask about phishing, passwords, scams, privacy, safe browsing, and 2FA." },
                { "who created you", "I was built as a cybersecurity awareness assistant." },
                { "why is cybersecurity important", "Cybersecurity matters because it protects your accounts, identity, money, and private information." },
                { "hello", $"Hello {this.userName}. What cybersecurity topic are we handling today?" },
                { "help", "Ask me about phishing, passwords, scams, privacy, safe browsing, or two-factor authentication." }
            };

            // Topic responses are stored in lists so answers can vary
            topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "phishing",
                    new List<string>
                    {
                        "Phishing is when attackers pretend to be trusted people or companies to steal sensitive information.",
                        "A phishing message usually tries to rush you, scare you, or make you click before thinking.",
                        "Before clicking links, check the sender address, spelling, and whether the URL looks legitimate."
                    }
                },
                {
                    "password",
                    new List<string>
                    {
                        "A strong password should be long, unique, and difficult to guess.",
                        "Avoid using names, birthdays, favourite teams, or simple patterns in your passwords.",
                        "Use different passwords for different accounts. Reusing one password everywhere is dangerous."
                    }
                },
                {
                    "safe browsing",
                    new List<string>
                    {
                        "Safe browsing means checking websites carefully before entering personal information.",
                        "Look for HTTPS, correct spelling in the domain, and avoid strange pop-ups or fake download buttons.",
                        "On public Wi-Fi, avoid logging into banking or sensitive accounts unless you are using extra protection."
                    }
                },
                {
                    "privacy",
                    new List<string>
                    {
                        "Privacy is about controlling what personal information you share and who can see it.",
                        "Review your app permissions and social media privacy settings regularly.",
                        "Avoid sharing your ID number, home address, live location, or daily routine online."
                    }
                },
                {
                    "scam",
                    new List<string>
                    {
                        "Scams often use urgency, fear, or fake rewards to make people act quickly.",
                        "Never share OTPs, banking details, passwords, or personal documents with random people online.",
                        "If an offer looks too perfect, pause and verify it before trusting it."
                    }
                },
                {
                    "2fa",
                    new List<string>
                    {
                        "Two-factor authentication adds a second check after your password.",
                        "2FA helps protect your account even if someone discovers your password.",
                        "Authenticator apps are usually safer than receiving login codes through SMS."
                    }
                }
            };

            // Simple sentiment detection responses
            emotionResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "worried",
                    new List<string>
                    {
                        "That is understandable. Online threats can feel stressful, but good habits reduce most of the risk.",
                        "Do not panic. Start with stronger passwords, 2FA, and being careful with suspicious links."
                    }
                },
                {
                    "frustrated",
                    new List<string>
                    {
                        "Fair enough. Cybersecurity can feel like extra admin, but it saves you from bigger problems later.",
                        "I hear you. Let us keep it simple and focus on one clear step at a time."
                    }
                },
                {
                    "curious",
                    new List<string>
                    {
                        "Good. Curiosity is useful because the more you understand scams, the harder you are to trick.",
                        "That is the right attitude. Ask about phishing, passwords, privacy, scams, safe browsing, or 2FA."
                    }
                }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please type a message first.";
            }

            input = input.ToLower().Trim();

            // Delegate handles emotion checking
            BotResponseDelegate emotionChecker = DetectEmotion;
            string emotionReply = emotionChecker(input);

            string memoryReply = HandleMemory(input);
            if (!string.IsNullOrWhiteSpace(memoryReply))
            {
                return memoryReply;
            }

            string directReply = DetectDirectResponse(input);
            string topicReply = DetectTopic(input);

            if (!string.IsNullOrWhiteSpace(emotionReply) &&
                !string.IsNullOrWhiteSpace(topicReply))
            {
                return emotionReply + Environment.NewLine + Environment.NewLine + topicReply;
            }

            if (!string.IsNullOrWhiteSpace(directReply))
            {
                return directReply;
            }

            if (!string.IsNullOrWhiteSpace(emotionReply))
            {
                return emotionReply;
            }

            if (!string.IsNullOrWhiteSpace(topicReply))
            {
                return topicReply;
            }

            if (input.Contains("tell me more") ||
                input.Contains("another tip") ||
                input.Contains("explain more"))
            {
                return GiveFollowUpResponse();
            }

            if (!string.IsNullOrWhiteSpace(favouriteTopic))
            {
                return "I did not fully understand that. Since you like "
                       + favouriteTopic
                       + ", you can ask me for another tip about it.";
            }

            return "I did not quite understand that. Please rephrase your question.";
        }

        private string DetectDirectResponse(string input)
        {
            if (directResponses.ContainsKey(input))
            {
                return directResponses[input];
            }

            foreach (string key in directResponses.Keys)
            {
                if (input.Contains(key))
                {
                    return directResponses[key];
                }
            }

            return "";
        }

        private string DetectTopic(string input)
        {
            foreach (string topic in topicResponses.Keys)
            {
                if (input.Contains(topic))
                {
                    currentTopic = topic;
                    return GetRandomResponse(topicResponses[topic]);
                }
            }

            return "";
        }

        private string DetectEmotion(string input)
        {
            foreach (string emotion in emotionResponses.Keys)
            {
                if (input.Contains(emotion))
                {
                    return GetRandomResponse(emotionResponses[emotion]);
                }
            }

            return "";
        }

        private string HandleMemory(string input)
        {
            if (input.Contains("my name is"))
            {
                userName = input.Replace("my name is", "").Trim();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = "Friend";
                }

                return "Got it. I will call you " + userName + ".";
            }

            if (input.Contains("interested in") || input.Contains("i like"))
            {
                foreach (string topic in topicResponses.Keys)
                {
                    if (input.Contains(topic))
                    {
                        favouriteTopic = topic;
                        currentTopic = topic;

                        return "Noted, " + userName + ". I will remember that you are interested in "
                               + favouriteTopic + ".";
                    }
                }
            }

            if (input.Contains("remember") ||
                input.Contains("what do you know about me"))
            {
                if (!string.IsNullOrWhiteSpace(favouriteTopic))
                {
                    return "I remember that your name is "
                           + userName
                           + " and your main interest is "
                           + favouriteTopic + ".";
                }

                return "I remember that your name is " + userName + ".";
            }

            return "";
        }

        private string GiveFollowUpResponse()
        {
            if (string.IsNullOrWhiteSpace(currentTopic))
            {
                return "Tell me the topic first: phishing, password, privacy, scam, safe browsing, or 2FA.";
            }

            if (topicResponses.ContainsKey(currentTopic))
            {
                return GetRandomResponse(topicResponses[currentTopic]);
            }

            return "Ask me about phishing, passwords, privacy, scams, safe browsing, or 2FA.";
        }

        private string GetRandomResponse(List<string> responses)
        {
            int index = random.Next(responses.Count);
            return responses[index];
        }

        public string GetAsciiArt()
        {
            return AsciiArt.GetArt();
        }
    }
}