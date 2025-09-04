using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Models;

namespace 記帳本.Repositories
{
    //IRecordRepository recordRepository.GetRecords(date)
    //IRecordRepository recordRepository.GetRecords(start,end)
    //RecordDTO => Record
    //ExpenseModel <= RecordDTO <=Record
    public interface IRecordRepository
    {
        // 丟東西給我存
        bool AddRecord(RecordModel model);
        // 跟我要一包東西
        List<RecordModel> GetRecords(DateTime date);

        List<RecordModel> GetRecords(DateTime start, DateTime end);

        void DeleteRecord(RecordModel record);

        void UpdateRecord(RecordModel record);
    }
}
