using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class EducationDataRepository : ISubCategoryDataRepository
    {
        public string Name = "育";
        public string[] Items = { "學費", "書籍", "其它" };
        public List<string> GetItems(string category)
        {
            return Items.ToList();
        }
    }
}
