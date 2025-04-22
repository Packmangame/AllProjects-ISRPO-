using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ychet.Forms;
using Ychet.Models;
using Ychet.Services;

namespace Ychet
{
    public partial class MainForm : Form
    {
        private Button btnLogin;
        private Button btnRegister;
        private ListView transactionsListView;
        private ListBox accountsListBox;
        private Label lblTotalBalance;
        private Button btnAddIncome;
        private Button btnAddExpense;
        private Button btnManageCategories;
        private Button btnGenerateReport;
        private Button btnSettings;

        private readonly AuthService _authService;
        private readonly DatabaseService _dbService;
        private readonly ReportService _reportService;
        private readonly ChartService _chartService;
        private User _currentUser; // Изменяем на не-readonly поле

        // Обновленный конструктор с 5 параметрами
        public MainForm(AuthService authService,
                       DatabaseService dbService,
                       ReportService reportService,
                       ChartService chartService,
                       User authenticatedUser) // Добавляем 5-й параметр
        {
            _authService = authService;
            _dbService = dbService;
            _reportService = reportService;
            _chartService = chartService;
            _currentUser = authenticatedUser; // Сохраняем пользователя

            InitializeComponent();
            InitializeMainForm();
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Инициализация всех элементов управления
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.transactionsListView = new ListView();
            this.accountsListBox = new ListBox();
            this.lblTotalBalance = new Label();
            this.btnAddIncome = new Button();
            this.btnAddExpense = new Button();
            this.btnManageCategories = new Button();
            this.btnGenerateReport = new Button();
            this.btnSettings = new Button();

            // Настройка свойств элементов
            this.Text = "Финансовый трекер";
            this.ClientSize = new Size(800, 600);

            // Кнопка входа/выхода
            btnLogin.Text = _currentUser != null ? "Выйти" : "Войти";
            btnLogin.Location = new Point(20, 200);
            btnLogin.Width = 150;
            btnLogin.Click += btnLogin_Click;

            // Кнопка регистрации
            btnRegister.Text = "Регистрация";
            btnRegister.Location = new Point(20, 225);
            btnRegister.Width = 150;
            btnRegister.Click += btnRegister_Click;
            btnRegister.Visible = _currentUser == null;

            // Label общего баланса
            lblTotalBalance.Location = new Point(20, 20);
            lblTotalBalance.AutoSize = true;
            lblTotalBalance.Font = new Font(lblTotalBalance.Font, FontStyle.Bold);

            // ListBox для счетов
            accountsListBox.Location = new Point(20, 50);
            accountsListBox.Size = new Size(200, 150);

            // ListView для транзакций
            transactionsListView.View = View.Details;
            transactionsListView.Location = new Point(250, 50);
            transactionsListView.Size = new Size(500, 400);
            transactionsListView.Columns.Add("Дата", 100);
            transactionsListView.Columns.Add("Тип", 80);
            transactionsListView.Columns.Add("Сумма", 80);
            transactionsListView.Columns.Add("Категория", 120);
            transactionsListView.Columns.Add("Счет", 120);
            transactionsListView.Columns.Add("Описание", 200);

            // Кнопка добавления дохода
            btnAddIncome.Text = "Добавить доход";
            btnAddIncome.Location = new Point(20, 250);
            btnAddIncome.Width = 150;
            btnAddIncome.Click += btnAddIncome_Click;

            // Кнопка добавления расхода
            btnAddExpense.Text = "Добавить расход";
            btnAddExpense.Location = new Point(20, 275);
            btnAddExpense.Width = 150;
            btnAddExpense.Click += btnAddExpense_Click;

            // Кнопка управления категориями
            btnManageCategories.Text = "Управление категориями";
            btnManageCategories.Location = new Point(20, 300);
            btnManageCategories.Width = 150;
            btnManageCategories.Click += btnManageCategories_Click;

            // Кнопка генерации отчета
            btnGenerateReport.Text = "Генерировать отчет";
            btnGenerateReport.Location = new Point(20, 325);
            btnGenerateReport.Width = 150;
            btnGenerateReport.Click += btnGenerateReport_Click;
            
            // Кнопка настроек
            btnSettings.Text = "Настройки";
            btnSettings.Location = new Point(20, 350);
            btnSettings.Width = 150;
            btnSettings.Click += btnSettings_Click;



            // Добавляем элементы на форму
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(lblTotalBalance);
            this.Controls.Add(accountsListBox);
            this.Controls.Add(transactionsListView);
            this.Controls.Add(btnAddIncome);
            this.Controls.Add(btnAddExpense);
            this.Controls.Add(btnManageCategories);
            this.Controls.Add(btnGenerateReport);
            this.Controls.Add(btnSettings);
            
        }

        private void InitializeMainForm()
        {
            Text = $"Финансовый трекер - {_currentUser.Username}";
            UpdateBalance();
            LoadRecentTransactions();
            CheckAutoPayments();
            if (_currentUser != null)
            {
                Text += $" - {_currentUser.Username}";
                UpdateBalance();
                LoadRecentTransactions();
                CheckAutoPayments();
            }
           
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (_currentUser != null)
            {
                // Выход из системы
                _currentUser = null;
                btnLogin.Text = "Войти";
                btnRegister.Visible = true;
                lblTotalBalance.Text = "";
                accountsListBox.Items.Clear();
                transactionsListView.Items.Clear();
                MessageBox.Show("Вы успешно вышли из системы");
            }
            else
            {
                // Вход в систему
                using (var loginForm = new LoginForm(_authService))
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        _currentUser = loginForm.AuthenticatedUser;
                        btnLogin.Text = "Выйти";
                        btnRegister.Visible = false;
                        InitializeMainForm();
                    }
                }
            }
        }


      
        private void MainForm_Load(object sender, EventArgs e)
        { 
        }


        private void UpdateBalance()
        {
            var accounts = _dbService.GetAccounts(_currentUser.Id);
            decimal totalBalance = accounts.Sum(a => a.Balance);
            lblTotalBalance.Text = $"Общий баланс: {totalBalance:C}";

            accountsListBox.Items.Clear();
            foreach (var account in accounts)
            {
                accountsListBox.Items.Add($"{account.Name}: {account.Balance:C}");
            }
        }

        private void btnAddIncome_Click(object sender, EventArgs e)
        {
            using (var form = new TransactionForm(_dbService, _currentUser, true))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateBalance();
                    LoadRecentTransactions();
                }
            }
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            using (var form = new TransactionForm(_dbService, _currentUser, false))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateBalance();
                    LoadRecentTransactions();
                }
            }
        }

        private void btnManageCategories_Click(object sender, EventArgs e)
        {
            using (var form = new CategoryForm(_dbService, _currentUser))
            {
                form.ShowDialog();
            }
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            using (var form = new ReportForm(_reportService, _chartService, _currentUser))
            {
                form.ShowDialog();
            }
        }

        private void btnAutoPayments_Click(object sender, EventArgs e)
        {
            using (var form = new AutoPaymentForm(_dbService, _currentUser))
            {
                form.ShowDialog();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm(_authService, _dbService, _currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем данные, если пользователь изменил настройки
                    UpdateBalance();
                    LoadRecentTransactions();
                }
            }
        }

        private void CheckAutoPayments()
        {
            var today = DateTime.Today;
            var autoPayments = _dbService.GetAutoPayments(_currentUser.Id)
                .Where(ap => ap.NextPaymentDate.Date <= today).ToList();

            foreach (var payment in autoPayments)
            {
                // Выполняем автоплатеж
                _dbService.TransferBetweenAccounts(
                    payment.FromAccountId,
                    payment.ToAccountId,
                    payment.Amount,
                    payment.Description,
                    payment.CategoryId,
                    _currentUser.Id);

                // Обновляем дату следующего платежа
                payment.NextPaymentDate = payment.NextPaymentDate.AddDays(payment.FrequencyDays);
                _dbService.UpdateAutoPayment(payment);
            }

            if (autoPayments.Any())
            {
                UpdateBalance();
                LoadRecentTransactions();
                MessageBox.Show("Выполнены автоматические платежи", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadRecentTransactions()
        {
            var transactions = _dbService.GetRecentTransactions(_currentUser.Id, 10);
            transactionsListView.Items.Clear();

            foreach (var t in transactions)
            {
                var item = new ListViewItem(t.Date.ToShortDateString());
                item.SubItems.Add(t.IsIncome ? "Доход" : "Расход");
                item.SubItems.Add(t.Amount.ToString("C"));
                item.SubItems.Add(_dbService.GetCategoryName(t.CategoryId));
                item.SubItems.Add(_dbService.GetAccountName(t.AccountId));
                item.SubItems.Add(t.Description);

                transactionsListView.Items.Add(item);
            }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm(_authService))
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Теперь вы можете войти с новыми учетными данными",
                                  "Регистрация успешна",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
        }


    }
}
