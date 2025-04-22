using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Biblioteka.Forms
{
    public partial class BookDetailsForm : Form
    {
        private readonly int _bookId;
        private readonly int _userId;
        private PictureBox pbCover;
        private Label lblTitle;
        private Label lblAuthor;
        private Label lblGenre;
        private Label lblYear;
        private TextBox txtDescription;
        private DataGridView dgvReviews;
        private NumericUpDown numRating;
        private TextBox txtReview;
        private Button btnAddReview;
        private Button btnDownload;
        private Button btnClose;

        public BookDetailsForm(int bookId, int userId)
        {
            _bookId = bookId;
            _userId = userId;
            InitializeComponents();
            LoadBookDetails();
            LoadReviews();
        }

        private void InitializeComponents()
        {
            this.Text = "Подробности о книге";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // Cover image
            pbCover = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(pbCover, 0, 0);
            mainPanel.SetRowSpan(pbCover, 3);

            // Book details
            var detailsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(5)
            };

            lblTitle = new Label { Dock = DockStyle.Fill, Font = new Font(Font, FontStyle.Bold) };
            detailsPanel.Controls.Add(lblTitle, 0, 0);
            detailsPanel.SetColumnSpan(lblTitle, 2);

            lblAuthor = new Label { Dock = DockStyle.Fill };
            detailsPanel.Controls.Add(new Label { Text = "Автор:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            detailsPanel.Controls.Add(lblAuthor, 1, 1);

            lblGenre = new Label { Dock = DockStyle.Fill };
            detailsPanel.Controls.Add(new Label { Text = "Жанр:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            detailsPanel.Controls.Add(lblGenre, 1, 2);

            lblYear = new Label { Dock = DockStyle.Fill };
            detailsPanel.Controls.Add(new Label { Text = "Год:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            detailsPanel.Controls.Add(lblYear, 1, 3);

            mainPanel.Controls.Add(detailsPanel, 1, 0);

            // Description
            txtDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };
            mainPanel.Controls.Add(txtDescription, 1, 1);

            // Download button
            btnDownload = new Button
            {
                Text = "Скачать книгу",
                Dock = DockStyle.Fill,
                Height = 30
            };
            btnDownload.Click += BtnDownload_Click;
            mainPanel.Controls.Add(btnDownload, 1, 2);

            // Reviews
            var reviewsPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(reviewsPanel, 0, 3);
            mainPanel.SetColumnSpan(reviewsPanel, 2);

            dgvReviews = new DataGridView
            {
                Dock = DockStyle.Fill,
                Height = 150,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            var addReviewPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };

            numRating = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 5,
                Value = 5,
                Width = 50,
                Location = new Point(10, 10)
            };

            txtReview = new TextBox
            {
                Multiline = true,
                Location = new Point(70, 10),
                Size = new Size(400, 50)
            };

            btnAddReview = new Button
            {
                Text = "Добавить отзыв",
                Location = new Point(480, 10),
                Size = new Size(120, 50)
            };
            btnAddReview.Click += BtnAddReview_Click;

            addReviewPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Оценка:", Location = new Point(10, 13) },
                numRating,
                txtReview,
                btnAddReview
            });

            reviewsPanel.Controls.Add(dgvReviews);
            reviewsPanel.Controls.Add(addReviewPanel);

            // Close button
            btnClose = new Button
            {
                Text = "Закрыть",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(mainPanel);
            this.Controls.Add(btnClose);
        }

        private void LoadBookDetails()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT Title, Author, Genre, Year, Description, CoverImage, FilePath FROM Books WHERE BookId = @BookId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", _bookId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTitle.Text = reader["Title"].ToString();
                                lblAuthor.Text = reader["Author"].ToString();
                                lblGenre.Text = reader["Genre"].ToString();
                                lblYear.Text = reader["Year"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                btnDownload.Enabled = !string.IsNullOrEmpty(reader["FilePath"].ToString());

                                if (reader["CoverImage"] != DBNull.Value)
                                {
                                    var imageBytes = (byte[])reader["CoverImage"];
                                    using (var ms = new MemoryStream(imageBytes))
                                    {
                                        pbCover.Image = Image.FromStream(ms);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReviews()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT u.Username, r.Rating, r.Comment, r.ReviewDate 
                               FROM Reviews r
                               JOIN Users u ON r.UserId = u.UserId
                               WHERE r.BookId = @BookId
                               ORDER BY r.ReviewDate DESC";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", _bookId);
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            dgvReviews.DataSource = table;
                            ConfigureReviewsGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureReviewsGrid()
        {
            dgvReviews.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReviews.Columns["Username"].HeaderText = "Пользователь";
            dgvReviews.Columns["Rating"].HeaderText = "Оценка";
            dgvReviews.Columns["Comment"].HeaderText = "Комментарий";
            dgvReviews.Columns["ReviewDate"].HeaderText = "Дата";
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT FilePath FROM Books WHERE BookId = @BookId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", _bookId);
                        var filePath = cmd.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            var saveDialog = new SaveFileDialog
                            {
                                FileName = Path.GetFileName(filePath),
                                Filter = "All Files|*.*"
                            };

                            if (saveDialog.ShowDialog() == DialogResult.OK)
                            {
                                File.Copy(filePath, saveDialog.FileName, true);
                                MessageBox.Show("Книга успешно скачана", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Файл книги не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при скачивании книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddReview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReview.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст отзыва", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Reviews (BookId, UserId, Rating, Comment) VALUES (@BookId, @UserId, @Rating, @Comment)", conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", _bookId);
                        cmd.Parameters.AddWithValue("@UserId", _userId);
                        cmd.Parameters.AddWithValue("@Rating", numRating.Value);
                        cmd.Parameters.AddWithValue("@Comment", txtReview.Text);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Отзыв успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtReview.Clear();
                            LoadReviews();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

