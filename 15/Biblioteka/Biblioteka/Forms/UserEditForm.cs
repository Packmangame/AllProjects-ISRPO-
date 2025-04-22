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

namespace Biblioteka.Forms
{
    public partial class UserEditForm : Form
    {
        private readonly int _userId;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private Button btnSave;
        private Button btnCancel;

        public UserEditForm(int userId)
        {
            _userId = userId;
            InitializeComponents();
            LoadUserData();
        }

        private void InitializeComponents()
        {
            this.Text = "Редактирование пользователя";
            this.Size = new Size(350, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // Username
            panel.Controls.Add(new Label { Text = "Имя пользователя*:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtUsername = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtUsername, 1, 0);

            // Password
            panel.Controls.Add(new Label { Text = "Пароль:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtPassword = new TextBox { Dock = DockStyle.Fill, PasswordChar = '*', /*PlaceholderText = "Оставьте пустым, чтобы не менять"*/ };
            panel.Controls.Add(txtPassword, 1, 1);

            // Full Name
            panel.Controls.Add(new Label { Text = "Полное имя:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtFullName = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtFullName, 1, 2);

            // Email
            panel.Controls.Add(new Label { Text = "Email:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            txtEmail = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtEmail, 1, 3);

            // Role
            panel.Controls.Add(new Label { Text = "Роль:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            cmbRole = new ComboBox { Dock = DockStyle.Fill };
            cmbRole.Items.AddRange(new object[] { "User", "Librarian", "Admin" });
            panel.Controls.Add(cmbRole, 1, 4);

            // Buttons
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnSave = new Button
            {
                Text = "Сохранить",
                Size = new Size(100, 30),
                Location = new Point(50, 10)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Size = new Size(100, 30),
                Location = new Point(160, 10)
            };
            btnCancel.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });

            this.Controls.Add(panel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadUserData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT Username, FullName, Email, Role FROM Users WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", _userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUsername.Text = reader["Username"].ToString();
                                txtFullName.Text = reader["FullName"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                cmbRole.SelectedItem = reader["Role"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Имя пользователя обязательно для заполнения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = string.IsNullOrWhiteSpace(txtPassword.Text) ?
                        @"UPDATE Users SET 
                        Username = @Username, 
                        FullName = @FullName, 
                        Email = @Email, 
                        Role = @Role 
                        WHERE UserId = @UserId" :
                        @"UPDATE Users SET 
                        Username = @Username, 
                        Password = @Password, 
                        FullName = @FullName, 
                        Email = @Email, 
                        Role = @Role 
                        WHERE UserId = @UserId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                        }
                        cmd.Parameters.AddWithValue("@FullName", string.IsNullOrWhiteSpace(txtFullName.Text) ? DBNull.Value : (object)txtFullName.Text);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@UserId", _userId);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Данные пользователя успешно обновлены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Ошибка при обновлении данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
