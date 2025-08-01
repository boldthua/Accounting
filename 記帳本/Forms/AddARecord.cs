using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;
using static 記帳本.Contracts.AddRecordContract;

namespace 記帳本
{
    [DisplayName("記一筆")]
    [Order(3)]
    public partial class AddARecord : Form, IAddView
    {
        string picture1Location = "C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg";
        string picture2Location = "C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg";

        public AddARecord()
        {
            InitializeComponent();


            // AppDataPresenter.GetComboBoxDatas();
            // 
            comboBox1.DataSource = AppData.catagory;
            comboBox2.DataSource = AppData.food;
            comboBox3.DataSource = AppData.recipient;
            pictureBox1.Image = Image.FromFile(@picture1Location);
            pictureBox2.Image = Image.FromFile(@picture2Location);

        }
        // public void RenderComboBoxDatas(ComboBoxData data){
        //
        //   comboBox1.DataSource = data.catagory;
        //   comboBox2.DataSource = data.food;
        //   comboBox3.DataSource = data.recipient;
        // }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string catagory = comboBox1.SelectedItem.ToString();
            comboBox2.DataSource = AppData.expends[catagory];
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

            ExpenseModel anItem = new ExpenseModel();
            anItem.time = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            anItem.money = textBox1.Text;
            anItem.catagory = comboBox1.Text;
            anItem.item = comboBox2.Text;
            anItem.recipient = comboBox3.Text;

            // 先檢查有沒有資料指定儲存日的資料夾，沒有就新增
            string saveDay = anItem.time;
            string path = @"C:\Users\User\source\repos\記帳本\記帳本\Datas\" + saveDay;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 先指定 壓縮圖片 和 縮小圖片 的位址。

            string picture1Path = $"{path}\\w40_{Guid.NewGuid()}.jpg";
            string compressed1Path = picture1Path.Replace("w40_", "");

            CompressPictureNSave(pictureBox1.Image, compressed1Path);
            ResizePictureNSave(pictureBox1.Image, picture1Path);
            anItem.picture1 = picture1Path;

            string picture2Path = $"{path}\\w40_{Guid.NewGuid()}.jpg";
            string compressed2Path = picture2Path.Replace("w40_", "");

            CompressPictureNSave(pictureBox2.Image, compressed2Path);
            ResizePictureNSave(pictureBox2.Image, picture2Path);
            anItem.picture2 = picture2Path;

            CSVLibrary.CSVHelper.Write<ExpenseModel>(Path.Combine(path, "record.csv"), anItem, true);

            MessageBox.Show("已儲存");
            pictureBox1.Image.Dispose();
            pictureBox2.Image.Dispose();
            pictureBox1.Image = Image.FromFile("C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg");
            pictureBox2.Image = Image.FromFile("C:\\Users\\User\\source\\repos\\記帳本\\記帳本\\UpLoad.jpg");
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

        public void ResizePictureNSave(Image image, string saveTo)
        {
            Bitmap originalImage = new Bitmap(image);
            int newWidth = 40;
            int newHeight = (int)(originalImage.Height * ((float)newWidth / originalImage.Width));

            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            resizedImage.Save(saveTo, ImageFormat.Jpeg);
        }

        public void CompressPictureNSave(Image image, string saveTo)
        {
            // Get a bitmap. The using statement ensures objects  
            // are automatically disposed from memory after use.
            ImageFormat imageFormat = image.RawFormat;
            using (Bitmap bmp1 = new Bitmap(image))
            {
                // 使用 GetEncoder 獲取對應的編碼器
                ImageCodecInfo encoder = GetEncoder(imageFormat);
                // Create an Encoder object based on the GUID  
                // for the Quality parameter category.  
                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.  
                // An EncoderParameters object has an array of EncoderParameter  
                // objects. In this case, there is only one  
                // EncoderParameter object in the array.  
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);//品質0到100分的中間值50分
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp1.Save(saveTo, encoder, myEncoderParameters);
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public void UpLoadResponse(bool result)
        {

            MessageBox.Show("已儲存");
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
