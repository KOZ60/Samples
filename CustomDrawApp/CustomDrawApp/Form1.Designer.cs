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
            this.dateTimePickerEx1 = new CustomDrawApp.DateTimePickerEx();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // dateTimePickerEx1
            // 
            this.dateTimePickerEx1.BackColor = System.Drawing.Color.Green;
            this.dateTimePickerEx1.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePickerEx1.ForeColor = System.Drawing.Color.Yellow;
            this.dateTimePickerEx1.Location = new System.Drawing.Point(12, 34);
            this.dateTimePickerEx1.Name = "dateTimePickerEx1";
            this.dateTimePickerEx1.ShowCheckBox = true;
            this.dateTimePickerEx1.ShowUpDown = true;
            this.dateTimePickerEx1.Size = new System.Drawing.Size(241, 31);
            this.dateTimePickerEx1.TabIndex = 1;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 303);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.dateTimePickerEx1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private DateTimePickerEx dateTimePickerEx1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

