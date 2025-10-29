using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Repositories.Models;

namespace 記帳本.Builders
{
    internal class LineChartBuilder : ChartBuilder
    {
        // 3個series所需要的List<List<string, double>>
        private List<List<string>> PeriodsPerDate = new List<List<string>>();
        private List<Dictionary<string, double>> periodDates = new List<Dictionary<string, double>>();
        protected List<List<RecordModel>> LineRowDates = new List<List<RecordModel>>();
        protected List<List<RecordModel>> LineFilterDates = new List<List<RecordModel>>();

        public LineChartBuilder(int width, int height) : base(width, height)
        {
        }

        public override ChartBuilder setDates(KeyValuePair<DateTime, DateTime> startToEnd)
        {
            var periods = Get3Periods(startToEnd);

            foreach (var timeSet in periods)
            {
                List<RecordModel> record = new List<RecordModel>();
                record = _repository.GetRecords(timeSet.Key, timeSet.Value);
                var periodDates = GetPeriodDates(timeSet);
                PeriodsPerDate.Add(periodDates);
                LineRowDates.Add(record);
            }
            return this;
        }

        public override ChartBuilder FilterDates(Dictionary<string, List<string>> conditions)
        {

            foreach (var record in LineRowDates)
            {
                var filter = new List<RecordModel>();
                var props = typeof(RecordModel)
                            .GetProperties()
                            .ToList();
                var filterProps = props.Where(x => conditions.ContainsKey(x.Name)).ToList();
                var allValues = conditions.Values.SelectMany(x => x).ToList();
                filter = record.Where(x => filterProps.All(y => allValues.Contains(y.GetValue(x).ToString()))).ToList();
                LineFilterDates.Add(filter);
            }

            for (int i = 0; i < LineFilterDates.Count(); i++)
            {
                LineFilterDates[i] = FillEmptyDateMoneyZero(LineFilterDates[i], PeriodsPerDate[i]);
            }
            ;

            return this;
        }
        public override ChartBuilder GroupByDates(List<string> groupByList)
        {
            foreach (var record in LineFilterDates)
            {
                var monthRecord = record.GroupBy(x => x.Time)
                      .Select(x => new KeyValuePair<string, double>
                      (
                          x.Key,
                          (double)x.Sum(y => int.Parse(y.Money))
                      )).ToDictionary(x => x.Key, x => x.Value);
                periodDates.Add(monthRecord);
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

            area.AxisY.Title = "金額（元）";
            area.AxisY.TextOrientation = TextOrientation.Horizontal;
            area.AxisY.Title = "金\n額\n(元)";
            area.AxisY.TitleForeColor = Color.Blue;
            area.AxisY.TitleFont = new Font("微軟正黑體", 12f);
            return this;
        }

        public override ChartBuilder SetSeries()
        {
            for (int i = 0; i < periodDates.Count(); i++)
            {
                string topic = i == 0 ? "本期" : $"前 {i} 期";

                var series = new Series(topic)
                {
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.String,
                    IsValueShownAsLabel = false,
                    Label = string.Empty,
                    ChartArea = "main",
                    Font = new Font("Arial", 6f),
                    BorderWidth = 3,
                };

                // 只控制數字格式；第三段為空 → 0 不會顯示「元」
                series.LabelFormat = "#,##0'元';;";


                series.Points.DataBindXY(
                    periodDates[i].Keys.Select(x => string.Join("-", x.Split('-').Skip(1))).ToList(),
                    periodDates[i].Values
                );

                foreach (var p in series.Points)
                {
                    p.IsValueShownAsLabel = false;
                    if (p.YValues.Length > 0 && p.YValues[0] != 0)
                    {
                        p.Label = "#VAL元";
                        p.IsValueShownAsLabel = true;
                    }
                }

                _chart.Series.Add(series);
            }
            return this;
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

        private List<RecordModel> FillEmptyDateMoneyZero(List<RecordModel> records, List<string> dates)
        {
            List<RecordModel> list = new List<RecordModel>();
            foreach (var date in dates)
            {
                bool hasThisDay = false;
                foreach (var record in records)
                {
                    if (record.Time == date)
                    {
                        list.Add(record);
                        hasThisDay = true;
                    }
                }
                if (hasThisDay == false)
                {
                    list.Add(new RecordModel(date, "0"));
                }
            }
            return list;
        }

        private List<string> GetPeriodDates(KeyValuePair<DateTime, DateTime> dateSet)
        {
            List<string> dates = new List<string>();

            for (DateTime date = dateSet.Key; date <= dateSet.Value; date = date.AddDays(1))
                dates.Add(date.ToString("yyyy-MM-dd"));
            return dates;
        }
    }
}
