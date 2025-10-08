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
using 記帳本.Utility;
using static 記帳本.Contracts.AddRecordContract;
using static 記帳本.Contracts.AccoutContract;
namespace 記帳本.Presenters
{

    public class AccountPresenter : IAccountPresenter
    {
        IAccountView view;
        IRecordRepository repository;
        ICategoryRepository categoryRepository { get; set; }


        public AccountPresenter(IAccountView view)
        {
            categoryRepository = new CategoryRepository();
            repository = new RecordRepository();
            this.view = view;
        }

        public void GetRecord(DateTime start, DateTime end, List<string> groupByList, Dictionary<string, List<string>> conditions)
        {
            var props = typeof(RecordModel)
                .GetProperties()
                .ToList();
            var filterProps = props.Where(x => conditions.ContainsKey(x.Name)).ToList();
            List<RecordModel> datas = repository.GetRecords(start, end);
            var allValues = conditions.Values.SelectMany(x => x).ToList();
            var filter = datas.Where(x => filterProps.All(y => allValues.Contains(y.GetValue(x).ToString()))).ToList();


            var groupByProps = props.Where(x => groupByList.Contains(x.Name)).ToList(); // payment,receipent


            var renderData = filter.GroupBy(x =>
            {
                return string.Join(",", groupByProps.Select(prop => prop.GetValue(x).ToString()));
            }).Select(x => new AccountAnalyzeDTO
            {
                GroupByName = x.Key,
                Money = x.Sum(y => int.Parse(y.Money))
            }).ToList();

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
