namespace TraCuuThongBaoPhatHanh
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.comboBoxYearTo = new System.Windows.Forms.ComboBox();
            this.textBoxTaxCode = new System.Windows.Forms.TextBox();
            this.checkBoxAutoOpenLatest = new System.Windows.Forms.CheckBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelNote = new System.Windows.Forms.Label();
            this.labelHyphen = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
            this.labelTaxCode = new System.Windows.Forms.Label();
            this.comboBoxYearFrom = new System.Windows.Forms.ComboBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.labelSerial = new System.Windows.Forms.Label();
            this.checkBoxHeadless = new System.Windows.Forms.CheckBox();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.textBoxTaxCode2 = new System.Windows.Forms.TextBox();
            this.buttonSubmit2 = new System.Windows.Forms.Button();
            this.labelTaxCode2 = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSubmit.BackColor = System.Drawing.SystemColors.Control;
            this.buttonSubmit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSubmit.Location = new System.Drawing.Point(229, 134);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(266, 27);
            this.buttonSubmit.TabIndex = 4;
            this.buttonSubmit.Text = "Tra cứu";
            this.buttonSubmit.UseVisualStyleBackColor = false;
            this.buttonSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);
            // 
            // comboBoxYearTo
            // 
            this.comboBoxYearTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxYearTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxYearTo.FormattingEnabled = true;
            this.comboBoxYearTo.Location = new System.Drawing.Point(424, 81);
            this.comboBoxYearTo.Name = "comboBoxYearTo";
            this.comboBoxYearTo.Size = new System.Drawing.Size(71, 24);
            this.comboBoxYearTo.TabIndex = 2;
            this.comboBoxYearTo.SelectedIndexChanged += new System.EventHandler(this.ComboBoxYearTo_SelectedValueChanged);
            // 
            // textBoxTaxCode
            // 
            this.textBoxTaxCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTaxCode.Location = new System.Drawing.Point(229, 27);
            this.textBoxTaxCode.Name = "textBoxTaxCode";
            this.textBoxTaxCode.Size = new System.Drawing.Size(266, 23);
            this.textBoxTaxCode.TabIndex = 0;
            this.textBoxTaxCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxTaxCode_KeyDown);
            // 
            // checkBoxAutoOpenLatest
            // 
            this.checkBoxAutoOpenLatest.AutoSize = true;
            this.checkBoxAutoOpenLatest.Checked = true;
            this.checkBoxAutoOpenLatest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoOpenLatest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxAutoOpenLatest.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBoxAutoOpenLatest.Location = new System.Drawing.Point(29, 140);
            this.checkBoxAutoOpenLatest.Name = "checkBoxAutoOpenLatest";
            this.checkBoxAutoOpenLatest.Size = new System.Drawing.Size(178, 17);
            this.checkBoxAutoOpenLatest.TabIndex = 3;
            this.checkBoxAutoOpenLatest.Text = "Tự động mở thông báo mới nhất";
            this.checkBoxAutoOpenLatest.UseVisualStyleBackColor = true;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelVersion.Location = new System.Drawing.Point(30, 10);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelVersion.Size = new System.Drawing.Size(28, 13);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "v.10";
            // 
            // labelNote
            // 
            this.labelNote.AutoSize = true;
            this.labelNote.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelNote.Location = new System.Drawing.Point(299, 10);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(200, 13);
            this.labelNote.TabIndex = 5;
            this.labelNote.Text = "Hỗ trợ và báo lỗi: dieppn@softdreams.vn";
            // 
            // labelHyphen
            // 
            this.labelHyphen.AutoSize = true;
            this.labelHyphen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHyphen.Location = new System.Drawing.Point(350, 79);
            this.labelHyphen.Name = "labelHyphen";
            this.labelHyphen.Size = new System.Drawing.Size(15, 16);
            this.labelHyphen.TabIndex = 3;
            this.labelHyphen.Text = "_";
            // 
            // labelYear
            // 
            this.labelYear.AutoSize = true;
            this.labelYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYear.Location = new System.Drawing.Point(26, 86);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(125, 16);
            this.labelYear.TabIndex = 3;
            this.labelYear.Text = "Thời gian phát hành";
            // 
            // labelTaxCode
            // 
            this.labelTaxCode.AutoSize = true;
            this.labelTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTaxCode.Location = new System.Drawing.Point(26, 30);
            this.labelTaxCode.Name = "labelTaxCode";
            this.labelTaxCode.Size = new System.Drawing.Size(78, 17);
            this.labelTaxCode.TabIndex = 3;
            this.labelTaxCode.Text = "Mã số thuế";
            // 
            // comboBoxYearFrom
            // 
            this.comboBoxYearFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxYearFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxYearFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxYearFrom.FormattingEnabled = true;
            this.comboBoxYearFrom.Location = new System.Drawing.Point(229, 81);
            this.comboBoxYearFrom.Name = "comboBoxYearFrom";
            this.comboBoxYearFrom.Size = new System.Drawing.Size(71, 24);
            this.comboBoxYearFrom.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(529, 220);
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxAutoOpenLatest);
            this.tabPage1.Controls.Add(this.textBoxTaxCode);
            this.tabPage1.Controls.Add(this.buttonSubmit);
            this.tabPage1.Controls.Add(this.comboBoxYearTo);
            this.tabPage1.Controls.Add(this.labelHyphen);
            this.tabPage1.Controls.Add(this.comboBoxYearFrom);
            this.tabPage1.Controls.Add(this.labelYear);
            this.tabPage1.Controls.Add(this.labelTaxCode);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(521, 194);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tra cứu thông báo phát hành";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.labelSerial);
            this.tabPage2.Controls.Add(this.checkBoxHeadless);
            this.tabPage2.Controls.Add(this.textBoxSerial);
            this.tabPage2.Controls.Add(this.textBoxTaxCode2);
            this.tabPage2.Controls.Add(this.buttonSubmit2);
            this.tabPage2.Controls.Add(this.labelTaxCode2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(521, 194);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tra cứu serial chứng thư";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // labelSerial
            // 
            this.labelSerial.AutoSize = true;
            this.labelSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerial.Location = new System.Drawing.Point(26, 86);
            this.labelSerial.Name = "labelSerial";
            this.labelSerial.Size = new System.Drawing.Size(43, 16);
            this.labelSerial.TabIndex = 8;
            this.labelSerial.Text = "Serial";
            // 
            // checkBoxHeadless
            // 
            this.checkBoxHeadless.AutoSize = true;
            this.checkBoxHeadless.Checked = true;
            this.checkBoxHeadless.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHeadless.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxHeadless.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBoxHeadless.Location = new System.Drawing.Point(29, 140);
            this.checkBoxHeadless.Name = "checkBoxHeadless";
            this.checkBoxHeadless.Size = new System.Drawing.Size(126, 17);
            this.checkBoxHeadless.TabIndex = 1;
            this.checkBoxHeadless.Text = "Không mở trình duyệt";
            this.checkBoxHeadless.UseVisualStyleBackColor = true;
            // 
            // textBoxSerial
            // 
            this.textBoxSerial.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSerial.Location = new System.Drawing.Point(229, 83);
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.ReadOnly = true;
            this.textBoxSerial.Size = new System.Drawing.Size(266, 23);
            this.textBoxSerial.TabIndex = 4;
            this.textBoxSerial.Text = "Không hỗ trợ thuế điện tử";
            // 
            // textBoxTaxCode2
            // 
            this.textBoxTaxCode2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxTaxCode2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTaxCode2.Location = new System.Drawing.Point(229, 27);
            this.textBoxTaxCode2.Name = "textBoxTaxCode2";
            this.textBoxTaxCode2.Size = new System.Drawing.Size(266, 23);
            this.textBoxTaxCode2.TabIndex = 0;
            this.textBoxTaxCode2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxTaxCode_KeyDown);
            // 
            // buttonSubmit2
            // 
            this.buttonSubmit2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSubmit2.BackColor = System.Drawing.SystemColors.Control;
            this.buttonSubmit2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubmit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSubmit2.Location = new System.Drawing.Point(229, 134);
            this.buttonSubmit2.Name = "buttonSubmit2";
            this.buttonSubmit2.Size = new System.Drawing.Size(266, 27);
            this.buttonSubmit2.TabIndex = 2;
            this.buttonSubmit2.Text = "Tra cứu";
            this.buttonSubmit2.UseVisualStyleBackColor = false;
            this.buttonSubmit2.Click += new System.EventHandler(this.ButtonSubmit2_Click);
            // 
            // labelTaxCode2
            // 
            this.labelTaxCode2.AutoSize = true;
            this.labelTaxCode2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTaxCode2.Location = new System.Drawing.Point(26, 30);
            this.labelTaxCode2.Name = "labelTaxCode2";
            this.labelTaxCode2.Size = new System.Drawing.Size(78, 17);
            this.labelTaxCode2.TabIndex = 6;
            this.labelTaxCode2.Text = "Mã số thuế";
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.labelNote);
            this.panelFooter.Controls.Add(this.labelVersion);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 209);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(529, 36);
            this.panelFooter.TabIndex = 5;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 245);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Công cụ tra cứu lưu hành nội bộ - Easy Invoice";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.panelFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.ComboBox comboBoxYearTo;
        private System.Windows.Forms.TextBox textBoxTaxCode;
        private System.Windows.Forms.Label labelTaxCode;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.CheckBox checkBoxAutoOpenLatest;
        private System.Windows.Forms.ComboBox comboBoxYearFrom;
        private System.Windows.Forms.Label labelHyphen;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.TextBox textBoxTaxCode2;
        private System.Windows.Forms.Button buttonSubmit2;
        private System.Windows.Forms.Label labelTaxCode2;
        private System.Windows.Forms.CheckBox checkBoxHeadless;
        private System.Windows.Forms.Label labelSerial;
        private System.Windows.Forms.TextBox textBoxSerial;
    }
}

