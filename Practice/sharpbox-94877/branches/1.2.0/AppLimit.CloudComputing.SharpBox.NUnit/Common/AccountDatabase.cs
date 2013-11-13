using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    public class AccountDatabase
    {
        public readonly String User;
        public readonly String Password;

        public AccountDatabase(String User, String Password)
        {
            this.User = User;
            this.Password = Password;
        }

        public static AccountDatabase CreateByDatabase(String accountDb, String accountTag)
        {
            var doc = XElement.Load(accountDb);

            String User = "";
            String Password = "";

            var elems = doc.Elements();
            
            foreach(var e in elems)
            {
                XAttribute attr = e.Attribute("tag");
                if ( attr == null )
                    continue;
                
                if ( accountTag.Equals(attr.Value) )
                {
                    XAttribute attrUser = e.Attribute("user");
                    if (attrUser != null)
                       User = attrUser.Value;

                    XAttribute attrPwd = e.Attribute("password");
                    if (attrPwd != null)
                        Password = attrPwd.Value;

                    break;
                }                
            }

            return new AccountDatabase(User, Password);
        }
    }
}
