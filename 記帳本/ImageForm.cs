using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 記帳本
{
    public partial class ImageForm : Form
    {
        public ImageForm(string imagePath)
        {
            InitializeComponent();

            // Dock > Fill
            // SizeMode > StretchImage
            pictureBox1.Image = Image.FromFile(imagePath);


        }

        private void ImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pictureBox1.Image.Dispose();
        }
    }
}
