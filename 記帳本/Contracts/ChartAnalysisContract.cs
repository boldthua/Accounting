using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class ChartAnalysisContract

    {
        public interface IChartAnalysisView // 應該只有給一包資料
        {
            // 顯示ExpenseDTO
            void RenderDatas(Chart chart);
            void PopulateMainCheckBox(AllItemData data);
        }

        public interface IChartAnalysisPresenter
        {
            // 拿ExpenseDTO
            void GetRecord(KeyValuePair<DateTime, DateTime> startToEnd, string chartType, List<string> groupByList, Dictionary<string, List<string>> conditions, int width, int height);
            void GetAppDatas();
        }
    }
}
