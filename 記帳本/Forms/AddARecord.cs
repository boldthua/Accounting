using CSVLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Repositories.Appdatas;
using static 記帳本.Contracts.AddRecordContract;

namespace 記帳本
{
    [DisplayName("記一筆")]
    [Order(3)]
    public partial class AddARecord : Form, IAddView
    {
        IAddPresenter presenter { get; set; }


        public AddARecord()
        {
            InitializeComponent();
            presenter = new AddPresenters(this);
            presenter.GetAppDatas();

            pictureBox1.Image = Image.FromFile(ConfigurationManager.AppSettings["TrashCan"]);
            pictureBox2.Image = Image.FromFile(ConfigurationManager.AppSettings["TrashCan"]);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string category = comboBox1.SelectedItem.ToString();
            presenter.GetSubcategories(category);
        }

        private void pictureBoxs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "圖片檔|*.png;*.jpg;*.jpeg";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                PictureBox pictureBox = sender as PictureBox;
                pictureBox.Image.Dispose();
                pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                string imageLocation = openFileDialog.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ORM => Object Relaction Mapping  用物件操作資料
            //Library?

            RecordDTO anItem = new RecordDTO();
            anItem.Time = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            anItem.Money = textBox1.Text;
            anItem.Catagory = comboBox1.Text;
            anItem.Item = comboBox2.Text;
            anItem.Recipient = comboBox3.Text;
            anItem.Picture1 = new Bitmap(pictureBox1.Image);
            anItem.Picture2 = new Bitmap(pictureBox2.Image);

            presenter.SaveRecord(anItem);

            pictureBox1.Image.Dispose();
            pictureBox2.Image.Dispose();
            pictureBox1.Image = Image.FromFile(ConfigurationManager.AppSettings["TrashCan"]);
            pictureBox2.Image = Image.FromFile(ConfigurationManager.AppSettings["TrashCan"]);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            bool isNumeric = int.TryParse(str, out _);

            if (!isNumeric)
            {
                MessageBox.Show("只能輸入數字！");
                textBox1.Text = "";
            }
        }

        public void SaveResponse(bool result)
        {
            MessageBox.Show("已儲存");
        }

        public void PopulateComboBox(ComboBoxData data)
        {
            comboBox1.DataSource = data.category;
            comboBox2.DataSource = data.item;
            comboBox3.DataSource = data.recipient;
        }

        public void PopulateItemComboBox(List<string> items)
        {
            comboBox2.DataSource = items;
        }
    }
}
// 寫入用streamWriter 讀取用 StreamReader
// 存到csv
// csv是啥, 逗號分隔檔

// 日期, 金額, 類別, 項目, 對象, 圖片位址1, 圖片位址2



// 0518
// comboBox.DataSource = string[] 
// pictureBox1.Image = Image.FromFile(@"位址");
