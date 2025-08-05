using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal interface ICategoryRepository
    {
        List<string> GetCategories();

        List<string> GetSubcategories(string category);

        List<string> GetRecipients();
    }
}
