using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Models;
using 記帳本.Repositories;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Builders
{
    internal class PieChartBuilder : ChartBuilder
    {
        // ChartBuilder builder = new PieChartBuilder();
        // builder => 型別是ChartBuilder 骨子裡是 PieChartBuilder

        private Dictionary<string, Double> PieChartDates = new Dictionary<string, Double>();

        public PieChartBuilder(int width, int height) : base(width, height)
        {
        }

        public override ChartBuilder GroupByDates(List<string> groupByList)
        {
            var props = typeof(RecordModel)
            .GetProperties()
            .ToList();
            var groupByProps = props.Where(x => groupByList.Contains(x.Name)).ToList(); // payment,receipent

            PieChartDates = _filterDates.GroupBy(x =>
                {
                    return string.Join(",", groupByProps.Select(prop => prop.GetValue(x).ToString()));
                })
                .Select(x => new
                {
                   Key = x.Key,
                   Value =  (Double)x.Sum(y => int.Parse(y.Money))
                }).ToDictionary(x=>x.Key,x=>x.Value);             
            return this;
        }

        public override ChartBuilder SetSeries()
        {
            var group = PieChartDates.Select(x=> x.Key).ToList();
            var money = PieChartDates.Select(x=> x.Value).ToList();
            string groupNameLabel = "分析類別";
            string moneyLabel = "類別總金額";

            var series = new Series("pie")
            {
                ChartType = SeriesChartType.Pie,
                XValueType = ChartValueType.String,
                IsValueShownAsLabel = true,
                Label = "#VALX #PERCENT{P0}",
                ChartArea = "main"
            };
            series.Points.DataBindXY(group, money);

            _chart.Series.Add(series);
            return this;
        }
        public override ChartBuilder SetAxisXY()
        {
            return this;
        }
    }
}
