using ArcEconomics.Models;
using ArcEconomics.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArcEconomics.Services
{
    public class EditorService
    {
        private ControllerContext ControllerContext;
        public EditorService(ControllerContext controllerContext)
        {
            this.ControllerContext = controllerContext;
        }
        private string GetFilePathFromViewName(string actionName)
        {
            string filePath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/EditorData-{0}.html", actionName));
            return filePath;
        }
        public void Save(Editor editor)
        {
            string filePath = GetFilePathFromViewName(editor.ActionName);
            File.WriteAllText(filePath, editor.EditorData);            
        }

        public Editor GetEditor(string actionName)
        {
            Editor editor = new Editor();
            string editorText;
            string filePath;

            editorText = "Add some text here.";
            filePath = GetFilePathFromViewName(actionName);
            if (File.Exists(filePath))
            {
                editorText = File.ReadAllText(filePath);
            }

            editor.EditorData = editorText;
            editor.ActionName = actionName;

            return editor;
        }

        public EditorViewModel PopulateEditorViewModel()
        {
            EditorViewModel model;
            DropBoxService box;
            SiteNavigationService nav;
            ContactInfoService cSvc;

            string currentAction;

            currentAction = this.ControllerContext.RouteData.Values["action"].ToString();

            box = new DropBoxService();
            cSvc = new ContactInfoService();

            model = new EditorViewModel();

            if(this.ControllerContext != null)
            {
                nav = new SiteNavigationService(this.ControllerContext);
                model.SiteNavigation = nav.GetSiteNavigation();
            }
                        
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.Editor = GetEditor(currentAction);        
            model.ContactInfo = cSvc.GetContactInfo();    

            return model;
        }
    }
}