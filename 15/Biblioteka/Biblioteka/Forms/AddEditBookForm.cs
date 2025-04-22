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
    public partial class AddEditBookForm : Form
    {
        private readonly int? _bookId;
        private byte[] _coverImage;
        private string _filePath;

        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtGenre;
        private NumericUpDown numYear;
        private TextBox txtDescription;
        private PictureBox pbCover;
        private Label lblFileName;
        private Button btnUploadCover;
        private Button btnUploadFile;
        private Button btnSave;
        private Button btnCancel;

        public AddEditBookForm(int? bookId = null)
        {
            _bookId = bookId;
            InitializeComponents();

            if (_bookId.HasValue)
            {
                this.Text = "Редактирование книги";
                btnSave.Text = "Обновить";
                LoadBookData();
            }
            else
            {
                this.Text = "Добавление новой книги";
                btnSave.Text = "Добавить";
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Title
            panel.Controls.Add(new Label { Text = "Название*:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtTitle = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtTitle, 1, 0);

            // Author
            panel.Controls.Add(new Label { Text = "Автор*:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtAuthor = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtAuthor, 1, 1);

            // Genre
            panel.Controls.Add(new Label { Text = "Жанр:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtGenre = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txtGenre, 1, 2);

            // Year
            panel.Controls.Add(new Label { Text = "Год:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            numYear = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1000, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            panel.Controls.Add(numYear, 1, 3);

            // Description
            panel.Controls.Add(new Label { Text = "Описание:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            panel.Controls.Add(txtDescription, 1, 4);

            // Cover Image
            panel.Controls.Add(new Label { Text = "Обложка:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 5);
            var coverPanel = new Panel { Dock = DockStyle.Fill };
            pbCover = new PictureBox { SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Left, Width = 100 };
            btnUploadCover = new Button { Text = "Загрузить", Dock = DockStyle.Right, Width = 80 };
            btnUploadCover.Click += BtnUploadCover_Click;
            coverPanel.Controls.Add(pbCover);
            coverPanel.Controls.Add(btnUploadCover);
            panel.Controls.Add(coverPanel, 1, 5);

            // File
            panel.Controls.Add(new Label { Text = "Файл книги:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 6);
            var filePanel = new Panel { Dock = DockStyle.Fill };
            lblFileName = new Label { Text = "Файл не выбран", Dock = DockStyle.Left, AutoEllipsis = true };
            btnUploadFile = new Button { Text = "Выбрать", Dock = DockStyle.Right, Width = 80 };
            btnUploadFile.Click += BtnUploadFile_Click;
            filePanel.Controls.Add(lblFileName);
            filePanel.Controls.Add(btnUploadFile);
            panel.Controls.Add(filePanel, 1, 6);

            // Buttons
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnSave = new Button { Text = "Сохранить", Size = new Size(100, 30), Location = new Point(150, 10) };
            btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Отмена", Size = new Size(100, 30), Location = new Point(260, 10) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });

            this.Controls.Add(panel);
            this.Controls.Add(buttonPanel);
        }

        private void BtnUploadCover_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _coverImage = File.ReadAllBytes(openFileDialog.FileName);
                        pbCover.Image = Image.FromFile(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnUploadFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PDF Files|*.pdf|EPUB Files|*.epub|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = openFileDialog.FileName;
                    lblFileName.Text = Path.GetFileName(_filePath);
                }
            }
        }

        private void LoadBookData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT Title, Author, Genre, Year, Description, CoverImage, FilePath FROM Books WHERE BookId = @BookId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BookId", _bookId.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtTitle.Text = reader["Title"].ToString();
                                txtAuthor.Text = reader["Author"].ToString();
                                txtGenre.Text = reader["Genre"].ToString();
                                numYear.Value = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : DateTime.Now.Year;
                                txtDescription.Text = reader["Description"].ToString();
                                _filePath = reader["FilePath"].ToString();
                                lblFileName.Text = Path.GetFileName(_filePath);

                                if (reader["CoverImage"] != DBNull.Value)
                                {
                                    _coverImage = (byte[])reader["CoverImage"];
                                    using (var ms = new MemoryStream(_coverImage))
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля (Название и Автор)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (_bookId.HasValue)
                    {
                        // Update existing book
                        using (var cmd = new SQLiteCommand(
                            @"UPDATE Books SET 
                            Title = @Title, 
                            Author = @Author, 
                            Genre = @Genre, 
                            Year = @Year, 
                            Description = @Description, 
                            CoverImage = @CoverImage, 
                            FilePath = @FilePath 
                            WHERE BookId = @BookId", conn))
                        {
                            AddParametersToCommand(cmd);
                            cmd.Parameters.AddWithValue("@BookId", _bookId.Value);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Книга успешно обновлена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        // Insert new book
                        using (var cmd = new SQLiteCommand(
                            @"INSERT INTO Books 
                            (Title, Author, Genre, Year, Description, CoverImage, FilePath, AddedDate) 
                            VALUES 
                            (@Title, @Author, @Genre, @Year, @Description, @CoverImage, @FilePath, datetime('now'))", conn))
                        {
                            AddParametersToCommand(cmd);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Книга успешно добавлена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddParametersToCommand(SQLiteCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
            cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
            cmd.Parameters.AddWithValue("@Genre", string.IsNullOrWhiteSpace(txtGenre.Text) ? DBNull.Value : (object)txtGenre.Text);
            cmd.Parameters.AddWithValue("@Year", numYear.Value);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text);
            cmd.Parameters.AddWithValue("@CoverImage", _coverImage ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FilePath", string.IsNullOrWhiteSpace(_filePath) ? DBNull.Value : (object)_filePath);
        }
    }
}
