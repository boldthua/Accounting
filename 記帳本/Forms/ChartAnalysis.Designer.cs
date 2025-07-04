using System.Windows.Forms;
using System;

namespace 記帳本
{
    partial class ChartAnalysis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.navBar1 = new NavBar();

            this.SuspendLayout();
            // 
            // navBar1
            // 
            this.navBar1.BackColor = System.Drawing.Color.White;
            this.navBar1.Location = new System.Drawing.Point(12, 613);
            this.navBar1.Name = "navBar1";
            this.navBar1.Size = new System.Drawing.Size(500, 100);
            this.navBar1.TabIndex = 0;
            // 
            // ChartAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(524, 725);
            this.Controls.Add(this.navBar1);
            this.Name = "ChartAnalysis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChartAnalysis";
            this.ResumeLayout(false);

        }

        #endregion

        private NavBar navBar1;
    }
}