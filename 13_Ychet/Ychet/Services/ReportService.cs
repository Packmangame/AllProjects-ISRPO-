using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ychet.Models;

namespace Ychet.Services
{
    // Services/ReportService.cs
    public class ReportService
    {
        private readonly DatabaseService _dbService;

        public ReportService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public IEnumerable<CategorySummary> GetIncomeByCategory(int userId, DateTime fromDate, DateTime toDate)
        {
            return _dbService.GetTransactions(userId)
                .Where(t => t.IsIncome && t.Date >= fromDate && t.Date <= toDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySummary
                {
                    CategoryName = _dbService.GetCategoryName(g.Key),
                    Amount = g.Sum(t => t.Amount)
                });
        }

        public IEnumerable<CategorySummary> GetExpensesByCategory(int userId, DateTime fromDate, DateTime toDate)
        {
            return _dbService.GetTransactions(userId)
                .Where(t => !t.IsIncome && t.Date >= fromDate && t.Date <= toDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySummary
                {
                    CategoryName = _dbService.GetCategoryName(g.Key),
                    Amount = g.Sum(t => t.Amount)
                });
        }

        public IEnumerable<Transaction> GetTransactions(int userId, DateTime fromDate, DateTime toDate)
        {
            return _dbService.GetTransactions(userId)
                .Where(t => t.Date >= fromDate && t.Date <= toDate)
                .OrderByDescending(t => t.Date);
        }

        public void ExportToPdf(int userId, DateTime fromDate, DateTime toDate, string filePath)
        {
            // Реализация экспорта в PDF (используйте iTextSharp или другую библиотеку)
        }

        public void ExportToCsv(int userId, DateTime fromDate, DateTime toDate, string filePath)
        {
            // Реализация экспорта в CSV
        }
    }
}
