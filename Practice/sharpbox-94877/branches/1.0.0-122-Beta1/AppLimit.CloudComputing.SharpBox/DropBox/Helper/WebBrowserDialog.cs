using System;
using System.Windows.Forms;
using System.Threading;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Helper
{
    internal partial class WebBrowserDialog : Form
    {
        public static DialogResult ShowWebDialogInSeperateThread(Type dialogType, WebBrowserDialogParameter parameter)
        {
            // build parameter
            WebBrowserDialogParameterHelper p = new WebBrowserDialogParameterHelper();
            p.DlgType = dialogType;
            p.Parameters = parameter;

            var th = new Thread(DisplayThread);
            th.SetApartmentState(ApartmentState.STA);

            th.Start(p);
            th.Join();

            return (DialogResult)p.Result;
        }

        private static void DisplayThread(object parameter)
        {
            WebBrowserDialogParameterHelper dlgParameter = parameter as WebBrowserDialogParameterHelper;

            WebBrowserDialog dlg = Activator.CreateInstance(dlgParameter.DlgType != null ? dlgParameter.DlgType : typeof(WebBrowserDialog)) as WebBrowserDialog;

            if (dlgParameter.Parameters.ShowHidden)
            {
                dlg.ShowInTaskbar = false;
                dlg.Opacity = 0;
            }
            else
            {
                dlg.TopMost = true;
            }

            dlgParameter.Result = dlg.ShowDialog(dlgParameter.Parameters);
        }

        public WebBrowserDialog()
        {
            InitializeComponent();


        }

        public virtual DialogResult ShowDialog(WebBrowserDialogParameter parameter)
        {
            textBox1.Text = parameter.Url;

            Browser.Navigate(parameter.Url);

            return base.ShowDialog();
        }
        
        protected WebBrowser Browser
        {
            get { return webBrowser1; }
        }

        protected TextBox UrlField
        {
            get { return textBox1; }
        }        
    }
}
