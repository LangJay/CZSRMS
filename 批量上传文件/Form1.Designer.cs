namespace 批量上传文件
{
    partial class form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFile = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.packDirLab = new System.Windows.Forms.Label();
            this.excelDirLab = new System.Windows.Forms.Label();
            this.FileNumLab = new System.Windows.Forms.Label();
            this.curNumLab = new System.Windows.Forms.Label();
            this.BeginBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(22, 13);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(98, 23);
            this.btnFile.TabIndex = 0;
            this.btnFile.Text = "选择pack文件夹";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(22, 43);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "选择Excel文件";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // packDirLab
            // 
            this.packDirLab.AutoSize = true;
            this.packDirLab.Location = new System.Drawing.Point(126, 18);
            this.packDirLab.Name = "packDirLab";
            this.packDirLab.Size = new System.Drawing.Size(77, 12);
            this.packDirLab.TabIndex = 2;
            this.packDirLab.Text = "pack文件路径";
            // 
            // excelDirLab
            // 
            this.excelDirLab.AutoSize = true;
            this.excelDirLab.Location = new System.Drawing.Point(126, 48);
            this.excelDirLab.Name = "excelDirLab";
            this.excelDirLab.Size = new System.Drawing.Size(83, 12);
            this.excelDirLab.TabIndex = 3;
            this.excelDirLab.Text = "Excel文件路径";
            // 
            // FileNumLab
            // 
            this.FileNumLab.AutoSize = true;
            this.FileNumLab.Location = new System.Drawing.Point(22, 73);
            this.FileNumLab.Name = "FileNumLab";
            this.FileNumLab.Size = new System.Drawing.Size(53, 12);
            this.FileNumLab.TabIndex = 4;
            this.FileNumLab.Text = "文件数量";
            // 
            // curNumLab
            // 
            this.curNumLab.AutoSize = true;
            this.curNumLab.Location = new System.Drawing.Point(24, 103);
            this.curNumLab.Name = "curNumLab";
            this.curNumLab.Size = new System.Drawing.Size(35, 12);
            this.curNumLab.TabIndex = 5;
            this.curNumLab.Text = "第0个";
            // 
            // BeginBtn
            // 
            this.BeginBtn.Location = new System.Drawing.Point(564, 73);
            this.BeginBtn.Name = "BeginBtn";
            this.BeginBtn.Size = new System.Drawing.Size(75, 23);
            this.BeginBtn.TabIndex = 6;
            this.BeginBtn.Text = "开  始";
            this.BeginBtn.UseVisualStyleBackColor = true;
            this.BeginBtn.Click += new System.EventHandler(this.BeginBtn_Click);
            // 
            // form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 261);
            this.Controls.Add(this.BeginBtn);
            this.Controls.Add(this.curNumLab);
            this.Controls.Add(this.FileNumLab);
            this.Controls.Add(this.excelDirLab);
            this.Controls.Add(this.packDirLab);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnFile);
            this.Name = "form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label packDirLab;
        private System.Windows.Forms.Label excelDirLab;
        private System.Windows.Forms.Label FileNumLab;
        private System.Windows.Forms.Label curNumLab;
        private System.Windows.Forms.Button BeginBtn;
    }
}

