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

        public void GetRecord(DateTime start, DateTime end, Dictionary<string, List<string>> conditions)
        {
            var props = typeof(RecordModel)
                .GetProperties()
                //.Where(x => x.Name == "Catagory" || x.Name == "Item" || x.Name == "Recipient")
                .ToList();
            List<RecordModel> datas = repository.GetRecords(start, end);

            //var filter = datas.Where(x => props.All(y => conditions.Contains(y.GetValue(x).ToString()))).ToList();




            //datas = datas.Where(x => group.Contains(x.Catagory)).ToList();
            //datas = datas.Where(x => detail.Contains(x.Item)).ToList();
            //datas = datas.Where(x => detail.Contains(x.Recipient)).ToList();

            //List<AccountDTO> list = Mapper.Map<RecordModel, AccountDTO>(filter) as List<AccountDTO>;
            // 通知view顯示
            //view.RenderDatas(list);
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
