using Biblioteka.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Biblioteka
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblUsername;
        private Label lblPassword;

        public LoginForm()
        {
            InitializeComponents();
            DatabaseHelper.InitializeDatabase();
        }

        private void InitializeComponents()
        {
            this.Text = "Вход в электронную библиотеку";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(30, 30),
                Size = new Size(120, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(160, 30),
                Size = new Size(150, 20)
            };

            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(30, 70),
                Size = new Size(120, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(160, 70),
                Size = new Size(150, 20),
                PasswordChar = '*'
            };

            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(50, 120),
                Size = new Size(100, 30)
            };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(180, 120),
                Size = new Size(100, 30)
            };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, btnLogin, btnRegister });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT UserId, Role FROM Users WHERE Username = @Username AND Password = @Password", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Hide();
                            var mainForm = new MainForm(reader.GetInt32(0), reader.GetString(1));
                            mainForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegistrationForm();
            registerForm.ShowDialog();
        }
    }
}
