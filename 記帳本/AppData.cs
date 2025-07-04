using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本
{
    internal class AppData
    {
        public static string[] catagory = { "食", "衣", "住", "行", "育", "樂" };
        public static string[] food = { "早餐", "中餐", "晚餐", "宵夜", "點心" };
        public static string[] cloth = { "衣物" };
        public static string[] living = { "家電", "水電瓦斯網路等", "設備維修等" };
        public static string[] trasportation = { "公共設施通勤費", "油錢", "汽機車維修" };
        public static string[] education = { "學費", "書籍", "其它" };
        public static string[] entertainment = { "旅行", "遊戲", "逛街" };
        public static string[] recipient = { "家人", "同事", "朋友", "其它" };
        public static Dictionary<string, string[]> expends = new Dictionary<string, string[]>()
        {
            { "類別", catagory },
            { "食", food},
            { "衣", cloth},
            { "住", living},
            { "行", trasportation},
            { "育", education},
            { "樂", entertainment },
            { "對象", recipient }
        };

    }
}
