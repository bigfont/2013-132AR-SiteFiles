using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Models
{
    public class PageLink : IEquatable<PageLink>
    {
        public string ActionName;
        public string ControllerName;
        public string LinkText;
        public string Path;
        public PageLink(string linkText, string actionName, string controllerName, string path = "")
        {
            this.LinkText = linkText;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Path = path;
        }       
        public override int GetHashCode()
        {
            return ActionName.GetHashCode() ^ ControllerName.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as PageLink);
        }

        public bool Equals(PageLink other)
        {
            return (other != null &&
                    other.ActionName == this.ActionName &&
                    other.ControllerName == this.ControllerName &&
                    other.LinkText == this.LinkText && 
                    other.Path == this.Path);
        }
    }
    public class SiteNavigation
    {
        public PageLink CurrentPage;
        public PageLink[] BreadCrumb;
        public PageLink[] TopLevelPages;
    }
}