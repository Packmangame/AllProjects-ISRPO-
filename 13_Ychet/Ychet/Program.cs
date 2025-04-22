using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ychet.Forms;
using Ychet.Services;

namespace Ychet
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var connectionString = "Data Source=finance.db;Version=3;";
            var dbService = new DatabaseService(connectionString);
            var authService = new AuthService(dbService);
            var reportService = new ReportService(dbService);
            var chartService = new ChartService();

            // Показываем форму входа
            using (var loginForm = new LoginForm(authService))
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Передаем все 5 параметров, включая аутентифицированного пользователя
                    Application.Run(new MainForm(
                        authService,
                        dbService,
                        reportService,
                        chartService,
                        loginForm.AuthenticatedUser)); // 5-й параметр
                }
            }
        }
    }
}
