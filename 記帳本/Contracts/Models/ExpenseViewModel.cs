using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;

namespace 記帳本
{
    internal class ExpenseViewModel
    {
        [DisplayName("日期")]
        public string time { get; set; }
        [DisplayName("金額")]
        public string money { get; set; }
        [DisplayName("類別")]
        [ComboBoxColumn]
        public string catagory { get; set; }
        [DisplayName("項目")]
        [ComboBoxColumn]
        public string item { get; set; }
        [DisplayName("對象")]
        [ComboBoxColumn]
        public string recipient { get; set; }

        [DisplayName("證明1")]
        [ImageColumn]
        public string picture1 { get; set; }

        [DisplayName("證明2")]
        [ImageColumn]
        public string picture2 { get; set; }


        public ExpenseViewModel(string str)
        {
            string[] strs = str.Split(new char[] { ',' });
            this.time = strs[0];
            this.money = strs[1];
            this.catagory = strs[2];
            this.item = strs[3];
            this.recipient = strs[4];
            this.picture1 = strs[5];
            this.picture2 = strs[6];
        }

        public ExpenseViewModel()
        {
        }
    }
}
