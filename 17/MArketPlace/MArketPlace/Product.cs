using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MArketPlace
{
    internal class Product
    {
        public string name;
        public double price;
        public int count;
        public string link;
        
        

        public Product(string name, double price, int count, string link)
        {
            this.name = name;
            this.price = price;
            this.count = count;
            this.link = link;
        }
    }
}
