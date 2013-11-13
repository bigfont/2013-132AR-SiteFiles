using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    public partial class AccountForm : Form
    {
        public String User { get { return edtUser.Text; } }
        public String Password { get { return edtPassword.Text; } }
        public String ConsumerKey { get { return edtKey.Text; } }
        public String ConsumerSecret { get { return edtSecret.Text; } }

        public AccountForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
