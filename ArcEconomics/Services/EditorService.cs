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
            using (StreamWriter outfile = File.CreateText(filePath))
            {
                outfile.Write(editor.EditorData);
            }
        }

        public string Get(string actionName)
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

        public EditorViewModel PopulateEditorViewModel(string actionName)
        {
            EditorViewModel model;
            DropBoxService box;
            SiteNavigationService nav;
            EditorService editorSvc;

            box = new DropBoxService();
            nav = new SiteNavigationService(this.ControllerContext);
            editorSvc = new EditorService();

            model = new EditorViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = nav.GetSiteNavigation();
            model.EditorData = editorSvc.Get(actionName);
            model.ActionName = actionName;

            return model;
        }
    }
}