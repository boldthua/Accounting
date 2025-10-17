using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Attributes;

namespace 記帳本.Contracts.Models.DTOs
{
    public class AccountAnalyzeDTO
    {
        [DisplayName("分析類別")]
        public string GroupByName { get; set; }
        [DisplayName("類別總金額")]
        public int ToTalMoney { get; set; }
        [DisplayName("消費日期")]
        public List<string> dates { get; set; }
        [DisplayName("單筆金額")]
        public List<int> Money { get; set; }

    }
}
