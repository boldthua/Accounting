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
using static 記帳本.Contracts.AccoutingContract;
using static 記帳本.Contracts.AddRecordContract;

namespace 記帳本.Presenters
{

    public class AccountingPresenter : IAccountingPresenter
    {
        IAccountingView view;
        IRecordRepository repository;
        ICategoryRepository categoryRepository { get; set; }


        public AccountingPresenter(IAccountingView view)
        {
            categoryRepository = new CategoryRepository();
            repository = new RecordRepository();
            this.view = view;
        }
        public void DeleteRecord(ExpenseDTO recordToBeDeleted)
        {
            // 從dtos裡面挑出要刪掉的
            // repository.Update<dtos>;
            // 把ExpenseDTO 轉成 RecordModel;
            RecordModel recordModel = Mapper.Map<ExpenseDTO, RecordModel>(recordToBeDeleted);
            repository.DeleteRecord(recordModel);

        }
        public void GetRecord(DateTime start, DateTime end)
        {
            List<RecordModel> datas = repository.GetRecords(start, end);
            List<ExpenseDTO> list = Mapper.Map<RecordModel, ExpenseDTO>(datas) as List<ExpenseDTO>;
            // 通知view顯示
            view.RenderDatas(list);
        }

        public void UpdateRecord(ExpenseDTO record)
        {
            RecordModel model = Mapper.Map<ExpenseDTO, RecordModel>(record);
            repository.UpdateRecord(model);
        }

        public void GetAppDatas()
        {
            List<string> majorCat = categoryRepository.GetCategories();
            List<string> item = categoryRepository.GetSubcategories(majorCat[0]);
            List<string> recipient = categoryRepository.GetRecipients();

            ComboBoxData data = new ComboBoxData(majorCat, item, recipient);
            view.PopulateComboBox(data);
        }

        public void GetSubcategories(string category)
        {
            List<string> items = categoryRepository.GetSubcategories(category);
            view.ReceiveItems(items);
        }

    }
}
