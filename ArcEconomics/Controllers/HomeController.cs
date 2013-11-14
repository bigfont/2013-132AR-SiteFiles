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
                .Select<Metadata, DropBoxDirectory>((m, d) => new DropBoxDirectory() { Name = m.Path, ChildDirectories = null })
                .ToArray<DropBoxDirectory>();

            return dirs;
        }
        private DropBoxFile[] GetFilesFromMetadata(Metadata metadata)
        {
            DropBoxFile[] files;

            files = metadata.Contents
                .Where<Metadata>(m => !m.Is_Dir)
                .Select<Metadata, DropBoxFile>((m, d) => new DropBoxFile() { Name = m.Path, PublicUrl = "TODO" })
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
            dir.Name = metadata.Path;
            dir.ChildDirectories = this.GetDirectoriesFromMetadata(metadata);
            dir.ChildFiles = this.GetFilesFromMetadata(metadata);

            return dir;
        }
    }
    public class HomeController : Controller
    {
        private SiteNavigation GetSiteNavigation(string id = "")
        {
            SiteNavigation nav = new SiteNavigation();

            string currentAction = this.ControllerContext.RouteData.Values["action"].ToString();
            string currentController = this.ControllerContext.RouteData.Values["controller"].ToString();

            PageLink[] hardPageLinks = new PageLink[] {
            
                new PageLink("Home", "Index", "Home"),
                new PageLink("About", "About", "Home"),
                new PageLink("Contact", "Contact", "Home")
                
            };

            // TODO Cache the dropbox meta for performance reasons else where query dropbox on every page load.
            DropBoxService box = new DropBoxService();
            PageLink[] directoryPageLinks = box.GetRootDirectory()
                .ChildDirectories.Select<DropBoxDirectory, PageLink>((d, pl) => new PageLink(d.Name, "Directory", "Home", d.Name))
                .ToArray<PageLink>();

            PageLink[] allPageLinks = new PageLink[hardPageLinks.Length + directoryPageLinks.Length];
            Array.Copy(hardPageLinks, allPageLinks, hardPageLinks.Length);
            Array.Copy(directoryPageLinks, 0, allPageLinks, hardPageLinks.Length, directoryPageLinks.Length);

            allPageLinks.FirstOrDefault<PageLink>(pl => pl.ControllerName.Equals(currentController) && pl.ActionName.Equals(currentAction)).IsCurrentPage = true;

            nav.Pages = allPageLinks;
            nav.CurrentPage = allPageLinks.FirstOrDefault<PageLink>(pl => 
                pl.ControllerName.Equals(currentController) && 
                pl.ActionName.Equals(currentAction) &&
                pl.Id.Equals(id));

            nav.CurrentPage.IsCurrentPage = true;            

            return nav;
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

        public ActionResult Directory(string id)
        {
            DirectoryViewModel model;
            DropBoxService box;

            box = new DropBoxService();

            // populate the root dir
            model = new DirectoryViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();            
            model.CurrentDropBoxDirectory = box.GetDirectoryByName(id);
            model.SiteNavigation = GetSiteNavigation(id);

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
