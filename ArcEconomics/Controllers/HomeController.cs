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
        public ActionResult Index()
        {
            HomeViewModel model;
            DropBoxService box;
                                    
            box = new DropBoxService();

            // populate viewmodel
            model = new HomeViewModel();
            model.RootDropBoxDirectory = box.GetRootDirectory();

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

            // populate the current dir
            model.CurrentDropBoxDirectory = box.GetDirectoryByName(id);

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

            return View(model);
        }
    }
}
