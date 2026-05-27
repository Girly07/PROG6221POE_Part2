using System;
using System.Collections.Generic;

namespace PROG6221POE
{
    /*
     * ==========================================================
     * CYBER BOT ENGINE
     * ----------------------------------------------------------
     * Handles:
     * - Natural conversations
     * - Sentiment analysis
     * - Memory system
     * - Topic recognition
     * - Follow-up responses
     * - Random dynamic responses
     * ==========================================================
     */

    public delegate string BotResponseDelegate(string input);

    public class ChatbotEngine
    {
        // ======================================================
        // MEMORY VARIABLES
        // ======================================================

        private string userName;

        private string favouriteTopic = "";

        private string activeTopic = "";

        private string recentMood = "";

        // Random generator
        private readonly Random random = new Random();

        // ======================================================
        // DATABASES
        // ======================================================

        private readonly Dictionary<string, List<string>> topicResponses;

        private readonly Dictionary<string, List<string>> topicKeywords;

        private readonly Dictionary<string, List<string>> followUps;

        private readonly Dictionary<string, List<string>> moodKeywords;

        // ======================================================
        // CONSTRUCTOR
        // ======================================================

        public ChatbotEngine(string userName)
        {
            this.userName = string.IsNullOrWhiteSpace(userName)
                ? "Friend"
                : userName;

            topicResponses = InitializeResponses();

            topicKeywords = InitializeKeywords();

            followUps = InitializeFollowUps();

            moodKeywords = InitializeMoodKeywords();
        }

        // ======================================================
        // MAIN RESPONSE METHOD
        // ======================================================

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a message.";
            }

            input = input.ToLower().Trim();

            // Detect mood
            string moodReply = DetectMood(input);

            // Handle memory
            string memoryReply = HandleMemory(input);

            if (!string.IsNullOrWhiteSpace(memoryReply))
            {
                return memoryReply;
            }

            // Follow-up detection
            if (IsFollowUp(input))
            {
                return ContinueConversation();
            }

            // Detect topic
            string topic = DetectTopic(input);

            if (!string.IsNullOrWhiteSpace(topic))
            {
                activeTopic = topic;

                string response =
                    GetRandomItem(topicResponses[topic]);

                string followUp =
                    GetRandomItem(followUps[topic]);

                if (!string.IsNullOrWhiteSpace(moodReply))
                {
                    return moodReply
                           + Environment.NewLine
                           + Environment.NewLine
                           + response
                           + Environment.NewLine
                           + Environment.NewLine
                           + followUp;
                }

                return response
                       + Environment.NewLine
                       + Environment.NewLine
                       + followUp;
            }

            // Default fallback
            return GetFallbackResponse();
        }

        // ======================================================
        // MOOD DETECTION
        // ======================================================

        private string DetectMood(string input)
        {
            foreach (var mood in moodKeywords)
            {
                foreach (string keyword in mood.Value)
                {
                    if (input.Contains(keyword))
                    {
                        recentMood = mood.Key;

                        switch (mood.Key)
                        {
                            case "worried":
                                return "I understand your concern. Online threats can definitely feel stressful.";

                            case "frustrated":
                                return "Cybersecurity can feel overwhelming sometimes, but we can simplify it.";

                            case "curious":
                                return "Curiosity is one of the best cybersecurity skills.";

                            case "happy":
                                return "Glad to hear you are feeling positive today.";

                            case "nervous":
                                return "That is understandable. Many cyber threats rely on fear and pressure.";

                            case "confused":
                                return "No problem. I can explain things more clearly.";

                            case "excited":
                                return "Great energy. Learning cybersecurity is very valuable.";

                            case "angry":
                                return "That sounds frustrating. Cybercrime affects many people.";

                            case "overwhelmed":
                                return "Take it step by step. Small habits make a big difference.";
                        }
                    }
                }
            }

            return "";
        }

        // ======================================================
        // MEMORY SYSTEM
        // ======================================================

        private string HandleMemory(string input)
        {
            if (input.Contains("my name is"))
            {
                userName =
                    input.Replace("my name is", "").Trim();

                return "Nice to meet you, "
                       + userName
                       + ". I will remember your name.";
            }

            if (input.Contains("i like")
                || input.Contains("interested in"))
            {
                string topic = DetectTopic(input);

                if (!string.IsNullOrWhiteSpace(topic))
                {
                    favouriteTopic = topic;

                    return "I will remember that you are interested in "
                           + favouriteTopic + ".";
                }
            }

            if (input.Contains("what do you remember"))
            {
                return GetMemorySummary();
            }

            return "";
        }

        private string GetMemorySummary()
        {
            string summary =
                "Here is what I remember:"
                + Environment.NewLine;

            summary += "- Name: "
                       + userName
                       + Environment.NewLine;

            if (!string.IsNullOrWhiteSpace(favouriteTopic))
            {
                summary += "- Favourite topic: "
                           + favouriteTopic
                           + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(recentMood))
            {
                summary += "- Recent mood: "
                           + recentMood;
            }

            return summary;
        }

        // ======================================================
        // TOPIC DETECTION
        // ======================================================

        private string DetectTopic(string input)
        {
            foreach (var topic in topicKeywords)
            {
                foreach (string keyword in topic.Value)
                {
                    if (input.Contains(keyword))
                    {
                        return topic.Key;
                    }
                }
            }

            return "";
        }

        // ======================================================
        // FOLLOW-UP SYSTEM
        // ======================================================

        private bool IsFollowUp(string input)
        {
            return input.Contains("tell me more")
                || input.Contains("continue")
                || input.Contains("another tip")
                || input.Contains("explain more")
                || input.Contains("go deeper");
        }

        private string ContinueConversation()
        {
            if (string.IsNullOrWhiteSpace(activeTopic))
            {
                return "Choose a cybersecurity topic first.";
            }

            return GetRandomItem(followUps[activeTopic]);
        }

        // ======================================================
        // RANDOM RESPONSE HELPER
        // ======================================================

        private string GetRandomItem(List<string> items)
        {
            int index = random.Next(items.Count);

            return items[index];
        }

        // ======================================================
        // FALLBACK RESPONSES
        // ======================================================

        private string GetFallbackResponse()
        {
            List<string> replies = new List<string>()
            {
                "I did not fully understand that. Try asking about scams, passwords, phishing, privacy, or safe browsing.",

                "Could you rephrase that? I specialise in cybersecurity awareness.",

                "Interesting. Ask me something related to online safety.",

                "I can help with phishing, scams, passwords, privacy, and 2FA.",

                "Try asking a cybersecurity-related question."
            };

            return GetRandomItem(replies);
        }

        // ======================================================
        // RESPONSE DATABASE
        // ======================================================

        private Dictionary<string, List<string>> InitializeResponses()
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "phishing",
                    new List<string>()
                    {
                        "Phishing attacks pretend to be trusted messages to steal personal information.",

                        "Always inspect suspicious emails carefully before clicking links.",

                        "Cybercriminals use phishing to steal passwords and banking details.",

                        "Phishing emails often create urgency to pressure victims.",

                        "Hover over links before clicking them.",

                        "Some phishing websites look identical to real websites.",

                        "Never trust unexpected attachments from unknown senders.",

                        "Banks rarely request passwords through email.",

                        "Smishing is phishing done through SMS messages.",

                        "Vishing is phishing performed through phone calls."
                    }
                },

                {
                    "password",
                    new List<string>()
                    {
                        "Strong passwords should be long and unique.",

                        "Avoid using birthdays or names in passwords.",

                        "Password managers help store secure passwords safely.",

                        "Never reuse passwords across accounts.",

                        "Passphrases are easier to remember and stronger.",

                        "Weak passwords are one of the biggest cybersecurity risks.",

                        "Your email password should be especially strong.",

                        "2FA greatly improves account protection.",

                        "Avoid saving passwords in screenshots.",

                        "Long passwords are usually harder to crack."
                    }
                },

                {
                    "privacy",
                    new List<string>()
                    {
                        "Privacy means controlling your personal information online.",

                        "Avoid oversharing on social media.",

                        "Review app permissions regularly.",

                        "Location sharing should only be enabled when necessary.",

                        "Tracking cookies monitor browsing behaviour.",

                        "Public posts can reveal more than expected.",

                        "Identity theft often starts with exposed information.",

                        "Strong privacy settings improve online safety.",

                        "Apps sometimes collect unnecessary data.",

                        "Personal information should be protected carefully."
                    }
                },

                {
                    "safe browsing",
                    new List<string>()
                    {
                        "Always check for HTTPS before entering sensitive information.",

                        "Avoid suspicious downloads and pop-ups.",

                        "Public Wi-Fi can expose sensitive data.",

                        "Keep browsers updated for security patches.",

                        "Fake websites often imitate trusted brands.",

                        "Be careful with browser extensions.",

                        "Shortened links can hide dangerous websites.",

                        "Avoid entering passwords on suspicious websites.",

                        "Secure browsing habits reduce cyber risks.",

                        "Public computers are risky for sensitive accounts."
                    }
                },

                {
                    "scam",
                    new List<string>()
                    {
                        "Scams often use urgency or fear to manipulate victims.",

                        "Never send money without proper verification.",

                        "Fake giveaways and prize scams are very common.",

                        "Scammers often impersonate trusted organisations.",

                        "If something sounds too good to be true, it probably is.",

                        "OTP scams continue increasing online.",

                        "Romance scams build emotional trust before requesting money.",

                        "Always verify suspicious requests independently.",

                        "Cybercriminals rely on emotional reactions.",

                        "Scammers often pressure victims into quick decisions."
                    }
                },

                {
                    "2fa",
                    new List<string>()
                    {
                        "Two-factor authentication adds extra account security.",

                        "2FA protects accounts even if passwords are stolen.",

                        "Authenticator apps are safer than SMS verification.",

                        "2FA greatly reduces hacking risks.",

                        "Recovery codes should be stored safely.",

                        "Many cyberattacks fail because of 2FA.",

                        "Authentication apps generate temporary secure codes.",

                        "2FA is important for banking and email accounts.",

                        "Security improves when passwords and 2FA are combined.",

                        "2FA creates an extra verification step during login."
                    }
                }
            };
        }

        // ======================================================
        // FOLLOW-UP DATABASE
        // ======================================================

        private Dictionary<string, List<string>> InitializeFollowUps()
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "phishing",
                    new List<string>()
                    {
                        "Tip: Verify suspicious emails independently.",

                        "Avoid clicking links in unexpected messages.",

                        "Cybercriminals often copy company branding.",

                        "Always inspect sender addresses carefully.",

                        "Fake login pages commonly steal credentials."
                    }
                },

                {
                    "password",
                    new List<string>()
                    {
                        "Use unique passwords for every account.",

                        "Passphrases improve both security and memorability.",

                        "Avoid common passwords like 123456.",

                        "Password managers improve security significantly.",

                        "2FA adds another layer of protection."
                    }
                },

                {
                    "privacy",
                    new List<string>()
                    {
                        "Review social media privacy settings regularly.",

                        "Avoid sharing your live location publicly.",

                        "Privacy awareness reduces identity theft risks.",

                        "Some apps request unnecessary permissions.",

                        "Be careful about public online posts."
                    }
                },

                {
                    "safe browsing",
                    new List<string>()
                    {
                        "Keep devices updated regularly.",

                        "Avoid suspicious browser extensions.",

                        "Public Wi-Fi should be used carefully.",

                        "Secure browsing habits protect personal data.",

                        "Download software only from trusted sources."
                    }
                },

                {
                    "scam",
                    new List<string>()
                    {
                        "Scammers often create panic or urgency.",

                        "Always verify requests for money.",

                        "Prize scams usually request personal details.",

                        "Scammers frequently impersonate trusted companies.",

                        "Cybercriminals rely on emotional reactions."
                    }
                },

                {
                    "2fa",
                    new List<string>()
                    {
                        "Authenticator apps provide stronger protection.",

                        "2FA helps stop many account attacks.",

                        "Recovery codes should be stored securely.",

                        "Security keys offer advanced authentication.",

                        "2FA greatly improves account security."
                    }
                }
            };
        }

        // ======================================================
        // KEYWORD DATABASE
        // ======================================================

        private Dictionary<string, List<string>> InitializeKeywords()
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "phishing",
                    new List<string>()
                    {
                        "phishing",
                        "fake email",
                        "suspicious email",
                        "smishing",
                        "vishing"
                    }
                },

                {
                    "password",
                    new List<string>()
                    {
                        "password",
                        "login",
                        "credentials",
                        "passphrase"
                    }
                },

                {
                    "privacy",
                    new List<string>()
                    {
                        "privacy",
                        "tracking",
                        "personal information"
                    }
                },

                {
                    "safe browsing",
                    new List<string>()
                    {
                        "browser",
                        "website",
                        "wifi",
                        "https"
                    }
                },

                {
                    "scam",
                    new List<string>()
                    {
                        "scam",
                        "fraud",
                        "fake offer",
                        "otp"
                    }
                },

                {
                    "2fa",
                    new List<string>()
                    {
                        "2fa",
                        "mfa",
                        "verification code",
                        "authenticator"
                    }
                }
            };
        }

        // ======================================================
        // MOOD KEYWORDS
        // ======================================================

        private Dictionary<string, List<string>> InitializeMoodKeywords()
        {
            return new Dictionary<string, List<string>>()
            {
                { "worried", new List<string>() { "worried", "scared", "unsafe" } },

                { "frustrated", new List<string>() { "frustrated", "annoyed" } },

                { "curious", new List<string>() { "curious", "interested" } },

                { "happy", new List<string>() { "happy", "great", "good" } },

                { "nervous", new List<string>() { "nervous", "anxious" } },

                { "confused", new List<string>() { "confused", "lost" } },

                { "excited", new List<string>() { "excited", "motivated" } },

                { "angry", new List<string>() { "angry", "mad" } },

                { "overwhelmed", new List<string>() { "overwhelmed", "stressed" } }
            };
        }

        // ======================================================
        // ASCII ART
        // ======================================================

        public string GetAsciiArt()
        {
            return AsciiArt.GetArt();
        }
    }
}