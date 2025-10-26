using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Repositories;
using 記帳本.Repositories.Models;

namespace 記帳本.Builders
{
    public abstract class ChartBuilder
    {
        protected IRecordRepository _repository = new RecordRepository();
        protected List<string> _dates = new List<string>();
        protected List<RecordModel> _rowDates;
        protected List<RecordModel> _filterDates;
        protected List<string> _groupByList;
        protected Dictionary<string, List<string>> _conditions;
        protected Chart _chart = new Chart();

        protected ChartBuilder(int width, int height)
        {
            _chart.Width = width;
            _chart.Height = height;
            _chart.BackColor = Color.Transparent;
            ChartArea area = new ChartArea("main")
            {
                Position = new ElementPosition(0, 10, 70, 80)
            };
            _chart.ChartAreas.Add(area);
        }
        public ChartBuilder setDates(KeyValuePair<DateTime, DateTime> startToEnd)
        {
            for (DateTime date = startToEnd.Key; date > startToEnd.Value; date = date.AddDays(-1))
                _dates.Add(date.ToString("yyyy-mm-dd"));
            _rowDates = _repository.GetRecords(startToEnd.Key, startToEnd.Value);
            return this;
        }
        public ChartBuilder SetTitle(string chartType)
        {
            var title = new Title(chartType)
            {
                Font = new Font("微軟正黑體", 14f, FontStyle.Regular),
                Alignment = ContentAlignment.TopCenter
            };
            _chart.Titles.Add(title);
            return this;
        }
        public ChartBuilder FilterDates(Dictionary<string, List<string>> conditions)
        {
            var props = typeof(RecordModel)
            .GetProperties()
            .ToList();
            var filterProps = props.Where(x => conditions.ContainsKey(x.Name)).ToList();
            var allValues = conditions.Values.SelectMany(x => x).ToList();
            var filter = _rowDates.Where(x => filterProps.All(y => allValues.Contains(y.GetValue(x).ToString()))).ToList();
            return this;
        }
        public abstract ChartBuilder GroupByDates(List<string> groupByList);
        public abstract ChartBuilder SetSeries();

        public abstract ChartBuilder SetAxisXY();
        public ChartBuilder SetLegend()
        {
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
            _chart.Legends.Add(legend);
            return this;
        }
        public Chart Build()
        {
            return _chart;
        }
    }
}
