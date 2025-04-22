using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;

namespace MArketPlace
{
   
    public partial class Buyers : Form
    {
        Product product;
        List<Product> list = new List<Product>();

        public Buyers()
        {
            InitializeComponent();
            StreamReader sr = new StreamReader("product.txt");
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(';', '\n');
                string link = line[3];
                string name = line[1];
                double price = Convert.ToDouble(line[2].Replace('.', ','));
                int count = int.Parse(line[4]);
                product = new Product(name, price, count, link);
                list.Add(product);
            }
            sr.Close();

            GenerateTovar(list);

            
            
        }

        int kk = 0;
        void Value(object sender, EventArgs e)
        {
            kk = Convert.ToInt32((sender as NumericUpDown).Value);
        }


        private void Buyers_Load(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            label19.Text = Form1.ValueToPass;
        }


        private void OnClick(object sender, EventArgs e)
        {
            do
            {
                if (File.Exists("Requests.txt"))
                {
                    StreamWriter sw = new StreamWriter("Requests.txt", true);
                    sw.WriteLine($"{product.name};{kk};{product.price}");
                    sw.Close();
                    dataGridView1.ForeColor = Color.Black;
                   // string data = (sender as TextBox).Text;
                    dataGridView1.Rows.Add($"{product.name}", $"{kk}", $"{product.price}");
                }
                else
                {
                    File.Create("Requests.txt");
                }
            } while (!File.Exists("Requests.txt"));

        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            string searchNumber = textBox1.Text;
            var filteredProducts = list.Where(p => p.name.Contains(searchNumber)).ToList();

            foreach (Control control in flowLayoutPanel1.Controls.OfType<Panel>().ToList())
            {
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose(); 
            }
            GenerateTovar(filteredProducts);

            if (!filteredProducts.Any())
            {
                flowLayoutPanel1.Controls.Add(new System.Windows.Forms.Label { Text = "The product was not found.", AutoSize = true });
            }
        }

        private void GenerateTovar(List<Product> Array)
        {

            int i = 0;
            foreach (var product in Array)
            {
                bool pr = true;
                Panel panel = new Panel();
                panel.Size = new Size(200, 250);
                panel.Name = "panel" + i.ToString();
                panel.BorderStyle = BorderStyle.FixedSingle;

                PictureBox pic = new PictureBox();
                pic.Size = new Size(115, 115);
                pic.Location = new Point(36, 0);
                pic.BorderStyle = BorderStyle.FixedSingle;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Image = Image.FromFile($@"{product.link}");
                panel.Controls.Add(pic);


                NumericUpDown textbox = new NumericUpDown();
                textbox.Size = new Size(125, 30);
                textbox.Location = new Point(36, 180);
                textbox.Minimum = 1;
                textbox.ValueChanged += Value;
                textbox.Maximum = product.count;
                if (product.count <= 0) pr = false;
                panel.Controls.Add(textbox);

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.ForeColor = Color.White;
                label.Font = new Font("Old English Text MT", 10);
                label.Size = new Size(200, 60);
                label.Location = new Point(2, 120);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = $"Name : {product.name}\nPraice: {product.price}\nCount: {product.count}";
                panel.Controls.Add(label);

                System.Windows.Forms.Button button2 = new System.Windows.Forms.Button();
                button2.Name = i.ToString();
                button2.Font = new Font("Old English Text MT", 12);
                button2.BackgroundImage = global::MArketPlace.Properties.Resources._8001;
                button2.Size = new Size(125, 30);
                button2.Location = new Point(36, 215);
                button2.Text = "Add";
                button2.Click += OnClick;
                panel.Controls.Add(button2);
                if (pr == true)
                {
                    i++;
                    
                    
                    flowLayoutPanel1.Controls.Add(panel);

                }

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            throw new Exception();
        }

       
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            List<Product> sortedProducts =list.ToList();
            
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    sortedProducts = sortedProducts.OrderBy(p => p.name).ToList();
                    break;
                case 1:
                    sortedProducts = sortedProducts.OrderByDescending(p => p.name).ToList();
                    break;
                case 2:
                    sortedProducts = sortedProducts.OrderByDescending(p => p.price).ToList();
                    break;
                case 3:
                    sortedProducts = sortedProducts.OrderBy(p => p.price).ToList();
                    break;
                case 4:
                    sortedProducts= sortedProducts.OrderBy(p => p.count).ToList();
                    break;
            }
            foreach (Control control in flowLayoutPanel1.Controls.OfType<Panel>().ToList())
            {
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }
            GenerateTovar(sortedProducts);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocaldb;Initial Catalog=Romanov_1SQL;Integrated Security=True"); connect.Open();
            SqlDataAdapter adptr = new SqlDataAdapter("select * from ", connect); DataTable table = new DataTable();
            adptr.Fill(table); dataGridView1.DataSource = table;
            connect.Close();
        }
    }
}
