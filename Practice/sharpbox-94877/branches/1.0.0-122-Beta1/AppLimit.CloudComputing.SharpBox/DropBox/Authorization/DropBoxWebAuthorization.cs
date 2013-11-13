using System;
using System.Windows.Forms;
using System.Threading;

using AppLimit.CloudComputing.SharpBox.DropBox.Helper;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Authorization
{   
    internal class DropBoxWebAuthorization
    {
        public Boolean ShowHidden;

        public DropBoxWebAuthorization()
        {
            ShowHidden = true;
        }

        public Boolean Authorize(String authorizeString, String userName, String password, String callbackUrl)
        {
            // fill the parameter class
            DropBoxAuthorizationParameter AuthParameter = new DropBoxAuthorizationParameter();

            AuthParameter.Url = authorizeString;
            AuthParameter.UserName = userName;
            AuthParameter.Password = password;
            AuthParameter.CallbackUrl = callbackUrl;

            AuthParameter.ShowHidden = ShowHidden;

            // perform auhtorization
            DialogResult r = WebBrowserDialog.ShowWebDialogInSeperateThread(typeof(DropBoxAuthForm), AuthParameter);
            return (r == DialogResult.Yes);
        }    
    }
}

 
 
 