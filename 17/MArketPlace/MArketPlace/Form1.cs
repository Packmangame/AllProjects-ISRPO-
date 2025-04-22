using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Text.RegularExpressions;
using MArketPlace.Properties;

namespace MArketPlace
{
    public partial class Form1 : Form
    {
        public static string ValueToPass;
        //creation of dictionaries for login and password Users and SellersSS
        Dictionary<string , string> users = new Dictionary<string, string>();
        Dictionary<string , string> salers = new Dictionary<string, string>();
        private SoundPlayer _soundPlayer;
        public Form1()
        {
            InitializeComponent();
            //sound start
            _soundPlayer = new SoundPlayer(@"D:\Программы\Project\ИС РПО\MArketPlace\MArketPlace\Resources\music.wav");
            _soundPlayer.PlayLooping();

            //password symbol
            textBox2.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;
            textBox11.UseSystemPasswordChar = true;
            textBox3.UseSystemPasswordChar = true;

            //Lgin and password users
            if (File.Exists("users.txt"))
            {
                string content = File.ReadAllText("users.txt");
                if (!string.IsNullOrEmpty(content))
                {
                    string[] u = File.ReadAllText("users.txt").Split(':');
                    users.Add(u[0], $"{u[1]}:{u[2]}");
                }
            }

            ////Lgin and password sellers
            if (File.Exists("sellers.txt"))
            {
                string content = File.ReadAllText("sellers.txt");
                if (!string.IsNullOrEmpty(content))
                {
                    string[] s = File.ReadAllText("sellers.txt").Split(':');
                    salers.Add(s[0], $"{s[1]}:{s[2]}");
                }
            }


            
        }
        
        //button of registarion
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        //Registration on buyers
        //TextBox6=login TextBox7=password TextBox8=email
        private void button5_Click(object sender, EventArgs e)
        {
            //serching for empty fields
            if (!string.IsNullOrEmpty(textBox6.Text) || !string.IsNullOrEmpty(textBox7.Text) || !string.IsNullOrEmpty(textBox8.Text))
            {
                //checking for valid data
                if (IsValidLogin(textBox6.Text) == true && IsValidPassword(textBox5.Text) == true && IsValidEmail(textBox8.Text) == true)
                {
                    //adding new user to file
                    File.AppendAllText("users.txt", $"{textBox6.Text}:{textBox5.Text}:{textBox7.Text}\n");
                    users.Add(textBox6.Text, $"{textBox5.Text}:{textBox7.Text}");
                    MessageBox.Show("Вы успешно зарегистрировались");
                    textBox6.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox5.Clear();
                   
                    panel1.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Visible= true;
        }

        //selers
        //TextBox12=login TextBox11=password TextBox9=email
        private void button6_Click(object sender, EventArgs e)
        {

            //serching for empty fields
            if (!string.IsNullOrEmpty(textBox12.Text) || !string.IsNullOrEmpty(textBox11.Text) || !string.IsNullOrEmpty(textBox10.Text)|| !string.IsNullOrEmpty(textBox9.Text))
            {
                //checking for valid data
                if (IsValidLogin(textBox12.Text) == true && IsValidPassword(textBox11.Text) == true && IsValidEmail(textBox10.Text) == true)
                {
                    //adding new seller to file
                    File.AppendAllText("sellers.txt", $"{textBox12.Text}:{textBox11.Text}:{textBox9.Text}\n");
                    salers.Add(textBox12.Text, $"{textBox11.Text}:{textBox9.Text}");
                    MessageBox.Show("Вы успешно зарегистрировались");
                    textBox12.Clear();
                    textBox11.Clear();
                    textBox10.Clear();
                    textBox9.Clear();
                    panel2.Visible = false;
                }
                else
                {
                    MessageBox.Show("Проверьте введенные данные");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
            
            
        }

        //sound stop if u close window
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _soundPlayer.Stop();
        }

        //methods checking for data entry(login/password/email)
        public static bool IsValidLogin(string login)
        {
            // Длина логина не должна превышать 50 символов
            if (login.Length > 50)
                return false;

            // Логин должен содержать только буквы и цифры
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9]+$"))
                return false;
            // Логин не должен содержать пробелы
            if (login.Contains(" "))
                return false;

            return true;
        }

        public static bool IsValidPassword(string password)
        {
            // Пароль должен быть длиной не менее 8 символов
            if (password.Length < 8)
                return false;

            // Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру
            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUppercase = true;
                else if (char.IsLower(c))
                    hasLowercase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
            }
            if (!hasUppercase || !hasLowercase || !hasDigit)
                return false;

            // Пароль не должен содержать пробелы
            if (password.Contains(" "))
                return false;

            return true;
        }

        public static bool IsValidEmail(string email)
        {
            // Email не должен содержать пробелы
            if (email.Contains(" "))
                return false;

            // Проверка на соответствие стандартному формату email-адреса
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        
        //button log in on seller
        //textBox4=login textBox3=password
        private void button4_Click(object sender, EventArgs e)
        {
            var matchingEntry = (from entry in salers
                                 where entry.Key == textBox4.Text.ToString()
                                 select entry).FirstOrDefault();
            //entrance as admin
            if (textBox4.Text == "admin" && textBox3.Text == "admin")
            {
                Sellers sellers = new Sellers();
                sellers.ShowDialog();
                
            }
            //entrance as seller
            else
            {
                //checking for empty fields
                if (string.IsNullOrEmpty(textBox4.Text.ToString()) || string.IsNullOrEmpty(textBox3.Text.ToString())) MessageBox.Show("Заполните все поля");
                else
                {
                    string[] s = matchingEntry.Value.Split(';');
                    string pass = s[0];
                    ValueToPass = s[1];
                    //checking for valid data on file
                    if (matchingEntry.Equals(default(KeyValuePair<string, string>)))
                    {
                        MessageBox.Show("Данный пользователь не существует");
                    }
                    //checking for valid data
                       else if (textBox4.Text == matchingEntry.Key && textBox3.Text == pass)
                    {
                        textBox4.Text = "";
                        textBox3.Text = "";
                        Sellers sellers = new Sellers();
                        //open seller window
                        sellers.ShowDialog();   
                        
                    }
                    else
                    {
                        MessageBox.Show("Логин или пароль были введены не праивльно");
                    }

                }
            }
            
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        { 
        }

        //button log in on buyer
        //textBox1=login textBox2=password
        private void button1_Click(object sender, EventArgs e)
        {
            var matchingEntry = (from entry in users
                                 where entry.Key == textBox1.Text.ToString()
                                 select entry).FirstOrDefault();
            //entrance as admin
            if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                Buyers buyers = new Buyers();
                buyers.ShowDialog();
                
            }
            //entrance as buyer
            else
            {
                //checking for empty fields
                if (string.IsNullOrEmpty(textBox1.Text.ToString()) || string.IsNullOrEmpty(textBox2.Text.ToString())) MessageBox.Show("Заполните все поля");
                else
                {
                    
                    string[] s = matchingEntry.Value.Split(':');
                    string pass = s[0];
                    ValueToPass = s[1];


                    //checking for valid data on file
                    if (matchingEntry.Equals(default(KeyValuePair<string, string>)))
                    {
                        MessageBox.Show("Данный пользователь не существует");
                    }
                    //checking for valid data
                    else if (textBox1.Text == matchingEntry.Key && textBox2.Text == pass)//text.box2=pass
                    {
                        textBox1.Text = "";
                        textBox2.Text = "";
                        Buyers buyers = new Buyers();
                        //open buyer window
                        buyers.ShowDialog();
                        
                    }
                    else
                    {
                        MessageBox.Show("Логин или пароль были введены не праивльно");
                    }
                }
            }

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else if(textBox2.UseSystemPasswordChar==false)
            {
                textBox2.UseSystemPasswordChar=true;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (textBox5.UseSystemPasswordChar == true)
            {
                textBox5.UseSystemPasswordChar = false;
            }
            else if (textBox5.UseSystemPasswordChar == false)
            {
                textBox5.UseSystemPasswordChar = true;
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (textBox3.UseSystemPasswordChar == true)
            {
                textBox3.UseSystemPasswordChar = false;
            }
            else if (textBox3.UseSystemPasswordChar == false)
            {
                textBox3.UseSystemPasswordChar = true;
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (textBox11.UseSystemPasswordChar == true)
            {
                textBox11.UseSystemPasswordChar = false;
            }
            else if (textBox11.UseSystemPasswordChar == false)
            {
                textBox11.UseSystemPasswordChar = true;
            }
        }
    }
}
