using CSVLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Repositories.Models;

namespace 記帳本.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        string path { get; set; }
        public RecordRepository()
        {
            path = ConfigurationManager.AppSettings["DocumentPath"];
        }
        public bool AddRecord(RecordModel model)
        {
            string saveDay = model.Time;
            string savePath = path + saveDay;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            CSVHelper.Write<RecordModel>(Path.Combine(savePath, "record.csv"), model, true);

            return true;
        }

        public List<RecordModel> GetRecords(DateTime date)
        {
            List<RecordModel> list = new List<RecordModel>();

            string documentName = date.ToString("yyyy-MM-dd");
            list.AddRange(CSVHelper.Read<RecordModel>(Path.Combine(path + documentName, "record.csv")));

            return list;
        }

        public List<RecordModel> GetRecords(DateTime start, DateTime end)
        {
            List<RecordModel> list = new List<RecordModel>();
            TimeSpan diff = end - start;
            int dayLasting = diff.Days; // 5

            for (int i = 0; i <= dayLasting; i++)
            {
                string documentName = start.AddDays(i).ToString("yyyy-MM-dd");
                list.AddRange(CSVLibrary.CSVHelper.Read<RecordModel>(Path.Combine(path, documentName, "record.csv")));
            }
            return list;
        }

    }
}
