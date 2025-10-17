using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace 記帳本.ChartBuilder
{
    public interface IChartBuilder
    {
        IChartBuilder SetTitle();
        IChartBuilder SetArea();
        IChartBuilder SetSeries();
        IChartBuilder SetAxisXY();
        IChartBuilder SetLegend();
        Chart Build();

    }
}
