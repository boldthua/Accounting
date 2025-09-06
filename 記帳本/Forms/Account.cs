using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;

namespace 記帳本
{
    [DisplayName("帳戶分析")]
    [Order(2)]
    public partial class Account : Form
    {

        public Account()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlQuery = "SELECT * FROM user_selections WHERE 1=1";
            bool anyZone = false;
            bool anyPrice = false;
            bool anyType = false;
            bool anyArea = false;
            bool anyFloor = false;
            bool anyEqip = false;
            foreach(CheckBox checkbox in this.Controls)
            {
                if (checkbox.Tag =="Zone" )
                {
                    if (anyZone) 
                        continue;
                    if (checkbox.Text == "不限" )
                }
            }
        }

    }
}
