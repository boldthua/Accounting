using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        List<Expense> list;

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
                    CSVLibrary.CSVHelper.Write<Expense>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv", list, true);

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
            if (e.ColumnIndex == dataGridView1.Columns["catagoryComboBox"].Index)
            {
                string currentCat = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
                cell.DataSource = AppData.expends[currentCat];
                cell.Value = AppData.expends[currentCat][0];
            }
        }

        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView.CurrentCell.ColumnIndex == dataGridView1.Columns["catagoryComboBox"].Index)
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
            GC.Collect();

            list = CSVLibrary.CSVHelper.Read<Expense>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv");
            dataGridView1.DataSource = list;

            DataGridViewComboBoxColumn catagoryColumn = new DataGridViewComboBoxColumn()
            {
                HeaderText = "類別",
                Name = "catagoryComboBox",
            };
            catagoryColumn.DataSource = AppData.catagory;
            DataGridViewComboBoxColumn itemColumn = new DataGridViewComboBoxColumn()
            {
                HeaderText = "項目",
                Name = "itemComboBox",
            };
            DataGridViewComboBoxColumn recipientColumn = new DataGridViewComboBoxColumn()
            {
                HeaderText = "對象",
                Name = "recipientComboBox",
            };
            recipientColumn.DataSource = AppData.recipient;
            dataGridView1.Columns.Add(catagoryColumn);
            dataGridView1.Columns.Add(itemColumn);
            dataGridView1.Columns.Add(recipientColumn);

            DataGridViewImageColumn imageColumn1 = new DataGridViewImageColumn()
            {
                HeaderText = "單據1",
                Name = "receipt1",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            DataGridViewImageColumn imageColumn2 = new DataGridViewImageColumn()
            {
                HeaderText = "單據2",
                Name = "receipt2",
                ImageLayout = DataGridViewImageCellLayout.Zoom,

            };
            dataGridView1.Columns.Add(imageColumn1);
            dataGridView1.Columns.Add(imageColumn2);

            DataGridViewImageColumn trashCanColumn = new DataGridViewImageColumn()
            {
                HeaderText = "刪除",
                Name = "trashCan",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            dataGridView1.Columns.Add(trashCanColumn);

            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                string image1Path = dataGridView1.Rows[i].Cells["picture1"].Value.ToString();
                string image2Path = dataGridView1.Rows[i].Cells["picture2"].Value.ToString();
                dataGridView1.Rows[i].Cells["receipt1"].Value = new Bitmap(image1Path);
                dataGridView1.Rows[i].Cells["receipt2"].Value = new Bitmap(image2Path);
                dataGridView1.Rows[i].Cells["trashCan"].Value = new Bitmap(@"C:\Users\User\source\repos\記帳本\記帳本\trashCan.jpg");

                string currentCat = dataGridView1.Rows[i].Cells["catagory"].Value.ToString();
                dataGridView1.Rows[i].Cells["catagoryComboBox"].Value = currentCat;
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells["itemComboBox"];
                cell.DataSource = AppData.expends[currentCat];
                dataGridView1.Rows[i].Cells["itemComboBox"].Value = dataGridView1.Rows[i].Cells["item"].Value;
                dataGridView1.Rows[i].Cells["recipientComboBox"].Value = dataGridView1.Rows[i].Cells["recipient"].Value;
            }
            dataGridView1.Columns["picture1"].Visible = false;
            dataGridView1.Columns["picture2"].Visible = false;
            dataGridView1.Columns["catagory"].Visible = false;
            dataGridView1.Columns["item"].Visible = false;
            dataGridView1.Columns["recipient"].Visible = false;
        }
    }
}
