using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Contracts
{
    internal class AddRecordContract
    {
        public interface IAddView
        {
            void UpLoadResponse(bool result);
        }

        public interface IAddPresenter
        {
            void UpLoad(ExpenseModel model);
        }
    }
}
