using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ychet.Models;
using Ychet.Services;

namespace Ychet.Forms
{
    public partial class LoginForm : Form
    {
            // Объявляем элементы управления
            private TextBox txtUsername;
            private TextBox txtPassword;
        private Button btnRegister;
        private Button btnLogin;
            private Button btnCancel;
            private Label lblUsername;
            private Label lblPassword;

            private readonly AuthService _authService;
            public User AuthenticatedUser { get; private set; }

            public LoginForm(AuthService authService)
            {
                _authService = authService;
                InitializeComponent();
                InitializeControls();
            }

            private void InitializeControls()
            {
                // Настройка формы
                this.Text = "Вход в систему";
                this.ClientSize = new Size(300, 200);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.StartPosition = FormStartPosition.CenterScreen;

                // Label для имени пользователя
                lblUsername = new Label
                {
                    Text = "Имя пользователя:",
                    Location = new Point(20, 20),
                    Width = 100
                };

                // TextBox для имени пользователя
                txtUsername = new TextBox
                {
                    Location = new Point(130, 20),
                    Width = 150
                };

                // Label для пароля
                lblPassword = new Label
                {
                    Text = "Пароль:",
                    Location = new Point(20, 60),
                    Width = 100
                };

                // TextBox для пароля
                txtPassword = new TextBox
                {
                    Location = new Point(130, 60),
                    Width = 150,
                    PasswordChar = '*'
                };

                // Кнопка входа
                btnLogin = new Button
                {
                    Text = "Войти",
                    Location = new Point(70, 100),
                    Width = 80
                };
                btnLogin.Click += btnLogin_Click;

                // Кнопка отмены
                btnCancel = new Button
                {
                    Text = "Отмена",
                    Location = new Point(160, 100),
                    Width = 80
                };
                btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(100, 140),
                Width = 80
            };
            btnRegister.Click += btnRegister_Click;

            // Добавляем кнопку на форму
            this.Controls.Add(btnRegister);
            // Добавляем элементы на форму
            this.Controls.Add(lblUsername);
                this.Controls.Add(txtUsername);
                this.Controls.Add(lblPassword);
                this.Controls.Add(txtPassword);
                this.Controls.Add(btnLogin);
                this.Controls.Add(btnCancel);
            }

            private void btnLogin_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Введите имя пользователя и пароль", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = _authService.Login(txtUsername.Text, txtPassword.Text);
                if (user != null)
                {
                    AuthenticatedUser = user;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
       
                if (user != null)
                {
                    AuthenticatedUser = user; // Сохраняем аутентифицированного пользователя
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль");
                }
        
            }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем форму входа
            using (var registerForm = new RegisterForm(_authService))
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    // После успешной регистрации показываем форму входа снова
                    this.Show();
                    txtUsername.Focus();
                }
                else
                {
                    this.Show();
                }
            }
        }


       

    }
}
