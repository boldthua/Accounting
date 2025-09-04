using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Contracts.Models.DTOs
{
    public class ExpenseDTO
    {
        public string Time { get; set; }
        public string Money { get; set; }
        public string Catagory { get; set; }
        public string Item { get; set; }
        public string Recipient { get; set; }

        public string Picture1 { get; set; }

        public string Picture2 { get; set; }

    }
}
