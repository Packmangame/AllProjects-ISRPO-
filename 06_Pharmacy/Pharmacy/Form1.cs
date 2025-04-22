using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetDataFromDatabase();
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void flowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void Liquid_Click(object sender, EventArgs e)
        {

        }

        private void Solid_Click(object sender, EventArgs e)
        {

        }

        private void Aerosol_Click(object sender, EventArgs e)
        {

        }

        private void Soft_Click(object sender, EventArgs e)
        {

        }

        private Panel CreateMedicineCard(int id, string name, string category, string type)
        {
            Panel card = new Panel();
            card.BackColor = Color.AliceBlue;
            card.Size = new Size(300, 250);
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(10);


            Label nameLabel = new Label
            {
                Text = $"Название: {name}",
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label categoryLabel = new Label
            {
                Text = $"Категория: {category}",
                Location = new Point(10, 40),
                AutoSize = true
            };

            Label typeLabel = new Label
            {
                Text = $"Вид: {type}",
                Location = new Point(10, 70),
                AutoSize = true
            };

            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(10, 100),
                Image = Image.FromFile(""),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            card.Controls.Add(nameLabel);
            card.Controls.Add(categoryLabel);
            card.Controls.Add(typeLabel);
            card.Controls.Add(pictureBox);

            return card;
        }
        private string connectionString = "Server=DESKTOP-1IJPGFA;Database=Apteka;Trusted_Connection=True;";
        public void GetDataFromDatabase()
        {
                
            string query = "SELECT Preporati.ID, Preporati.Name, Preporati.Category, Preporati.Pic, Preporati.Vid FROM Apteka.dbo.Preporati";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                /*try
                {*/
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        //string vid = reader.GetString(3);

                        //Panel card =CreateMedicineCard(id, name, category, vid);
                        //flowLayoutPanel1.Controls.Add(card);
                    }
                /*}
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }*/
            }
        }

        
    }
}
    

