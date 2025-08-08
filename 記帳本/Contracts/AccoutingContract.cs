using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class AccoutingContract
    {
        public interface IAccountingView // 應該只有給一包資料
        {
            // 顯示RecordDTO
            void DisplayRecord(List<RecordDTO> records);
            // 顯示完整圖片
        }

        public interface IAccountingPresenter // 應該只有Read Update Delete 
        {
            // 拿RecordDTO
            void GetRecord(DateTime start, DateTime end);
            // 拿完整圖片
            void GetFullPicture(string Path);
            // 刪除資料
            void DeleteRecord(List<RecordDTO> records, RecordDTO recordToBeDeleted);
        }
    }
}
