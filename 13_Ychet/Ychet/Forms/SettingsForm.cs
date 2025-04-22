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
    public partial class SettingsForm : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _dbService;
        private readonly User _user;

        public SettingsForm(AuthService authService, DatabaseService dbService, User user)
        {
            InitializeComponent();
            _authService = authService;
            _dbService = dbService;
            _user = user;
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Загрузка настроек пользователя
        }
    }
}
