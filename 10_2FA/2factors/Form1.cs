using System;
using System.Windows.Forms;
using MimeKit;
using System.Text.RegularExpressions;
using System.Net;
using Twilio.Types;

namespace _2factors
{

    public partial class Form1 : Form
    {
        string pass, log, data, code;
        private static readonly string smsApiId = "9D4BF4A1-0F53-2326-2A29-E46F29CFADEC"; // Получите API ID на сайте SMS.ru

        // Настройки для отправки email через SMTP
        private static readonly string smtpServer = "smtp.gmail.com"; // Например, smtp.gmail.com
        private static readonly int smtpPort = 587;
        private static readonly string emailFrom = "romandevg222@gmail.com";
        private static readonly string emailPassword = "ikK-fuy-BD7-u4e2";


       
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                label1.Visible = true;
                label1.Text = "Email";
                textBox3.Visible = true;
                var emailMessage = new MimeMessage();
                
            }
            else
            {
                label1.Visible = true;
                label1.Text = "Phone Number";
                textBox3.Visible = true;
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (code == textBox4.Text)
            {
                MessageBox.Show("Успешно","Secses");
                panel2.Visible = false;
                panel1.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(textBox6.Text) || !string.IsNullOrEmpty(textBox5.Text) ||!string.IsNullOrEmpty(textBox3.Text))
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    if (IsValidEmail(textBox3.Text) == true)
                    {
                        pass = textBox5.Text;
                        log = textBox6.Text;
                        data = textBox3.Text;
                        panel2.Visible = true;

                        var emailMessage = new MimeMessage();
                        emailMessage.From.Add(new MailboxAddress("Служба поддержки", emailFrom));
                        emailMessage.To.Add(new MailboxAddress("",data));
                        emailMessage.Subject = "Код подтверждения";
                        emailMessage.Body = new TextPart("plain")
                        {
                            Text = $"Ваш код подтверждения: {code}"
                        };

                        MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                        try
                        {
                            client.Connect(smtpServer, smtpPort, false);
                            client.Authenticate(emailFrom, emailPassword);
                            client.Send(emailMessage);
                            client.Disconnect(true);

                            Console.WriteLine($"Код отправлен на почту: {data}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при отправке email: {ex.Message}");
                        }
                    }
                }
                else
                {
                    if (textBox3.Text.Length>=10 && textBox3.Text.Length<=11)
                    {

                        pass = textBox5.Text;
                        log = textBox6.Text;
                        data = textBox3.Text;
                        panel2.Visible = true;

                        string url = $"https://sms.ru/sms/send?api_id={smsApiId}&to={data}&msg={Uri.EscapeDataString($"Ваш код подтверждения: {code}")}&json=1";

                        using (var client = new WebClient())
                        {
                            try
                            {
                                string response = client.DownloadString(url);
                                Console.WriteLine($"Ответ от SMS.ru: {response}");
                                Console.WriteLine($"Код отправлен на номер: {data}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при отправке SMS: {ex.Message}");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
            
        }
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }
        
        public string GenerateCode()
        {
            Random random = new Random();
            code= random.Next(100000, 999999).ToString(); // 6-значный код
            return code;
        }
    }
}

