using ArcEconomics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Services
{
    public class ContactInfoService
    {
        public void Save(ContactInfo contactInfo)
        { 
        
        }

        public ContactInfo GetContactInfo()
        { 
            return new ContactInfo() { FirstName = "Andy" };
        }
    }
}