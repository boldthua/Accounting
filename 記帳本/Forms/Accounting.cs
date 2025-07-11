using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using 記帳本.Attributes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 記帳本
{
    [DisplayName("記帳本")]
    [Order(1)]
    public partial class Accounting : Form
    {
        public Accounting()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
        }
        List<ExpenseModel> list;
        Queue<Bitmap> bitmaps = new Queue<Bitmap>();
        private void button1_Click(object sender, EventArgs e)
        {






            showDataGridView();
            // 解決重複跳出的問題
            // 增加一個刪除column 可刪除該列資料

            //HW:
            //1. 完成圖片欄位的插入，能正常顯示每一筆中的兩個圖片
            //2. 對圖片點擊兩下後能顯示完整圖片


            //DataGridView 組成:
            //直行(Column)橫列(Row)單一格(Cell)
            //DataGridColumn 作為資料的每一個欄位 => DataGridTextBoxColumn
            //DataGridRow 做為每一筆資料
            //DataGridCell 填充每一筆資料的每一個欄位的值 => DataGridTextBoxCell

            //dataGridView1.Rows[1].Cells[3].Value = 123;
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

                    string picture1Path = list[e.RowIndex].picture1;
                    string picture2Path = list[e.RowIndex].picture2;

                    this.dataGridView1.Rows[e.RowIndex].Cells
                        .OfType<DataGridViewImageCell>()
                        .Select(x => (Bitmap)x.Value)
                        .ToList()
                        .ForEach(x => x.Dispose());
                    this.dataGridView1.DataSource = null;
                    this.dataGridView1.Columns.Clear();
                    File.Delete(picture1Path);
                    File.Delete(picture2Path);
                    File.Delete(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv");

                    list.RemoveAt(e.RowIndex);
                    CSVLibrary.CSVHelper.Write<ExpenseModel>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv", list, true);

                    showDataGridView();
                }
                return;
            }
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell cell)
            {
                string imagePath = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 5].Value.ToString();
                ImageForm imageForm = new ImageForm(imagePath);
                imageForm.Show();
                return;
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell is DataGridViewComboBoxCell)
            {
                string currentCat = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                string originCat = dataGridView1.Columns[e.ColumnIndex].Name.Replace("ComboBox", "");
                DataGridViewCell originCell = dataGridView1.Rows[e.RowIndex].Cells[originCat];
                originCell.Value = currentCat;

                if (e.ColumnIndex == dataGridView1.Columns["catagoryComboBox"].Index)
                {
                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
                    cell.DataSource = AppData.expends[currentCat];
                    cell.Value = AppData.expends[currentCat][0];
                    string relativeCell = dataGridView1.Columns[e.ColumnIndex + 1].Name.Replace("ComboBox", "");
                    Console.WriteLine(relativeCell);
                    dataGridView1.Rows[e.RowIndex].Cells[relativeCell].Value = cell.Value;
                    Console.WriteLine(dataGridView1.Rows[e.RowIndex].Cells[relativeCell].GetType().Name);
                }
            }
            File.Delete(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv");
            CSVLibrary.CSVHelper.Write<ExpenseModel>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv", list, true);
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

        void showDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            //TODO: 這裡應該可以先不用寫這行

            //GC.Collect();

            //bitmaps.Clear();
            // 0709 把所有bitmap貯存起來再一起回收
            // 找20張 高清風景照 > 20mb

            list = CSVLibrary.CSVHelper.Read<ExpenseModel>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv");
            dataGridView1.DataSource = list;
            dataGridView1.Columns["time"].ReadOnly = true;

            foreach (PropertyInfo property in typeof(ExpenseModel).GetProperties())
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
                    };
                    if (property.Name != "item")
                        comboBoxColumn.DataSource = typeof(AppData).GetField(property.Name).GetValue(null);
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
            trashCanColumn.DefaultCellStyle.NullValue = new Bitmap(@"C:\Users\User\source\repos\記帳本\記帳本\trashCan.jpg");

            dataGridView1.Columns.Add(trashCanColumn);

            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                string currentCat = dataGridView1.Rows[i].Cells["catagory"].Value.ToString();
                DataGridViewComboBoxCell itemCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells["itemComboBox"];
                itemCell.DataSource = AppData.expends[currentCat];
                foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                {
                    if (cell is DataGridViewImageCell imageCell && cell.OwningColumn.Name != "trashCan")
                    {
                        string cellName = imageCell.OwningColumn.Name.Replace("ImageBox", "");
                        string imagePath = dataGridView1.Rows[i].Cells[cellName].Value.ToString();
                        Bitmap picture = new Bitmap(imagePath);
                        dataGridView1.Rows[i].Cells[cellName + "ImageBox"].Value = picture;
                    }

                    if (cell is DataGridViewComboBoxCell comboBoxCell)
                    {
                        string columnName = comboBoxCell.OwningColumn.Name.Replace("ComboBox", "");
                        dataGridView1.Rows[i].Cells[columnName + "ComboBox"].Value = dataGridView1.Rows[i].Cells[columnName].Value;
                    }
                    // 0711 再看一次
                    // 0711 處理 oom 的問題 

                }
            }
        }
    }
}
