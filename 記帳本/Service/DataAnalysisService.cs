using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Repositories;
using 記帳本.Repositories.Models;

namespace 記帳本.Service
{
    public class DataAnalysisService
    {
        IRecordRepository recordRepository;
        public DataAnalysisService(IRecordRepository recordRepository)
        {
            this.recordRepository = recordRepository;
        }
        public List<AccountAnalyzeDTO> GetAccountAnalyzeDatas(DateTime start, DateTime end, List<string> groupByList, Dictionary<string, List<string>> conditions)
        {
            var props = typeof(RecordModel)
                        .GetProperties()
                        .ToList();
            var filterProps = props.Where(x => conditions.ContainsKey(x.Name)).ToList();
            List<RecordModel> datas = recordRepository.GetRecords(start, end);
            var allValues = conditions.Values.SelectMany(x => x).ToList();
            var filter = datas.Where(x => filterProps.All(y => allValues.Contains(y.GetValue(x).ToString()))).ToList();
            var groupByProps = props.Where(x => groupByList.Contains(x.Name)).ToList(); // payment,receipent

            var returnData = filter.GroupBy(x =>
            {
                return string.Join(",", groupByProps.Select(prop => prop.GetValue(x).ToString()));
            }).Select(x =>
            {
                // 將已分組的資料再次以日期分組，讓同日期的資料在同一組內
                var byDayData = x.GroupBy(r => r.Time)
                             .ToDictionary(r => r.Key, r => r.ToList());

                var datesList = new List<string>();
                var moneyList = new List<int>();

                for (var d = start; d <= end; d = d.AddDays(1))
                {
                    string dateKey = d.ToString("yyyy-MM-dd");
                    string strD = d.ToString("MM-dd");
                    if (byDayData.TryGetValue(dateKey, out var items))
                    {
                        // 當天有 N 筆 -> 重複加入 N 個日期字串＋各自金額
                        foreach (var r in items)
                        {
                            datesList.Add(strD);
                            moneyList.Add(int.TryParse(r.Money, out var m) ? m : 0);
                        }
                    }
                    else
                    {
                        // 當天沒有 -> 仍加一筆日期＋金額 0
                        datesList.Add(strD);
                        moneyList.Add(0);
                    }
                }
                return new AccountAnalyzeDTO()
                {
                    GroupByName = x.Key,
                    dates = datesList,
                    ToTalMoney = x.Sum(y => int.Parse(y.Money)),
                    Money = moneyList
                };
            }).ToList();
            return returnData;
        }
    }
}
