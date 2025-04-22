using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Biblioteka.Forms
{
    public partial class MainForm : Form
    {
        private readonly int _userId;
        private readonly string _userRole;

        public MainForm(int userId, string userRole)
        {
            _userId = userId;
            _userRole = userRole;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Электронная библиотека ({_userRole})";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var exitItem = new ToolStripMenuItem("Выход", null, (s, e) => Application.Exit());
            fileMenu.DropDownItems.Add(exitItem);
            menuStrip.Items.Add(fileMenu);

            var booksMenu = new ToolStripMenuItem("Книги");
            var searchItem = new ToolStripMenuItem("Поиск книг", null, (s, e) => ShowBookSearchForm());
            booksMenu.DropDownItems.Add(searchItem);

            if (_userRole != "User")
            {
                var manageItem = new ToolStripMenuItem("Управление книгами", null, (s, e) => ShowBookManagementForm());
                booksMenu.DropDownItems.Add(manageItem);
            }

            var myBooksItem = new ToolStripMenuItem("Мои книги", null, (s, e) => ShowMyBooksForm());
            booksMenu.DropDownItems.Add(myBooksItem);
            menuStrip.Items.Add(booksMenu);

            if (_userRole == "Admin")
            {
                var usersMenu = new ToolStripMenuItem("Пользователи");
                var manageUsersItem = new ToolStripMenuItem("Управление пользователями", null, (s, e) => ShowUserManagementForm());
                usersMenu.DropDownItems.Add(manageUsersItem);
                menuStrip.Items.Add(usersMenu);
            }

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            var statusStrip = new StatusStrip();
            var userLabel = new ToolStripStatusLabel($"Пользователь: {_userId} ({_userRole})");
            statusStrip.Items.Add(userLabel);
            this.Controls.Add(statusStrip);
        }

        private void ShowBookSearchForm()
        {
            var searchForm = new BookSearchForm(_userId, _userRole);
            searchForm.ShowDialog();
        }

        private void ShowBookManagementForm()
        {
            var managementForm = new BookManagementForm();
            managementForm.ShowDialog();
        }

        private void ShowMyBooksForm()
        {
            var myBooksForm = new MyBooksForm(_userId);
            myBooksForm.ShowDialog();
        }

        private void ShowUserManagementForm()
        {
            var userForm = new UserManagementForm();
            userForm.ShowDialog();
        }
    }
}
