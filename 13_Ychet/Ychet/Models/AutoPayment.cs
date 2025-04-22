using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ychet.Models
{
    public class AutoPayment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public int FrequencyDays { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
