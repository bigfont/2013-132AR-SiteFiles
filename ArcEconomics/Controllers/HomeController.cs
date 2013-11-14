using ArcEconomics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspDropBox.Core;
using AspDropBox.Core.Models;
using System.Configuration;
using ArcEconomics.Models;

namespace ArcEconomics.Controllers
{
    public class DropBoxService
    {
        private DropBoxCoreApi coreApi;
        public DropBoxService()
        {
            coreApi = GetCoreApi();
        }
        private DropBoxCoreApi GetCoreApi()
        {

            DropBoxCoreApi api;
            api = new DropBoxCoreApi(ConfigurationManager.AppSettings["DropBoxBearerCode"]);
            return api;

        }
        public DropBoxDirectory[] GetDirectoriesFromMetadata(Metadata metadata)
        {
            DropBoxDirectory[] dirs;

            dirs = metadata.Contents
                .Where<Metadata>(m => m.Is_Dir)
                .Select<Metadata, DropBoxDirectory>((m, d) => new DropBoxDirectory() { Path = m.Path, ChildDirectories = null })
                .ToArray<DropBoxDirectory>();

            return dirs;
        }
        private DropBoxFile[] GetFilesFromMetadata(Metadata metadata)
        {
            DropBoxFile[] files;

            files = metadata.Contents
                .Where<Metadata>(m => !m.Is_Dir)
                .Select<Metadata, DropBoxFile>((m, d) => new DropBoxFile() { Path = m.Path, PublicUrl = coreApi.GetShare(RootType.Sandbox, m.Path).Url })
                .ToArray<DropBoxFile>();

            return files;
        }
        public DropBoxDirectory GetRootDirectory()
        {
            return GetDirectoryByName(String.Empty);
        }
        public DropBoxDirectory GetDirectoryByName(string directoryName)
        {
            Metadata metadata;
            DropBoxDirectory dir;

            metadata = coreApi.GetMetadata(RootType.Sandbox, directoryName);

            // instantiate the current dir
            dir = new DropBoxDirectory();
            dir.Path = metadata.Path;
            dir.ChildDirectories = this.GetDirectoriesFromMetadata(metadata);
            dir.ChildFiles = this.GetFilesFromMetadata(metadata);

            return dir;
        }
    }
    public class HomeController : Controller
    {
        private SiteNavigation GetSiteNavigation(DropBoxDirectory currentDropBoxDir = null)
        {
            SiteNavigation nav = new SiteNavigation();

            nav.TopLevelPages = GetTopLevelPages();

            nav.CurrentPage = GetCurrentPage(nav.TopLevelPages, currentDropBoxDir);

            nav.BreadCrumb = GetBreadCrumb(nav.CurrentPage);

            return nav;
        }

        private PageLink GetCurrentPage(PageLink[] allTopLevelPageLinks, DropBoxDirectory currentDropBoxDir)
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
                currentPage = new PageLink(currentDropBoxDir.Name, "Directory", "Home", currentDropBoxDir.Path);

            }

            return currentPage;
        }

        private PageLink[] GetBreadCrumb(PageLink currentPage)
        {
            PageLink[] breadcrumb;

            string[] directoryNames = currentPage.Path.Split('/').Where<string>(s => s.Length > 0).ToArray<string>();
            breadcrumb = new PageLink[directoryNames.Length];

            // HACK I don't know how to do this in LINQ
            string path = String.Empty;
            for (int i = 0; i < directoryNames.Length; ++i)
            {
                path += "/" + directoryNames[i];
                breadcrumb[i] = new PageLink(directoryNames[i], "Directory", "Home", path);
            }

            return breadcrumb;
        }

        private PageLink[] GetTopLevelPages()
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

        public ActionResult Index()
        {
            HomeViewModel model;
            DropBoxService box;

            box = new DropBoxService();

            // populate viewmodel
            model = new HomeViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = GetSiteNavigation();

            // return view      
            return View(model);
        }

        public ActionResult About()
        {
            ViewModelBase model;
            DropBoxService box;

            box = new DropBoxService();

            model = new ViewModelBase();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = GetSiteNavigation();

            return View(model);
        }

        public ActionResult Directory(string path)
        {
            DirectoryViewModel model;
            DropBoxService box;

            box = new DropBoxService();

            // populate the root dir
            model = new DirectoryViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = box.GetDirectoryByName(path);
            model.SiteNavigation = GetSiteNavigation(model.CurrentDropBoxDirectory);

            // return view   
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewModelBase model;
            DropBoxService box;

            box = new DropBoxService();

            model = new ViewModelBase();

            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = GetSiteNavigation();

            return View(model);
        }
    }
}
