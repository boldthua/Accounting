using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class AccoutContract

    {
        public interface IAccountView // 應該只有給一包資料
        {
            // 顯示ExpenseDTO
            void RenderDatas(List<AccountDTO> records);
            void PopulateMainCheckBox(AllItemData data);
        }

        public interface IAccountPresenter
        {
            // 拿ExpenseDTO
            void GetRecord(DateTime start, DateTime end, Dictionary<string, List<string>> conditions);
            void GetAppDatas();
        }
    }
}
