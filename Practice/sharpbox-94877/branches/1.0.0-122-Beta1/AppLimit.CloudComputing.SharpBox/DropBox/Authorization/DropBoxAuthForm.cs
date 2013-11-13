using System;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

using AppLimit.CloudComputing.SharpBox.DropBox.Helper;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Authorization
{
    internal partial class DropBoxAuthForm : WebBrowserDialog
    {
        private String _userName;
        private String _password;
        private String _callBack;
        private int _navCount;

        private DialogResult _localResult;

        private System.Timers.Timer _closeTimer;

        public override DialogResult ShowDialog(WebBrowserDialogParameter parameter)
        {
            DropBoxAuthorizationParameter p = parameter as DropBoxAuthorizationParameter;

            _userName = p.UserName;
            _password = p.Password;
            _callBack = p.CallbackUrl;
            _navCount = 0;            

            Browser.DocumentCompleted += Browser_DocumentCompleted;

            return base.ShowDialog(parameter);
        }
        
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Browser.DocumentCompleted -= Browser_DocumentCompleted;

            if (Browser.Document != null)
            {
                var form = Browser.Document.GetElementById("form");
                if (form != null)
                {
                    var user = Browser.Document.GetElementById("login_email");
                    if (user != null)
                        user.SetAttribute("value", _userName);

                    var password = Browser.Document.GetElementById("login_password");
                    if (password != null)
                        password.SetAttribute("value", _password);

                    //Finally we submit our form                
                    Browser.Navigated += Browser_Navigated;
                    form.InvokeMember("submit");

                    return;
                }
            }

            // at this point it could be we are directly redirected to 
            // our callback page in case of cached authentication
            var callback = new Uri(_callBack);
            if (e.Url.DnsSafeHost.Equals(callback.DnsSafeHost))
            {
                // ok we have a cached authentication
                NotifyForDelayedWindowClose(0, DialogResult.Yes);
            }
            else
            {
                // at this point we have an error                        
                // delayed close
                NotifyForDelayedWindowClose(10, DialogResult.No);
            }
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            DialogResult result = DialogResult.No;

            _navCount++;

            if (_navCount != 2)
                return;                                             
            else
            {
                var callback = new Uri(_callBack);
                result = e.Url.DnsSafeHost.Equals(callback.DnsSafeHost) ? DialogResult.Yes : DialogResult.No;
            }

            if (result != DialogResult.Yes)
                NotifyForDelayedWindowClose(10, result);
            else
                NotifyForDelayedWindowClose(0, result);
        }

        private void NotifyForDelayedWindowClose(int seconds, DialogResult result)
        {
            if (seconds == 0 )
            {
                DialogResult = result;
                Close();
            }
            else
            {
                // cache the result
                _localResult = result;

                // build a timer object of needed
                if (_closeTimer == null)
                    _closeTimer = new System.Timers.Timer(seconds * 1000);

                // attach this to a callbac
                _closeTimer.Elapsed += _closeTimer_Elapsed;
                
                // enable
                _closeTimer.Enabled = true;
            }
        }

        void _closeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // set the result
            DialogResult = _localResult;

            // close the form 
            Close();
        }
    }
}
