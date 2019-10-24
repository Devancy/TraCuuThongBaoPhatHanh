namespace TraCuuThongBaoPhatHanh_v2
{
    partial class FormEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEntry));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanelInner = new System.Windows.Forms.TableLayoutPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelSourcePath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDateRange = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.panelMid = new System.Windows.Forms.Panel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.textBoxCaptcha = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyRight = new System.Windows.Forms.Label();
            this.textBoxTaxCodeList = new System.Windows.Forms.TextBox();
            this.buttonSelectDataSource = new System.Windows.Forms.Button();
            this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
            this.buttonF5 = new System.Windows.Forms.Button();
            this.pictureBoxCaptcha = new System.Windows.Forms.PictureBox();
            this.labelStep1 = new System.Windows.Forms.Label();
            this.labelStep2 = new System.Windows.Forms.Label();
            this.labelStep3 = new System.Windows.Forms.Label();
            this.labelStep4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tableLayoutPanelInner.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelMid.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.panelMain, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxTaxCodeList, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(876, 450);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.tableLayoutPanelInner);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(155, 4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(717, 442);
            this.panelMain.TabIndex = 0;
            // 
            // tableLayoutPanelInner
            // 
            this.tableLayoutPanelInner.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanelInner.ColumnCount = 1;
            this.tableLayoutPanelInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelInner.Controls.Add(this.panelTop, 0, 0);
            this.tableLayoutPanelInner.Controls.Add(this.panelMid, 0, 1);
            this.tableLayoutPanelInner.Controls.Add(this.panelBottom, 0, 2);
            this.tableLayoutPanelInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelInner.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelInner.Name = "tableLayoutPanelInner";
            this.tableLayoutPanelInner.RowCount = 3;
            this.tableLayoutPanelInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelInner.Size = new System.Drawing.Size(717, 442);
            this.tableLayoutPanelInner.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.labelStep2);
            this.panelTop.Controls.Add(this.labelStep1);
            this.panelTop.Controls.Add(this.labelSourcePath);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.labelDateRange);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.labelInstruction);
            this.panelTop.Controls.Add(this.dateTimePickerTo);
            this.panelTop.Controls.Add(this.dateTimePickerFrom);
            this.panelTop.Controls.Add(this.buttonSelectDataSource);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(5, 5);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(711, 186);
            this.panelTop.TabIndex = 0;
            // 
            // labelSourcePath
            // 
            this.labelSourcePath.AutoSize = true;
            this.labelSourcePath.Location = new System.Drawing.Point(228, 59);
            this.labelSourcePath.Name = "labelSourcePath";
            this.labelSourcePath.Size = new System.Drawing.Size(0, 13);
            this.labelSourcePath.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(464, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 28;
            this.label1.Text = "đến";
            // 
            // labelDateRange
            // 
            this.labelDateRange.AutoSize = true;
            this.labelDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateRange.Location = new System.Drawing.Point(61, 133);
            this.labelDateRange.Name = "labelDateRange";
            this.labelDateRange.Size = new System.Drawing.Size(183, 17);
            this.labelDateRange.TabIndex = 30;
            this.labelDateRange.Text = "Chọn thời gian thông báo từ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(61, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 31;
            this.label2.Text = "Hoặc chọn từ";
            // 
            // labelInstruction
            // 
            this.labelInstruction.AutoSize = true;
            this.labelInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstruction.Location = new System.Drawing.Point(61, 34);
            this.labelInstruction.Name = "labelInstruction";
            this.labelInstruction.Size = new System.Drawing.Size(498, 17);
            this.labelInstruction.TabIndex = 32;
            this.labelInstruction.Text = "Nhập danh sách mã số thuế cần tra cứu ở panel bên trái (theo dòng) - ưu tiên";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.CustomFormat = "dd/MM/yyyy";
            this.dateTimePickerTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerTo.Location = new System.Drawing.Point(538, 128);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(122, 23);
            this.dateTimePickerTo.TabIndex = 25;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.CustomFormat = "dd/MM/yyyy";
            this.dateTimePickerFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(303, 128);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(122, 23);
            this.dateTimePickerFrom.TabIndex = 26;
            // 
            // panelMid
            // 
            this.panelMid.Controls.Add(this.labelStep4);
            this.panelMid.Controls.Add(this.labelStep3);
            this.panelMid.Controls.Add(this.pictureBoxLoading);
            this.panelMid.Controls.Add(this.labelInfo);
            this.panelMid.Controls.Add(this.buttonF5);
            this.panelMid.Controls.Add(this.buttonExecute);
            this.panelMid.Controls.Add(this.pictureBoxCaptcha);
            this.panelMid.Controls.Add(this.textBoxCaptcha);
            this.panelMid.Controls.Add(this.label3);
            this.panelMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMid.Location = new System.Drawing.Point(5, 199);
            this.panelMid.Name = "panelMid";
            this.panelMid.Size = new System.Drawing.Size(711, 186);
            this.panelMid.TabIndex = 1;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(63)))), ((int)(((byte)(52)))));
            this.labelInfo.Location = new System.Drawing.Point(351, 130);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(20, 17);
            this.labelInfo.TabIndex = 36;
            this.labelInfo.Text = "...";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExecute.Location = new System.Drawing.Point(64, 121);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(86, 34);
            this.buttonExecute.TabIndex = 23;
            this.buttonExecute.Text = "Go";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.ButtonExecute_Click);
            // 
            // textBoxCaptcha
            // 
            this.textBoxCaptcha.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaptcha.Location = new System.Drawing.Point(303, 33);
            this.textBoxCaptcha.Name = "textBoxCaptcha";
            this.textBoxCaptcha.Size = new System.Drawing.Size(122, 23);
            this.textBoxCaptcha.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(61, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 17);
            this.label3.TabIndex = 29;
            this.label3.Text = "Nhập captcha từ hình bên";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.labelVersion);
            this.panelBottom.Controls.Add(this.labelCopyRight);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(5, 393);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(711, 44);
            this.panelBottom.TabIndex = 2;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(582, 16);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelVersion.Size = new System.Drawing.Size(78, 13);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version 2.0.0.0";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCopyRight
            // 
            this.labelCopyRight.AutoSize = true;
            this.labelCopyRight.Location = new System.Drawing.Point(43, 16);
            this.labelCopyRight.Name = "labelCopyRight";
            this.labelCopyRight.Size = new System.Drawing.Size(143, 13);
            this.labelCopyRight.TabIndex = 0;
            this.labelCopyRight.Text = "Copyright @ Softdreams JSC";
            // 
            // textBoxTaxCodeList
            // 
            this.textBoxTaxCodeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTaxCodeList.Location = new System.Drawing.Point(4, 4);
            this.textBoxTaxCodeList.Multiline = true;
            this.textBoxTaxCodeList.Name = "textBoxTaxCodeList";
            this.textBoxTaxCodeList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTaxCodeList.Size = new System.Drawing.Size(144, 442);
            this.textBoxTaxCodeList.TabIndex = 1;
            // 
            // buttonSelectDataSource
            // 
            this.buttonSelectDataSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonSelectDataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSelectDataSource.Image = ((System.Drawing.Image)(resources.GetObject("buttonSelectDataSource.Image")));
            this.buttonSelectDataSource.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSelectDataSource.Location = new System.Drawing.Point(155, 52);
            this.buttonSelectDataSource.Name = "buttonSelectDataSource";
            this.buttonSelectDataSource.Size = new System.Drawing.Size(71, 27);
            this.buttonSelectDataSource.TabIndex = 24;
            this.buttonSelectDataSource.Text = " File ";
            this.buttonSelectDataSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSelectDataSource.UseVisualStyleBackColor = true;
            this.buttonSelectDataSource.Click += new System.EventHandler(this.ButtonSelectDataSource_Click);
            // 
            // pictureBoxLoading
            // 
            this.pictureBoxLoading.Image = global::TraCuuThongBaoPhatHanh_v2.Properties.Resources.Pacman_0_6s_44px;
            this.pictureBoxLoading.Location = new System.Drawing.Point(303, 122);
            this.pictureBoxLoading.Name = "pictureBoxLoading";
            this.pictureBoxLoading.Size = new System.Drawing.Size(51, 34);
            this.pictureBoxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxLoading.TabIndex = 35;
            this.pictureBoxLoading.TabStop = false;
            this.pictureBoxLoading.Visible = false;
            // 
            // buttonF5
            // 
            this.buttonF5.Image = ((System.Drawing.Image)(resources.GetObject("buttonF5.Image")));
            this.buttonF5.Location = new System.Drawing.Point(508, 32);
            this.buttonF5.Name = "buttonF5";
            this.buttonF5.Size = new System.Drawing.Size(31, 28);
            this.buttonF5.TabIndex = 30;
            this.buttonF5.UseVisualStyleBackColor = true;
            this.buttonF5.Click += new System.EventHandler(this.ButtonF5_Click);
            // 
            // pictureBoxCaptcha
            // 
            this.pictureBoxCaptcha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCaptcha.Location = new System.Drawing.Point(538, 33);
            this.pictureBoxCaptcha.Name = "pictureBoxCaptcha";
            this.pictureBoxCaptcha.Size = new System.Drawing.Size(122, 26);
            this.pictureBoxCaptcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxCaptcha.TabIndex = 21;
            this.pictureBoxCaptcha.TabStop = false;
            // 
            // labelStep1
            // 
            this.labelStep1.AutoSize = true;
            this.labelStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep1.Location = new System.Drawing.Point(39, 36);
            this.labelStep1.Name = "labelStep1";
            this.labelStep1.Size = new System.Drawing.Size(20, 17);
            this.labelStep1.TabIndex = 34;
            this.labelStep1.Text = "1.";
            // 
            // labelStep2
            // 
            this.labelStep2.AutoSize = true;
            this.labelStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep2.Location = new System.Drawing.Point(43, 133);
            this.labelStep2.Name = "labelStep2";
            this.labelStep2.Size = new System.Drawing.Size(20, 17);
            this.labelStep2.TabIndex = 34;
            this.labelStep2.Text = "2.";
            // 
            // labelStep3
            // 
            this.labelStep3.AutoSize = true;
            this.labelStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep3.Location = new System.Drawing.Point(39, 36);
            this.labelStep3.Name = "labelStep3";
            this.labelStep3.Size = new System.Drawing.Size(20, 17);
            this.labelStep3.TabIndex = 34;
            this.labelStep3.Text = "3.";
            // 
            // labelStep4
            // 
            this.labelStep4.AutoSize = true;
            this.labelStep4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep4.Location = new System.Drawing.Point(39, 130);
            this.labelStep4.Name = "labelStep4";
            this.labelStep4.Size = new System.Drawing.Size(20, 17);
            this.labelStep4.TabIndex = 34;
            this.labelStep4.Text = "4.";
            // 
            // FormEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 450);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormEntry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tra cứu thông báo phát hành - Easy Invoice";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEntry_FormClosing);
            this.Load += new System.EventHandler(this.FormEntry_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.tableLayoutPanelInner.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMid.ResumeLayout(false);
            this.panelMid.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TextBox textBoxTaxCodeList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInner;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelSourcePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelDateRange;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Button buttonSelectDataSource;
        private System.Windows.Forms.Panel panelMid;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.PictureBox pictureBoxCaptcha;
        private System.Windows.Forms.TextBox textBoxCaptcha;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonF5;
        private System.Windows.Forms.PictureBox pictureBoxLoading;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelCopyRight;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelStep2;
        private System.Windows.Forms.Label labelStep1;
        private System.Windows.Forms.Label labelStep4;
        private System.Windows.Forms.Label labelStep3;
    }
}

