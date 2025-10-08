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
        public int Money { get; set; }

    }
}
