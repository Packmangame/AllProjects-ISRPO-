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
    public partial class ReportForm : Form
    {
        private readonly ReportService _reportService;
        private readonly ChartService _chartService;
        private readonly User _user;

        // Элементы управления
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Label lblIncome;
        private Label lblExpenses;
        private Label lblBalance;
        private DataGridView dgvTransactions;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartIncome;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartExpenses;
        private Button btnGenerate;
        private Button btnExportPdf;
        private Button btnExportCsv;

        public ReportForm(ReportService reportService, ChartService chartService, User user)
        {
            InitializeComponent();
            _reportService = reportService;
            _chartService = chartService;
            _user = user;

            InitializeControls();
            GenerateReport();
        }

        private void InitializeControls()
        {
            // Инициализация всех элементов управления
            this.dtpFrom = new DateTimePicker();
            this.dtpTo = new DateTimePicker();
            this.lblIncome = new Label();
            this.lblExpenses = new Label();
            this.lblBalance = new Label();
            this.dgvTransactions = new DataGridView();
            this.chartIncome = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartExpenses = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnGenerate = new Button();
            this.btnExportPdf = new Button();
            this.btnExportCsv = new Button();

            // Настройка свойств элементов управления
            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpTo.Value = DateTime.Now;

            btnGenerate.Text = "Сформировать отчет";
            btnGenerate.Click += btnGenerate_Click;

            btnExportPdf.Text = "Экспорт в PDF";
            btnExportPdf.Click += btnExportPdf_Click;

            btnExportCsv.Text = "Экспорт в CSV";
            btnExportCsv.Click += btnExportCsv_Click;

            // Добавление элементов на форму
            Controls.Add(dtpFrom);
            Controls.Add(dtpTo);
            Controls.Add(lblIncome);
            Controls.Add(lblExpenses);
            Controls.Add(lblBalance);
            Controls.Add(dgvTransactions);
            Controls.Add(chartIncome);
            Controls.Add(chartExpenses);
            Controls.Add(btnGenerate);
            Controls.Add(btnExportPdf);
            Controls.Add(btnExportCsv);

            // Настройка layout (можно использовать TableLayoutPanel для лучшего размещения)
        }

        private void GenerateReport()
        {
            var fromDate = dtpFrom.Value;
            var toDate = dtpTo.Value;

            var income = _reportService.GetIncomeByCategory(_user.Id, fromDate, toDate);
            var expenses = _reportService.GetExpensesByCategory(_user.Id, fromDate, toDate);
            var transactions = _reportService.GetTransactions(_user.Id, fromDate, toDate);

            decimal totalIncome = income.Sum(i => i.Amount);
            decimal totalExpenses = expenses.Sum(e => e.Amount);
            decimal balance = totalIncome - totalExpenses;

            lblIncome.Text = $"Доходы: {totalIncome:C}";
            lblExpenses.Text = $"Расходы: {totalExpenses:C}";
            lblBalance.Text = $"Баланс: {balance:C}";

            dgvTransactions.DataSource = transactions.ToList();

            _chartService.CreatePieChart(chartIncome, income, "Доходы по категориям");
            _chartService.CreatePieChart(chartExpenses, expenses, "Расходы по категориям");
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Экспорт отчета в PDF"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                _reportService.ExportToPdf(_user.Id, dtpFrom.Value, dtpTo.Value, saveDialog.FileName);
                MessageBox.Show("Отчет успешно экспортирован в PDF");
            }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Экспорт отчета в CSV"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                _reportService.ExportToCsv(_user.Id, dtpFrom.Value, dtpTo.Value, saveDialog.FileName);
                MessageBox.Show("Отчет успешно экспортирован в CSV");
            }
        }
    }
}
