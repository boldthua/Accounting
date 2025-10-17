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
using static 記帳本.Contracts.AccoutContract;
using static 記帳本.Contracts.AddRecordContract;
using static 記帳本.Contracts.ChartAnalysisContract;
namespace 記帳本.Presenters
{

    public class ChartAnalysisPresenter : IChartAnalysisPresenter
    {
        IChartAnalysisView view;
        IRecordRepository repository;
        ICategoryRepository categoryRepository { get; set; }
        DataAnalysisService dataAnalysisService { get; set; }

        public ChartAnalysisPresenter(IChartAnalysisView view)
        {
            categoryRepository = new CategoryRepository();
            repository = new RecordRepository();
            dataAnalysisService = new DataAnalysisService(repository);
            this.view = view;
        }

        public void GetRecord(DateTime start, DateTime end, List<string> groupByList, Dictionary<string, List<string>> conditions)
        {
            var renderData = dataAnalysisService.GetAccountAnalyzeDatas(start, end, groupByList, conditions);
            view.RenderDatas(renderData);
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
