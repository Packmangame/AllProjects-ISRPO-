using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace AmoCRM
{
    
    
    public partial class Form1 : Form
    {
        
        string API = "7444770745:AAGaPJnWAcwrL_LmgtiA-78QYGHxLl7Knkc";
        string ChatID = "7444770745";

        string Namei = "";
        string FName = "";
        string SName = "";

        string kont1 = "";
        string kont2 = "";
        string kont3 = "";
        public Form1()
        {
            InitializeComponent();
            
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }


        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        //MessagueShow
        private void ShowMessague2(object sender, MouseEventArgs e)
        {
            panel5.Visible = true;
            label1.Visible = true;
            if (Namei == "" && FName == "" && SName == "")
            {
                label1.Text = "Name \nFirst Name \nSecond Name";
            }
            else
            {
                label1.Text= $"{Namei} \n{FName} \n{SName}";
            }

        }

        private void ShowMessague3(object sender, MouseEventArgs e)
        {
            panel4.Visible= true;
            label2.Visible = true;
            label2.Text = "Task 1 \nTask 2 \nTask 3";
        }


        private void ShowMessague4(object sender, MouseEventArgs e)
        {
            panel2.Visible = true;
            label3.Visible = true;
            if (kont1 != "")
            {
                label3.Text = $"{kont1} \nKontact 2 \nKontact 3";
            }
            else if (kont1 != "" && kont2 != "")
            {
                label3.Text = $"{kont1} \n{kont2} \nKontact 3";
            }
            else if (kont1 != "" && kont2 != "" && kont3 != "")
            {
                label3.Text = $"{kont1} \n{kont2} \n{kont3}";
            }
            else
            {
                label3.Text = "Kontact 1 \nKontact 2 \nKontact 3";
            }
        }

        private void ShowMessague5(object sender, MouseEventArgs e)
        {
            panel7.Visible = true;
            Red.Visible = true;
            Yell.Visible = true;
            SeaGreen.Visible = true;
            Deff.Visible = true;
        }

        private void Red_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Red;
            panel6.BackColor = Color.Red;

            panel7.Visible = false;
            Red.Visible = false;
            Yell.Visible = false;
            SeaGreen.Visible = false;
            Deff.Visible = false;
        }

        private void Yell_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Yellow;
            panel6.BackColor = Color.Yellow;

            panel7.Visible = false;
            Red.Visible = false;
            Yell.Visible = false;
            SeaGreen.Visible = false;
            Deff.Visible = false;
        }

        private void SeaGreen_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.SeaGreen;
            panel6.BackColor = Color.SeaGreen;

            panel7.Visible = false;
            Red.Visible = false;
            Yell.Visible = false;
            SeaGreen.Visible = false;
            Deff.Visible = false;
        }

        private void Deff_Click(object sender, EventArgs e)
        {
            this.BackColor = default;
            panel6.BackColor = default;

            panel7.Visible = false;
            Red.Visible = false;
            Yell.Visible = false;
            SeaGreen.Visible = false;
            Deff.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            System.Diagnostics.Process.Start("cmd", "/c curl parrot.live");

        }

        private void HideMessague2(object sender, EventArgs e)
        {
            panel5.Visible=false;
            label1.Visible=false;
        }

        private void HideMessague3(object sender, EventArgs e)
        {
            panel4.Visible=false;
            label2.Visible=false;
        }

        private void HideMessague4(object sender, EventArgs e)
        {
            panel2.Visible = false;
            label3.Visible=false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
            panel8.Visible = false;
            panel10.Visible= false;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Namei = textBox1.Text;
            FName= textBox2.Text;
            SName= textBox3.Text;
        }
        private int taskcount=0;
        private void button3_Click(object sender, EventArgs e)
        {
            panel6.Visible = false;
            panel8.Visible = false;
            panel10.Visible = true;
            if (taskcount == 0)
            {
                int ColCount = 4, RowCount = 1;
                Tablelayout1HedderText(ColCount, RowCount);
                taskcount++;
            }

        }

        private void Tablelayout1HedderText(int ColCount, int RowCount)
        {
            
            //heder text
            Label header1 = new Label();
            header1.Text = "Yesterday";
            header1.Dock = DockStyle.Fill;
            header1.TextAlign = ContentAlignment.MiddleCenter;

            Label header2 = new Label();
            header2.Text = "Today";
            header2.Dock = DockStyle.Fill;
            header2.TextAlign = ContentAlignment.MiddleCenter;

            Label header3 = new Label();
            header3.Text = "Tomorrow";
            header3.Dock = DockStyle.Fill;
            header3.TextAlign = ContentAlignment.MiddleCenter;

            Label header4 = new Label();
            header4.Text = "Next time";
            header4.Dock = DockStyle.Fill;
            header4.TextAlign = ContentAlignment.MiddleCenter;
            

            tableLayoutPanel1.Controls.Add(header1, 0, 0);
            tableLayoutPanel1.Controls.Add(header2, 1, 0);
            tableLayoutPanel1.Controls.Add(header3, 2, 0);
            tableLayoutPanel1.Controls.Add(header4, 3, 0);
        }

        DataGridView dataGridView = new DataGridView();
        int inq = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            panel6.Visible = false;
            panel8.Visible = true;
            button8.Visible = true;
            panel10.Visible = false;
            
            if (inq == 0)
            {
                // Создание столбцов
                dataGridView.Columns.Add("Id", "ID");
                dataGridView.Columns.Add("Name", "Name");
                dataGridView.Columns.Add("FName", "First Name");
                dataGridView.Columns.Add("SName", "Second Name");
                dataGridView.Columns.Add("Title", "Job Title");
                dataGridView.Columns.Add("Com", "Company");
                dataGridView.Columns.Add("Phon", "Phone Number");
                inq++;
            }

            // Добавление DataGridView на форму
           panel8.Controls.Add(dataGridView);
            
            dataGridView.Location=new Point(15, 109);
            dataGridView.Size = new Size(450, 150);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            panel9.Visible=true;
        }
        int i = 0;
        

        private void button9_Click(object sender, EventArgs e)
        {
            String N, FN, SN, T, C,P;

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(@"D:\Программы\Project\AmoCRM\AmoCRM\bin\Debug\tasks.xlsx");
            Excel.Worksheet worksheet = workbook.Sheets[2];

            i++;
            panel9.Visible= false;
            
            N = textBox9.Text;
            FN = textBox8.Text;
            SN = textBox7.Text;
            T = textBox6.Text;
            C = textBox5.Text;
            P = textBox4.Text;

            // Добавление данных
            for (int j=0;j<1;j++)
            {
                dataGridView.Rows.Add(i, $"{N}", $"{FN}", $"{SN}", $"{T}", $"{C}", $"{P}");
                worksheet.Cells[i+1, 1].Value = i;
                worksheet.Cells[i+1, 2].Value = $"{N.Substring(0, 1)}.{SN.Substring(0, 1)} {FN}";
                worksheet.Cells[i + 1, 3].Value = T;
                worksheet.Cells[i + 1, 4].Value = C;
                worksheet.Cells[i + 1, 5].Value = P;
            }
            comboBox1.Items.Add(worksheet.Cells[i + 1, 2].Value.ToString());
            
           
            workbook.Save();
            workbook.Close();
            excelApp.Quit();
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            panel11.Visible = true;
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            int TC = 0;
            panel11.Visible=false;
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(@"D:\Программы\Project\AmoCRM\AmoCRM\bin\Debug\tasks.xlsx");
            Excel.Worksheet worksheet = workbook.Sheets[1];
            int rows = worksheet.UsedRange.Rows.Count;
            var bot = new TelegramBotClient(API);
            if (dateTimePicker1.Value.Date == DateTime.Now)
            {
                await bot.SendTextMessageAsync(ChatID, $"U are have a tiket \nDate: {dateTimePicker1.Value} \nText: {textBox10.Text}");
                
            }
            else
            {
                for (int l = 2; l <= rows; l++)
                {
                    worksheet.Cells[l, 1].Value = comboBox1.SelectedItem.ToString();
                    worksheet.Cells[l, 2].Value = dateTimePicker1.Value.ToString();
                    worksheet.Cells[l, 3].Value = textBox10.ToString();
                }
            }
            workbook.Save();
            workbook.Close();
            excelApp.Quit();

            //tablelayoutpanel1.AddTickets
            Label TicketsInfo=new Label();
            TicketsInfo.Text = $"{comboBox1.SelectedItem.ToString()}";
            TicketsInfo.Location = new Point(0,5);

            Label TicketsData = new Label();
            TicketsData.Text = $"{dateTimePicker1.Value}";
            TicketsData.Location = new Point(0,15);

            Label TicketsZametka = new Label();
            TicketsZametka.Text = $"{textBox10.Text.ToString()}";
            TicketsInfo.Location = new Point(0,25);
            TicketsZametka.Dock = DockStyle.Fill;

            DateTime selectedDate = dateTimePicker1.Value.Date;
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            DateTime tommorow = today.AddDays(+1);

           
            if (selectedDate == today)
            {
                Panel panel = new Panel();
                panel.AutoSize = true;
                panel.Name = $"Ticket {TC}";
                panel.Controls.Add(TicketsInfo);
                panel.Controls.Add(TicketsZametka);
                panel.Controls.Add(TicketsData);
                panel.BorderStyle = BorderStyle.FixedSingle;
                tableLayoutPanel1.Controls.Add(panel, 1, 1);
            }
            else if (selectedDate == yesterday)
            {
                Panel panel = new Panel();
                panel.Name = $"Ticket {TC}";
                panel.Controls.Add(TicketsInfo);
                panel.Controls.Add(TicketsData);
                panel.Controls.Add(TicketsZametka);
                panel.BorderStyle = BorderStyle.FixedSingle;
                tableLayoutPanel1.Controls.Add(panel, 0, 1);
            }
            else if (selectedDate==tommorow)
            {
                Panel panel= new Panel();
                panel.Name = $"Ticket {TC}";
                panel.Controls.Add(TicketsInfo);
                panel.Controls.Add(TicketsData);
                panel.Controls.Add(TicketsZametka);
                panel.BorderStyle = BorderStyle.FixedSingle;
                tableLayoutPanel1.Controls.Add(panel, 2, 1);
            }
            else
            {
                Panel panel = new Panel();
                panel.Name = $"Ticket {TC}";
                panel.Controls.Add(TicketsInfo);
                panel.Controls.Add(TicketsData);
                panel.Controls.Add(TicketsZametka);
                panel.BorderStyle = BorderStyle.FixedSingle;
                tableLayoutPanel1.Controls.Add(panel, 3, 1);
            }
                
            
       
        }
    }
}

