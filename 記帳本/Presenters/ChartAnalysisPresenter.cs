using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Repositories;
using 記帳本.Repositories.Appdatas;
using 記帳本.Repositories.Models;
using 記帳本.Service;
using 記帳本.Utility;
using System.Windows.Forms.DataVisualization.Charting;
using static 記帳本.Contracts.AccoutContract;
using static 記帳本.Contracts.AddRecordContract;
using static 記帳本.Contracts.ChartAnalysisContract;
using 記帳本.Builders;

namespace 記帳本.Presenters
{

    public class ChartAnalysisPresenter : IChartAnalysisPresenter
    {
        IChartAnalysisView view;
        ICategoryRepository categoryRepository { get; set; }

        public ChartAnalysisPresenter(IChartAnalysisView view)
        {
            categoryRepository = new CategoryRepository();
            this.view = view;
        }

        public void GetRecord(KeyValuePair<DateTime, DateTime> startToEnd, string chartType, List<string> groupByList, Dictionary<string, List<string>> conditions, int width, int height)
        {
            string chartName = $"記帳本.Builders.{chartType}Builder";
            ChartBuilder chartBuilder = Activator.CreateInstance(Type.GetType(chartName), width, height) as ChartBuilder;

            Chart chart = chartBuilder
                        .setDates(startToEnd)
                        .SetTitle(chartType)
                        .FilterDates(conditions)
                        .GroupByDates(groupByList)
                        .SetSeries()
                        .SetAxisXY()
                        .SetLegend()
                        .Build();

            view.RenderDatas(chart);
        }

        public void GetAppDatas()
        {
            List<string> majorCat = categoryRepository.GetCategories();
            Dictionary<string, List<string>> items = majorCat
                .ToDictionary(
                x => x,
                x => categoryRepository.GetSubcategories(x)
                );
            List<string> recipient = categoryRepository.GetRecipients();
            List<string> payment = categoryRepository.GetPayments();
            AllItemData data = new AllItemData(majorCat, items, recipient, payment);
            view.PopulateMainCheckBox(data);
        }
    }
}
