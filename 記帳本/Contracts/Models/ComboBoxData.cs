using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Appdatas;

namespace 記帳本.Contracts.Models
{
    public class ComboBoxData
    {
        public List<string> category { get; set; }
        public List<string> item { get; set; }
        public List<string> recipient { get; set; }

        public ComboBoxData(List<string> category, List<string> item, List<string> recipient)
        {
            this.category = category;
            this.item = item;
            this.recipient = recipient;
        }

    }
}