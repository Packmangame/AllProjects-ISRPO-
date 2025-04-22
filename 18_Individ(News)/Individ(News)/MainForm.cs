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
    public partial class MainForm : Form
    {
        // Список новостей (источник, заголовок, ссылка)
        private List<NewsItem> newsFeed = new List<NewsItem>
        {
            new NewsItem("Технологии", "Новые разработки в AI", "https://giddata.timepad.ru/event/3287556/?utm_source=yandex&utm_medium=cpc&utm_campaign=poisk_tematika&utm_content=1&utm_term=программирование%20искусственного%20интеллекта&yclid=11709090287795306495"),
            new NewsItem("Спорт", "Финал чемпионата мира", "https://example.com/sport"),
            new NewsItem("Политика", "Встреча лидеров стран", "https://example.com/politics"),
            new NewsItem("Технологии", "Выпуск нового смартфона", "https://example.com/smartphone"),
            new NewsItem("Наука", "Открытие в области космоса", "https://example.com/science")
        };

        // Сохраненные статьи
        private List<string> savedArticles = new List<string>();

        // Интересы пользователя
        private string userInterest = "Технологии";

        // Поле для FlowLayoutPanel
        private FlowLayoutPanel newsPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Агрегатор новостей";
            this.Size = new Size(800, 600);

            // Создание панели для фильтрации интересов
            Panel interestPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray
            };

            Label interestLabel = new Label
            {
                Text = "Выберите интерес:",
                AutoSize = true,
                Location = new Point(10, 15)
            };

            ComboBox interestComboBox = new ComboBox
            {
                Location = new Point(120, 10),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            interestComboBox.Items.AddRange(new string[] { "Технологии", "Спорт", "Политика", "Наука" });
            interestComboBox.SelectedItem = userInterest;
            interestComboBox.SelectedIndexChanged += (s, e) =>
            {
                userInterest = interestComboBox.SelectedItem.ToString();
                UpdateNewsFeedByInterest();
            };

            // Кнопка "Показать все новости"
            Button showAllButton = new Button
            {
                Text = "Показать все новости",
                Location = new Point(300, 10),
                Width = 150
            };
            showAllButton.Click += (s, e) => ShowAllNews();

            interestPanel.Controls.Add(interestLabel);
            interestPanel.Controls.Add(interestComboBox);
            interestPanel.Controls.Add(showAllButton);

            // Создание панели для новостей
            newsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown
            };

            // Создание кнопки для сохранения статьи
            Button saveButton = new Button
            {
                Text = "Сохранить выбранную статью",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            saveButton.Click += (s, e) =>
            {
                MessageBox.Show("Для сохранения статьи выберите её из списка.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Создание кнопки для просмотра сохраненных статей
            Button viewSavedButton = new Button
            {
                Text = "Просмотреть сохраненные статьи",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            viewSavedButton.Click += (s, e) =>
            {
                SavedArticlesForm savedForm = new SavedArticlesForm(savedArticles);
                savedForm.ShowDialog();
            };

            // Инициализация новостной ленты
            UpdateNewsFeedByInterest();

            // Добавление элементов на форму
            this.Controls.Add(newsPanel);
            this.Controls.Add(saveButton);
            this.Controls.Add(viewSavedButton);
            this.Controls.Add(interestPanel);
        }

        // Метод обновления новостей по интересам
        private void UpdateNewsFeedByInterest()
        {
            newsPanel.Controls.Clear();
            foreach (var news in newsFeed)
            {
                if (news.Category == userInterest)
                {
                    AddNewsToPanel(news);
                }
            }
        }

        // Метод вывода всех новостей
        private void ShowAllNews()
        {
            newsPanel.Controls.Clear();
            foreach (var news in newsFeed)
            {
                AddNewsToPanel(news);
            }
        }

        // Вспомогательный метод для добавления новости в панель
        private void AddNewsToPanel(NewsItem news)
        {
            LinkLabel linkLabel = new LinkLabel
            {
                Text = $"{news.Title}",
                AutoSize = true,
                Tag = news.Link // Сохраняем ссылку в свойстве Tag
            };
            linkLabel.LinkClicked += (sender, args) =>
            {
                // Открываем ссылку в браузере
                System.Diagnostics.Process.Start(linkLabel.Tag.ToString());
            };
            newsPanel.Controls.Add(linkLabel);
        }

    }
  
}