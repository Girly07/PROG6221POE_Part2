using PROG6221POE;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PROG6221POE
{
    public partial class Form1 : Form
    {
        private ChatbotEngine? bot;

        private Label? lblTitle;
        private Label? lblSubtitle;
        private Label? lblName;

        private TextBox? txtName;
        private TextBox? txtInput;

        private Button? btnStart;
        private Button? btnSend;

        private RichTextBox? rtbChat;

        public Form1()
        {
            InitializeComponent();

            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "CYBER BOT";

            Size = new Size(960, 720);

            StartPosition = FormStartPosition.CenterScreen;

            BackColor = Color.FromArgb(28, 10, 10);

            FormBorderStyle = FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            // Main title
            lblTitle = new Label();

            lblTitle.Text = "CYBER BOT";

            lblTitle.ForeColor = Color.FromArgb(255, 90, 90);

            lblTitle.Font = new Font("Consolas", 30, FontStyle.Bold);

            lblTitle.Location = new Point(35, 20);

            lblTitle.AutoSize = true;

            Controls.Add(lblTitle);

            // Subtitle under title
            lblSubtitle = new Label();

            lblSubtitle.Text = "Interactive Cybersecurity Assistant";

            lblSubtitle.ForeColor = Color.FromArgb(255, 180, 180);

            lblSubtitle.Font = new Font("Consolas", 10);

            lblSubtitle.Location = new Point(40, 75);

            lblSubtitle.AutoSize = true;

            Controls.Add(lblSubtitle);

            // Name label
            lblName = new Label();

            lblName.Text = "USER";

            lblName.ForeColor = Color.FromArgb(255, 200, 200);

            lblName.Font = new Font("Consolas", 10, FontStyle.Bold);

            lblName.Location = new Point(40, 120);

            lblName.AutoSize = true;

            Controls.Add(lblName);

            // Name input field
            txtName = new TextBox();

            txtName.Location = new Point(105, 115);

            txtName.Size = new Size(300, 35);

            txtName.Font = new Font("Consolas", 11);

            txtName.BackColor = Color.FromArgb(55, 20, 20);

            txtName.ForeColor = Color.Gray;

            txtName.BorderStyle = BorderStyle.FixedSingle;

            txtName.Text = "Enter your name...";

            txtName.Enter += (s, e) =>
            {
                if (txtName.Text == "Enter your name...")
                {
                    txtName.Text = "";

                    txtName.ForeColor = Color.White;
                }
            };

            txtName.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Text = "Enter your name...";

                    txtName.ForeColor = Color.Gray;
                }
            };

            Controls.Add(txtName);

            // Start button
            btnStart = new Button();

            btnStart.Text = "START";

            btnStart.Location = new Point(425, 112);

            btnStart.Size = new Size(145, 42);

            btnStart.BackColor = Color.FromArgb(170, 45, 45);

            btnStart.ForeColor = Color.White;

            btnStart.FlatStyle = FlatStyle.Flat;

            btnStart.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);

            btnStart.FlatAppearance.BorderSize = 1;

            btnStart.Font = new Font("Consolas", 10, FontStyle.Bold);

            btnStart.Cursor = Cursors.Hand;

            btnStart.Click += BtnStart_Click;

            Controls.Add(btnStart);

            // Chat area
            rtbChat = new RichTextBox();

            rtbChat.Location = new Point(40, 185);

            rtbChat.Size = new Size(870, 395);

            rtbChat.ReadOnly = true;

            rtbChat.BackColor = Color.FromArgb(15, 5, 5);

            rtbChat.ForeColor = Color.FromArgb(255, 210, 210);

            rtbChat.BorderStyle = BorderStyle.FixedSingle;

            rtbChat.Font = new Font("Consolas", 10);

            Controls.Add(rtbChat);

            // User message input
            txtInput = new TextBox();

            txtInput.Location = new Point(40, 610);

            txtInput.Size = new Size(710, 40);

            txtInput.Font = new Font("Consolas", 11);

            txtInput.BackColor = Color.FromArgb(55, 20, 20);

            txtInput.ForeColor = Color.White;

            txtInput.BorderStyle = BorderStyle.FixedSingle;

            txtInput.Enabled = false;

            txtInput.KeyDown += TxtInput_KeyDown;

            Controls.Add(txtInput);

            // Send button
            btnSend = new Button();

            btnSend.Text = "SEND";

            btnSend.Location = new Point(770, 606);

            btnSend.Size = new Size(140, 42);

            btnSend.BackColor = Color.FromArgb(170, 45, 45);

            btnSend.ForeColor = Color.White;

            btnSend.FlatStyle = FlatStyle.Flat;

            btnSend.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);

            btnSend.FlatAppearance.BorderSize = 1;

            btnSend.Font = new Font("Consolas", 10, FontStyle.Bold);

            btnSend.Cursor = Cursors.Hand;

            btnSend.Enabled = false;

            btnSend.Click += BtnSend_Click;

            Controls.Add(btnSend);
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            string name = txtName!.Text.Trim();

            // Prevents placeholder text from becoming the username
            if (string.IsNullOrWhiteSpace(name) || name == "Enter your name...")
            {
                name = "Friend";
            }

            bot = new ChatbotEngine(name);

            rtbChat!.Clear();

            AddBotMessage(bot.GetAsciiArt());

            AddBotMessage("Session initialized successfully.");

            AddBotMessage("Welcome, " + name + ".");

            AddBotMessage("Ask about phishing, passwords, scams, privacy, or safe browsing.");

            AddBotMessage("Try: 'I am worried about scams' or 'tell me more'.");

            AudioPlayer.PlayGreeting("Audio.wav");

            txtInput!.Enabled = true;

            btnSend!.Enabled = true;

            txtInput.Focus();
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            // Allows Enter key to send messages
            if (e.KeyCode == Keys.Enter)
            {
                ProcessUserInput();

                e.SuppressKeyPress = true;
            }
        }

        private void ProcessUserInput()
        {
            if (bot == null)
            {
                MessageBox.Show("Start the assistant first.");

                return;
            }

            string userInput = txtInput!.Text.Trim();

            // Prevents empty submissions
            if (string.IsNullOrWhiteSpace(userInput))
            {
                AddBotMessage("Input required.");

                return;
            }

            AddUserMessage(userInput);

            string response = bot.GetResponse(userInput);

            AddBotMessage(response);

            txtInput.Clear();

            txtInput.Focus();
        }

        private void AddUserMessage(string message)
        {
            rtbChat!.SelectionColor = Color.FromArgb(255, 160, 160);

            rtbChat.AppendText("USER > "
                               + message
                               + Environment.NewLine
                               + Environment.NewLine);

            rtbChat.SelectionColor = Color.FromArgb(255, 210, 210);
        }

        private void AddBotMessage(string message)
        {
            rtbChat!.SelectionColor = Color.FromArgb(255, 210, 210);

            rtbChat.AppendText("BOT > "
                               + message
                               + Environment.NewLine
                               + Environment.NewLine);

            rtbChat.SelectionColor = Color.FromArgb(255, 210, 210);

            rtbChat.ScrollToCaret();
        }
    }
}