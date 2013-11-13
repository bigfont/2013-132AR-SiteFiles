using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppLimit.CloudComputing.SharpBox.ClientSample.Tasks
{
    public partial class EnterFileNameDialog : Form
    {
        public String FileName
        {
            get { return edtFileName.Text; }
        }
        
        public EnterFileNameDialog()
        {
            InitializeComponent();
        }

        public EnterFileNameDialog(String Title)
            : this()
        {
            this.Text = Title;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
