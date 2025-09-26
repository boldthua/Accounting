using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class AccoutingContract
    {
        public interface IAccountingView // 應該只有給一包資料
        {
            // 顯示ExpenseDTO
            void RenderDatas(List<ExpenseDTO> records);
            // 顯示完整圖片
            void PopulateComboBox(CategoryData data);
            void ReceiveItems(List<string> items);
        }

        public interface IAccountingPresenter // 應該只有Read Update Delete 
        {
            // 拿ExpenseDTO
            void GetRecord(DateTime start, DateTime end);

            // 更新資料
            void UpdateRecord(ExpenseDTO records);

            // 刪除資料
            void DeleteRecord(ExpenseDTO recordToBeDeleted);

            void GetAppDatas();
            void GetSubcategories(string category);

        }
    }
}
