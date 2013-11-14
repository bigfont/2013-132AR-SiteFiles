using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Models
{
    public class PageLink
    { 
        public PageLink(string linkText, string actionName, string controllerName, string id = null)
        { 
            this.LinkText = linkText;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Id = id;
        }
        public string ActionName;
        public string ControllerName;
        public string LinkText;
        public string Id;
        public bool IsCurrentPage;
    }
    public class SiteNavigation
    {
        public PageLink[] Pages;
    }
}