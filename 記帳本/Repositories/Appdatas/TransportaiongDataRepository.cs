using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class TransportaiongDataRepository : ISubCategoryDataRepository
    {
        public string Name = "行";
        public string[] Items = { "公共設施通勤費", "油錢", "汽機車維修"};
        public List<string> GetItems(string category)
        {
            return Items.ToList();
        }
    }
}
