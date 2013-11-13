namespace AppLimit.CloudComputing.SharpBox.DropBoxApplicationAuthorization
{
    partial class Form1
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.edtAppKey = new System.Windows.Forms.TextBox();
			this.edtAppSecret = new System.Windows.Forms.TextBox();
			this.edtOutput = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnSearchFile = new System.Windows.Forms.Button();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.btnGo = new System.Windows.Forms.Button();
			this.btnTestToken = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Application Key:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Application Secret:";
			// 
			// edtAppKey
			// 
			this.edtAppKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtAppKey.Location = new System.Drawing.Point(114, 12);
			this.edtAppKey.Name = "edtAppKey";
			this.edtAppKey.Size = new System.Drawing.Size(1012, 20);
			this.edtAppKey.TabIndex = 2;
			// 
			// edtAppSecret
			// 
			this.edtAppSecret.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtAppSecret.Location = new System.Drawing.Point(114, 39);
			this.edtAppSecret.Name = "edtAppSecret";
			this.edtAppSecret.Size = new System.Drawing.Size(1012, 20);
			this.edtAppSecret.TabIndex = 3;
			// 
			// edtOutput
			// 
			this.edtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtOutput.Location = new System.Drawing.Point(114, 67);
			this.edtOutput.Name = "edtOutput";
			this.edtOutput.Size = new System.Drawing.Size(974, 20);
			this.edtOutput.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(47, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(61, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Output-File:";
			// 
			// btnSearchFile
			// 
			this.btnSearchFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSearchFile.Location = new System.Drawing.Point(1094, 65);
			this.btnSearchFile.Name = "btnSearchFile";
			this.btnSearchFile.Size = new System.Drawing.Size(32, 23);
			this.btnSearchFile.TabIndex = 6;
			this.btnSearchFile.Text = "...";
			this.btnSearchFile.UseVisualStyleBackColor = true;
			this.btnSearchFile.Click += new System.EventHandler(this.btnSearchFile_Click);
			// 
			// webBrowser
			// 
			this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser.Location = new System.Drawing.Point(12, 141);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(1114, 491);
			this.webBrowser.TabIndex = 7;
			// 
			// btnGo
			// 
			this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGo.Location = new System.Drawing.Point(1044, 93);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(82, 42);
			this.btnGo.TabIndex = 8;
			this.btnGo.Text = "Authorize";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// btnTestToken
			// 
			this.btnTestToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTestToken.Location = new System.Drawing.Point(956, 94);
			this.btnTestToken.Name = "btnTestToken";
			this.btnTestToken.Size = new System.Drawing.Size(82, 42);
			this.btnTestToken.TabIndex = 9;
			this.btnTestToken.Text = "Test Token";
			this.btnTestToken.UseVisualStyleBackColor = true;
			this.btnTestToken.Click += new System.EventHandler(this.btnTestToken_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1138, 644);
			this.Controls.Add(this.btnTestToken);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.webBrowser);
			this.Controls.Add(this.btnSearchFile);
			this.Controls.Add(this.edtOutput);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.edtAppSecret);
			this.Controls.Add(this.edtAppKey);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "DropBox Token Authorization Tool ";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edtAppKey;
        private System.Windows.Forms.TextBox edtAppSecret;
        private System.Windows.Forms.TextBox edtOutput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSearchFile;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnTestToken;
    }
}

