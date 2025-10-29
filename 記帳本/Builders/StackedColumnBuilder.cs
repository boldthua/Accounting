using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Repositories.Models;

namespace 記帳本.Builders
{
    internal class StackedColumnBuilder : ChartBuilder
    {
        private Dictionary<string, (List<string>, List<int>)> StackedChartDates = new Dictionary<string, (List<string>, List<int>)>();
        public StackedColumnBuilder(int width, int height) : base(width, height)
        {
        }

        public override ChartBuilder GroupByDates(List<string> groupByList)
        {
            var props = typeof(RecordModel)
            .GetProperties()
            .ToList();
            var groupByProps = props.Where(x => groupByList.Contains(x.Name)).ToList(); // payment,receipent

            StackedChartDates = _filterDates.GroupBy(x =>
            {
                return string.Join(",", groupByProps.Select(prop => prop.GetValue(x).ToString()));
            }).Select(x =>
            {
                // 將已分組的資料再次以日期分組，讓同日期的資料在同一組內
                var byDayData = x.GroupBy(r => r.Time)
                             .ToDictionary(r => r.Key, r => r.ToList());

                var datesList = new List<string>();
                var moneyList = new List<int>();

                foreach (string dateKey in _dates)
                {
                    string strD = string.Join("-", dateKey.Split('-').Skip(1));
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
                return new
                {
                    GroupByName = x.Key,
                    dates = datesList,
                    Money = moneyList
                };
            }).ToDictionary(x => x.GroupByName, x => (x.dates, x.Money));

            return this;
        }
        public override ChartBuilder SetSeries()
        {
            foreach (var groupData in StackedChartDates)
            {
                var series = new Series($"{groupData.Key}")
                {
                    ChartType = SeriesChartType.StackedColumn,
                    XValueType = ChartValueType.String,
                    IsValueShownAsLabel = true,
                    Label = "#VAL元",
                    ChartArea = "main",
                    Font = new Font("Arial", 6f)
                };
                series.Points.DataBindXY(groupData.Value.Item1, groupData.Value.Item2);
                foreach (var p in series.Points)
                {
                    // 0 就視為空點
                    if (p.YValues.Length > 0 && p.YValues[0] == 0)
                    {
                        p.IsEmpty = true;               // 不會畫出來，也不會有標籤
                        p.Label = string.Empty;         // 保險起見清空
                        p.IsValueShownAsLabel = false;
                    }
                }
                _chart.Series.Add(series);
            }
            return this;
        }

        public override ChartBuilder SetAxisXY()
        {
            var area = _chart.ChartAreas.FirstOrDefault(x => x.Name == "main");
            area.AxisX.Interval = 1;
            area.AxisX.Title = "日期";
            area.AxisX.TitleForeColor = Color.Blue;
            area.AxisX.TitleFont = new Font("微軟正黑體", 12f);
            area.AxisX.IsMarginVisible = true;          // 左右留白
            area.AxisX.MajorGrid.Enabled = false;       // 讓畫面更乾淨

            area.AxisY.Title = "金額(元)";
            area.AxisY.TitleForeColor = Color.Blue;
            area.AxisY.TitleFont = new Font("微軟正黑體", 12f);
            return this;
        }
    }
}
