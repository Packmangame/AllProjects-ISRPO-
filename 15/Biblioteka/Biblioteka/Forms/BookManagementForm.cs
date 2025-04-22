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

namespace Biblioteka.Forms
{
    public partial class BookManagementForm : Form
    {
        private DataGridView dgvBooks;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;

        public BookManagementForm()
        {
            InitializeComponents();
            LoadBooks();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление книгами";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            dgvBooks = new DataGridView
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

            this.Controls.AddRange(new Control[] { dgvBooks, buttonPanel });
        }

        private void LoadBooks()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var adapter = new SQLiteDataAdapter("SELECT BookId, Title, Author, Genre, Year FROM Books", conn))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        dgvBooks.DataSource = table;
                        ConfigureDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке книг: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.Columns["BookId"].Visible = false;
            dgvBooks.Columns["Title"].HeaderText = "Название";
            dgvBooks.Columns["Author"].HeaderText = "Автор";
            dgvBooks.Columns["Genre"].HeaderText = "Жанр";
            dgvBooks.Columns["Year"].HeaderText = "Год";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new AddEditBookForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для редактирования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookId"].Value);
            var editForm = new AddEditBookForm(bookId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookId"].Value);
            string bookTitle = dgvBooks.SelectedRows[0].Cells["Title"].Value.ToString();

            if (MessageBox.Show($"Вы уверены, что хотите удалить книгу '{bookTitle}'?", "Подтверждение удаления",
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
                                using (var cmd = new SQLiteCommand("DELETE FROM BookLoans WHERE BookId = @BookId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@BookId", bookId);
                                    cmd.ExecuteNonQuery();
                                }

                                using (var cmd = new SQLiteCommand("DELETE FROM Reviews WHERE BookId = @BookId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@BookId", bookId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Then delete the book
                                using (var cmd = new SQLiteCommand("DELETE FROM Books WHERE BookId = @BookId", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@BookId", bookId);
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        transaction.Commit();
                                        MessageBox.Show("Книга успешно удалена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadBooks();
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
                    MessageBox.Show($"Ошибка при удалении книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
