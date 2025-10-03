using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Utility
{
    internal static class FlowLayOutPanelExtension
    {
        static Dictionary<string, List<string>> itemsMap = new Dictionary<string, List<string>>();

        public static void GenerateCheckboxs(this FlowLayoutPanel panel, AllItemData data, EventHandler ConditionCheckedChange, EventHandler GroupbyCheckedChange)
        {
            itemsMap = data.items;
            System.Windows.Forms.Label label = new System.Windows.Forms.Label() { Text = "請選擇分析方式：", AutoSize = true };
            panel.Controls.Add(label);

            var groupByList = typeof(AccountDTO).GetProperties()
                .Where(x => x.GetCustomAttribute<RecordAttribute>() != null)
                .Select(x => x.GetCustomAttribute<RecordAttribute>()._displayName).ToList();

            FlowLayoutPanel groupPanel = CreateCheckBox("group", groupByList, panel.Width, GroupbyCheckedChange);
            panel.Controls.Add(groupPanel);

            System.Windows.Forms.Label label1 = new System.Windows.Forms.Label() { Text = "請選擇資料條件：", AutoSize = true };
            panel.Controls.Add(label1);
            FlowLayoutPanel catagoryPanel = CreateCheckBox("Catagory", data.catagory, panel.Width, ConditionCheckedChange);

            catagoryPanel.Controls.OfType<CheckBox>()
                                  .Where(x => x.Text != "全選")
                                  .ToList()
                                  .ForEach(x => x.CheckedChanged += CategoryCheck_CheckedChanged);

            FlowLayoutPanel detailPanel = new FlowLayoutPanel()
            {
                Width = panel.Width - 5,
                //Height = 30,
                AutoSize = true,
                //BorderStyle = BorderStyle.FixedSingle,
                Tag = "Detail",
            };
            FlowLayoutPanel recipientPanel = CreateCheckBox("Recipient", data.recipient, panel.Width, ConditionCheckedChange);
            FlowLayoutPanel paymentPanel = CreateCheckBox("Payment", data.payment, panel.Width, ConditionCheckedChange);
            panel.Controls.Add(catagoryPanel);
            panel.Controls.Add(detailPanel);
            panel.Controls.Add(recipientPanel);
            panel.Controls.Add(paymentPanel);

            foreach (var itemPair in itemsMap)
            {
                FlowLayoutPanel partPanel = CreateCheckBox(itemPair.Key, itemPair.Value, panel.Width, ConditionCheckedChange);
                detailPanel.Controls.Add(partPanel);
                //detailPanel.Height += partPanel.Height;
                detailPanel.Height = 30;
            }

        }

        private static FlowLayoutPanel CreateCheckBox(string typeName, List<string> list, int width, EventHandler handler)
        {
            string[] types = { "Catagory", "Recipient", "Payment" };
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Width = width - 5,
                Tag = typeName,
                Height = 40,
                //BorderStyle = BorderStyle.FixedSingle,
            };
            CheckBox allCheck = new CheckBox() { Text = "全選", Tag = "", Margin = new Padding(0), AutoSize = true };
            allCheck.CheckedChanged += AllCheck_CheckedChanged;
            panel.Controls.Add(allCheck);
            foreach (string str in list)
            {
                typeName = types.Contains(typeName) ? typeName : "Item";
                CheckBox checkBox = new CheckBox() { Text = str, Tag = typeName, Margin = new Padding(0), AutoSize = true };
                checkBox.CheckedChanged += CheckBoxes_CheckedChanged;
                checkBox.CheckedChanged += handler;
                panel.Controls.Add(checkBox);
            }
            allCheck.Checked = true;
            return panel;
        }

        private static void AllCheck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox allCheck = (CheckBox)sender;

            bool isAllCheckBoxesChecked = allCheck.Parent.Controls
                                          .OfType<CheckBox>()
                                          .Where(x => x != allCheck)
                                          .All(x => x.Checked);
            if (isAllCheckBoxesChecked && !allCheck.Checked)
            {
                allCheck.Checked = true;
                return;
            }
            if (allCheck.Tag?.ToString() == "被動")
            {
                allCheck.Tag = "";
                return;
            }
            allCheck.Parent.Controls
                .OfType<CheckBox>()
                .Where(x => x != allCheck)
                .ToList()
                .ForEach(x =>
                {
                    x.Name = "被動";
                    x.Checked = allCheck.Checked;
                });
        }
        private static void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Name == "被動")
            {
                checkBox.Name = "";
                return;
            }
            //CheckBox allCheckBox = (CheckBox)checkBox.Parent.Controls.OfType<CheckBox>().Where(x => x.Text == "全選");

            var checkboxList = checkBox.Parent.Controls.OfType<CheckBox>().ToList();
            CheckBox allCheckBox = checkboxList.First(x => x.Text == "全選");
            checkboxList = checkboxList.Where(x => x != allCheckBox).ToList();

            bool isSelectedAll = checkboxList.All(x => x.Checked);
            bool isSelectedAny = checkboxList.Any(x => x.Checked);
            if (allCheckBox.Checked != isSelectedAll)
            {
                allCheckBox.Tag = "被動";
                allCheckBox.Checked = isSelectedAll;
            }    // 再想一想， 108行 
            if (!isSelectedAny)
                allCheckBox.Checked = true;



            //if (checkBox.Checked) // 動作是勾選時 檢查是否全勾 
            //{
            //    if (isSelectedAll) // 全勾時 全選打勾 是被動
            //    {
            //        allCheckBox.Checked = true;
            //    }
            //    if (allCheckBox.Checked) // 如果全選已打勾，那就是全選通知全體打勾
            //        checkboxList.ForEach(x => x.Checked = true);
            //}
            //else // 動作是取消時 檢查allcheck是否勾選，有的話註記為被動後取消勾選
            //{
            //    if (allCheckBox.Checked)
            //    {
            //        allCheckBox.Tag = "被動";
            //        allCheckBox.Checked = false;
            //    }
            //}
        }

        private static void CategoryCheck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            FlowLayoutPanel grandPanel = checkBox.Parent.Parent as FlowLayoutPanel;
            var panels = grandPanel
                             .Controls
                             .OfType<FlowLayoutPanel>()
                             .ToList()
                             .Where(x => x.Tag != "Category");
            FlowLayoutPanel ditailPanel = panels.FirstOrDefault(x => x.Tag == "Detail");
            FlowLayoutPanel recipientPanel = panels.FirstOrDefault(x => x.Tag == "Recipient");

            bool allNotSelected = checkBox.Parent.Controls.OfType<CheckBox>().All(x => x.Checked == false);
            var currentPanel = ditailPanel.Controls.OfType<FlowLayoutPanel>().ToList().FirstOrDefault(x => x.Tag?.ToString() == checkBox.Text);
            currentPanel.Visible = checkBox.Checked;

            if (currentPanel.Visible == false)
                currentPanel.Controls.OfType<CheckBox>().ToList().ForEach(x => x.Checked = false);

            if (allNotSelected)
            {
                ditailPanel.Visible = false;
            }
            else
            {
                ditailPanel.Visible = true;
            }
            //foreach (FlowLayoutPanel flPanel in ditailPanel.Controls)
            //{
            //    if (flPanel.Tag == checkBox.Text)
            //        flPanel.Visible = checkBox.Checked;
            //}
            //if (ditailPanel.Controls.OfType<FlowLayoutPanel>().Any(x => x.Visible))
            //    recipientPanel.Visible = true;

        }
        #region 舊版
        //static FlowLayoutPanel panel;

        //public static void GenerateCheckboxs(this FlowLayoutPanel panel, AllItemData data)
        //{
        //    List<string> categoryList = data.catagory;
        //    Dictionary<string, List<string>> items = data.items;
        //    List<string> recipient = data.recipient;

        //    FlowLayoutPanel categoryPanel = new FlowLayoutPanel();
        //    System.Windows.Forms.Label label = new System.Windows.Forms.Label() { Text = "請勾選您要分析的消費類別：" };
        //    categoryPanel.SetFlowBreak(label, true); // 強制換行

        //    categoryPanel.CreateCheckBox(categoryList, "category");
        //    panel.Controls.Add(categoryPanel);

        //    FlowLayoutPanel itemPanel = new FlowLayoutPanel();
        //    foreach (var itemList in items)
        //    {
        //        itemPanel.CreateCheckBox(itemList, "item");
        //    }

        //    itemPanel.CreateCheckBox(recipient, "recipient");
        //    panel.Controls.Add(itemPanel);
        //}

        //private static void CreateCheckBox(this FlowLayoutPanel panel, List<string> list, string propertyName)
        //{
        //    FlowLayoutPanel smallPanel = new FlowLayoutPanel
        //    {
        //        FlowDirection = FlowDirection.LeftToRight,
        //        AutoSize = true,
        //        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        //        WrapContents = true
        //    };
        //    CheckBox allCheck = new CheckBox() { Text = "全選", Margin = new Padding(0), AutoSize = true };
        //    allCheck.CheckedChanged += AllCheck_CheckedChanged;
        //    smallPanel.Controls.Add(allCheck);
        //    foreach (string str in list)
        //    {
        //        CheckBox checkBox = new CheckBox() { Text = str, Margin = new Padding(0), AutoSize = true };
        //        checkBox.Tag = propertyName;
        //        checkBox.CheckedChanged += CheckBoxes_CheckedChanged;
        //        smallPanel.Controls.Add(checkBox);
        //        panel.SetFlowBreak(smallPanel, true); // 強制換行
        //    }
        //    if (propertyName != "category")
        //        smallPanel.Visible = false;
        //    panel.Controls.Add(smallPanel);
        //}

        //private static void CreateCheckBox(this FlowLayoutPanel panel, KeyValuePair<string, List<string>> list, string propertyName)
        //{
        //    FlowLayoutPanel smallPanel = new FlowLayoutPanel
        //    {
        //        FlowDirection = FlowDirection.LeftToRight,
        //        AutoSize = true,
        //        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        //        WrapContents = true,
        //        Tag = list.Key
        //    };
        //    CheckBox allCheck = new CheckBox() { Text = "全選", Margin = new Padding(0), AutoSize = true };
        //    allCheck.CheckedChanged += AllCheck_CheckedChanged;
        //    smallPanel.Controls.Add(allCheck);
        //    foreach (string str in list.Value)
        //    {
        //        CheckBox checkBox = new CheckBox() { Text = str, Margin = new Padding(0), AutoSize = true };
        //        checkBox.Tag = propertyName;
        //        checkBox.CheckedChanged += CheckBoxes_CheckedChanged;
        //        smallPanel.Controls.Add(checkBox);
        //        panel.SetFlowBreak(smallPanel, true); // 強制換行
        //    }
        //    if (propertyName != "category")
        //        smallPanel.Visible = false;
        //    panel.Controls.Add(smallPanel);
        //}

        //private static void AllCheck_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox allCheck = (CheckBox)sender;
        //    if (allCheck.Tag == "被動")
        //    {
        //        allCheck.Tag = "";
        //        return;
        //    }
        //    FlowLayoutPanel parentPanel = allCheck.Parent as FlowLayoutPanel;

        //    foreach (Control control in parentPanel.Controls)
        //    {
        //        if (control is CheckBox checkBox && checkBox != allCheck)
        //        {
        //            checkBox.Checked = allCheck.Checked; // 同步勾選狀態
        //        }
        //    }
        //}

        //private static void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox checkBox = (CheckBox)sender;
        //    FlowLayoutPanel parentPanel = checkBox.Parent as FlowLayoutPanel;
        //    FlowLayoutPanel grandParentPanel = checkBox.Parent.Parent as FlowLayoutPanel;
        //    FlowLayoutPanel itemPanel = new FlowLayoutPanel();
        //    foreach(Control control in grandParentPanel.Controls)
        //    {
        //        if(control is FlowLayoutPanel fPanel && fPanel != parentPanel)
        //            itemPanel = fPanel;
        //    }

        //    if (checkBox.Checked == false)
        //    {
        //        if (checkBox.Tag == "category")
        //        {
        //            foreach (FlowLayoutPanel flPanel in itemPanel.Controls) // 這裡不會寫
        //            {
        //                if (flPanel.Tag == checkBox.Text)
        //                    flPanel.Visible = false;
        //            }
        //            if (itemFlPanelList.All(x => x.Visible == false))
        //                recipientPanel.Visible = false;
        //        }
        //        FlowLayoutPanel parentPanel = checkBox.Parent as FlowLayoutPanel;
        //        foreach (Control control in parentPanel.Controls)
        //        {
        //            if (control is CheckBox allCheck && allCheck.Text == "全選" && allCheck.Checked == true)
        //            {
        //                allCheck.Tag = "被動";
        //                allCheck.Checked = false;
        //                return;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (checkBox.Tag == "category")
        //        {
        //            foreach (FlowLayoutPanel flPanel in itemFlPanelList)
        //            {
        //                if (flPanel.Tag == checkBox.Text)
        //                    flPanel.Visible = true;
        //            }
        //            if (itemFlPanelList.Any(x => x.Visible == true))
        //                recipientPanel.Visible = true;
        //        }
        //        CheckBox allCheckBox = new CheckBox();
        //        FlowLayoutPanel pPanel = checkBox.Parent as FlowLayoutPanel;
        //        foreach (Control control in pPanel.Controls)
        //        {
        //            if (control is CheckBox CBox && CBox.Text != "全選")
        //            {
        //                if (CBox.Checked == false)
        //                    return;
        //            }
        //            else if (control is CheckBox BBox && BBox.Text == "全選")
        //                allCheckBox = BBox;
        //        }
        //        allCheckBox.Checked = true;
        //    }
        //}
        #endregion

    }
}
