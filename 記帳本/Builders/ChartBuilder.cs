using System;
using System.Collections.Generic;
using System.Linq;
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
        protected List<string> _dates;
        protected List<RecordModel> _rowDates;
        protected List<RecordModel> _filterDates;
        protected Dictionary<string, List<string>> groupByCondition;
        protected Chart _chart = new Chart();

        protected ChartBuilder(int width, int height)
        {
            _chart.Width = width;
            _chart.Height = height;
        }
        public ChartBuilder setDates(KeyValuePair<DateTime, DateTime> startToEnd)
        {
            for (DateTime date = startToEnd.Key; date > startToEnd.Value; date = date.AddDays(1))
                _dates.Add(date.ToString("yyyy-mm-dd"));
            return this;
        }
        public abstract ChartBuilder SetTitle();
        public abstract ChartBuilder SetArea();
        public virtual ChartBuilder SetSeries();
        public virtual ChartBuilder SetAxisXY();
        public abstract ChartBuilder SetLegend();
        public Chart Build();
    }
}
