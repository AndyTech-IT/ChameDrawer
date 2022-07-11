
namespace ChameDrawer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Drawing_PBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Drawing_PBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Drawing_PBox
            // 
            this.Drawing_PBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Drawing_PBox.Location = new System.Drawing.Point(0, 0);
            this.Drawing_PBox.Name = "Drawing_PBox";
            this.Drawing_PBox.Size = new System.Drawing.Size(984, 561);
            this.Drawing_PBox.TabIndex = 0;
            this.Drawing_PBox.TabStop = false;
            this.Drawing_PBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Drawing_PBox_MouseDown);
            this.Drawing_PBox.MouseLeave += new System.EventHandler(this.Drawing_PBox_MouseLeave);
            this.Drawing_PBox.MouseHover += new System.EventHandler(this.Drawing_PBox_MouseHover);
            this.Drawing_PBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Drawing_PBox_MouseMove);
            this.Drawing_PBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Drawing_PBox_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.Drawing_PBox);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.UpdateDrawer);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.Drawing_PBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Drawing_PBox;
    }
}

