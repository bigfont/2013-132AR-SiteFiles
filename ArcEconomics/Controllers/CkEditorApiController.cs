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
    public class CkEditorApiController : ApiController
    {
        public HttpResponseMessage Post(Editor editor)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            EditorService editorSvc = new EditorService(null);
            editorSvc.Save(editor);

            return response;
        }

    }
}
