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

namespace 記帳本
{
    [DisplayName("記一筆")]
    [Order(3)]
    public partial class AddARecord : Form
    {
        string picture1Location = "C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg";
        string picture2Location = "C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg";

        public AddARecord()
        {
            InitializeComponent();

            comboBox1.DataSource = AppData.catagory;
            comboBox2.DataSource = AppData.food;
            comboBox3.DataSource = AppData.recipient;
            pictureBox1.Image = Image.FromFile(@picture1Location);
            pictureBox2.Image = Image.FromFile(@picture2Location);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string catagory = comboBox1.SelectedItem.ToString();
            comboBox2.DataSource = AppData.expends[catagory];
        }

        private void pictureBoxs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "圖片檔|*.png;*.jpg";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {

                PictureBox pictureBox = sender as PictureBox;
                pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                string imageLocation = openFileDialog.FileName;
                if (pictureBox.Name == pictureBox1.Name)
                {
                    picture1Location = imageLocation;
                }
                else
                {
                    picture2Location = imageLocation;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ORM => Object Relaction Mapping  用物件操作資料
            //Library?

            AccountingModel anItem = new AccountingModel();
            anItem.time = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            anItem.money = textBox1.Text;
            anItem.catagory = comboBox1.Text;
            anItem.item = comboBox2.Text;
            anItem.recipient = comboBox3.Text;

            string picture1Path = $@"C:\Users\User\source\repos\記帳本\記帳本\save\{Guid.NewGuid()}.jpg";
            pictureBox1.Image.Save(picture1Path);
            anItem.picture1 = picture1Path;

            string picture2Path = $@"C:\Users\User\source\repos\記帳本\記帳本\save\{Guid.NewGuid()}.jpg";
            pictureBox2.Image.Save(picture2Path);
            anItem.picture2 = picture2Path;

            CSVLibrary.CSVHelper.Write<AccountingModel>(@"C:\Users\User\source\repos\記帳本\記帳本\123.csv", anItem, true);
            MessageBox.Show("已儲存");

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
    }
}
// 寫入用streamWriter 讀取用 StreamReader
// 存到csv
// csv是啥, 逗號分隔檔

// 日期, 金額, 類別, 項目, 對象, 圖片位址1, 圖片位址2



// 0518
// comboBox.DataSource = string[] 
// pictureBox1.Image = Image.FromFile(@"位址");
