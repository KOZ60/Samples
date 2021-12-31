
namespace WindowsFormsApp2
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
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.ownerDrawTextBox1 = new WindowsFormsApp2.OwnerDrawTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ownerDrawTextBox2 = new WindowsFormsApp2.OwnerDrawTextBox();
            this.textEditor1 = new Koz.Windows.Forms.TextEditor();
            this.SuspendLayout();
            // 
            // ownerDrawTextBox1
            // 
            this.ownerDrawTextBox1.Font = new System.Drawing.Font("ＭＳ 明朝", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ownerDrawTextBox1.Location = new System.Drawing.Point(12, 12);
            this.ownerDrawTextBox1.Name = "ownerDrawTextBox1";
            this.ownerDrawTextBox1.Size = new System.Drawing.Size(429, 42);
            this.ownerDrawTextBox1.TabIndex = 1;
            this.ownerDrawTextBox1.Text = "I am a boy.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(447, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 43);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ownerDrawTextBox2
            // 
            this.ownerDrawTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ownerDrawTextBox2.Font = new System.Drawing.Font("ＭＳ 明朝", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ownerDrawTextBox2.Location = new System.Drawing.Point(12, 60);
            this.ownerDrawTextBox2.Name = "ownerDrawTextBox2";
            this.ownerDrawTextBox2.Size = new System.Drawing.Size(429, 42);
            this.ownerDrawTextBox2.TabIndex = 4;
            this.ownerDrawTextBox2.Text = "I am a boy.";
            this.ownerDrawTextBox2.Visible = false;
            // 
            // textEditor1
            // 
            this.textEditor1.Location = new System.Drawing.Point(25, 142);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(573, 419);
            this.textEditor1.TabIndex = 5;
            this.textEditor1.Text = "textEditor1";
            this.textEditor1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 589);
            this.Controls.Add(this.textEditor1);
            this.Controls.Add(this.ownerDrawTextBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ownerDrawTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OwnerDrawTextBox ownerDrawTextBox1;
        private System.Windows.Forms.Button button1;
        private OwnerDrawTextBox ownerDrawTextBox2;
        private Koz.Windows.Forms.TextEditor textEditor1;
    }
}

