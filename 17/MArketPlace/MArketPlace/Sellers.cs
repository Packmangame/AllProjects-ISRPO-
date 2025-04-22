using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MArketPlace
{
    public partial class Sellers : Form
    {
        public Sellers()
        {
            
            InitializeComponent();
            StreamReader sr = new StreamReader("product.txt");
            
            dataGridView1.Rows.Clear();
            dataGridView3.Rows.Clear();
            while (!sr.EndOfStream)
            {
                string[] s = sr.ReadLine().Split(';', '\n');
                dataGridView1.ForeColor = Color.Black;
                dataGridView1.Rows.Add($"{s[0]}", $"{s[1]}",$"{s[2]}",$"{s[3]}", $"{s[4]}", $"{s[5]}");
                dataGridView3.ForeColor = Color.Black;
                dataGridView3.Rows.Add($"{s[0]}", $"{s[1]}", $"{s[4]}", $"{s[5]}", $"{s[4]}", "None");
                
            }
            sr.Close();
        
        }
   
        
        private void Sellers_Load(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            label23.Text=Form1.ValueToPass;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что нажата ячейка
            if (e.RowIndex >= 0 && e.ColumnIndex == 3)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    openFileDialog.Title = "Выберите изображение";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                        // Обновляем содержимое ячейки с изображением
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = selectedFilePath;
                    }
                }
            }
        }


        private void SaveData(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("product.txt");
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    // Проверка на null
                    sw.Write(dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? string.Empty);
                    if (j < dataGridView1.Columns.Count - 1)
                        sw.Write(";");
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }
    }
}
