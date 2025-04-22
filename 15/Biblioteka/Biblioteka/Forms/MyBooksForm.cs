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
    public partial class MyBooksForm : Form
    {
        private readonly int _userId;
        private DataGridView dgvBooks;
        private Button btnReturn;
        private Button btnClose;

        public MyBooksForm(int userId)
        {
            _userId = userId;
            InitializeComponents();
            LoadBooks();
        }

        private void InitializeComponents()
        {
            this.Text = "Мои книги";
            this.Size = new Size(600, 400);
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

            btnReturn = new Button
            {
                Text = "Вернуть книгу",
                Size = new Size(120, 30),
                Location = new Point(10, 10)
            };
            btnReturn.Click += BtnReturn_Click;

            btnClose = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 30),
                Location = new Point(140, 10)
            };
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnReturn, btnClose });

            this.Controls.AddRange(new Control[] { dgvBooks, buttonPanel });
        }

        private void LoadBooks()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT b.BookId, b.Title, b.Author, bl.LoanDate, bl.Status 
                               FROM BookLoans bl
                               JOIN Books b ON bl.BookId = b.BookId
                               WHERE bl.UserId = @UserId AND bl.Status = 'OnLoan'";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", _userId);
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            dgvBooks.DataSource = table;
                            ConfigureDataGridView();
                        }
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
            dgvBooks.Columns["LoanDate"].HeaderText = "Дата выдачи";
            dgvBooks.Columns["Status"].HeaderText = "Статус";
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для возврата", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int loanId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookId"].Value);
            string bookTitle = dgvBooks.SelectedRows[0].Cells["Title"].Value.ToString();

            if (MessageBox.Show($"Вы уверены, что хотите вернуть книгу '{bookTitle}'?", "Подтверждение возврата",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new SQLiteCommand(
                            "UPDATE BookLoans SET Status = 'Returned', ReturnDate = datetime('now') WHERE BookId = @BookId AND UserId = @UserId", conn))
                        {
                            cmd.Parameters.AddWithValue("@BookId", loanId);
                            cmd.Parameters.AddWithValue("@UserId", _userId);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Книга успешно возвращена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadBooks();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при возврате книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
