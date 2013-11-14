using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Models
{
    public class PageLink
    {
        public PageLink(string linkText, string actionName, string controllerName, string path = "")
        {
            this.LinkText = linkText;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Path = path;
        }
        public string ActionName;
        public string ControllerName;
        public string LinkText;
        public string Path;
    }
    public class SiteNavigation
    {
        public PageLink CurrentPage;
        public PageLink[] BreadCrumb;
        public PageLink[] TopLevelPages;
    }
}