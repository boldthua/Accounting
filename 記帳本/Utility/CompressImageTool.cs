using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 記帳本.Utility
{
    internal static class CompressImageTool
    {

        public static Image ResizePicture(Image image)
        {
            Bitmap originalImage = new Bitmap(image);
            int newWidth = 40;
            int newHeight = (int)(originalImage.Height * ((float)newWidth / originalImage.Width));

            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            MemoryStream memoryStream = new MemoryStream();
            resizedImage.Save(memoryStream, ImageFormat.Jpeg);
            return new Bitmap(memoryStream);
        }

        public static Image CompressImage(Image image)
        {
            ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg); // encoder 可以將png 轉成Jpeg
            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters encoderParameters = new EncoderParameters(1);
            EncoderParameter qualityParameter = new EncoderParameter(qualityEncoder, 1L); // 品質設為 50
            encoderParameters.Param[0] = qualityParameter;
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, encoder, encoderParameters);
            return new Bitmap(memoryStream);

            // 取得圖片原始格式
            //ImageFormat imageFormat = image.RawFormat;

            //// 使用 MemoryStream 儲存壓縮後的圖片數據
            //MemoryStream memoryStream = new MemoryStream();

            //// 創建 Bitmap 並確保資源釋放
            //Bitmap bmp = new Bitmap(image);

            //// 獲取對應的編碼器
            //ImageCodecInfo encoder = GetEncoder(imageFormat);

            //// 設定壓縮品質
            //System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            //EncoderParameters encoderParameters = new EncoderParameters(1);
            //EncoderParameter qualityParameter = new EncoderParameter(qualityEncoder, 50L); // 品質設為 50
            //encoderParameters.Param[0] = qualityParameter;

            //// 將壓縮後的圖片儲存到 MemoryStream
            //bmp.Save(memoryStream, encoder, encoderParameters);

            //// 從 MemoryStream 創建新的 Image 物件並返回
            //memoryStream.Position = 0;
            //return Image.FromStream(memoryStream);


        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
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

    }
}
