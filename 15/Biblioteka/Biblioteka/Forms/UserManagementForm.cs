using Biblioteka.Forms;
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
    public partial class UserManagementForm : Form
    {
        private DataGridView dgvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;

        public UserManagementForm()
        {
            InitializeComponents();
            LoadUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление пользователями";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnAdd = new Button
            {
                Text = "Добавить",
                Size = new Size(100, 30),
                Location = new Point(10, 10)
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Редактировать",
                Size = new Size(100, 30),
                Location = new Point(120, 10)
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Удалить",
                Size = new Size(100, 30),
                Location = new Point(230, 10)
            };
            btnDelete.Click += BtnDelete_Click;

            btnClose = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 30),
                Location = new Point(340, 10)
            };
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnClose });

            this.Controls.AddRange(new Control[] { dgvUsers, buttonPanel });
        }

        private void LoadUsers()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var adapter = new SQLiteDataAdapter("SELECT UserId, Username, FullName, Email, Role FROM Users", conn))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        dgvUsers.DataSource = table;
                        ConfigureDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.Columns["UserId"].Visible = false;
            dgvUsers.Columns["Username"].HeaderText = "Логин";
            dgvUsers.Columns["FullName"].HeaderText = "Полное имя";
            dgvUsers.Columns["Email"].HeaderText = "Email";
            dgvUsers.Columns["Role"].HeaderText = "Роль";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new RegistrationForm(true);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);
            var editForm = new UserEditForm(userId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            if (username == "admin")
            {
                MessageBox.Show("Нельзя удалить администратора по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя '{username}'?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Delete related records first
                                using (var cmd = new SQLiteCommand("DELETE FROM BookLoans WHERE UserId = @UserId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    cmd.ExecuteNonQuery();
                                }

                                using (var cmd = new SQLiteCommand("DELETE FROM Reviews WHERE UserId = @UserId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Then delete the user
                                using (var cmd = new SQLiteCommand("DELETE FROM Users WHERE UserId = @UserId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        transaction.Commit();
                                        MessageBox.Show("Пользователь успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadUsers();
                                    }
                                }
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
