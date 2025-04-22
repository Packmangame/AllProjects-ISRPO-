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
    public partial class AutoPaymentForm : Form
    {
        private readonly DatabaseService _dbService;
        private readonly User _user;

        public AutoPaymentForm(DatabaseService dbService, User user)
        {
            InitializeComponent();
            _dbService = dbService;
            _user = user;
            LoadAutoPayments();
        }

        private void LoadAutoPayments()
        {
            // Загрузка списка автоплатежей
        }
    }
}
