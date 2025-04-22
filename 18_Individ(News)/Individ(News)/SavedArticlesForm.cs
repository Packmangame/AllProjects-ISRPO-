using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Individ_News_
{
    public partial class SavedArticlesForm : Form
    {
        public SavedArticlesForm(List<string> savedArticles)
        {
            this.Text = "Сохраненные статьи";
            this.Size = new Size(600, 400);

            ListBox savedListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 12)
            };

            foreach (var article in savedArticles)
            {
                savedListBox.Items.Add(article);
            }

            this.Controls.Add(savedListBox);
        }
    }
}
