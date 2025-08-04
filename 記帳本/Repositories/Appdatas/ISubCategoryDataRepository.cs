using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories
{
    internal interface ISubCategoryDataRepository
    {
        // 每一class都有主category和子category

        List<string> GetItems(string subCategoryName); // 根據類別名取得子類別

    }
}
