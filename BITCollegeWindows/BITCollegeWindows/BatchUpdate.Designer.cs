namespace BITCollegeWindows
{
    partial class BatchUpdate
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.academicProgramComboBox = new System.Windows.Forms.ComboBox();
            this.academicProgramBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.lnkProcess = new System.Windows.Forms.LinkLabel();
            this.radSelect = new System.Windows.Forms.RadioButton();
            this.radAll = new System.Windows.Forms.RadioButton();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.academicProgramBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.academicProgramComboBox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.lnkProcess);
            this.groupBox1.Controls.Add(this.radSelect);
            this.groupBox1.Controls.Add(this.radAll);
            this.groupBox1.Location = new System.Drawing.Point(26, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(714, 169);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Batch Selection";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Program:";
            // 
            // academicProgramComboBox
            // 
            this.academicProgramComboBox.DataSource = this.academicProgramBindingSource;
            this.academicProgramComboBox.DisplayMember = "Description";
            this.academicProgramComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.academicProgramComboBox.FormattingEnabled = true;
            this.academicProgramComboBox.Location = new System.Drawing.Point(114, 94);
            this.academicProgramComboBox.Name = "academicProgramComboBox";
            this.academicProgramComboBox.Size = new System.Drawing.Size(242, 21);
            this.academicProgramComboBox.TabIndex = 5;
            this.academicProgramComboBox.ValueMember = "ProgramAcronym";
            // 
            // academicProgramBindingSource
            // 
            this.academicProgramBindingSource.DataSource = typeof(BITCollege_JL.Models.AcademicProgram);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(394, 59);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 19);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lnkProcess
            // 
            this.lnkProcess.AutoSize = true;
            this.lnkProcess.Location = new System.Drawing.Point(41, 133);
            this.lnkProcess.Name = "lnkProcess";
            this.lnkProcess.Size = new System.Drawing.Size(85, 15);
            this.lnkProcess.TabIndex = 2;
            this.lnkProcess.TabStop = true;
            this.lnkProcess.Text = "Process Batch";
            this.lnkProcess.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkProcess_LinkClicked);
            // 
            // radSelect
            // 
            this.radSelect.AutoSize = true;
            this.radSelect.Location = new System.Drawing.Point(44, 59);
            this.radSelect.Name = "radSelect";
            this.radSelect.Size = new System.Drawing.Size(246, 19);
            this.radSelect.TabIndex = 1;
            this.radSelect.TabStop = true;
            this.radSelect.Text = "Select a Program to Grade and Register";
            this.radSelect.UseVisualStyleBackColor = true;
            // 
            // radAll
            // 
            this.radAll.AutoSize = true;
            this.radAll.Location = new System.Drawing.Point(44, 36);
            this.radAll.Name = "radAll";
            this.radAll.Size = new System.Drawing.Size(233, 19);
            this.radAll.TabIndex = 0;
            this.radAll.TabStop = true;
            this.radAll.Text = "Grade and Register for ALL Programs";
            this.radAll.UseVisualStyleBackColor = true;
            this.radAll.CheckedChanged += new System.EventHandler(this.radAll_CheckedChanged);
            // 
            // rtxtLog
            // 
            this.rtxtLog.Location = new System.Drawing.Point(26, 239);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.Size = new System.Drawing.Size(714, 169);
            this.rtxtLog.TabIndex = 1;
            this.rtxtLog.Text = "";
            // 
            // BatchUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtxtLog);
            this.Controls.Add(this.groupBox1);
            this.Name = "BatchUpdate";
            this.Text = "Batch Student Update";
            this.Load += new System.EventHandler(this.BatchUpdate_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.academicProgramBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkProcess;
        private System.Windows.Forms.RadioButton radSelect;
        private System.Windows.Forms.RadioButton radAll;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox academicProgramComboBox;
        private System.Windows.Forms.BindingSource academicProgramBindingSource;
    }
}