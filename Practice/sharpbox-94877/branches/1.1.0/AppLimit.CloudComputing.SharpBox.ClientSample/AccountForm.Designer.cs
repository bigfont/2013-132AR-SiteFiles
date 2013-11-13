namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    partial class AccountForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.edtKey = new System.Windows.Forms.TextBox();
            this.edtSecret = new System.Windows.Forms.TextBox();
            this.loginControl1 = new AppLimit.CloudComputing.SharpBox.UI.LoginControl();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Consumer-Key:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Consumer-Secret:";
            // 
            // edtKey
            // 
            this.edtKey.Location = new System.Drawing.Point(163, 105);
            this.edtKey.Name = "edtKey";
            this.edtKey.Size = new System.Drawing.Size(182, 21);
            this.edtKey.TabIndex = 1;
            // 
            // edtSecret
            // 
            this.edtSecret.Location = new System.Drawing.Point(163, 131);
            this.edtSecret.Name = "edtSecret";
            this.edtSecret.Size = new System.Drawing.Size(182, 21);
            this.edtSecret.TabIndex = 2;
            // 
            // loginControl1
            // 
            this.loginControl1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginControl1.Location = new System.Drawing.Point(13, 13);
            this.loginControl1.Name = "loginControl1";
            this.loginControl1.Size = new System.Drawing.Size(332, 86);
            this.loginControl1.TabIndex = 0;
            // 
            // AccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 164);
            this.Controls.Add(this.loginControl1);
            this.Controls.Add(this.edtSecret);
            this.Controls.Add(this.edtKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AccountForm";
            this.Text = "AccountForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox edtKey;
        private System.Windows.Forms.TextBox edtSecret;
        private UI.LoginControl loginControl1;
    }
}