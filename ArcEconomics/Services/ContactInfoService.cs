using ArcEconomics.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ArcEconomics.Services
{
    public class ContactInfoService
    {
        private string FilePath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/ContactInfo.json"));
        public void Save(ContactInfo contactInfo)
        {            
            string json = JsonConvert.SerializeObject(contactInfo);
            File.WriteAllText(this.FilePath, json);
        }

        public ContactInfo GetContactInfo()
        {
            string json;
            json = null;
            if (File.Exists(this.FilePath))
            {
                json = File.ReadAllText(this.FilePath);
            }

            ContactInfo info = new ContactInfo();
            if(json != null)
            { 
                info = JsonConvert.DeserializeObject<ContactInfo>(json);
            }

            return info;           
        }
    }
}