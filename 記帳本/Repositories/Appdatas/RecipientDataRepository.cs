using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class RecipientDataRepository : IRecipinetDataRepository
    {
        public string Name = "對象";
        public string[] Recipients = { "家人", "同事", "朋友", "其它" };

        public List<string> GetRecipinets()
        {
            return Recipients.ToList();
        }

    }
}
