using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 記帳本.Attributes;
using 記帳本.Contracts.Models;

namespace 記帳本.Utility
{
    public static class DataGridViewExtension
    {
        public static DataGridView dataGridView { get; set; }

        public static void AddAdditionalColumn(this DataGridView view, PropertyInfo property, CategoryData data)
        {
            var attributes = property.GetCustomAttributes();
            if (attributes.Count() < 1)
                return;

            if (attributes.Any(x => x is ComboBoxColumnAttribute))
            {
                AddComboBoxColumn(view, property, data);
            }

            if (attributes.Any(x => x is ImageColumnAttribute))
            {
                AddImageColumn(view, property);
            }
        }

        public static void AddComboBoxColumn(DataGridView view, PropertyInfo property, CategoryData data)
        {
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn()
            {
                HeaderText = property.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                Name = property.Name + "ComboBox",
                DataPropertyName = property.Name
            };
            if (property.Name != "item")
                comboBoxColumn.DataSource = typeof(CategoryData).GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance).GetValue(data);
            view.Columns.Add(comboBoxColumn);
            view.Columns[property.Name].Visible = false;
        }

        public static void AddImageColumn(DataGridView view, PropertyInfo property)
        {
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn()
            {
                HeaderText = property.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                Name = property.Name + "ImageBox",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            view.Columns.Add(imageColumn);
            view.Columns[property.Name].Visible = false;
        }

        public static void AddImageColumn(this DataGridView view,string columnName, Bitmap bitmap,string headerText = "")
        {
            DataGridViewImageColumn trashCanColumn = new DataGridViewImageColumn()
            {
                HeaderText = String.IsNullOrEmpty(headerText) ? columnName : headerText,
                Name = columnName,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            //string path = ConfigurationManager.AppSettings["TrashCan"];
            //Bitmap trashPicture = new Bitmap(path);
            //bitmaps.Append(trashPicture);
            //trashCanColumn.DefaultCellStyle.NullValue = new Bitmap(trashPicture);
            trashCanColumn.DefaultCellStyle.NullValue = bitmap;
            view.Columns.Add(trashCanColumn);
        }

        public static void RowSetting(DataGridViewRow row, Queue<Bitmap> bitmaps)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell is DataGridViewImageCell imageCell && cell.OwningColumn.Name != "trashCan")
                {
                    ImageCellSetting(imageCell, row, bitmaps);
                }

                if (cell is DataGridViewComboBoxCell comboBoxCell)
                {
                    ComboBoxCellSetting(comboBoxCell, row);
                }
            }
        }

        public static void ImageCellSetting(DataGridViewImageCell imageCell, DataGridViewRow row, Queue<Bitmap> bitmaps)
        {
            string cellName = imageCell.OwningColumn.Name.Replace("ImageBox", "");
            string imagePath = row.Cells[cellName].Value.ToString();
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            MemoryStream memoryStream = new MemoryStream(imageBytes);
            Bitmap picture = new Bitmap(memoryStream); // 這行最浪費
            row.Cells[cellName + "ImageBox"].Value = picture;
            bitmaps.Append(picture);
        }

        public static void ComboBoxCellSetting(DataGridViewComboBoxCell comboBoxCell, DataGridViewRow row)
        {
            string cellName = comboBoxCell.OwningColumn.Name;
            string sourceColumn = comboBoxCell.OwningColumn.Name.Replace("ComboBox", "");
            row.Cells[cellName].Value = row.Cells[sourceColumn].Value;
        }
    }
}
