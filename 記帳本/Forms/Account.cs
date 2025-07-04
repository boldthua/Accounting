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
    }
}
