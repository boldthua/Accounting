using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Presenters;
using 記帳本.Repositories.Appdatas;
using static 記帳本.Contracts.AddRecordContract;

namespace 記帳本.Presenters
{
    internal class AddPresenters : IAddPresenter
    {
        ICategoryRepository categoryRepository;
        public AddPresenters()
        {
            categoryRepository = new CategoryRepository();
        }
        public void UpLoad(ExpenseModel model)
        {

        }
        public void GetAppDatas()
        {
            categoryRepository.GetCategories();

            //categoryRepository.GetSubcategories(category);

            categoryRepository.GetRecipients();
        }
    }
}