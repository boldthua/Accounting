using CSVLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public bool DeleteRecord(RecordModel record)
        {
            // 先找出來
            // 如果檔案空了，記得要把資料夾一併刪除
            DateTime date = DateTime.Parse(record.Time);
            string fileLocation = Path.Combine(ConfigurationManager.AppSettings["DocumentPath"], record.Time);

            //撇除自己以外的所有資料
            List<RecordModel> records = GetRecords(date).Where(x => x.Picture1 != record.Picture1).ToList();

            if (records.Count == 0)
            {
                Directory.Delete(fileLocation);
            }
            else
            {
                string bigPicPath1 = record.Picture1.Replace("micro", "");
                string bigPicPath2 = record.Picture2.Replace("micro", "");
                File.Delete(bigPicPath1);
                File.Delete(bigPicPath2);
                File.Delete(record.Picture1);
                File.Delete(record.Picture2);
            }

            CSVHelper.Write<RecordModel>(fileLocation, records, true);
            return false;
        }

        public bool UpdateRecord(RecordModel record)
        {
            DateTime date = DateTime.Parse(record.Time);
            string fileLocation = Path.Combine(ConfigurationManager.AppSettings["DocumentPath"], record.Time);

            //撇除自己以外的所有資料
            List<RecordModel> datas = GetRecords(date);
            RecordModel data = datas.FirstOrDefault(x => x.Picture1 == record.Picture1);
            data = record;
            CSVHelper.Write(fileLocation, datas, false);


            string currentDate = "";
            foreach (RecordModel record in records)
            {
                string filePath = Path.Combine(path, record.Time, "record.csv");
                if (record.Time != currentDate) // 不同天表示是第一筆
                {
                    CSVHelper.Write<RecordModel>(filePath, record, false);
                    currentDate = record.Time;
                }
                else // 表示不是當天第一筆
                {
                    CSVHelper.Write<RecordModel>(filePath, record, true);
                }
            }
            return true;
        }
    }
}
