using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;

namespace 記帳本
{
    [DisplayName("圖表分析")]
    [Order(4)]
    public partial class ChartAnalysis : Form
    {

        public ChartAnalysis()
        {
            InitializeComponent();
        }
    }
}
