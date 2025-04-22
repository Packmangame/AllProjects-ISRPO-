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
    public partial class TransactionForm : Form
    {

        private Label lblType;
        private DateTimePicker dtpDate;
        private ComboBox cmbAccount;
        private ComboBox cmbCategory;
        private TextBox txtAmount;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;

        private readonly DatabaseService _dbService;
        private readonly User _user;
        private readonly bool _isIncome;

        public TransactionForm(DatabaseService dbService, User user, bool isIncome)
        {
            _dbService = dbService;
            _user = user;
            _isIncome = isIncome;

            InitializeComponent();
            InitializeControls();
            LoadData();
        }

        private void InitializeControls()
        {
            // Инициализация и настройка элементов управления
            this.Text = _isIncome ? "Добавление дохода" : "Добавление расхода";
            this.ClientSize = new Size(400, 300);

            // Label для типа операции
            lblType = new Label
            {
                Text = _isIncome ? "Доход" : "Расход",
                Location = new Point(20, 20),
                Width = 100
            };

            // DateTimePicker для даты
            dtpDate = new DateTimePicker
            {
                Location = new Point(20, 50),
                Width = 200,
                Value = DateTime.Now
            };

            // ComboBox для счетов
            cmbAccount = new ComboBox
            {
                Location = new Point(20, 80),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // ComboBox для категорий
            cmbCategory = new ComboBox
            {
                Location = new Point(20, 110),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // TextBox для суммы
            txtAmount = new TextBox
            {
                Location = new Point(20, 140),
                Width = 200,
               
            };

            // TextBox для описания
            txtDescription = new TextBox
            {
                Location = new Point(20, 170),
                Width = 200,
               
            };

            // Кнопка сохранения
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(20, 220),
                Width = 100
            };
            btnSave.Click += btnSave_Click;

            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(140, 220),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавляем элементы на форму
            this.Controls.Add(lblType);
            this.Controls.Add(dtpDate);
            this.Controls.Add(cmbAccount);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(txtAmount);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            // Загрузка счетов
            var accounts = _dbService.GetAccounts(_user.Id);
            cmbAccount.DataSource = accounts;
            cmbAccount.DisplayMember = "Name";
            cmbAccount.ValueMember = "Id";

            // Загрузка категорий
            var categories = _dbService.GetCategories(_user.Id)
                .Where(c => c.IsIncome == _isIncome).ToList();
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var transaction = new Transaction
            {
                Amount = amount,
                Date = dtpDate.Value,
                Description = txtDescription.Text,
                CategoryId = (int)cmbCategory.SelectedValue,
                AccountId = (int)cmbAccount.SelectedValue,
                IsIncome = _isIncome,
                UserId = _user.Id
            };

            _dbService.AddTransaction(transaction);

            // Обновляем баланс счета
            var account = _dbService.GetAccount((int)cmbAccount.SelectedValue);
            account.Balance += _isIncome ? amount : -amount;
            _dbService.UpdateAccount(account);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
