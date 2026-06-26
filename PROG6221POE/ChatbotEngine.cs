using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PROG6221POE
{
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
        private Random random = new Random();

        // Task and database
        private DatabaseHelper dbHelper;
        private Quiz quiz;

        // Activity Log - Enhanced with show more
        private List<ActivityLogEntry> activityLog = new List<ActivityLogEntry>();
        private int activityLogPage = 0;
        private const int ITEMS_PER_PAGE = 5;

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
            this.userName = string.IsNullOrWhiteSpace(userName) ? "Friend" : userName;

            dbHelper = new DatabaseHelper();
            quiz = new Quiz();

            topicResponses = InitializeResponses();
            topicKeywords = InitializeKeywords();
            followUps = InitializeFollowUps();
            moodKeywords = InitializeMoodKeywords();

            LogActivity("System initialized", $"Chatbot started for user: {this.userName}");
        }

        // ======================================================
        // MAIN RESPONSE METHOD - Enhanced NLP
        // ======================================================

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please enter a message.";

            input = input.ToLower().Trim();

            // ==================================================
            // 1. ACTIVITY LOG COMMANDS (Enhanced with show more)
            // ==================================================
            if (input.Contains("show activity log") || input.Contains("what have you done") ||
                input.Contains("activity log") || input.Contains("show more") || input.Contains("view log"))
            {
                if (input.Contains("show more") || input.Contains("more"))
                {
                    activityLogPage++;
                    return GetActivityLog(activityLogPage);
                }
                else
                {
                    activityLogPage = 0;
                    return GetActivityLog(0);
                }
            }

            // ==================================================
            // 2. QUIZ COMMANDS (Enhanced)
            // ==================================================
            if (input.Contains("start quiz") || input.Contains("begin quiz") || input.Contains("take quiz"))
            {
                LogActivity("Quiz started", "User began the cybersecurity quiz");
                return quiz.StartQuiz();
            }

            if (quiz.QuizActive)
            {
                string answerResponse = HandleQuizAnswer(input);
                if (!string.IsNullOrEmpty(answerResponse))
                    return answerResponse;
            }

            // ==================================================
            // 3. TASK MANAGEMENT COMMANDS (Enhanced)
            // ==================================================
            if (IsTaskCommand(input))
            {
                return HandleTaskCommand(input);
            }

            // ==================================================
            // 4. MEMORY COMMANDS
            // ==================================================
            string memoryReply = HandleMemory(input);
            if (!string.IsNullOrWhiteSpace(memoryReply))
                return memoryReply;

            // ==================================================
            // 5. NLP - Mood Detection
            // ==================================================
            string moodReply = DetectMood(input);

            // ==================================================
            // 6. NLP - Follow-up Detection
            // ==================================================
            if (IsFollowUp(input))
                return ContinueConversation(moodReply);

            // ==================================================
            // 7. NLP - Topic Detection
            // ==================================================
            string topic = DetectTopic(input);
            if (!string.IsNullOrWhiteSpace(topic))
            {
                activeTopic = topic;
                string response = GetRandomItem(topicResponses[topic]);
                string followUp = GetRandomItem(followUps[topic]);

                LogActivity("Topic discussed", $"User asked about: {topic}");

                if (!string.IsNullOrWhiteSpace(moodReply))
                    return moodReply + Environment.NewLine + Environment.NewLine + response + Environment.NewLine + Environment.NewLine + followUp;

                return response + Environment.NewLine + Environment.NewLine + followUp;
            }

            // ==================================================
            // 8. NLP - Question Detection (Enhanced)
            // ==================================================
            if (IsQuestion(input))
            {
                return HandleQuestion(input);
            }

            // ==================================================
            // 9. DEFAULT FALLBACK
            // ==================================================
            return GetFallbackResponse();
        }

        // ======================================================
        // ENHANCED QUIZ ANSWER HANDLING
        // ======================================================

        private string HandleQuizAnswer(string input)
        {
            // Try to parse as number
            if (int.TryParse(input, out int answerIndex))
            {
                if (answerIndex >= 1 && answerIndex <= 4)
                    return quiz.SubmitAnswer(answerIndex - 1);
            }

            // Try to match letter (A, B, C, D)
            string upperInput = input.ToUpper().Trim();
            if (upperInput.Length == 1 && "ABCD".Contains(upperInput))
            {
                int index = "ABCD".IndexOf(upperInput);
                return quiz.SubmitAnswer(index);
            }

            // Try to match option text
            var currentQuestion = quiz.GetCurrentQuestion();
            if (currentQuestion.Contains("A.") && input.Contains("a.") ||
                currentQuestion.Contains("1.") && input.Contains("1"))
                return quiz.SubmitAnswer(0);
            else if (currentQuestion.Contains("B.") && input.Contains("b.") ||
                     currentQuestion.Contains("2.") && input.Contains("2"))
                return quiz.SubmitAnswer(1);
            else if (currentQuestion.Contains("C.") && input.Contains("c.") ||
                     currentQuestion.Contains("3.") && input.Contains("3"))
                return quiz.SubmitAnswer(2);
            else if (currentQuestion.Contains("D.") && input.Contains("d.") ||
                     currentQuestion.Contains("4.") && input.Contains("4"))
                return quiz.SubmitAnswer(3);

            return "Please enter a valid answer (A, B, C, D or 1, 2, 3, 4).";
        }

        // ======================================================
        // ENHANCED TASK COMMAND HANDLING
        // ======================================================

        private bool IsTaskCommand(string input)
        {
            return input.Contains("add task") || input.Contains("new task") ||
                   input.Contains("view tasks") || input.Contains("show tasks") ||
                   input.Contains("list tasks") || input.Contains("complete task") ||
                   input.Contains("delete task") || input.Contains("remove task") ||
                   input.Contains("remind me") || input.Contains("set reminder");
        }

        private string HandleTaskCommand(string input)
        {
            // ADD TASK
            if (input.Contains("add task") || input.Contains("new task") || input.Contains("remind me"))
            {
                string taskText = input.Replace("add task", "")
                                       .Replace("new task", "")
                                       .Replace("remind me", "")
                                       .Trim();

                if (string.IsNullOrEmpty(taskText) && !input.Contains("remind me"))
                    return "Please specify what task you want to add.\nExample: add task - Enable two-factor authentication";

                // Check for reminder
                if (input.Contains("remind me") || input.Contains("reminder") ||
                    input.Contains("tomorrow") || input.Contains("days"))
                {
                    DateTime reminderDate = DateTime.Now.AddDays(3);

                    if (input.Contains("tomorrow"))
                        reminderDate = DateTime.Now.AddDays(1);
                    else if (input.Contains("7 days"))
                        reminderDate = DateTime.Now.AddDays(7);
                    else if (input.Contains("5 days"))
                        reminderDate = DateTime.Now.AddDays(5);
                    else if (input.Contains("3 days"))
                        reminderDate = DateTime.Now.AddDays(3);
                    else if (input.Contains("1 day"))
                        reminderDate = DateTime.Now.AddDays(1);
                    else if (input.Contains("2 weeks"))
                        reminderDate = DateTime.Now.AddDays(14);
                    else if (input.Contains("1 month"))
                        reminderDate = DateTime.Now.AddDays(30);
                    else if (input.Contains("6 months"))
                        reminderDate = DateTime.Now.AddDays(180);
                    else if (input.Contains("1 year"))
                        reminderDate = DateTime.Now.AddDays(365);

                    dbHelper.AddTask(taskText, "Cybersecurity task", reminderDate);
                    LogActivity("Task added with reminder", $"Added task: '{taskText}' with reminder for {reminderDate.ToString("yyyy-MM-dd HH:mm")}");
                    return $"Task added: '{taskText}'\nReminder set for: {reminderDate.ToString("yyyy-MM-dd HH:mm")}";
                }
                else
                {
                    dbHelper.AddTask(taskText, "Cybersecurity task", null);
                    LogActivity("Task added", $"Added task: '{taskText}' (no reminder)");
                    return $"Task added: '{taskText}'\nWould you like to set a reminder? Type 'remind me' with the task.";
                }
            }

            // VIEW TASKS
            if (input.Contains("view tasks") || input.Contains("show tasks") || input.Contains("list tasks"))
            {
                return GetTaskList();
            }

            // COMPLETE TASK
            if (input.Contains("complete task") || input.Contains("mark complete"))
            {
                int taskId = ExtractTaskId(input);
                if (taskId > 0)
                {
                    bool success = dbHelper.MarkTaskAsCompleted(taskId);
                    if (success)
                    {
                        LogActivity("Task completed", $"Marked task ID {taskId} as completed");
                        return $"Task {taskId} marked as completed! Great progress!";
                    }
                    else
                    {
                        return $"Task {taskId} not found. Please check the ID and try again.";
                    }
                }
                else
                {
                    return "Please specify the task ID to complete.\nExample: complete task 1";
                }
            }

            // DELETE TASK
            if (input.Contains("delete task") || input.Contains("remove task"))
            {
                int taskId = ExtractTaskId(input);
                if (taskId > 0)
                {
                    bool success = dbHelper.DeleteTask(taskId);
                    if (success)
                    {
                        LogActivity("Task deleted", $"Deleted task ID {taskId}");
                        return $"Task {taskId} deleted successfully!";
                    }
                    else
                    {
                        return $"Task {taskId} not found. Please check the ID and try again.";
                    }
                }
                else
                {
                    return "Please specify the task ID to delete.\nExample: delete task 1";
                }
            }

            return "I didn't quite understand that task command. Try:\n- add task [description]\n- view tasks\n- complete task [id]\n- delete task [id]";
        }

        private int ExtractTaskId(string input)
        {
            // Use regex to find numbers
            var match = Regex.Match(input, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return -1;
        }

        // ======================================================
        // ENHANCED ACTIVITY LOG WITH SHOW MORE
        // ======================================================

        private void LogActivity(string action, string details)
        {
            activityLog.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = details
            });
        }

        public string GetActivityLog(int page = 0)
        {
            if (activityLog.Count == 0)
                return "No activities logged yet. Start using the bot to build your activity log!";

            int totalItems = activityLog.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / ITEMS_PER_PAGE);

            if (page >= totalPages)
                page = totalPages - 1;

            int startIndex = Math.Max(0, totalItems - ((page + 1) * ITEMS_PER_PAGE));
            int endIndex = Math.Min(totalItems, startIndex + ITEMS_PER_PAGE);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Activity Log");
            sb.AppendLine($"Showing {startIndex + 1}-{endIndex} of {totalItems} entries");
            sb.AppendLine("===================");
            sb.AppendLine();

            for (int i = startIndex; i < endIndex; i++)
            {
                var entry = activityLog[i];
                sb.AppendLine($"* {entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"  Action: {entry.Action}");
                sb.AppendLine($"  Details: {entry.Details}");
                sb.AppendLine();
            }

            if (totalPages > 1)
            {
                sb.AppendLine($"--- Page {page + 1} of {totalPages} ---");
                sb.AppendLine("Type 'show more' to view the next page.");
                sb.AppendLine("Type 'show activity log' to go back to the first page.");
            }

            return sb.ToString();
        }

        // ======================================================
        // ENHANCED NLP - QUESTION DETECTION
        // ======================================================

        private bool IsQuestion(string input)
        {
            return input.Contains("?") ||
                   input.StartsWith("what") || input.StartsWith("how") ||
                   input.StartsWith("why") || input.StartsWith("when") ||
                   input.StartsWith("where") || input.StartsWith("who") ||
                   input.Contains("explain") || input.Contains("tell me");
        }

        private string HandleQuestion(string input)
        {
            if (input.Contains("what is phishing") || input.Contains("explain phishing"))
                return GetRandomItem(topicResponses["phishing"]) + "\n\n" + GetRandomItem(followUps["phishing"]);

            if (input.Contains("what is 2fa") || input.Contains("what is two-factor"))
                return GetRandomItem(topicResponses["2fa"]) + "\n\n" + GetRandomItem(followUps["2fa"]);

            if (input.Contains("what is social engineering"))
                return "Social engineering is a manipulation technique that exploits human psychology to gain unauthorized access to systems, networks, or physical locations. Attackers often use social engineering to trick users into revealing confidential information.";

            if (input.Contains("what is ransomware"))
                return "Ransomware is a type of malicious software that encrypts files on a victim's system, demanding payment (usually in cryptocurrency) in exchange for the decryption key. It's one of the most damaging cyber threats today.";

            if (input.Contains("how to create strong password") || input.Contains("how to make strong password"))
                return "Here are key tips for creating strong passwords:\n" +
                       "1. Use at least 12 characters\n" +
                       "2. Include uppercase, lowercase, numbers, and symbols\n" +
                       "3. Avoid common words or personal info\n" +
                       "4. Use a passphrase (multiple words)\n" +
                       "5. Don't reuse passwords across accounts";

            // If we don't have a specific answer, use topic detection
            string topic = DetectTopic(input);
            if (!string.IsNullOrWhiteSpace(topic))
                return GetRandomItem(topicResponses[topic]) + "\n\n" + GetRandomItem(followUps[topic]);

            return "That's a great question! I'd recommend checking our cybersecurity topics like phishing, passwords, or 2FA for more information.";
        }

        // ======================================================
        // ENHANCED TASK LIST DISPLAY
        // ======================================================

        private string GetTaskList()
        {
            var tasks = dbHelper.GetAllTasks();

            if (tasks.Count == 0)
                return "You have no tasks. Add a task to get started!\nExample: add task - Review privacy settings";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your Tasks");
            sb.AppendLine("===================");
            sb.AppendLine();

            int pendingCount = 0;
            int completedCount = 0;

            foreach (var task in tasks)
            {
                if (task.IsCompleted)
                    completedCount++;
                else
                    pendingCount++;

                string status = task.IsCompleted ? "[X]" : "[ ]";
                string reminder = task.ReminderDate.HasValue
                    ? $"Reminder: {task.ReminderDate.Value.ToString("yyyy-MM-dd HH:mm")}"
                    : "";
                string titleDisplay = task.IsCompleted ? $"~~{task.Title}~~" : task.Title;

                sb.AppendLine($"{status} #{task.Id} {titleDisplay}");
                if (!string.IsNullOrEmpty(task.Description))
                    sb.AppendLine($"   Description: {task.Description}");
                if (!string.IsNullOrEmpty(reminder))
                    sb.AppendLine($"   {reminder}");
                sb.AppendLine();
            }

            sb.AppendLine($"Summary: {pendingCount} pending, {completedCount} completed");
            sb.AppendLine();
            sb.AppendLine("Commands:");
            sb.AppendLine("- complete task [id] - Mark as complete");
            sb.AppendLine("- delete task [id] - Remove task");

            return sb.ToString();
        }

        // ======================================================
        // ENHANCED FOLLOW-UP HANDLING
        // ======================================================

        private string ContinueConversation(string moodReply)
        {
            if (string.IsNullOrWhiteSpace(activeTopic))
            {
                if (!string.IsNullOrEmpty(moodReply))
                    return moodReply + "\n\nChoose a cybersecurity topic like phishing, passwords, or 2FA.";
                return "Choose a cybersecurity topic first. Try asking about phishing, passwords, or 2FA.";
            }

            string response = GetRandomItem(followUps[activeTopic]);

            if (!string.IsNullOrEmpty(moodReply))
                return moodReply + "\n\n" + response;

            return response;
        }

        // ======================================================
        // ENHANCED MOOD DETECTION WITH MORE KEYWORDS
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
                        LogActivity("Mood detected", $"User feeling: {mood.Key}");
                        return GetMoodResponse(mood.Key);
                    }
                }
            }
            return "";
        }

        private string GetMoodResponse(string mood)
        {
            switch (mood)
            {
                case "worried": return "I understand your concern. Online threats can definitely feel stressful. Let's address your security together.";
                case "frustrated": return "Cybersecurity can feel overwhelming sometimes, but we can simplify it step by step.";
                case "curious": return "Curiosity is one of the best cybersecurity skills! Keep asking questions to stay safe online.";
                case "happy": return "Glad to hear you're feeling positive today! Let's build on that momentum.";
                case "nervous": return "That's understandable. Many cyber threats rely on fear and pressure - knowledge is your best defense!";
                case "confused": return "No problem at all. I can explain things more clearly - just let me know what you'd like to understand better.";
                case "excited": return "Great energy! Learning cybersecurity is very valuable and can be exciting!";
                case "angry": return "That sounds frustrating. Cybercrime affects many people, but you're taking the right step by learning to protect yourself.";
                case "overwhelmed": return "Take it step by step. Small cybersecurity habits make a big difference over time.";
                case "scared": return "It's okay to be scared. Use that feeling to motivate yourself to learn protection strategies.";
                default: return "";
            }
        }

        // ======================================================
        // ENHANCED MEMORY SYSTEM
        // ======================================================

        private string HandleMemory(string input)
        {
            if (input.Contains("my name is") || input.Contains("i am") || input.Contains("call me"))
            {
                string newName = input.Replace("my name is", "")
                                      .Replace("i am", "")
                                      .Replace("call me", "")
                                      .Trim();
                if (!string.IsNullOrEmpty(newName))
                {
                    userName = newName;
                    LogActivity("User info updated", $"User set name to: {userName}");
                    return $"Nice to meet you, {userName}! I'll remember your name from now on.";
                }
            }

            if (input.Contains("i like") || input.Contains("interested in") || input.Contains("i enjoy"))
            {
                string topic = DetectTopic(input);
                if (!string.IsNullOrWhiteSpace(topic))
                {
                    favouriteTopic = topic;
                    LogActivity("User preference saved", $"User interested in: {favouriteTopic}");
                    return $"I'll remember that you're interested in {favouriteTopic}. I can share more tips about this!";
                }
            }

            if (input.Contains("what do you remember") || input.Contains("what do you know about me"))
                return GetMemorySummary();

            return "";
        }

        private string GetMemorySummary()
        {
            string summary = "Here's what I remember about you:\n\n";
            summary += $"Name: {userName}\n";
            if (!string.IsNullOrWhiteSpace(favouriteTopic))
                summary += $"Favorite topic: {favouriteTopic}\n";
            if (!string.IsNullOrWhiteSpace(recentMood))
                summary += $"Recent mood: {recentMood}\n";

            summary += $"\nQuiz stats: {quiz.CurrentScore} correct out of {quiz.TotalQuestions} attempted\n";
            summary += $"\nTotal actions logged: {activityLog.Count}";

            return summary;
        }

        // ======================================================
        // ENHANCED TOPIC DETECTION
        // ======================================================

        private string DetectTopic(string input)
        {
            foreach (var topic in topicKeywords)
            {
                foreach (string keyword in topic.Value)
                {
                    if (input.Contains(keyword))
                        return topic.Key;
                }
            }
            return "";
        }

        // ======================================================
        // FOLLOW-UP DETECTION
        // ======================================================

        private bool IsFollowUp(string input)
        {
            return input.Contains("tell me more") || input.Contains("continue") ||
                   input.Contains("another tip") || input.Contains("explain more") ||
                   input.Contains("go deeper") || input.Contains("more") ||
                   input.Contains("elaborate") || input.Contains("further");
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
        // ENHANCED FALLBACK RESPONSES
        // ======================================================

        private string GetFallbackResponse()
        {
            List<string> replies = new List<string>()
            {
                "I didn't fully understand that. Try asking about:\n- Scams\n- Passwords\n- Phishing\n- Privacy\n- Safe Browsing\n- 2FA",
                "Could you rephrase that? I specialize in cybersecurity awareness and can help with:\n- Security tips\n- Task management\n- Cybersecurity quizzes",
                "Interesting! Ask me something about online safety. I can also:\n- Add tasks\n- Start a quiz\n- Show activity log",
                "I can help with cybersecurity topics like phishing, passwords, privacy, and 2FA. What would you like to know?",
                "Try asking a cybersecurity-related question or use commands like:\n- add task\n- start quiz\n- show activity log",
                "You can also ask me to:\n- Add a task (e.g., 'add task - Enable 2FA')\n- Start the cybersecurity quiz\n- Show recent activities"
            };
            return GetRandomItem(replies);
        }

        // ======================================================
        // RESPONSE DATABASE - ENHANCED
        // ======================================================

        private Dictionary<string, List<string>> InitializeResponses()
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "phishing", new List<string>()
                    {
                        "Phishing attacks pretend to be trusted messages to steal personal information.",
                        "Always inspect suspicious emails carefully before clicking links.",
                        "Cybercriminals use phishing to steal passwords and banking details.",
                        "Phishing emails often create urgency to pressure victims.",
                        "Hover over links before clicking them to see the real destination.",
                        "Some phishing websites look identical to real websites.",
                        "Never trust unexpected attachments from unknown senders.",
                        "Banks rarely request passwords through email.",
                        "Smishing is phishing done through SMS messages.",
                        "Vishing is phishing performed through phone calls."
                    }
                },
                {
                    "password", new List<string>()
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
                    "privacy", new List<string>()
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
                    "safe browsing", new List<string>()
                    {
                        "Always check for HTTPS before entering sensitive information.",
                        "Avoid suspicious downloads and pop-ups.",
                        "Public Wi-Fi can expose sensitive data.",
                        "Keep browsers updated for security patches.",
                        "Fake websites often imitate trusted brands.",
                        "Be careful with browser extensions.",
                        "Shortened links can hide dangerous websites.",
                        "Secure browsing habits reduce cyber risks.",
                        "Public computers are risky for sensitive accounts."
                    }
                },
                {
                    "scam", new List<string>()
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
                    "2fa", new List<string>()
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
        // FOLLOW-UP DATABASE - ENHANCED
        // ======================================================

        private Dictionary<string, List<string>> InitializeFollowUps()
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "phishing", new List<string>()
                    {
                        "Tip: Verify suspicious emails independently.",
                        "Avoid clicking links in unexpected messages.",
                        "Cybercriminals often copy company branding.",
                        "Always inspect sender addresses carefully.",
                        "Fake login pages commonly steal credentials.",
                        "Enable spam filters to reduce phishing emails."
                    }
                },
                {
                    "password", new List<string>()
                    {
                        "Use unique passwords for every account.",
                        "Passphrases improve both security and memorability.",
                        "Avoid common passwords like 123456.",
                        "Password managers improve security significantly.",
                        "2FA adds another layer of protection.",
                        "Make passwords at least 12 characters long."
                    }
                },
                {
                    "privacy", new List<string>()
                    {
                        "Review social media privacy settings regularly.",
                        "Avoid sharing your live location publicly.",
                        "Privacy awareness reduces identity theft risks.",
                        "Some apps request unnecessary permissions.",
                        "Be careful about public online posts.",
                        "Use private browsing for sensitive searches."
                    }
                },
                {
                    "safe browsing", new List<string>()
                    {
                        "Keep devices updated regularly.",
                        "Avoid suspicious browser extensions.",
                        "Public Wi-Fi should be used carefully.",
                        "Secure browsing habits protect personal data.",
                        "Download software only from trusted sources.",
                        "Use a VPN on public networks."
                    }
                },
                {
                    "scam", new List<string>()
                    {
                        "Scammers often create panic or urgency.",
                        "Always verify requests for money.",
                        "Prize scams usually request personal details.",
                        "Scammers frequently impersonate trusted companies.",
                        "Cybercriminals rely on emotional reactions.",
                        "Don't respond to suspicious messages."
                    }
                },
                {
                    "2fa", new List<string>()
                    {
                        "Authenticator apps provide stronger protection.",
                        "2FA helps stop many account attacks.",
                        "Recovery codes should be stored securely.",
                        "Security keys offer advanced authentication.",
                        "2FA greatly improves account security.",
                        "Enable 2FA on all important accounts."
                    }
                }
            };
        }

        // ======================================================
        // KEYWORD DATABASE - ENHANCED
        // ======================================================

        private Dictionary<string, List<string>> InitializeKeywords()
        {
            return new Dictionary<string, List<string>>()
            {
                { "phishing", new List<string>() { "phishing", "fake email", "suspicious email", "smishing", "vishing", "phish" } },
                { "password", new List<string>() { "password", "login", "credentials", "passphrase", "passcode" } },
                { "privacy", new List<string>() { "privacy", "tracking", "personal information", "private" } },
                { "safe browsing", new List<string>() { "browser", "website", "wifi", "https", "secure browsing", "safe browsing" } },
                { "scam", new List<string>() { "scam", "fraud", "fake offer", "otp", "scammer" } },
                { "2fa", new List<string>() { "2fa", "mfa", "verification code", "authenticator", "two factor" } }
            };
        }

        // ======================================================
        // MOOD KEYWORDS - ENHANCED
        // ======================================================

        private Dictionary<string, List<string>> InitializeMoodKeywords()
        {
            return new Dictionary<string, List<string>>()
            {
                { "worried", new List<string>() { "worried", "scared", "unsafe", "concerned", "anxious" } },
                { "frustrated", new List<string>() { "frustrated", "annoyed", "fed up" } },
                { "curious", new List<string>() { "curious", "interested", "wondering" } },
                { "happy", new List<string>() { "happy", "great", "good", "excellent", "wonderful" } },
                { "nervous", new List<string>() { "nervous", "anxious", "uneasy" } },
                { "confused", new List<string>() { "confused", "lost", "confusing" } },
                { "excited", new List<string>() { "excited", "motivated", "pumped" } },
                { "angry", new List<string>() { "angry", "mad", "furious" } },
                { "overwhelmed", new List<string>() { "overwhelmed", "stressed", "too much" } },
                { "scared", new List<string>() { "scared", "terrified", "frightened" } }
            };
        }

        // ======================================================
        // ASCII ART
        // ======================================================

        public string GetAsciiArt()
        {
            return AsciiArt.GetArt();
        }

        public string GetUserName()
        {
            return userName;
        }
    }

    // ======================================================
    // ACTIVITY LOG ENTRY CLASS
    // ======================================================

    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}