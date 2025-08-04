using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class FoodDataRepository : ISubCategoryDataRepository
    {
        public string Name = "食";
        public string[] Items = { "早餐", "中餐", "晚餐", "宵夜", "點心" };

        public List<string> GetItems(string subCategoryName)
        {
            return Items.ToList();
        }
    }
}
