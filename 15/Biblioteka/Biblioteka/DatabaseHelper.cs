using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Biblioteka
{
    internal class DatabaseHelper
    {
        private static string _dbPath = Path.Combine(Application.StartupPath, "LibraryDB.sqlite");
        private static string _connectionString = $"Data Source={_dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using (var conn = GetConnection())
            {
                conn.Open();

                var commands = new[]
                {
                    @"CREATE TABLE IF NOT EXISTS Users (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        FullName TEXT,
                        Email TEXT,
                        Role TEXT DEFAULT 'User' CHECK (Role IN ('Admin', 'Librarian', 'User')))",

                    @"CREATE TABLE IF NOT EXISTS Books (
                        BookId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Genre TEXT,
                        Year INTEGER,
                        Description TEXT,
                        FilePath TEXT,
                        CoverImage BLOB,
                        AddedDate DATETIME DEFAULT CURRENT_TIMESTAMP)",

                    @"CREATE TABLE IF NOT EXISTS BookLoans (
                        LoanId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BookId INTEGER REFERENCES Books(BookId),
                        UserId INTEGER REFERENCES Users(UserId),
                        LoanDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                        ReturnDate DATETIME,
                        Status TEXT DEFAULT 'OnLoan' CHECK (Status IN ('OnLoan', 'Returned', 'Overdue')))",

                    @"CREATE TABLE IF NOT EXISTS Reviews (
                        ReviewId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BookId INTEGER REFERENCES Books(BookId),
                        UserId INTEGER REFERENCES Users(UserId),
                        Rating INTEGER CHECK (Rating BETWEEN 1 AND 5),
                        Comment TEXT,
                        ReviewDate DATETIME DEFAULT CURRENT_TIMESTAMP)"
                };

                foreach (var cmdText in commands)
                {
                    using (var cmd = new SQLiteCommand(cmdText, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Add default admin if not exists
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username = 'admin'", conn))
                {
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        using (var insertCmd = new SQLiteCommand(
                            "INSERT INTO Users (Username, Password, FullName, Role) VALUES ('admin', 'admin123', 'Администратор', 'Admin')", conn))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
