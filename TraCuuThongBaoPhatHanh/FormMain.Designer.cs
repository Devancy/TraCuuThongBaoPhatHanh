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
            this.panelMain = new System.Windows.Forms.Panel();
            this.checkBoxAutoOpenLatest = new System.Windows.Forms.CheckBox();
            this.labelNote = new System.Windows.Forms.Label();
            this.labelDiviner = new System.Windows.Forms.Label();
            this.labelHyphen = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
            this.labelTaxCode = new System.Windows.Forms.Label();
            this.comboBoxYearFrom = new System.Windows.Forms.ComboBox();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSubmit.BackColor = System.Drawing.SystemColors.Control;
            this.buttonSubmit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSubmit.Location = new System.Drawing.Point(257, 143);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(197, 27);
            this.buttonSubmit.TabIndex = 3;
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
            this.comboBoxYearTo.Location = new System.Drawing.Point(383, 98);
            this.comboBoxYearTo.Name = "comboBoxYearTo";
            this.comboBoxYearTo.Size = new System.Drawing.Size(71, 24);
            this.comboBoxYearTo.TabIndex = 2;
            this.comboBoxYearTo.SelectedIndexChanged += new System.EventHandler(this.ComboBoxYearTo_SelectedValueChanged);
            // 
            // textBoxTaxCode
            // 
            this.textBoxTaxCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTaxCode.Location = new System.Drawing.Point(257, 50);
            this.textBoxTaxCode.Name = "textBoxTaxCode";
            this.textBoxTaxCode.Size = new System.Drawing.Size(197, 23);
            this.textBoxTaxCode.TabIndex = 0;
            this.textBoxTaxCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxTaxCode_KeyDown);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.checkBoxAutoOpenLatest);
            this.panelMain.Controls.Add(this.labelNote);
            this.panelMain.Controls.Add(this.labelDiviner);
            this.panelMain.Controls.Add(this.labelHyphen);
            this.panelMain.Controls.Add(this.labelYear);
            this.panelMain.Controls.Add(this.labelTaxCode);
            this.panelMain.Controls.Add(this.textBoxTaxCode);
            this.panelMain.Controls.Add(this.comboBoxYearFrom);
            this.panelMain.Controls.Add(this.comboBoxYearTo);
            this.panelMain.Controls.Add(this.buttonSubmit);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(495, 251);
            this.panelMain.TabIndex = 3;
            // 
            // checkBoxAutoOpenLatest
            // 
            this.checkBoxAutoOpenLatest.AutoSize = true;
            this.checkBoxAutoOpenLatest.Checked = true;
            this.checkBoxAutoOpenLatest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoOpenLatest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxAutoOpenLatest.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBoxAutoOpenLatest.Location = new System.Drawing.Point(34, 149);
            this.checkBoxAutoOpenLatest.Name = "checkBoxAutoOpenLatest";
            this.checkBoxAutoOpenLatest.Size = new System.Drawing.Size(178, 17);
            this.checkBoxAutoOpenLatest.TabIndex = 2;
            this.checkBoxAutoOpenLatest.Text = "Tự động mở thông báo mới nhất";
            this.checkBoxAutoOpenLatest.UseVisualStyleBackColor = true;
            // 
            // labelNote
            // 
            this.labelNote.AutoSize = true;
            this.labelNote.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelNote.Location = new System.Drawing.Point(138, 215);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(200, 13);
            this.labelNote.TabIndex = 5;
            this.labelNote.Text = "Hỗ trợ và báo lỗi: dieppn@softdreams.vn";
            // 
            // labelDiviner
            // 
            this.labelDiviner.AutoSize = true;
            this.labelDiviner.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.labelDiviner.Location = new System.Drawing.Point(16, 187);
            this.labelDiviner.Name = "labelDiviner";
            this.labelDiviner.Size = new System.Drawing.Size(463, 13);
            this.labelDiviner.TabIndex = 4;
            this.labelDiviner.Text = "____________________________________________________________________________";
            // 
            // labelHyphen
            // 
            this.labelHyphen.AutoSize = true;
            this.labelHyphen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHyphen.Location = new System.Drawing.Point(348, 97);
            this.labelHyphen.Name = "labelHyphen";
            this.labelHyphen.Size = new System.Drawing.Size(15, 16);
            this.labelHyphen.TabIndex = 3;
            this.labelHyphen.Text = "_";
            // 
            // labelYear
            // 
            this.labelYear.AutoSize = true;
            this.labelYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYear.Location = new System.Drawing.Point(31, 101);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(125, 16);
            this.labelYear.TabIndex = 3;
            this.labelYear.Text = "Thời gian phát hành";
            // 
            // labelTaxCode
            // 
            this.labelTaxCode.AutoSize = true;
            this.labelTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTaxCode.Location = new System.Drawing.Point(31, 55);
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
            this.comboBoxYearFrom.Location = new System.Drawing.Point(257, 98);
            this.comboBoxYearFrom.Name = "comboBoxYearFrom";
            this.comboBoxYearFrom.Size = new System.Drawing.Size(71, 24);
            this.comboBoxYearFrom.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 251);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tra cứu thông báo phát hành - Lưu hành nội bộ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.ComboBox comboBoxYearTo;
        private System.Windows.Forms.TextBox textBoxTaxCode;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelTaxCode;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label labelDiviner;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.CheckBox checkBoxAutoOpenLatest;
        private System.Windows.Forms.ComboBox comboBoxYearFrom;
        private System.Windows.Forms.Label labelHyphen;
    }
}

