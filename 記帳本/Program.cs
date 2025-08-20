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
            ExpenseDTO expenseDTO = new ExpenseDTO();
            expenseDTO.Time = "2025-08-20";
            expenseDTO.Item = "dtoItem";
            expenseDTO.Catagory = "dtoCatagory";
            expenseDTO.Money = "1000";
            expenseDTO.Picture1 = "picPath1";
            expenseDTO.Picture2 = "picPath2";
            expenseDTO.Recipient = "dtoRecipient";
            expenseDTO.test = "test";


            var result = Mapper.Map<ExpenseDTO, ExpenseViewModel>(expenseDTO, x =>
            {
                x.ForMember(y => y.testtest, z => z.MapFrom(o => o.test));
            });


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = SingletonForm.GetFormInstance("Accounting");
            Application.Run(form);
        }
    }
}
