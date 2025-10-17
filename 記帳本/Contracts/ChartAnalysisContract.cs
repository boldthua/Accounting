using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class ChartAnalysisContract

    {
        public interface IChartAnalysisView // 應該只有給一包資料
        {
            // 顯示ExpenseDTO
            void RenderDatas(List<AccountAnalyzeDTO> records);
            void PopulateMainCheckBox(AllItemData data);
        }

        public interface IChartAnalysisPresenter
        {
            // 拿ExpenseDTO
            void GetRecord(DateTime start, DateTime end, List<string> groupByList, Dictionary<string, List<string>> conditions);
            void GetAppDatas();
        }
    }
}
