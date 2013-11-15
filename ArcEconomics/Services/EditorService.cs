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
        public void Save(EditorViewModel editor)
        {
            string filePath = GetFilePathFromViewName(editor.ActionName);
            File.WriteAllText(filePath, editor.EditorData);            
        }

        public string GetEditorData(string actionName)
        {
            string editorText;
            string filePath;

            editorText = "Add some text here.";
            filePath = GetFilePathFromViewName(actionName);
            if (File.Exists(filePath))
            {
                editorText = File.ReadAllText(filePath);
            }

            return editorText;
        }

        public EditorViewModel PopulateEditorViewModel()
        {
            EditorViewModel model;
            DropBoxService box;
            SiteNavigationService nav;
            string currentAction;

            currentAction = this.ControllerContext.RouteData.Values["action"].ToString();

            box = new DropBoxService();

            model = new EditorViewModel();

            if(this.ControllerContext != null)
            {
                nav = new SiteNavigationService(this.ControllerContext);
                model.SiteNavigation = nav.GetSiteNavigation();
            }
                        
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.EditorData = GetEditorData(currentAction);
            model.ActionName = currentAction;

            return model;
        }
    }
}