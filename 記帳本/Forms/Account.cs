using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using 記帳本.Attributes;
using 記帳本.Contracts;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Utility;
using static 記帳本.Contracts.AccoutContract;


namespace 記帳本
{
    [DisplayName("帳戶分析")]
    [Order(2)]
    public partial class Account : Form, IAccountView
    {
        IAccountPresenter presenter;
        List<AccountAnalyzeDTO> records;
        List<FlowLayoutPanel> itemFlPanelList = new List<FlowLayoutPanel>();
        FlowLayoutPanel recipientPanel = new FlowLayoutPanel();

        // conditions 用來維護最終要送去給 Presenter分析的資料
        // key為 食/衣/住/行/育/樂/支付方式/對象  value為每一個類型中有勾選的項目
        Dictionary<string, List<string>> conditions = new Dictionary<string, List<string>>();

        // 用來記錄需要進行群組的分類有哪些 ex:根據吃飯(食)跟交通(行) 進行資料群組
        // 例如:想知道過去這兩個月花在吃飯跟交通的費用有多少? 但是要扣除掉幫家人/朋友請客的
        // 所以conditions裡面只會有: 食 (早午晚餐) 行(火車 高鐵票) 對象(自己)
        // groupbyList 會有 食,行 這兩筆，因為只看這兩個項目的金額總計 ["食", "行"]
        List<string> groupByList = new List<string>();
        public Account()
        {
            InitializeComponent();
            presenter = new AccountPresenter(this);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            presenter.GetAppDatas();
        }

        #region Todo 待開發
        public void RenderDatas(List<AccountAnalyzeDTO> records)
        {
            this.records = records;
            showDataGridView();
        }

        private void showDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            GC.Collect();
            dataGridView1.DataSource = records;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DebounceTime(() =>
            {
                presenter.GetRecord(dateTimePicker1.Value, dateTimePicker2.Value, groupByList, conditions);
            }, 1000);
        }

        #endregion


        public void PopulateMainCheckBox(AllItemData data)
        {
            flowLayoutPanel1.GenerateCheckboxs(data, ConditionCheckedChange, GroupByCheckedChange);
        }

        public void ConditionCheckedChange(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string keyName = checkBox.Tag.ToString();

            if (checkBox.Checked)
            {
                if (conditions.TryGetValue(keyName, out List<string> conditionList) && !conditionList.Contains(checkBox.Text))
                {
                    conditionList.Add(checkBox.Text);
                }
                else
                {
                    conditions.Add(keyName, new List<string>() { checkBox.Text });
                }
            }
            else
            {
                conditions[keyName].Remove(checkBox.Text);
            }
        }

        public void GroupByCheckedChange(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            var propertyName = typeof(AccountDTO).GetProperties()
                                  .FirstOrDefault(x =>
                                  {
                                      var attr = x.GetCustomAttribute<RecordAttribute>();
                                      if (attr == null)
                                          return false;
                                      return attr._displayName == checkBox.Text;
                                  }).Name;
            if (checkBox.Checked)
            {
                groupByList.Add(propertyName);
            }
            else
            {
                groupByList.Remove(propertyName);
            }
        }
    }

    // 1008 89行trygetvalue 看看
    // 
}
