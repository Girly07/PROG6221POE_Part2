using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace PROG6221POE
{
    public class DatabaseHelper
    {
        private string connectionString;
        private bool isConnected = false;

        public DatabaseHelper()
        {
            // Connection string - update with your details
            connectionString = "Server=localhost;Database=cyberbot_db;Uid=root;Pwd=Girly@070322;";

            try
            {
                CreateTableIfNotExists();
                isConnected = true;
            }
            catch (Exception ex)
            {
                // Show error but don't crash - allow app to work without DB
                MessageBox.Show(
                    $"Database connection failed: {ex.Message}\n\nTasks will not be saved to database, but other features will work.",
                    "Database Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                isConnected = false;
            }
        }

        private void CreateTableIfNotExists()
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(255) NOT NULL,
                    description TEXT,
                    reminder_date DATETIME,
                    is_completed BOOLEAN DEFAULT FALSE,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddTask(string title, string description, DateTime? reminderDate)
        {
            if (!isConnected)
            {
                MessageBox.Show("Database not connected. Task will be stored in memory only.",
                    "Database Offline", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string query = @"
                    INSERT INTO tasks (title, description, reminder_date) 
                    VALUES (@title, @description, @reminderDate)";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title ?? "Untitled Task");
                        cmd.Parameters.AddWithValue("@description", description ?? "");
                        cmd.Parameters.AddWithValue("@reminderDate",
                            reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add task: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<Task> GetAllTasks()
        {
            List<Task> tasks = new List<Task>();

            if (!isConnected)
                return tasks; // Return empty list

            try
            {
                string query = "SELECT * FROM tasks ORDER BY is_completed ASC, created_at DESC";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Task
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description"))
                                    ? "" : reader.GetString("description"),
                                ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date"))
                                    ? (DateTime?)null : reader.GetDateTime("reminder_date"),
                                IsCompleted = reader.GetBoolean("is_completed"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve tasks: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return tasks;
        }

        public bool DeleteTask(int id)
        {
            if (!isConnected)
                return false;

            try
            {
                string query = "DELETE FROM tasks WHERE id = @id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete task: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool MarkTaskAsCompleted(int id)
        {
            if (!isConnected)
                return false;

            try
            {
                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to complete task: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool IsConnected() => isConnected;
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "⬜";
            string reminder = ReminderDate.HasValue
                ? $"📅 {ReminderDate.Value.ToString("yyyy-MM-dd HH:mm")}"
                : "";
            string titleDisplay = IsCompleted ? $"~~{Title}~~" : Title;
            return $"{status} #{Id} {titleDisplay} {reminder}".Trim();
        }
    }
}