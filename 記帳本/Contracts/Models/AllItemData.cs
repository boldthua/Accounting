using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Appdatas;

namespace 記帳本.Contracts.Models
{
    public class AllItemData
    {
        public List<string> catagory { get; set; }
        public Dictionary<string, List<string>> items { get; set; }
        public List<string> recipient { get; set; }

        public AllItemData(List<string> category, Dictionary<string, List<string>> items, List<string> recipient)
        {
            this.catagory = category;
            this.items = items;
            this.recipient = recipient;
        }

    }
}