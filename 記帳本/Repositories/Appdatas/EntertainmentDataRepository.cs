using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class EntertainmentDataRepository : ISubCategoryDataRepository
    {
        public string Name = "樂";
        public string[] Items = { "旅行", "遊戲", "逛街" };

        public List<string> GetItems(string category)
        {
            return Items.ToList();
        }
    }
}
