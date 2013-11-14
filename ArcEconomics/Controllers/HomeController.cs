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
using ArcEconomics.Services;

namespace ArcEconomics.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            HomeViewModel model;
            DropBoxService box;
            SiteNavigationService nav;

            box = new DropBoxService();
            nav = new SiteNavigationService(this.ControllerContext);

            // populate viewmodel
            model = new HomeViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = nav.GetSiteNavigation();

            // return view      
            return View(model);
        }

        public ActionResult About()
        {
            ViewModelBase model;
            DropBoxService box;
            SiteNavigationService nav;

            box = new DropBoxService();
            nav = new SiteNavigationService(this.ControllerContext);

            model = new ViewModelBase();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = nav.GetSiteNavigation();

            return View(model);
        }

        public ActionResult Directory(string path)
        {
            DirectoryViewModel model;
            DropBoxService box;
            SiteNavigationService nav;

            box = new DropBoxService();
            nav = new SiteNavigationService(this.ControllerContext);

            // populate the root dir
            model = new DirectoryViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = box.GetDirectoryByName(path);
            model.SiteNavigation = nav.GetSiteNavigation(model.CurrentDropBoxDirectory);

            // return view   
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewModelBase model;
            DropBoxService box;
            SiteNavigationService nav;

            box = new DropBoxService();
            nav = new SiteNavigationService(this.ControllerContext);

            model = new ViewModelBase();

            model.RootDropBoxDirectory = box.GetRootDirectory();
            model.CurrentDropBoxDirectory = null;
            model.SiteNavigation = nav.GetSiteNavigation();

            return View(model);
        }
    }
}
