using System;
using System.Collections.Generic;
using System.Linq;

namespace PROG6221POE
{
    public class Quiz
    {
        private List<Question> questions;
        private int currentQuestionIndex;
        private int score;
        private bool quizActive;

        public bool QuizActive => quizActive;
        public int TotalQuestions => questions.Count;

        public Quiz()
        {
            InitializeQuestions();
            Reset();
        }

        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                new Question
                {
                    Text = "What should you do if you receive an email asking for your password?",
                    Options = new List<string>
                    {
                        "Reply with your password",
                        "Delete the email",
                        "Report the email as phishing",
                        "Ignore it"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others."
                },
                new Question
                {
                    Text = "Which of the following is a strong password?",
                    Options = new List<string>
                    {
                        "password123",
                        "123456",
                        "M@nch3st3rUtd2024!",
                        "qwerty"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "A strong password includes uppercase, lowercase, numbers, and special characters."
                },
                new Question
                {
                    Text = "What does 2FA stand for?",
                    Options = new List<string>
                    {
                        "Two-Factor Authentication",
                        "Two-Factor Authorization",
                        "Two-Factor Access",
                        "Two-Factor Approval"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Two-Factor Authentication adds an extra layer of security to your accounts."
                },
                new Question
                {
                    Text = "Is it safe to use public Wi-Fi for online banking?",
                    Options = new List<string>
                    {
                        "True",
                        "False"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Public Wi-Fi networks are often unsecured and can expose your sensitive data."
                },
                new Question
                {
                    Text = "What is phishing?",
                    Options = new List<string>
                    {
                        "A type of fishing",
                        "A cyber attack using fake emails",
                        "A social media platform",
                        "A programming language"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Phishing is a cyber attack where criminals send fake emails to steal information."
                },
                new Question
                {
                    Text = "How often should you update your passwords?",
                    Options = new List<string>
                    {
                        "Never",
                        "Every 10 years",
                        "Every 3-6 months",
                        "Only when hacked"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Regular password updates help maintain account security."
                },
                new Question
                {
                    Text = "What should you check for before entering personal info on a website?",
                    Options = new List<string>
                    {
                        "The website design",
                        "The URL starts with HTTPS",
                        "The number of ads",
                        "The website's social media presence"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "HTTPS indicates a secure connection for transmitting sensitive data."
                },
                new Question
                {
                    Text = "Is it safe to share your location on social media?",
                    Options = new List<string>
                    {
                        "True",
                        "False"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Sharing your location can reveal your daily routines and compromise your privacy."
                },
                new Question
                {
                    Text = "What is social engineering in cybersecurity?",
                    Options = new List<string>
                    {
                        "Building social networks",
                        "Creating engineering systems",
                        "Manipulating people to reveal information",
                        "Social media marketing"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Social engineering uses psychological manipulation to trick people into sharing information."
                },
                new Question
                {
                    Text = "What should you do with suspicious attachments?",
                    Options = new List<string>
                    {
                        "Open them",
                        "Forward to friends",
                        "Delete and do not open",
                        "Save to desktop"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Suspicious attachments may contain malware that can harm your device."
                },
                new Question
                {
                    Text = "What is the purpose of a VPN?",
                    Options = new List<string>
                    {
                        "Speed up internet",
                        "Protect online privacy and security",
                        "Block all websites",
                        "Increase social media likes"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "VPNs encrypt your internet connection and protect your privacy online."
                },
                new Question
                {
                    Text = "Is it safe to use the same password for multiple accounts?",
                    Options = new List<string>
                    {
                        "True",
                        "False"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Using the same password across accounts means one breach compromises all accounts."
                }
            };
        }

        public void Reset()
        {
            currentQuestionIndex = 0;
            score = 0;
            quizActive = false;
        }

        public string StartQuiz()
        {
            Reset();
            quizActive = true;
            return $"Quiz started! Answer {questions.Count} questions to test your cybersecurity knowledge.\n\n" + GetCurrentQuestion();
        }

        public string GetCurrentQuestion()
        {
            if (!quizActive || currentQuestionIndex >= questions.Count)
                return "No active quiz.";

            return questions[currentQuestionIndex].GetDisplayText();
        }

        public string SubmitAnswer(int answerIndex)
        {
            if (!quizActive)
                return "Quiz is not active.";

            var question = questions[currentQuestionIndex];
            bool isCorrect = question.CorrectAnswerIndex == answerIndex;

            if (isCorrect)
                score++;

            string result = $"Question {currentQuestionIndex + 1}: " +
                           (isCorrect ? "✓ Correct!" : "✗ Incorrect.") +
                           $"\n{question.Explanation}\n";

            currentQuestionIndex++;

            if (currentQuestionIndex >= questions.Count)
            {
                quizActive = false;
                result += "\n" + GetFinalScore();
            }
            else
            {
                result += "\n" + GetCurrentQuestion();
            }

            return result;
        }

        public string GetFinalScore()
        {
            double percentage = (double)score / questions.Count * 100;
            string feedback;

            if (percentage >= 90)
                feedback = "🌟 Excellent! You're a Cybersecurity Pro!";
            else if (percentage >= 70)
                feedback = "👏 Good job! Keep learning to improve your cybersecurity knowledge!";
            else if (percentage >= 50)
                feedback = "📚 Nice try! Review the topics and try again to boost your score.";
            else
                feedback = "💪 Keep learning! Cybersecurity is important for everyone.";

            return $"Quiz Complete!\nScore: {score}/{questions.Count} ({percentage:F1}%)\n\n{feedback}";
        }
    }

    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }

        public string GetDisplayText()
        {
            string display = $"Question: {Text}\n\n";
            for (int i = 0; i < Options.Count; i++)
            {
                display += $"{i + 1}. {Options[i]}\n";
            }
            display += "\nType the number of your answer (e.g., '1') or the text.";
            return display;
        }
    }
}