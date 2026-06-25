using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace PROG6221POE
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            // Update this with your MySQL connection details
            connectionString = "Server=localhost;Database=cyberbot_db;Uid=root;Pwd=Girly@070322;";

            // Create table if it doesn't exist
            CreateTableIfNotExists();
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
            string query = @"
                INSERT INTO tasks (title, description, reminder_date) 
                VALUES (@title, @description, @reminderDate)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@reminderDate",
                        reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Task> GetAllTasks()
        {
            List<Task> tasks = new List<Task>();
            string query = "SELECT * FROM tasks ORDER BY created_at DESC";

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
            return tasks;
        }

        public void DeleteTask(int id)
        {
            string query = "DELETE FROM tasks WHERE id = @id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MarkTaskAsCompleted(int id)
        {
            string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
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
            string status = IsCompleted ? "[✓]" : "[ ]";
            string reminder = ReminderDate.HasValue
                ? $" (Reminder: {ReminderDate.Value.ToString("yyyy-MM-dd HH:mm")})"
                : "";
            return $"{status} {Title}{reminder}";
        }
    }
}