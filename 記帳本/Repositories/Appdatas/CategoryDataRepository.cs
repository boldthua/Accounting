using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class CategoryDataRepository : ICategoryDataRepository
    {
        public string Name = "類別";
        public string[] Catagory = { "食", "衣", "住", "行", "育", "樂" };
        public List<string> GetCategories()
        {
            return Catagory.ToList();
        }
    }
}
