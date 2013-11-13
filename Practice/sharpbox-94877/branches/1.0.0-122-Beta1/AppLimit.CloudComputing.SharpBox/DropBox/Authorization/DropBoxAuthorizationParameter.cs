using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.CloudComputing.SharpBox.DropBox.Helper;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Authorization
{
    internal class DropBoxAuthorizationParameter : WebBrowserDialogParameter
    {        
        public String UserName;
        public String Password;
        public String CallbackUrl;        
    }
}
