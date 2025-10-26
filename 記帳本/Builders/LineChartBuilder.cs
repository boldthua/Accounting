using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Builders
{
    internal class LineChartBuilder : ChartBuilder
    {
        public LineChartBuilder(int width, int height) : base(width, height)
        {
        }

        public override ChartBuilder GroupByDates(List<string> groupByList)
        {
            throw new NotImplementedException();
        }

        public override ChartBuilder SetAxisXY()
        {
            throw new NotImplementedException();
        }

        public override ChartBuilder SetSeries()
        {
            throw new NotImplementedException();
        }
    }
}
