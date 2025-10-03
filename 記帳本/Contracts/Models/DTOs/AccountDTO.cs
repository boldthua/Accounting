using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Attributes;

namespace 記帳本.Contracts.Models.DTOs
{
    public class AccountDTO
    {
        public string Time { get; set; }
        public string Money { get; set; }
        [Record("類別")]
        public string Catagory { get; set; }
        [Record("細項")]
        public string Item { get; set; }
        [Record("對象")]
        public string Recipient { get; set; }
        [Record("支付方式")]
        public string Payment { get; set; }

    }
}
