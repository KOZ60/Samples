namespace CustomDrawApp
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
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.textBoxEx1 = new CustomDrawApp.TextBoxEx();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.dateTimePickerEx1 = new CustomDrawApp.DateTimePickerEx();
            this.SuspendLayout();
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid2.Location = new System.Drawing.Point(316, 73);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.SelectedObject = this.textBoxEx1;
            this.propertyGrid2.Size = new System.Drawing.Size(252, 440);
            this.propertyGrid2.TabIndex = 0;
            // 
            // textBoxEx1
            // 
            this.textBoxEx1.CaretColor = System.Drawing.Color.Red;
            this.textBoxEx1.CaretWidth = 5;
            this.textBoxEx1.Font = new System.Drawing.Font("MS UI Gothic", 18F);
            this.textBoxEx1.Location = new System.Drawing.Point(316, 27);
            this.textBoxEx1.Name = "textBoxEx1";
            this.textBoxEx1.Size = new System.Drawing.Size(252, 31);
            this.textBoxEx1.TabIndex = 7;
            this.textBoxEx1.Text = "キャレット色変更テスト";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid1.Location = new System.Drawing.Point(12, 73);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.dateTimePickerEx1;
            this.propertyGrid1.Size = new System.Drawing.Size(252, 440);
            this.propertyGrid1.TabIndex = 6;
            // 
            // dateTimePickerEx1
            // 
            this.dateTimePickerEx1.BackColor = System.Drawing.Color.Green;
            this.dateTimePickerEx1.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePickerEx1.ForeColor = System.Drawing.SystemColors.Window;
            this.dateTimePickerEx1.Location = new System.Drawing.Point(12, 25);
            this.dateTimePickerEx1.Name = "dateTimePickerEx1";
            this.dateTimePickerEx1.ShowCheckBox = true;
            this.dateTimePickerEx1.ShowUpDown = true;
            this.dateTimePickerEx1.Size = new System.Drawing.Size(252, 31);
            this.dateTimePickerEx1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 517);
            this.Controls.Add(this.propertyGrid2);
            this.Controls.Add(this.textBoxEx1);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.dateTimePickerEx1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private DateTimePickerEx dateTimePickerEx1;
        private TextBoxEx textBoxEx1;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
    }
}

