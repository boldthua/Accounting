using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Attributes;

namespace 記帳本.Repositories.Models
{
    public class RecordModel //DAO(Data Access Object)
    {
        public string Time { get; set; }
        public string Money { get; set; }
        public string Catagory { get; set; }
        public string Item { get; set; }
        public string Recipient { get; set; }
        public string Payment { get; set; }

        public string Picture1 { get; set; }

        public string Picture2 { get; set; }

        public RecordModel(string str)
        {
            string[] strs = str.Split(new char[] { ',' });
            this.Time = strs[0];
            this.Money = strs[1];
            this.Catagory = strs[2];
            this.Item = strs[3];
            this.Recipient = strs[4];
            this.Payment = strs[5];
            this.Picture1 = strs[6];
            this.Picture2 = strs[7];
        }

        public RecordModel()
        {
        }
    }
}
