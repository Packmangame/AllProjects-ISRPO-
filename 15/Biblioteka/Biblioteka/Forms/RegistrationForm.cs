using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Biblioteka
{
    public partial class RegistrationForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private Button btnRegister;
        private Button btnCancel;
        private bool _isAdminMode;

        public RegistrationForm(bool isAdminMode = false)
        {
            _isAdminMode = isAdminMode;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = _isAdminMode ? "Добавление нового пользователя" : "Регистрация нового пользователя";
            this.Size = new Size(350, _isAdminMode ? 350 : 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = _isAdminMode ? 6 : 5,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // Username
            panel.Controls.Add(new Label { Text = "Имя пользователя*:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtUsername = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtUsername, 1, 0);

            // Password
            panel.Controls.Add(new Label { Text = "Пароль*:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtPassword = new TextBox { Dock = DockStyle.Fill, PasswordChar = '*' };
            panel.Controls.Add(txtPassword, 1, 1);

            // Full Name
            panel.Controls.Add(new Label { Text = "Полное имя:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtFullName = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtFullName, 1, 2);

            // Email
            panel.Controls.Add(new Label { Text = "Email:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            txtEmail = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtEmail, 1, 3);

            // Role (only in admin mode)
            if (_isAdminMode)
            {
                panel.Controls.Add(new Label { Text = "Роль:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
                cmbRole = new ComboBox { Dock = DockStyle.Fill };
                cmbRole.Items.AddRange(new object[] { "User", "Librarian", "Admin" });
                cmbRole.SelectedIndex = 0;
                panel.Controls.Add(cmbRole, 1, 4);
            }

            // Buttons
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnRegister = new Button
            {
                Text = _isAdminMode ? "Добавить" : "Зарегистрировать",
                Size = new Size(120, 30),
                Location = new Point(50, 10)
            };
            btnRegister.Click += BtnRegister_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Size = new Size(100, 30),
                Location = new Point(180, 10)
            };
            btnCancel.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnRegister, btnCancel });

            this.Controls.Add(panel);
            this.Controls.Add(buttonPanel);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Имя пользователя и пароль обязательны для заполнения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = _isAdminMode ?
                        "INSERT INTO Users (Username, Password, FullName, Email, Role) VALUES (@Username, @Password, @FullName, @Email, @Role)" :
                        "INSERT INTO Users (Username, Password, FullName, Email) VALUES (@Username, @Password, @FullName, @Email)";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                        cmd.Parameters.AddWithValue("@FullName", string.IsNullOrWhiteSpace(txtFullName.Text) ? DBNull.Value : (object)txtFullName.Text);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text);

                        if (_isAdminMode)
                        {
                            cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                        }

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show(_isAdminMode ? "Пользователь успешно добавлен" : "Регистрация прошла успешно",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
