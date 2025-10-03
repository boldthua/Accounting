using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Appdatas;

namespace 記帳本.Contracts.Models
{
    public class CategoryData
    {
        public List<string> catagory { get; set; }
        public List<string> item { get; set; }
        public List<string> recipient { get; set; }
        public List<string> payment { get; set; }

        public CategoryData(List<string> category, List<string> item, List<string> recipient, List<string> payment)
        {
            this.catagory = category;
            this.item = item;
            this.recipient = recipient;
            this.payment = payment;
        }

    }
}