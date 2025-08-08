using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Contracts
{
    public class AddRecordContract
    {
        public interface IAddView
        {
            void SaveResponse(bool result);

            void PopulateComboBox(ComboBoxData data);
            void PopulateItemComboBox(List<string> items);
        }

        public interface IAddPresenter
        {

            void SaveRecord(RecordDTO record);
            // 還要向reipository要List<string>來當comboBox的資料

            void GetAppDatas();
            void GetSubcategories(string category);
        }
    }
}


// 0806 homework
// addview 資料存成dto傳給presenter 
// presenter 資料存成 recordmodel 傳給 repository
// 
