using CSVLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 記帳本
{
    internal class SingletonForm
    {
        private static Dictionary<string, Form> forms = new Dictionary<string, Form>(); // 用dictionary
        private static Form lastForm = null;

        public static Form GetFormInstance(string formName)
        {
            if (lastForm != null)
            {
                lastForm.Hide();
            }

            // 把點選的button存在dictionary內，然後再次點選時能叫出來
            // 1. 先檢查點選的button是否已存在
            // 2. 不存在的話需跳到button指定的form，然後將form貯存於dictionary中
            // 3. 存在的話直接將form show出來

            if (forms.ContainsKey(formName))
            {
                lastForm = forms[formName];
            }
            else
            {

                Type type = Type.GetType("記帳本." + formName);
                lastForm = (Form)Activator.CreateInstance(type);
                // 列出lastForm裡的NavBar內的button，名字和formName一樣的就disable
                FieldInfo fieldNavBar = lastForm.GetType().GetField("navBar1", BindingFlags.NonPublic | BindingFlags.Instance);
                NavBar navBar = fieldNavBar.GetValue(lastForm) as NavBar;
                navBar.ButtonDisable(formName);

                // 點餐機和小算盤 為何棄用switch > createinstance  反射 解隅
                //switch (formName)
                //{
                //    case "記帳本":
                //        lastForm = new Accounting();
                //        break;
                //    case "帳戶":
                //        lastForm = new Account();
                //        break;
                //    case "記一筆":
                //        lastForm = new AddARecord();
                //        break;
                //    case "圖表分析":
                //        lastForm = new ChartAnalysis();
                //        break;
                //}

                forms.Add(formName, lastForm);
            }
            return lastForm;
        }

    }
}
