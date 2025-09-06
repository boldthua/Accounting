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
    public class AddPresenters : IAddPresenter
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
        public void SaveRecord(AddRecordDTO modelDTO)
        {
           
            RecordModel modelRecord = Mapper.Map<AddRecordDTO, RecordModel>(modelDTO, x =>
            {
                //x.ForMember(a => modelDTO.Picture1, b => b.Ignore());
                //x.ForMember(a => modelDTO.Picture2, b => b.Ignore());
            });

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
            // 新增micro小圖並儲存
            Image microPic1 = CompressImageTool.ResizePicture(image1);


            string microPic1Path = Path.Combine(savePath1, "micro" + picName1);
            microPic1.Save(microPic1Path);
            // 儲存compressed縮圖
            string compressedPath1 = Path.Combine(savePath1, picName1);
            compressedPic1.Save(compressedPath1);
            // DAO路徑給micro小圖位址
            modelRecord.Picture1 = microPic1Path;

            Image image2 = modelDTO.Picture2;
            Image compressedPic2 = CompressImageTool.CompressImage(image2);
            string picName2 = $"{Guid.NewGuid()}.jpg";
            string savePath2 = ConfigurationManager.AppSettings["DocumentPath"];

            string microPic2Path = Path.Combine(savePath2, saveDay, "micro" + picName2);
            string compressedPath2 = Path.Combine(savePath2, saveDay, picName2);

            Image microPic2 = CompressImageTool.ResizePicture(image2);
            microPic2.Save(microPic2Path);

            compressedPic2.Save(compressedPath2);
            modelRecord.Picture2 = microPic2Path;





            bool isSuccess = recordRepository.AddRecord(modelRecord);

            // todo 釋放記憶體

            view.SaveResponse(isSuccess);
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




