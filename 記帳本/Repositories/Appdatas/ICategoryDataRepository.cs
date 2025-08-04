using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal interface ICategoryDataRepository
    {
        List<string> GetCategories(); // 取得所有主類別
    }
}
