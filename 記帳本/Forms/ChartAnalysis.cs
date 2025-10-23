using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Attributes;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Service;
using 記帳本.Utility;
using static 記帳本.Contracts.AccoutContract;
using static 記帳本.Contracts.ChartAnalysisContract;

namespace 記帳本
{
    [DisplayName("圖表分析")]
    [Order(4)]
    public partial class ChartAnalysis : Form, IChartAnalysisView
    {
        IChartAnalysisPresenter presenter;
        List<AccountAnalyzeDTO> records;
        List<FlowLayoutPanel> itemFlPanelList = new List<FlowLayoutPanel>();
        FlowLayoutPanel recipientPanel = new FlowLayoutPanel();
        new Dictionary<string, string> chartSorts = new Dictionary<string, string>
        {
            { "圓餅圖", "PieChart" },
            { "折線圖", "LineChart" },
            { "堆疊圖", "StackedColumn" }
        };

        // conditions 用來維護最終要送去給 Presenter分析的資料
        // key為 食/衣/住/行/育/樂/支付方式/對象  value為每一個類型中有勾選的項目
        Dictionary<string, List<string>> conditions = new Dictionary<string, List<string>>();

        // 用來記錄需要進行群組的分類有哪些 ex:根據吃飯(食)跟交通(行) 進行資料群組
        // 例如:想知道過去這兩個月花在吃飯跟交通的費用有多少? 但是要扣除掉幫家人/朋友請客的
        // 所以conditions裡面只會有: 食 (早午晚餐) 行(火車 高鐵票) 對象(自己)
        // groupbyList 會有 食,行 這兩筆，因為只看這兩個項目的金額總計 ["食", "行"]
        List<string> groupByList = new List<string>();
        public ChartAnalysis()
        {
            InitializeComponent();
            presenter = new ChartAnalysisPresenter(this);
            comboBox1.DataSource = chartSorts.Keys;
            comboBox1.SelectedIndex = 0;
            presenter.GetAppDatas();
        }

        public void RenderDatas(Chart chart)
        {
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanel2.Controls.Add(chart);
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.records == null)
                return;
            SetChart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DebounceTime(() =>
            {
                SetChart();
            }, 1000);
        }

        public void PopulateMainCheckBox(AllItemData data)
        {
            flowLayoutPanel1.GenerateCheckboxs(data, ConditionCheckedChange, GroupByCheckedChange);
        }
        public void ConditionCheckedChange(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string keyName = checkBox.Tag.ToString();

            if (checkBox.Checked)
            {
                if (conditions.TryGetValue(keyName, out List<string> conditionList) && !conditionList.Contains(checkBox.Text))
                {
                    conditionList.Add(checkBox.Text);
                }
                else
                {
                    conditions.Add(keyName, new List<string>() { checkBox.Text });
                }
            }
            else
            {
                conditions[keyName].Remove(checkBox.Text);
            }
        }
        public void GroupByCheckedChange(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            var propertyName = typeof(AccountDTO).GetProperties()
                                  .FirstOrDefault(x =>
                                  {
                                      var attr = x.GetCustomAttribute<RecordAttribute>();
                                      if (attr == null)
                                          return false;
                                      return attr._displayName == checkBox.Text;
                                  }).Name;
            if (checkBox.Checked)
            {
                groupByList.Add(propertyName);
            }
            else
            {
                groupByList.Remove(propertyName);
            }
        }

        /// 借放
        static string GetDisplayName<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            var member = expr.Body as MemberExpression
                         ?? (expr.Body as UnaryExpression)?.Operand as MemberExpression;
            if (member?.Member is PropertyInfo pi)
                return pi.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pi.Name;
            throw new ArgumentException("Expression must be a property access");
        }

        public Chart CreatePieChart(string chartType)
        {
            List<AccountAnalyzeDTO> pieChartRecords = records;
            Chart chart = new Chart()
            {
                Width = flowLayoutPanel2.Width,
                Height = flowLayoutPanel2.Height,
                BackColor = Color.Transparent
            };

            var title = new Title(chartType)
            {
                Font = new Font("微軟正黑體", 14f, FontStyle.Regular),
                Alignment = ContentAlignment.TopCenter
            };
            chart.Titles.Add(title);

            ChartArea area = new ChartArea("main")
            {
                Position = new ElementPosition(0, 10, 70, 80)
            };
            chart.ChartAreas.Add(area);

            var group = pieChartRecords.Select(x => x.GroupByName).ToList();
            var money = pieChartRecords.Select(x => (double)x.ToTalMoney).ToList();
            string groupNameLabel = GetDisplayName<AccountAnalyzeDTO, string>(x => x.GroupByName); // "分析類別"
            string moneyLabel = GetDisplayName<AccountAnalyzeDTO, int>(x => x.ToTalMoney);              // "類別總金額"

            var series = new Series("pie")
            {
                ChartType = SeriesChartType.Pie,
                XValueType = ChartValueType.String,
                IsValueShownAsLabel = true,
                Label = "#VALX #PERCENT{P0}",
                ChartArea = "main"
            };
            series.Points.DataBindXY(group, money);

            chart.Series.Clear();
            chart.Series.Add(series);

            Legend legend = new Legend("legend");
            legend.Title = "";
            legend.TitleForeColor = Color.Blue;
            legend.TitleFont = new Font("微軟正黑體", 10f, FontStyle.Regular);
            legend.Font = new Font("微軟正黑體", 8f, FontStyle.Regular);
            legend.ForeColor = Color.Blue;
            legend.BackColor = Color.Transparent;

            legend.IsDockedInsideChartArea = true;
            legend.DockedToChartArea = chart.ChartAreas[0].Name;

            legend.Position.Auto = false;
            legend.Position = new ElementPosition(70, 10, 30, 90);

            legend.Alignment = StringAlignment.Far;

            chart.Legends.Add(legend);

            return chart;
        }

        public Chart CreateLineChart(string chartType)
        {
            Chart chart = new Chart()
            {
                Width = flowLayoutPanel2.Width,
                Height = flowLayoutPanel2.Height,
                BackColor = Color.Transparent
            };

            var title = new Title(chartType)
            {
                Font = new Font("微軟正黑體", 14f, FontStyle.Regular),
                Alignment = ContentAlignment.TopCenter
            };
            chart.Titles.Add(title);

            ChartArea area = new ChartArea("main")
            {
                Position = new ElementPosition(0, 10, 70, 90)
            };
            chart.ChartAreas.Add(area);

            for (int i = 0; i < records.Count; i++)
            {
                var series = new Series()
                {
                    Name = $"{i}個月資料",
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.String,
                    IsValueShownAsLabel = true,
                    Label = "#VAL元",
                    ChartArea = "main",
                    Font = new Font(Font.FontFamily, 8f),
                    Tag = i.ToString(),
                };
                List<int> dayCosts = new List<int>();
                int daySpent = 0;



                series.Points.DataBindXY(records[i][0].dates, records[i].Select(x => new
                {
                    DaySum =
                                                                        }).ToList());
                chart.Series.Add(series);            // 要得到每一筆x裡的money[i]加總的list 
                var checkBox = this.Controls.OfType<CheckBox>()
                                            .FirstOrDefault(cb => Equals(cb.Tag, series.Tag));
                checkBox.Tag = series;
            }

            area.AxisX.Interval = 1;
            area.AxisX.Title = "日期";
            area.AxisX.TitleForeColor = Color.Blue;
            area.AxisX.TitleFont = new Font("微軟正黑體", 12f);
            area.AxisX.IsMarginVisible = true;          // 左右留白
            area.AxisX.MajorGrid.Enabled = false;       // 讓畫面更乾淨

            area.AxisY.Title = "金額(元)";
            area.AxisY.TitleForeColor = Color.Blue;
            area.AxisY.TitleFont = new Font("微軟正黑體", 12f);

            var legend = new Legend("legend")
            {
                TitleBackColor = Color.Transparent,
                BackColor = Color.Transparent,
                TitleForeColor = Color.DarkRed,
                TitleFont = new Font("微軟正黑體", 12f),
                Font = new Font("微軟正黑體", 8f),
                ForeColor = Color.BlueViolet,
                DockedToChartArea = "main",      // 指定跟哪個 ChartArea 對齊
                IsDockedInsideChartArea = false,  // ← 放在「外面」
                Docking = Docking.Bottom,         // 在 ChartArea 的下方
                Position = new ElementPosition(70, 10, 30, 90)
            };
            chart.Legends.Add(legend);
            return chart;
        }

        public Chart CreateStackedChart(string chartType)
        {
            List<AccountAnalyzeDTO> pieChartRecords = records;

            Chart chart = new Chart()
            {
                Width = flowLayoutPanel2.Width,
                Height = flowLayoutPanel2.Height,
                BackColor = Color.Transparent
            };

            var title = new Title(chartType)
            {
                Font = new Font("微軟正黑體", 14f, FontStyle.Regular),
                Alignment = ContentAlignment.TopCenter
            };
            chart.Titles.Add(title);

            ChartArea area = new ChartArea("main")
            {
                Position = new ElementPosition(0, 10, 70, 90)
            };
            chart.ChartAreas.Add(area);

            foreach (var groupData in pieChartRecords)
            {
                var series = new Series($"{groupData.GroupByName}")
                {
                    ChartType = SeriesChartType.StackedColumn,
                    XValueType = ChartValueType.String,
                    IsValueShownAsLabel = true,
                    Label = "#VAL元",
                    ChartArea = "main",
                    Font = new Font(Font.FontFamily, 6f)
                };
                series.Points.DataBindXY(groupData.dates, groupData.Money);
                chart.Series.Add(series);
            }
            #region 呈在單一point在X中的百分比
            // 只統計堆疊的系列
            var stacked = chart.Series.Where(s => s.ChartType == SeriesChartType.StackedColumn);

            // 1) 先累加每個 X 的總額
            var totals = new Dictionary<string, double>();
            foreach (var s in stacked)
                foreach (var p in s.Points)
                    totals[p.AxisLabel] = (totals.TryGetValue(p.AxisLabel, out var t) ? t : 0) + p.YValues[0];

            // 2) 逐點設定百分比標籤（同一 X 的佔比）
            foreach (var s in stacked)
                foreach (var p in s.Points)
                {
                    var total = totals[p.AxisLabel];
                    var pct = total == 0 ? 0 : p.YValues[0] / total;
                    if (p.YValues[0] == 0)
                    {
                        p.Label = null;
                        p.IsValueShownAsLabel = false;
                    }
                    else
                        p.Label = p.Label += $"({pct:P0})";
                }
            #endregion

            area.AxisX.Interval = 1;
            area.AxisX.Title = "日期";
            area.AxisX.TitleForeColor = Color.Blue;
            area.AxisX.TitleFont = new Font("微軟正黑體", 12f);
            area.AxisX.IsMarginVisible = true;          // 左右留白
            area.AxisX.MajorGrid.Enabled = false;       // 讓畫面更乾淨

            area.AxisY.Title = "金額(元)";
            area.AxisY.TitleForeColor = Color.Blue;
            area.AxisY.TitleFont = new Font("微軟正黑體", 12f);

            var legend = new Legend("legend")
            {
                TitleBackColor = Color.Transparent,
                BackColor = Color.Transparent,
                TitleForeColor = Color.DarkRed,
                TitleFont = new Font("微軟正黑體", 12f),
                Font = new Font("微軟正黑體", 8f),
                ForeColor = Color.BlueViolet,
                DockedToChartArea = "main",      // 指定跟哪個 ChartArea 對齊
                IsDockedInsideChartArea = false,  // ← 放在「外面」
                Docking = Docking.Bottom,         // 在 ChartArea 的下方
                Position = new ElementPosition(70, 10, 30, 90)
            };
            chart.Legends.Add(legend);

            return chart;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            var s = cb.Tag as Series;
            if (s == null) return;

            s.Enabled = cb.Checked;
        }

        private List<KeyValuePair<DateTime, DateTime>> Get3Periods(KeyValuePair<DateTime, DateTime> period)
        {
            var periods = new List<KeyValuePair<DateTime, DateTime>>() { period };

            periods.Add(OffsetPeriodByMonths(1, period));
            periods.Add(OffsetPeriodByMonths(2, period));
            return periods;
        }

        private KeyValuePair<DateTime, DateTime> OffsetPeriodByMonths(int monthsAgo, KeyValuePair<DateTime, DateTime> period)
        {
            DateTime startDay = period.Key;
            DateTime endDay = period.Value;

            DateTime ShiftStartByMonths_EndAligned(DateTime start, int m)
            {
                bool isFeb29 = (start.Month == 2 && start.Day == 29);

                // 28（含）以下走「同日」，但 2/29 不能走這條
                if (start.Day < 29 && !isFeb29)
                    return start.AddMonths(-m);

                // 其餘（含 2/29、30、31）走「月底對齊」：用距月底偏移量來對齊
                int srcLast = DateTime.DaysInMonth(start.Year, start.Month);
                int offsetFromEnd = srcLast - start.Day;

                var target = start.AddMonths(-m);
                int tgtLast = DateTime.DaysInMonth(target.Year, target.Month);
                int targetDay = Math.Max(1, tgtLast - offsetFromEnd);

                return new DateTime(target.Year, target.Month, targetDay, start.Hour, start.Minute, start.Second, start.Kind);
            }

            DateTime ShiftEndByMonths_Rule(DateTime d, int m)
            {
                bool isMonthEnd = d.Day == DateTime.DaysInMonth(d.Year, d.Month);
                var target = d.AddMonths(-m);

                if (isMonthEnd)
                {
                    // 尾日是月底 → 對齊到目標月的月底
                    int tgtLast = DateTime.DaysInMonth(target.Year, target.Month);
                    return new DateTime(target.Year, target.Month, tgtLast, d.Hour, d.Minute, d.Second, d.Kind);
                }
                else
                {
                    // 尾日不是月底 → 保留同日（AddMonths 會自動在小月夾到月底）
                    return target;
                }
            }

            var newStart = ShiftStartByMonths_EndAligned(startDay, monthsAgo);
            var newEnd = ShiftEndByMonths_Rule(endDay, monthsAgo);

            return new KeyValuePair<DateTime, DateTime>(newStart, newEnd);
        }

        private List<int> CountDailyMoney(List<AccountAnalyzeDTO> record)
        {
            record.GroupBy(x => x.dates)
                  .Select(x =>
                  {

                  }

                  );
        }

        private void SetChart()
        {
            int width = flowLayoutPanel2.Width;
            int height = flowLayoutPanel2.Height;
            var period = new KeyValuePair<DateTime, DateTime>(dateTimePicker1.Value, dateTimePicker2.Value);
            chartSorts.TryGetValue(comboBox1.Text, out string chartType);
            presenter.GetRecord(period, chartType, groupByList, conditions, width, height);
        }
    }
}

// class 指揮者
// private 圖形
// 指揮者(圖形 圖形)
//   this圖形 = 圖形

// public void 畫圖
//   圖形.drawPartA
//   圖形.drawPartB
//   ...