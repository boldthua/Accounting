using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Attributes
{
    public class RecordAttribute : Attribute
    {
        public string _displayName;
        public RecordAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}
