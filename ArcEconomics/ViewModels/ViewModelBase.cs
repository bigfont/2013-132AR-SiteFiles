﻿using ArcEconomics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.ViewModels
{
    public class ViewModelBase
    {
        public ViewModelBase()
        {
            RootDropBoxDirectory = new DropBoxDirectory() { Path = "Root" };  // HACK Is this really Root?
        }
        public SiteNavigation SiteNavigation;
        public DropBoxDirectory RootDropBoxDirectory;
        public DropBoxDirectory CurrentDropBoxDirectory;
    }
}