using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 記帳本
{
    internal static class FormExtension
    {
        static System.Threading.Timer timer;
        static Action action;
        static Form form;

        public static void DebounceTime(this Form form, Action action, int Delay)
        {
            FormExtension.form = form;
            FormExtension.action = action;

            if (timer != null)
            {
                timer.Change(500, -1);
            }
            else
            {
                timer = new System.Threading.Timer(Callback, null, 500, -1);
            }
        }

        private static void Callback(object state)
        {
            form.Invoke(action);
        }
    }
}
