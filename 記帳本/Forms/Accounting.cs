using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using 記帳本.Attributes;
using 記帳本.Contracts;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static 記帳本.Contracts.AccoutingContract;

namespace 記帳本
{
    [DisplayName("記帳本")]
    [Order(1)]
    public partial class Accounting : Form, IAccountingView
    {
        IAccountingPresenter presenter;
        ComboBoxData data;
        List<string> items = new List<string>();
        BindingList<ExpenseViewModel> list = null;
        Queue<Bitmap> bitmaps = new Queue<Bitmap>();
        public Accounting()
        {
            InitializeComponent();
            presenter = new AccountingPresenter(this);
            presenter.GetAppDatas();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            dataGridView1.AllowUserToAddRows = false;

        }

        void showDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();

            while (bitmaps.Count > 0)
            {
                bitmaps.Dequeue().Dispose();
            }

            GC.Collect();

            dataGridView1.DataSource = list;
            dataGridView1.Columns["time"].ReadOnly = true;

            foreach (PropertyInfo property in typeof(ExpenseViewModel).GetProperties())
            {
                var attributes = property.GetCustomAttributes();
                if (attributes.Count() < 1)
                    continue;

                if (attributes.Any(x => x is ComboBoxColumnAttribute))
                {
                    DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn()
                    {
                        HeaderText = property.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                        Name = property.Name + "ComboBox",
                        DataPropertyName = property.Name
                    };
                    if (property.Name != "item")
                        comboBoxColumn.DataSource = typeof(ComboBoxData).GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance).GetValue(data);
                    dataGridView1.Columns.Add(comboBoxColumn);
                    dataGridView1.Columns[property.Name].Visible = false;


                }

                if (attributes.Any(x => x is ImageColumnAttribute))
                {
                    DataGridViewImageColumn imageColumn = new DataGridViewImageColumn()
                    {
                        HeaderText = property.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                        Name = property.Name + "ImageBox",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                    };
                    dataGridView1.Columns.Add(imageColumn);
                    dataGridView1.Columns[property.Name].Visible = false;
                }

            }
            DataGridViewImageColumn trashCanColumn = new DataGridViewImageColumn()
            {
                HeaderText = "刪除",
                Name = "trashCan",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            Bitmap trashPicture = new Bitmap(@"C:\Users\User\source\repos\記帳本\記帳本\trashCan.jpg");
            bitmaps.Append(trashPicture);
            trashCanColumn.DefaultCellStyle.NullValue = new Bitmap(trashPicture);

            dataGridView1.Columns.Add(trashCanColumn);

            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                string currentCat = dataGridView1.Rows[i].Cells["catagory"].Value.ToString();
                DataGridViewComboBoxCell itemCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells["itemComboBox"];
                presenter.GetSubcategories(currentCat);
                itemCell.DataSource = items;
                foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                {
                    if (cell is DataGridViewImageCell imageCell && cell.OwningColumn.Name != "trashCan")
                    {
                        string cellName = imageCell.OwningColumn.Name.Replace("ImageBox", "");
                        string imagePath = dataGridView1.Rows[i].Cells[cellName].Value.ToString();
                        byte[] imageBytes = File.ReadAllBytes(imagePath);
                        MemoryStream memoryStream = new MemoryStream(imageBytes);
                        Bitmap picture = new Bitmap(memoryStream); // 這行最浪費
                        dataGridView1.Rows[i].Cells[cellName + "ImageBox"].Value = picture;
                        bitmaps.Append(picture);
                    }

                    if (cell is DataGridViewComboBoxCell comboBoxCell)
                    {
                        string cellName = comboBoxCell.OwningColumn.Name;
                        string sourceColumn = comboBoxCell.OwningColumn.Name.Replace("ComboBox", "");
                        dataGridView1.Rows[i].Cells[cellName].Value = dataGridView1.Rows[i].Cells[sourceColumn].Value;
                    }
                    // 0711 再看一次
                    // 0711 處理 oom 的問題 
                }
            }
        }


        public void RenderDatas(List<ExpenseDTO> records)
        {
            list?.Clear();

            var viewModelList = Mapper.Map<ExpenseDTO, ExpenseViewModel>(records) as List<ExpenseViewModel>;

            list = new BindingList<ExpenseViewModel>(viewModelList);
            showDataGridView();
        }


        private void button1_Click(object sender, EventArgs e)
        {

            this.DebounceTime(() =>
            {
                presenter.GetRecord(dateTimePicker1.Value, dateTimePicker2.Value);
            }, 1000);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //找到圖片欄位後點擊開啟
            //提示:創建ImageForm用來顯示完整圖片
            if (dataGridView1.Columns[e.ColumnIndex].Name is "trashCan")
            {
                DialogResult result = MessageBox.Show("你確定要刪除本筆資料嗎？", "刪除資料", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ExpenseViewModel modelToBeDel = list[e.RowIndex];
                    list.Remove(modelToBeDel);
                    ExpenseDTO expenseDTO = Mapper.Map<ExpenseViewModel, ExpenseDTO>(modelToBeDel);
                    presenter.DeleteRecord(expenseDTO);
                    MessageBox.Show("資料刪除成功！");
                }
                return;
            }
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell cell)
            {
                string imagePath = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 5]
                    .Value.ToString().Replace("w40_", "");
                ImageForm imageForm = new ImageForm(imagePath);
                imageForm.Show();
                return;
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell is DataGridViewComboBoxCell && e.ColumnIndex == dataGridView1.Columns["catagoryComboBox"].Index)
            {
                presenter.GetSubcategories(list[e.RowIndex].catagory);
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells["itemComboBox"];
                cell.DataSource = items;
                cell.Value = items[0];
            }

            ExpenseViewModel modelToBeUpdated = list[e.RowIndex];
            ExpenseDTO updatedRecord = Mapper.Map<ExpenseViewModel, ExpenseDTO>(modelToBeUpdated);
            presenter.UpdateRecord(updatedRecord);
            MessageBox.Show("資料更新成功！");
        }

        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView.CurrentCell is DataGridViewComboBoxCell)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGridView1.EndEdit();
            }
        }

        public void PopulateComboBox(ComboBoxData data)
        {
            this.data = data;
        }

        public void ReceiveItems(List<string> items)
        {
            this.items = items;
        }
    }
}
