using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Ychet.Models;
namespace Ychet.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Accounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Balance DECIMAL NOT NULL DEFAULT 0,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    IsIncome INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS Transactions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Amount DECIMAL NOT NULL,
                    Date DATETIME NOT NULL,
                    Description TEXT,
                    CategoryId INTEGER NOT NULL,
                    AccountId INTEGER NOT NULL,
                    IsIncome INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY(CategoryId) REFERENCES Categories(Id),
                    FOREIGN KEY(AccountId) REFERENCES Accounts(Id),
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS AutoPayments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT,
                    Amount DECIMAL NOT NULL,
                    FromAccountId INTEGER NOT NULL,
                    ToAccountId INTEGER NOT NULL,
                    NextPaymentDate DATETIME NOT NULL,
                    FrequencyDays INTEGER NOT NULL,
                    CategoryId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY(FromAccountId) REFERENCES Accounts(Id),
                    FOREIGN KEY(ToAccountId) REFERENCES Accounts(Id),
                    FOREIGN KEY(CategoryId) REFERENCES Categories(Id),
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );";
                command.ExecuteNonQuery();
            }
        }

        // Реализация методов работы с базой данных
        public bool AddUser(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                INSERT INTO Users (Username, PasswordHash, Salt, CreatedAt)
                VALUES (@username, @passwordHash, @salt, @createdAt);
                SELECT last_insert_rowid();";

                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@salt", user.Salt);
                    command.Parameters.AddWithValue("@createdAt", user.CreatedAt);

                    user.Id = Convert.ToInt32(command.ExecuteScalar());
                    return user.Id > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации пользователя: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UserExists(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                return (long)command.ExecuteScalar() > 0;
            }
        }

        public void AddAccounts(List<Account> accounts)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                foreach (var account in accounts)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    INSERT INTO Accounts (Name, Balance, UserId)
                    VALUES (@name, @balance, @userId);
                    SELECT last_insert_rowid();";
                    command.Parameters.AddWithValue("@name", account.Name);
                    command.Parameters.AddWithValue("@balance", account.Balance);
                    command.Parameters.AddWithValue("@userId", account.UserId);
                    account.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void AddCategories(List<Category> categories)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                foreach (var category in categories)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    INSERT INTO Categories (Name, IsIncome, UserId)
                    VALUES (@name, @isIncome, @userId);
                    SELECT last_insert_rowid();";
                    command.Parameters.AddWithValue("@name", category.Name);
                    command.Parameters.AddWithValue("@isIncome", category.IsIncome ? 1 : 0);
                    command.Parameters.AddWithValue("@userId", category.UserId);
                    category.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public User GetUser(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Salt = reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4)
                        };
                    }
                }
            }
            return null;
        }


        public IEnumerable<Transaction> GetTransactions(int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Transactions WHERE UserId = @userId";
                command.Parameters.AddWithValue("@userId", userId);

                var transactions = new List<Transaction>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            Id = reader.GetInt32(0),
                            Amount = reader.GetDecimal(1),
                            Date = reader.GetDateTime(2),
                            Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CategoryId = reader.GetInt32(4),
                            AccountId = reader.GetInt32(5),
                            IsIncome = reader.GetBoolean(6),
                            UserId = reader.GetInt32(7)
                        });
                    }
                }
                return transactions;
            }
        }

        public string GetCategoryName(int categoryId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Categories WHERE Id = @id";
                command.Parameters.AddWithValue("@id", categoryId);
                return command.ExecuteScalar()?.ToString();
            }
        }
        // DatabaseService.cs
        public List<Account> GetAccounts(int userId)
        {
            var accounts = new List<Account>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Accounts WHERE UserId = @userId";
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(new Account
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Balance = reader.GetDecimal(2),
                            UserId = reader.GetInt32(3)
                        });
                    }
                }
            }
            return accounts;
        }

        public string GetAccountName(int accountId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Accounts WHERE Id = @accountId";
                command.Parameters.AddWithValue("@accountId", accountId);
                return command.ExecuteScalar()?.ToString();
            }
        }

        public List<Transaction> GetRecentTransactions(int userId, int count)
        {
            var transactions = new List<Transaction>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT * FROM Transactions 
            WHERE UserId = @userId 
            ORDER BY Date DESC 
            LIMIT @count";
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@count", count);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            Id = reader.GetInt32(0),
                            Amount = reader.GetDecimal(1),
                            Date = reader.GetDateTime(2),
                            Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CategoryId = reader.GetInt32(4),
                            AccountId = reader.GetInt32(5),
                            IsIncome = reader.GetBoolean(6),
                            UserId = reader.GetInt32(7)
                        });
                    }
                }
            }
            return transactions;
        }

        public List<AutoPayment> GetAutoPayments(int userId)
        {
            var payments = new List<AutoPayment>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM AutoPayments WHERE UserId = @userId";
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new AutoPayment
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Amount = reader.GetDecimal(2),
                            FromAccountId = reader.GetInt32(3),
                            ToAccountId = reader.GetInt32(4),
                            NextPaymentDate = reader.GetDateTime(5),
                            FrequencyDays = reader.GetInt32(6),
                            CategoryId = reader.GetInt32(7),
                            UserId = reader.GetInt32(8)
                        });
                    }
                }
            }
            return payments;
        }

        public void TransferBetweenAccounts(int fromAccountId, int toAccountId, decimal amount,
                                          string description, int categoryId, int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Снимаем деньги с исходного счета
                        var command1 = connection.CreateCommand();
                        command1.CommandText = "UPDATE Accounts SET Balance = Balance - @amount WHERE Id = @accountId";
                        command1.Parameters.AddWithValue("@amount", amount);
                        command1.Parameters.AddWithValue("@accountId", fromAccountId);
                        command1.ExecuteNonQuery();

                        // Добавляем деньги на целевой счет
                        var command2 = connection.CreateCommand();
                        command2.CommandText = "UPDATE Accounts SET Balance = Balance + @amount WHERE Id = @accountId";
                        command2.Parameters.AddWithValue("@amount", amount);
                        command2.Parameters.AddWithValue("@accountId", toAccountId);
                        command2.ExecuteNonQuery();

                        // Записываем транзакцию
                        var command3 = connection.CreateCommand();
                        command3.CommandText = @"
                    INSERT INTO Transactions (Amount, Date, Description, CategoryId, AccountId, IsIncome, UserId)
                    VALUES (@amount, @date, @description, @categoryId, @toAccountId, 1, @userId)";
                        command3.Parameters.AddWithValue("@amount", amount);
                        command3.Parameters.AddWithValue("@date", DateTime.Now);
                        command3.Parameters.AddWithValue("@description", description);
                        command3.Parameters.AddWithValue("@categoryId", categoryId);
                        command3.Parameters.AddWithValue("@toAccountId", toAccountId);
                        command3.Parameters.AddWithValue("@userId", userId);
                        command3.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UpdateAutoPayment(AutoPayment payment)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE AutoPayments 
            SET Description = @description, 
                Amount = @amount, 
                FromAccountId = @fromAccountId, 
                ToAccountId = @toAccountId, 
                NextPaymentDate = @nextPaymentDate, 
                FrequencyDays = @frequencyDays, 
                CategoryId = @categoryId
            WHERE Id = @id";

                command.Parameters.AddWithValue("@description", payment.Description);
                command.Parameters.AddWithValue("@amount", payment.Amount);
                command.Parameters.AddWithValue("@fromAccountId", payment.FromAccountId);
                command.Parameters.AddWithValue("@toAccountId", payment.ToAccountId);
                command.Parameters.AddWithValue("@nextPaymentDate", payment.NextPaymentDate);
                command.Parameters.AddWithValue("@frequencyDays", payment.FrequencyDays);
                command.Parameters.AddWithValue("@categoryId", payment.CategoryId);
                command.Parameters.AddWithValue("@id", payment.Id);

                command.ExecuteNonQuery();
            }
        }
        public List<Category> GetCategories(int userId)
        {
            var categories = new List<Category>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Categories WHERE UserId = @userId";
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IsIncome = reader.GetBoolean(2),
                            UserId = reader.GetInt32(3)
                        });
                    }
                }
            }
            return categories;
        }

        public void AddTransaction(Transaction transaction)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO Transactions (Amount, Date, Description, CategoryId, AccountId, IsIncome, UserId)
            VALUES (@amount, @date, @description, @categoryId, @accountId, @isIncome, @userId)";

                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@date", transaction.Date);
                command.Parameters.AddWithValue("@description", transaction.Description);
                command.Parameters.AddWithValue("@categoryId", transaction.CategoryId);
                command.Parameters.AddWithValue("@accountId", transaction.AccountId);
                command.Parameters.AddWithValue("@isIncome", transaction.IsIncome);
                command.Parameters.AddWithValue("@userId", transaction.UserId);

                command.ExecuteNonQuery();
            }
        }

        public Account GetAccount(int accountId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Accounts WHERE Id = @accountId";
                command.Parameters.AddWithValue("@accountId", accountId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Account
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Balance = reader.GetDecimal(2),
                            UserId = reader.GetInt32(3)
                        };
                    }
                }
            }
            return null;
        }

        public void UpdateAccount(Account account)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE Accounts 
            SET Name = @name, Balance = @balance 
            WHERE Id = @id";

                command.Parameters.AddWithValue("@name", account.Name);
                command.Parameters.AddWithValue("@balance", account.Balance);
                command.Parameters.AddWithValue("@id", account.Id);

                command.ExecuteNonQuery();
            }
        }

    }
}
