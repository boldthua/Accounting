using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Attributes
{
    public class OrderAttribute : Attribute
    {
        private int _order = 0;
        public int Order => _order;

        public OrderAttribute(int order)
        {
            this._order = order;
        }

        // 想一想
        // 為什麼有attribute了還需要欄位，兩者的差別？
        // 
    }
}
