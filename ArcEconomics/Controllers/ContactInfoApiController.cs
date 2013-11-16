using ArcEconomics.Models;
using ArcEconomics.Services;
using ArcEconomics.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ArcEconomics.Controllers
{
    public class ContactInfoApiController : ApiController
    {
        public HttpResponseMessage Post(ContactInfo contactInfo)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            ContactInfoService cSvc = new ContactInfoService();
            cSvc.Save(contactInfo);

            return response;
        }

    }
}
