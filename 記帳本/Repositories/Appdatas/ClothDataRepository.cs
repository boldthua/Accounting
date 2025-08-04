using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class ClothDataRepository : ISubCategoryDataRepository
    {
        public string Name = "衣";
        public string[] Items = { "衣物" };
        public List<string> GetItems(string subCategoryName)
        {
            return Items.ToList();
        }
    }
}
