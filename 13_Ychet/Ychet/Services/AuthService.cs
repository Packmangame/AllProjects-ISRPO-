using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ychet.Models;
using Ychet.Utilities;


namespace Ychet.Services
{
    
    public class AuthService
    {
        private readonly DatabaseService _dbService;

        public AuthService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public bool Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
            {
                MessageBox.Show("Имя пользователя должно содержать минимум 4 символа",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return false;
            }

            if (_dbService.UserExists(username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return false;
            }

            var salt = SecurityHelper.GenerateSalt();
            var hash = SecurityHelper.HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                PasswordHash = hash,
                Salt = salt,
                CreatedAt = DateTime.Now
            };

            if (_dbService.AddUser(user))
            {
                // Создаем стандартные счета для нового пользователя
                var accounts = new List<Account>
        {
            new Account { Name = "Наличные", Balance = 0, UserId = user.Id },
            new Account { Name = "Банковская карта", Balance = 0, UserId = user.Id },
            new Account { Name = "Сбережения", Balance = 0, UserId = user.Id }
        };
                _dbService.AddAccounts(accounts);

                // Создаем стандартные категории
                var categories = new List<Category>
        {
            new Category { Name = "Зарплата", IsIncome = true, UserId = user.Id },
            new Category { Name = "Продукты", IsIncome = false, UserId = user.Id },
            new Category { Name = "Транспорт", IsIncome = false, UserId = user.Id }
        };
                _dbService.AddCategories(categories);

                return true;
            }

            return false;
        }

        public User Login(string username, string password)
        {
            var user = _dbService.GetUser(username);
            if (user == null) return null;

            var hash = SecurityHelper.HashPassword(password, user.Salt);
            return hash == user.PasswordHash ? user : null;
        }


    }
}
