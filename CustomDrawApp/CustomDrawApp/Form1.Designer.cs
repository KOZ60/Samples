﻿namespace CustomDrawApp
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.textBoxEx2 = new CustomDrawApp.TextBoxEx();
            this.textBoxEx1 = new CustomDrawApp.TextBoxEx();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.dateTimePickerEx1 = new CustomDrawApp.DateTimePickerEx();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("MS UI Gothic", 18F);
            this.dateTimePicker1.Location = new System.Drawing.Point(13, 105);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowCheckBox = true;
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(239, 31);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // textBoxEx2
            // 
            this.textBoxEx2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBoxEx2.CaretColor = System.Drawing.Color.Yellow;
            this.textBoxEx2.CaretWidth = 5;
            this.textBoxEx2.Location = new System.Drawing.Point(12, 227);
            this.textBoxEx2.Name = "textBoxEx2";
            this.textBoxEx2.Size = new System.Drawing.Size(239, 19);
            this.textBoxEx2.TabIndex = 5;
            this.textBoxEx2.TextChanged += new System.EventHandler(this.textBoxEx2_TextChanged);
            // 
            // textBoxEx1
            // 
            this.textBoxEx1.CaretColor = System.Drawing.Color.Red;
            this.textBoxEx1.CaretWidth = 5;
            this.textBoxEx1.Location = new System.Drawing.Point(13, 192);
            this.textBoxEx1.Name = "textBoxEx1";
            this.textBoxEx1.Size = new System.Drawing.Size(239, 19);
            this.textBoxEx1.TabIndex = 4;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid1.Location = new System.Drawing.Point(279, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.dateTimePickerEx1;
            this.propertyGrid1.Size = new System.Drawing.Size(252, 303);
            this.propertyGrid1.TabIndex = 2;
            // 
            // dateTimePickerEx1
            // 
            this.dateTimePickerEx1.BackColor = System.Drawing.Color.Green;
            this.dateTimePickerEx1.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePickerEx1.ForeColor = System.Drawing.SystemColors.Window;
            this.dateTimePickerEx1.Location = new System.Drawing.Point(12, 45);
            this.dateTimePickerEx1.Name = "dateTimePickerEx1";
            this.dateTimePickerEx1.ShowCheckBox = true;
            this.dateTimePickerEx1.ShowUpDown = true;
            this.dateTimePickerEx1.Size = new System.Drawing.Size(241, 31);
            this.dateTimePickerEx1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 263);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 19);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(120, 263);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 19);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 303);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxEx2);
            this.Controls.Add(this.textBoxEx1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.dateTimePickerEx1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DateTimePickerEx dateTimePickerEx1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private TextBoxEx textBoxEx1;
        private TextBoxEx textBoxEx2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

