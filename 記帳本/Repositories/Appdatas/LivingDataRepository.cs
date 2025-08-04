using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class LivingDataRepository : ISubCategoryDataRepository
    {
        public string Name = "住";
        public string[] Items = { "家電", "水電瓦斯網路等", "設備維修等" };
        public List<string> GetItems(string category)
        {
            return Items.ToList();
        }
    }
}
