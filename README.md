# PROG6221 POE — CYBER BOT
Cybersecurity Awareness Chatbot in C# Windows Forms

---

# Project Overview

CYBER BOT is a Windows Forms cybersecurity awareness chatbot developed in C#.  
The application is designed to educate users about online safety through interactive conversations, intelligent responses, sentiment detection, memory features, ASCII art, and voice greetings.

The chatbot provides information about:
- Phishing
- Password safety
- Online scams
- Privacy
- Safe browsing
- Two-factor authentication (2FA)

The project focuses on creating a professional, engaging, and user-friendly cybersecurity assistant.

---

# Project Description

This project is a cybersecurity awareness chatbot built using C# and Windows Forms.  
The chatbot interacts with users and teaches important cybersecurity concepts through intelligent conversations and dynamic responses.

The chatbot recognises cybersecurity-related keywords and responds with useful educational information about online threats and digital safety practices.

The project demonstrates:
- Object-Oriented Programming (OOP)
- Event-driven programming
- User interaction handling
- Sentiment detection
- Memory recall systems
- Dynamic chatbot conversations

---

# Features

## Intelligent Chatbot Responses
The chatbot recognises cybersecurity-related keywords and responds naturally with a wide range of dynamic responses.

### Supported Topics
- Phishing
- Password security
- Online scams
- Privacy
- Safe browsing
- 2FA / MFA
- General cybersecurity awareness

---

## Random Dynamic Responses
The chatbot uses collections of multiple responses for each topic to avoid repetitive conversations.

This creates:
- More natural conversations
- Better user engagement
- Improved realism
- Smooth conversational flow

---

## Smart Keyword Recognition
The chatbot detects important cybersecurity keywords and provides relevant responses automatically.

Example keywords:
- phishing
- password
- scam
- privacy
- browser
- 2FA
- suspicious email

---

## Sentiment Detection
The chatbot detects user emotions and adjusts responses dynamically.

### Supported moods
- Worried
- Frustrated
- Curious
- Confused
- Nervous
- Angry
- Excited
- Overwhelmed
- Happy

Example:

```text
User: I am worried about scams

Bot:
I understand your concern. Online threats can definitely feel stressful.
```

---

## Memory System
The chatbot remembers:
- User name
- Favourite cybersecurity topic
- Recent mood

Example:

```text
User: My name is Alex

Bot:
Nice to meet you, Alex. I will remember your name.
```

This improves conversational realism and engagement.

---

## Follow-Up Conversations
The chatbot supports conversational continuation.

Example:

```text
tell me more
another tip
continue
go deeper
```

This improves conversational flow and realism.

---

## Menu-Driven User Interaction
The project uses interactive controls and event handling to create a smooth chatbot experience.

Features include:
- Send button
- Text input field
- Interactive chat display
- Keyboard Enter support

---

## User Input Handling
The chatbot validates user input and prevents:
- Empty messages
- Invalid submissions
- Crashes caused by missing information

---

## ASCII Art
The chatbot displays cybersecurity-themed ASCII art at startup for improved presentation.

Example:

```text
CYBER BOT

.-""""-.
/ -   -  \
|  o   o  |
|    ^    |
\  ---  /
'-___-'
```

---

## Voice Greeting
The application plays a WAV audio greeting when the chatbot starts.

Example:

```text
Welcome to Cyber Bot. Your cybersecurity assistant is now online.
```

---

## Professional User Interface
The Windows Forms interface includes:
- Custom colour theme
- Styled buttons
- Rich text chat display
- Input validation
- Professional layout
- Smooth user interaction
- Professional cybersecurity appearance

---

# Technologies Used

- C#
- .NET Windows Forms
- Visual Studio
- Object-Oriented Programming (OOP)
- Dictionaries and Lists
- Event-Driven Programming

---

# Object-Oriented Programming Concepts Used

## Encapsulation
Classes manage their own data and behaviour.

---

## Abstraction
Complex chatbot processes are simplified through methods.

---

## Modularity
The project is separated into multiple focused classes.

---

## Reusability
Methods and collections are reused throughout the application.

---

# Project Structure

```text
PROG6221POE
│
├── Audio
│   └── greeting.wav
│
├── AsciiArt.cs
├── AudioPlayer.cs
├── ChatbotEngine.cs
├── Form1.cs
├── Program.cs
└── README.md
```

---

# File Descriptions

## Program.cs
Application entry point.

Responsible for:
- Starting the Windows Forms application
- Launching Form1

---

## Form1.cs
Main graphical interface.

Responsible for:
- Chat display
- User input
- Buttons
- Interface styling
- Event handling

---

## ChatbotEngine.cs
Core chatbot logic.

Responsible for:
- Topic detection
- Random responses
- Memory system
- Sentiment analysis
- Follow-up handling
- Conversational flow

---

## AudioPlayer.cs
Handles WAV audio playback.

Responsible for:
- Loading greeting audio
- Error handling
- Playing startup greeting

---

## AsciiArt.cs
Stores and returns chatbot ASCII art.

---

# How To Run The Project

## Requirements
- Visual Studio 2022 or newer
- .NET Windows Forms support
- Windows OS

---

## Steps

### 1. Open the Project
Open the solution in Visual Studio.

---

### 2. Add Audio File
Create an `Audio` folder inside the project.

Add:

```text
greeting.wav
```

Set properties:

| Property | Value |
|---|---|
| Build Action | Content |
| Copy to Output Directory | Copy if newer |

---

### 3. Build the Project

Select:

```text
Build → Build Solution
```

---

### 4. Run the Application

Press:

```text
F5
```

or click:

```text
Start
```

---

# Example User Inputs

```text
What is phishing?
```

```text
How do I create a strong password?
```

```text
I am worried about scams
```

```text
Tell me more
```

```text
What do you remember about me?
```

---

# Topics Covered

- Password Safety
- Phishing Attacks
- Safe Browsing
- Online Scams
- Privacy Protection
- Two-Factor Authentication (2FA)
- General Cybersecurity Awareness

---

# Error Handling

The project includes:
- Empty input validation
- Audio file existence checks
- Exception handling for audio playback
- Safe response handling

---

# Educational Purpose

The chatbot was designed to:
- Promote cybersecurity awareness
- Educate users about online threats
- Encourage safer online behaviour
- Demonstrate practical C# programming skills

---

# Future Improvements

Possible future upgrades:
- AI/NLP integration
- Database storage
- User accounts
- Voice recognition
- Animated UI
- Online threat API integration
- Chat history export
- Dark/light themes

---

# References

OpenAI. (2026) ChatGPT (GPT-5.3). Available at: https://chat.openai.com/ (Accessed: 14 April 2026).

---

# Author

Mashilo Mamane Girly

Developed for:
PROG6221 Practical Assessment (POE)

Cybersecurity Awareness Chatbot Project

---

# Conclusion

CYBER BOT demonstrates:
- Strong object-oriented programming
- Professional Windows Forms development
- Dynamic conversational logic
- Sentiment-aware interaction
- User memory systems
- Cybersecurity education principles
- Smart keyword recognition
- Professional user interface design

The project provides an engaging and educational cybersecurity assistant experience while showcasing practical software development skills in C#.
