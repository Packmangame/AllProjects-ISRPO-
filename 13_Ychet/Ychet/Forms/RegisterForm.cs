using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ychet.Services;

namespace Ychet.Forms
{
    // Forms/RegisterForm.cs
    public partial class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirmPassword;

        private readonly AuthService _authService;

        public RegisterForm(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            this.Text = "Регистрация";
            this.ClientSize = new Size(350, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Label и TextBox для имени пользователя
            lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(20, 20),
                Width = 120
            };

            txtUsername = new TextBox
            {
                Location = new Point(150, 20),
                Width = 150
            };

            // Label и TextBox для пароля
            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(20, 60),
                Width = 120
            };

            txtPassword = new TextBox
            {
                Location = new Point(150, 60),
                Width = 150,
                PasswordChar = '*'
            };

            // Label и TextBox для подтверждения пароля
            lblConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(20, 100),
                Width = 120
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(150, 100),
                Width = 150,
                PasswordChar = '*'
            };

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(50, 150),
                Width = 120
            };
            btnRegister.Click += btnRegister_Click;

            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(180, 150),
                Width = 120
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавляем элементы на форму
            this.Controls.AddRange(new Control[] {
            lblUsername, txtUsername,
            lblPassword, txtPassword,
            lblConfirmPassword, txtConfirmPassword,
            btnRegister, btnCancel
        });
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_authService.Register(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
