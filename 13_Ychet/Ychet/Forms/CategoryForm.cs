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
    public partial class CategoryForm : Form
    {
        private readonly DatabaseService _dbService;
        private readonly User _user;

        public CategoryForm(DatabaseService dbService, User user)
        {
            InitializeComponent();
            _dbService = dbService;
            _user = user;
            LoadCategories();
        }

        private void LoadCategories()
        {
            // Загрузка списка категорий
        }
    }
}
