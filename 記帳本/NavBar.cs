using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;

namespace 記帳本
{
    public partial class NavBar : UserControl
    {
        // 多一個變數來存現在的form
        // 欄位加static 具有唯一性
        public List<Button> buttons = new List<Button>();

        public NavBar()
        {
            InitializeComponent();
        }

        private void NavBarButtom_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            //button.Enabled = false;
            Form form = SingletonForm.GetFormInstance(button.Tag.ToString());


            form.Show();
        }

        public void ButtonDisable(string buttonName)
        {
            foreach (Button btn in buttons)
            {
                if (btn.Tag.ToString() == buttonName)
                {
                    btn.Enabled = false;
                    return;
                }
            }
        }

        private void NavBar_SizeChanged(object sender, EventArgs e)
        {
            int buttonsCount = flowLayoutPanel1.Controls.Count;
            var buttons = flowLayoutPanel1.Controls.OfType<Button>();

            foreach (var button in buttons)
            {
                button.Width = flowLayoutPanel1.Width / buttonsCount - button.Margin.Horizontal;
            }
        }

        private void NavBar_Load(object sender, EventArgs e)
        {
            // NavBar 按鍵的動態生成
            var types = Assembly.GetExecutingAssembly().DefinedTypes;
            // 找到這個專案內的所有類別
            var NavBarForms = types.Where(x => x.BaseType == typeof(Form) && x.GetCustomAttribute<OrderAttribute>() != null).ToList();
            // 撈出是Form的類別
            var orderedForms = NavBarForms.OrderBy(x => x.GetCustomAttribute<OrderAttribute>().Order).ToList(); // ???? 
            // 用Attribute的order來排序

            NavBar formNavbar = this.Parent.Controls.OfType<NavBar>().First();

            // 取得flowoutpanel的size
            int fPanelWidth = formNavbar.Width;
            int fPanelHeight = formNavbar.Height;
            //string formName = usingForm.GetType().Name;
            this.flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < orderedForms.Count; i++)
            {
                Button btn = new Button();
                btn.Text = orderedForms[i].GetCustomAttribute<DisplayNameAttribute>().DisplayName;
                btn.Tag = orderedForms[i].Name;
                btn.Width = (fPanelWidth / NavBarForms.Count) - btn.Margin.Horizontal;
                btn.Height = fPanelHeight - btn.Margin.Vertical;
                btn.Click += NavBarButtom_Click;

                //if (btn.Tag.ToString() == formName)
                //{
                //    btn.Enabled = false;
                //}
                this.flowLayoutPanel1.Controls.Add(btn);
                buttons.Add(btn);
            }

            string formName = this.Parent.GetType().Name;
            ButtonDisable(formName);

        }
    }
}

// 跟記憶體有關

// 動態生成
//0501 attribute屬性？標籤？ 藏tag →讓類別更容易排序或其它功能來辦別
// 不要同一按鍵重覆按
//

// 0506
// 用反射做到 空專案 抓到欄位和值 創建物件 還有setvalue還有 getmethod 

// 0513
// 視窗拖拉 延展及放大縮小 生命週期????

// 0515
// 先在各form裡的navbar的屬性中的Dock改為buttom 置底後可隨視窗延展
// 到NavBar的屬性中找到 SizeChanged 就會出現SizedChanged的事件
// 在建構元中初始化後動能生成按鈕仍會被SizeChanged影響
// 所以按鍵在 Load事件中生成才能確保是最後一步form的size是正確時才生成
// 接著在SizeChanged中調整各按鈕大小就好，每次size變更會觸發十幾次SizeChanged
// 若每次調整大小就要生成十幾次按鈕感覺不好
// 想 記一筆


