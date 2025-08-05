using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Repositories.Appdatas;

namespace 記帳本.Contracts.Models
{
    internal class ComboBoxData
    {
        List<string> category { get; set; }
        List<string> item { get; set; }
        List<string> recipient { get; set; }

        public ComboBoxData(ICategoryRepository dataRepository, string comboBox1_SelectedIndexChanged)
        {
            category = dataRepository.GetCategories();

            item = dataRepository.GetSubcategories(comboBox1_SelectedIndexChanged);

            recipient = dataRepository.GetRecipients();
        }

    }
}