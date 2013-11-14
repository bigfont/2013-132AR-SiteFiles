using ArcEconomics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArcEconomics.Services
{
    public class SiteNavigationService
    {
        private ControllerContext ControllerContext;
        public SiteNavigationService(ControllerContext controllerContext)
        {
            this.ControllerContext = controllerContext;
        }
        public SiteNavigation GetSiteNavigation(DropBoxDirectory currentDropBoxDir = null)
        {
            SiteNavigation nav = new SiteNavigation();

            nav.TopLevelPages = GetTopLevelPages();

            nav.CurrentPage = GetCurrentPage(nav.TopLevelPages, currentDropBoxDir);

            nav.BreadCrumb = GetBreadCrumb(nav.CurrentPage);

            return nav;
        }
        public PageLink GetCurrentPage(PageLink[] allTopLevelPageLinks, DropBoxDirectory currentDropBoxDir)
        {
            PageLink currentPage;
            string currentAction;
            string currentController;

            currentAction = this.ControllerContext.RouteData.Values["action"].ToString();
            currentController = this.ControllerContext.RouteData.Values["controller"].ToString();

            currentPage = allTopLevelPageLinks.FirstOrDefault<PageLink>(pl =>
                pl.ControllerName.Equals(currentController) &&
                pl.ActionName.Equals(currentAction) &&
                (currentDropBoxDir == null || pl.Path.Equals(currentDropBoxDir.Path)));

            if (currentPage == null)
            {
                if (currentDropBoxDir != null)
                {
                    currentPage = new PageLink(currentDropBoxDir.Name, "Directory", "Home", currentDropBoxDir.Path);
                }
                else
                {
                    // HACK A nice-to-have is actually listing the admin pages.
                    currentPage = new PageLink("Admin Page", "Index", "Home");
                }
            }

            return currentPage;
        }
        public PageLink[] GetBreadCrumb(PageLink currentPage)
        {
            PageLink[] breadcrumb;

            breadcrumb = null;

            if (currentPage != null && currentPage.Path.IndexOf('/') >= 0)
            {
                string[] directoryNames = currentPage.Path.Split('/').Where<string>(s => s.Length > 0).ToArray<string>();
                breadcrumb = new PageLink[directoryNames.Length];

                // HACK I don't know how to do this in LINQ
                string path = String.Empty;
                for (int i = 0; i < directoryNames.Length; ++i)
                {
                    path += "/" + directoryNames[i];
                    breadcrumb[i] = new PageLink(directoryNames[i], "Directory", "Home", path);
                }
            }
            else if (currentPage != null)
            {
                breadcrumb = new PageLink[] { currentPage };
            }

            return breadcrumb;
        }
        public PageLink[] GetTopLevelPages()
        {
            PageLink[] hardPageLinks = new PageLink[] {
            
                new PageLink("Home", "Index", "Home"),
                new PageLink("About", "About", "Home"),
                new PageLink("Contact", "Contact", "Home")
                
            };

            DropBoxService box = new DropBoxService();
            PageLink[] directoryPageLinks = box.GetRootDirectory()
                .ChildDirectories.Select<DropBoxDirectory, PageLink>((d, pl) => new PageLink(d.Name, "Directory", "Home", d.Path))
                .ToArray<PageLink>();

            PageLink[] allTopLevelPageLinks = new PageLink[hardPageLinks.Length + directoryPageLinks.Length];
            Array.Copy(hardPageLinks, allTopLevelPageLinks, hardPageLinks.Length);
            Array.Copy(directoryPageLinks, 0, allTopLevelPageLinks, hardPageLinks.Length, directoryPageLinks.Length);

            return allTopLevelPageLinks;
        }
    }
}