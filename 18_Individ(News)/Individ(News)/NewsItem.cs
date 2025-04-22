using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individ_News_
{
    internal class NewsItem
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }

        public NewsItem(string category, string title, string link)
        {
            Category = category;
            Title = title;
            Link = link;
        }
    }
}
