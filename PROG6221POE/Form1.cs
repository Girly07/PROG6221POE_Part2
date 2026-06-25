using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PROG6221POE
{
    /*
     * ==========================================================
     * FORM1
     * ----------------------------------------------------------
     * Main graphical user interface for CYBER BOT.
     * Handles:
     * - User interaction
     * - Chat display
     * - Button events
     * - Input processing
     * - Startup sequence
     * ==========================================================
     */

    public partial class Form1 : Form
    {
        // ======================================================
        // CHATBOT ENGINE
        // ======================================================

        private ChatbotEngine? bot;

        // ======================================================
        // UI COMPONENTS
        // ======================================================

        private Label? lblTitle;
        private Label? lblSubtitle;
        private Label? lblName;
        private TextBox? txtName;
        private TextBox? txtInput;
        private Button? btnStart;
        private Button? btnSend;
        private RichTextBox? rtbChat;

        // ======================================================
        // CONSTRUCTOR
        // ======================================================

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        // ======================================================
        // UI CREATION
        // ======================================================

        private void BuildInterface()
        {
            // --------------------------------------------------
            // FORM SETTINGS
            // --------------------------------------------------

            Text = "CYBER BOT";
            Size = new Size(980, 740);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(20, 8, 8);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // --------------------------------------------------
            // TITLE
            // --------------------------------------------------

            lblTitle = new Label();
            lblTitle.Text = "CYBER BOT";
            lblTitle.ForeColor = Color.FromArgb(255, 80, 80);
            lblTitle.Font = new Font("Consolas", 30, FontStyle.Bold);
            lblTitle.Location = new Point(35, 20);
            lblTitle.AutoSize = true;
            Controls.Add(lblTitle);

            // --------------------------------------------------
            // SUBTITLE
            // --------------------------------------------------

            lblSubtitle = new Label();
            lblSubtitle.Text = "Interactive Cybersecurity Awareness Assistant";
            lblSubtitle.ForeColor = Color.FromArgb(255, 180, 180);
            lblSubtitle.Font = new Font("Consolas", 10);
            lblSubtitle.Location = new Point(40, 78);
            lblSubtitle.AutoSize = true;
            Controls.Add(lblSubtitle);

            // --------------------------------------------------
            // USER LABEL
            // --------------------------------------------------

            lblName = new Label();
            lblName.Text = "USER";
            lblName.ForeColor = Color.FromArgb(255, 210, 210);
            lblName.Font = new Font("Consolas", 10, FontStyle.Bold);
            lblName.Location = new Point(40, 125);
            lblName.AutoSize = true;
            Controls.Add(lblName);

            // --------------------------------------------------
            // USERNAME TEXTBOX
            // --------------------------------------------------

            txtName = new TextBox();
            txtName.Location = new Point(105, 120);
            txtName.Size = new Size(320, 35);
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

            // --------------------------------------------------
            // START BUTTON
            // --------------------------------------------------

            btnStart = new Button();
            btnStart.Text = "START";
            btnStart.Location = new Point(445, 118);
            btnStart.Size = new Size(150, 42);
            btnStart.BackColor = Color.FromArgb(170, 45, 45);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnStart.FlatAppearance.BorderSize = 1;
            btnStart.Font = new Font("Consolas", 10, FontStyle.Bold);
            btnStart.Cursor = Cursors.Hand;
            btnStart.Click += BtnStart_Click;
            Controls.Add(btnStart);

            // --------------------------------------------------
            // CHAT AREA
            // --------------------------------------------------

            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(40, 190);
            rtbChat.Size = new Size(885, 410);
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.FromArgb(10, 3, 3);
            rtbChat.ForeColor = Color.FromArgb(255, 210, 210);
            rtbChat.BorderStyle = BorderStyle.FixedSingle;
            rtbChat.Font = new Font("Consolas", 10);
            Controls.Add(rtbChat);

            // --------------------------------------------------
            // INPUT FIELD
            // --------------------------------------------------

            txtInput = new TextBox();
            txtInput.Location = new Point(40, 625);
            txtInput.Size = new Size(720, 40);
            txtInput.Font = new Font("Consolas", 11);
            txtInput.BackColor = Color.FromArgb(55, 20, 20);
            txtInput.ForeColor = Color.White;
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Enabled = false;
            txtInput.KeyDown += TxtInput_KeyDown;
            Controls.Add(txtInput);

            // --------------------------------------------------
            // SEND BUTTON
            // --------------------------------------------------

            btnSend = new Button();
            btnSend.Text = "SEND";
            btnSend.Location = new Point(780, 621);
            btnSend.Size = new Size(145, 42);
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

            // --------------------------------------------------
            // TITLE BAR COLOR
            // --------------------------------------------------
            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(170, 45, 45), 2))
                {
                    e.Graphics.DrawLine(pen, 0, 115, this.Width, 115);
                }
            };
        }

        // ======================================================
        // START BUTTON EVENT - UPDATED WITH ALL FEATURES
        // ======================================================

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            string name = txtName!.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || name == "Enter your name...")
                name = "Friend";

            bot = new ChatbotEngine(name);
            rtbChat!.Clear();

            AddBotMessage(bot.GetAsciiArt());
            AddBotMessage("SYSTEM ONLINE");
            AddBotMessage("Welcome, " + name + ".");
            AddBotMessage("Cybersecurity assistant initialized successfully.");

            // Updated welcome messages with all Part 3 features
            AddBotMessage("Topics available:\n- Phishing\n- Passwords\n- Scams\n- Privacy\n- Safe Browsing\n- 2FA");
            AddBotMessage("Task Management:\n- Add task [description]\n- View tasks\n- Complete task [id]\n- Delete task [id]");
            AddBotMessage("Quiz:\n- Start quiz - Test your cybersecurity knowledge!");
            AddBotMessage("Activity Log:\n- Show activity log - View recent actions");
            AddBotMessage("Examples:\n'I am worried about scams'\n'Tell me more about phishing'\n'How do I create a strong password?'\n'Add task - Enable 2FA'\n'Start quiz'");

            string audioPath = Path.Combine(Application.StartupPath, "Audio.wav");
            AudioPlayer.PlayGreeting(audioPath);

            txtInput!.Enabled = true;
            btnSend!.Enabled = true;
            txtInput.Focus();
        }

        // ======================================================
        // SEND BUTTON EVENT
        // ======================================================

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            ProcessUserInput();
        }

        // ======================================================
        // ENTER KEY EVENT
        // ======================================================

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessUserInput();
                e.SuppressKeyPress = true;
            }
        }

        // ======================================================
        // INPUT PROCESSING
        // ======================================================

        private void ProcessUserInput()
        {
            if (bot == null)
            {
                MessageBox.Show("Start the chatbot first.", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string userInput = txtInput!.Text.Trim();
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

        // ======================================================
        // USER MESSAGE DISPLAY
        // ======================================================

        private void AddUserMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            rtbChat!.SelectionColor = Color.FromArgb(255, 160, 160);
            rtbChat.AppendText("[" + timestamp + "] USER > " + message + Environment.NewLine + Environment.NewLine);
            rtbChat.SelectionColor = Color.FromArgb(255, 210, 210);
            rtbChat.ScrollToCaret();
        }

        // ======================================================
        // BOT MESSAGE DISPLAY
        // ======================================================

        private void AddBotMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            rtbChat!.SelectionColor = Color.FromArgb(255, 210, 210);
            rtbChat.AppendText("[" + timestamp + "] BOT > " + message + Environment.NewLine + Environment.NewLine);
            rtbChat.SelectionColor = Color.FromArgb(255, 210, 210);
            rtbChat.ScrollToCaret();
        }
    }
}