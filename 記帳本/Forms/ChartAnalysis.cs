using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using 記帳本.Attributes;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Service;
using 記帳本.Utility;
using static 記帳本.Contracts.AccoutContract;
using static 記帳本.Contracts.ChartAnalysisContract;

namespace 記帳本
{
    [DisplayName("圖表分析")]
    [Order(4)]
    public partial class ChartAnalysis : Form, IChartAnalysisView
    {
        IChartAnalysisPresenter presenter;
        new Dictionary<string, string> chartSorts = new Dictionary<string, string>
        {
            { "圓餅圖", "PieChart" },
            { "折線圖", "LineChart" },
            { "堆疊圖", "StackedColumn" }
        };


        // conditions 用來維護最終要送去給 Presenter分析的資料
        // key為 食/衣/住/行/育/樂/支付方式/對象  value為每一個類型中有勾選的項目
        Dictionary<string, List<string>> conditions = new Dictionary<string, List<string>>();

        // 用來記錄需要進行群組的分類有哪些 ex:根據吃飯(食)跟交通(行) 進行資料群組
        // 例如:想知道過去這兩個月花在吃飯跟交通的費用有多少? 但是要扣除掉幫家人/朋友請客的
        // 所以conditions裡面只會有: 食 (早午晚餐) 行(火車 高鐵票) 對象(自己)
        // groupbyList 會有 食,行 這兩筆，因為只看這兩個項目的金額總計 ["食", "行"]
        List<string> groupByList = new List<string>();
        public ChartAnalysis()
        {
            InitializeComponent();
            presenter = new ChartAnalysisPresenter(this);
            comboBox1.DataSource = chartSorts.Keys.ToList();
            comboBox1.SelectedIndex = 0;
            presenter.GetAppDatas();
        }

        public void RenderDatas(Chart chart)
        {
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanel2.Controls.Add(chart);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "折線圖")
                panel1.Visible = true;
            SetChart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DebounceTime(() =>
            {
                SetChart();
            }, 1000);
        }

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

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            Chart chart = flowLayoutPanel2.Controls.OfType<Chart>().FirstOrDefault();
            for (int i = 1; i < chart.Series.Count; i++)
            {
                if (cb.Tag.Equals(i.ToString()))
                {
                    chart.Series[i].Enabled = cb.Checked;
                }
            }
        }

        private void SetChart()
        {
            int width = flowLayoutPanel2.Width;
            int height = flowLayoutPanel2.Height;
            var period = new KeyValuePair<DateTime, DateTime>(dateTimePicker1.Value, dateTimePicker2.Value);
            chartSorts.TryGetValue(comboBox1.Text, out string chartType);
            presenter.GetRecord(period, chartType, groupByList, conditions, width, height);
        }
    }
}

// class 指揮者
// private 圖形
// 指揮者(圖形 圖形)
//   this圖形 = 圖形

// public void 畫圖
//   圖形.drawPartA
//   圖形.drawPartB
//   ...