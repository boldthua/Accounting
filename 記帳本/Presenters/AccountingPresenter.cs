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
            RecordModel recordModel = new RecordModel();

            recordModel.Time = recordToBeDeleted.Time;
            recordModel.Catagory = recordToBeDeleted.Catagory;
            recordModel.Money = recordToBeDeleted.Money;
            recordModel.Recipient = recordToBeDeleted.Recipient;
            recordModel.Picture1 = recordToBeDeleted.Picture1;
            recordModel.Picture2 = recordToBeDeleted.Picture2;

            bool isDelSeccess = repository.DeleteRecord(recordModel);

            view.IsDeleteResponse(isDelSeccess);

        }

        public void GetRecord(DateTime start, DateTime end)
        {
            List<RecordModel> datas = repository.GetRecords(start, end);
            List<ExpenseDTO> list = new List<ExpenseDTO>();


            foreach (RecordModel record in datas)
            {
                ExpenseDTO viewModel = new ExpenseDTO();
                viewModel.Time = record.Time;
                viewModel.Item = record.Item;
                viewModel.Catagory = record.Catagory;
                viewModel.Recipient = record.Recipient;
                viewModel.Money = record.Money;
                viewModel.Picture1 = record.Picture1;
                viewModel.Picture2 = record.Picture2;
                list.Add(viewModel);
            }
            // 通知view顯示
            view.RenderDatas(list);
        }

        public void UpdateRecord(List<ExpenseDTO> records)
        {
            List<RecordModel> recordModels = new List<RecordModel>();
            foreach (ExpenseDTO record in records)
            {
                RecordModel model = new RecordModel();
                model.Time = record.Time;
                model.Money = record.Money;
                model.Catagory = record.Catagory;
                model.Item = record.Item;
                model.Recipient = record.Recipient;
                model.Picture1 = record.Picture1;
                model.Picture2 = record.Picture2;
                recordModels.Add(model);
            }
            bool isUpdateSuccess = repository.UpdateRecord(recordModels);
            view.IsUpdateResponse(isUpdateSuccess);
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
