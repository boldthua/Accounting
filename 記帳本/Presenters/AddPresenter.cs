using CSVLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本;
using 記帳本.Contracts.Models;
using 記帳本.Contracts.Models.DTOs;
using 記帳本.Presenters;
using 記帳本.Repositories;
using 記帳本.Repositories.Appdatas;
using 記帳本.Repositories.Models;
using 記帳本.Utility;
using static System.Net.Mime.MediaTypeNames;
using static 記帳本.Contracts.AddRecordContract;
using Image = System.Drawing.Image;

namespace 記帳本.Presenters
{
    internal class AddPresenters : IAddPresenter
    {
        ICategoryRepository repository { get; set; }
        IRecordRepository recordRepository { get; set; }
        IAddView view { get; set; }
        public AddPresenters(IAddView view)
        {
            repository = new CategoryRepository();
            recordRepository = new RecordRepository();
            this.view = view;
        }
        public void SaveRecord(RecordDTO modelDTO)
        {
            RecordModel modelRecord = new RecordModel();

            modelRecord.Catagory = modelDTO.Catagory;
            modelRecord.Time = modelDTO.Time;
            modelRecord.Item = modelDTO.Item;
            modelRecord.Money = modelDTO.Money;
            modelRecord.Recipient = modelDTO.Recipient;

            //ICompressImage compress = CompressImageFactory.Create(xxxx);
            //Bitmap bitmap = compress.Compress(modelRecord.Image1);
            //IUploadFile uploadfile = UploadFileFactory.Create(UplodType.FTP);
            //uploadfile.Upload(bitmap,filePath);

            Image image1 = modelDTO.Picture1;
            Image compressedPic1 = CompressImageTool.CompressImage(image1);
            string picName1 = $"{Guid.NewGuid()}.jpg";

            string savePath1 = ConfigurationManager.AppSettings["DocumentPath"];
            string saveDay = modelRecord.Time;
            savePath1 = Path.Combine(savePath1, saveDay);
            if (!Directory.Exists(savePath1))
            {
                Directory.CreateDirectory(savePath1);
            }

            savePath1 = Path.Combine(savePath1, picName1);
            compressedPic1.Save(savePath1);
            modelRecord.Picture1 = savePath1;

            Image image2 = modelDTO.Picture2;
            Image compressedPic2 = CompressImageTool.CompressImage(image2);
            string picName2 = $"{Guid.NewGuid()}.jpg";
            string savePath2 = ConfigurationManager.AppSettings["DocumentPath"];
            savePath2 = Path.Combine(savePath2, modelRecord.Time, picName2);
            compressedPic2.Save(savePath2);
            modelRecord.Picture2 = savePath2;

            bool isSuccess = recordRepository.AddRecord(modelRecord);

            // 先縮圖，然後DAO裝的是縮圖路徑


        }
        public void GetAppDatas()
        {
            List<string> majorCat = repository.GetCategories();
            List<string> item = repository.GetSubcategories(majorCat[0]);
            List<string> recipient = repository.GetRecipients();

            ComboBoxData data = new ComboBoxData(majorCat, item, recipient);
            view.PopulateComboBox(data);
        }

        public void GetSubcategories(string category)
        {
            List<string> items = repository.GetSubcategories(category);
            view.PopulateItemComboBox(items);
        }


    }
}




