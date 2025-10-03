using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Repositories.Appdatas
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new List<Category>
    {
        new Category { Name = "類別", content = new[] { "食", "衣", "住", "行", "育", "樂" } },
        new Category { Name = "食", content = new[] { "早餐", "中餐", "晚餐", "宵夜", "點心" } },
        new Category { Name = "衣", content = new[] { "襯衫", "T恤", "褲子" } },
        new Category { Name = "住", content = new[] { "家電", "水電瓦斯網路等", "設備維修等" } },
        new Category { Name = "行", content = new[] { "公共設施通勤費", "油錢", "汽機車維修" } },
        new Category { Name = "育", content = new[] { "學費", "書籍", "其它" } },
        new Category { Name = "樂", content = new[] { "旅行", "遊戲", "逛街" } },
        new Category { Name = "對象", content = new[] { "自己", "家人", "同事", "朋友", "其它" } },
        new Category { Name = "支付方式", content = new[] {"現金", "信用卡", "電子支付"}}
    };

        public List<string> GetCategories()
        {
            return _categories.First(c => c.Name == "類別").content.ToList();
        }

        public List<string> GetSubcategories(string category)
        {
            var cat = _categories.FirstOrDefault(c => c.Name == category);
            return cat?.content.ToList() ?? new List<string>();
        }

        public List<string> GetRecipients()
        {
            return _categories.First(c => c.Name == "對象").content.ToList();
        }
        public List<string> GetPayments()
        {
            return _categories.First(c => c.Name == "支付方式").content.ToList();
        }
    }
}
