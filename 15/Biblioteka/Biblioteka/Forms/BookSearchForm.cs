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
    public partial class BookSearchForm : Form
    {
        private readonly int _userId;
        private readonly string _userRole;
        private DataGridView dgvBooks;
        private TextBox txtSearch;
        private ComboBox cmbGenre;
        private ComboBox cmbAvailability;
        private Button btnSearch;
        private Button btnViewDetails;
        private Button btnBorrow;
        private Button btnClose;

        public BookSearchForm(int userId, string userRole)
        {
            _userId = userId;
            _userRole = userRole;
            InitializeComponents();
            LoadGenres();
            LoadBooks();
        }

        private void InitializeComponents()
        {
            this.Text = "Поиск книг";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100
            };

            txtSearch = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(300, 20)
            };

            cmbGenre = new ComboBox
            {
                Location = new Point(10, 40),
                Size = new Size(150, 20)
            };

            cmbAvailability = new ComboBox
            {
                Location = new Point(170, 40),
                Size = new Size(150, 20)
            };
            cmbAvailability.Items.AddRange(new object[] { "Все", "Доступные", "Выданные" });
            cmbAvailability.SelectedIndex = 0;

            btnSearch = new Button
            {
                Text = "Поиск",
                Location = new Point(330, 10),
                Size = new Size(80, 50)
            };
            btnSearch.Click += BtnSearch_Click;

            panel.Controls.AddRange(new Control[] { txtSearch, cmbGenre, cmbAvailability, btnSearch });

            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(10)
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnViewDetails = new Button
            {
                Text = "Подробнее",
                Size = new Size(100, 30),
                Location = new Point(10, 10)
            };
            btnViewDetails.Click += BtnViewDetails_Click;

            btnBorrow = new Button
            {
                Text = "Взять книгу",
                Size = new Size(100, 30),
                Location = new Point(120, 10),
                Enabled = _userRole == "User" || _userRole == "Librarian"
            };
            btnBorrow.Click += BtnBorrow_Click;

            btnClose = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 30),
                Location = new Point(230, 10)
            };
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnViewDetails, btnBorrow, btnClose });

            this.Controls.AddRange(new Control[] { panel, dgvBooks, buttonPanel });
        }

        private void LoadGenres()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT DISTINCT Genre FROM Books WHERE Genre IS NOT NULL", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        cmbGenre.Items.Add("Все жанры");
                        while (reader.Read())
                        {
                            cmbGenre.Items.Add(reader["Genre"].ToString());
                        }
                        cmbGenre.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке жанров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBooks()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT b.BookId, b.Title, b.Author, b.Genre, b.Year, 
                                CASE WHEN bl.UserId IS NULL THEN 'Доступна' ELSE 'Выдана' END AS Status
                                FROM Books b
                                LEFT JOIN BookLoans bl ON b.BookId = bl.BookId AND bl.Status = 'OnLoan'";

                    using (var adapter = new SQLiteDataAdapter(query, conn))
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
            dgvBooks.Columns["Status"].HeaderText = "Статус";
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            string selectedGenre = cmbGenre.SelectedItem.ToString();
            string availability = cmbAvailability.SelectedItem?.ToString();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT b.BookId, b.Title, b.Author, b.Genre, b.Year, 
                                 CASE WHEN bl.UserId IS NULL THEN 'Доступна' ELSE 'Выдана' END AS Status
                                 FROM Books b
                                 LEFT JOIN BookLoans bl ON b.BookId = bl.BookId AND bl.Status = 'OnLoan'
                                 WHERE (@SearchTerm = '' OR b.Title LIKE '%' || @SearchTerm || '%' OR b.Author LIKE '%' || @SearchTerm || '%')
                                 AND (@Genre = 'Все жанры' OR b.Genre = @Genre)";

                    if (availability == "Доступные")
                    {
                        query += " AND bl.UserId IS NULL";
                    }
                    else if (availability == "Выданные")
                    {
                        query += " AND bl.UserId IS NOT NULL";
                    }

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                        cmd.Parameters.AddWithValue("@Genre", selectedGenre);

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            dgvBooks.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске книг: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите книгу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookId"].Value);
            var detailsForm = new BookDetailsForm(bookId, _userId);
            detailsForm.ShowDialog();
            BtnSearch_Click(sender, e); // Refresh the list
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите книгу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string status = dgvBooks.SelectedRows[0].Cells["Status"].Value.ToString();
            if (status == "Выдана")
            {
                MessageBox.Show("Эта книга уже выдана другому пользователю", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookId"].Value);

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO BookLoans (BookId, UserId, Status) VALUES (@BookId, @UserId, 'OnLoan')", conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", bookId);
                        cmd.Parameters.AddWithValue("@UserId", _userId);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Книга успешно выдана", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            BtnSearch_Click(sender, e); // Refresh the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выдаче книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
