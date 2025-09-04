using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.AccountingMappings;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Repositories;
using 記帳本.Utility;

namespace 記帳本
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = SingletonForm.GetFormInstance("Accounting");
            Application.Run(form);
        }
    }
}
