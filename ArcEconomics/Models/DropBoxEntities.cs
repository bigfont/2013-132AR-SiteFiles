using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Models
{
    public class DropBoxEntity
    {        
        public string Name
        {
            get
            {
                string name; 
                int index;

                index = this.Path.LastIndexOf("/");
                name = this.Path.Substring(index + 1);

                return name;
            }
        }
        public string Path;

    }
    public class DropBoxFile : DropBoxEntity
    {
        public string PublicUrl { get; set; }
    }
    public class DropBoxDirectory : DropBoxEntity
    {
        public DropBoxDirectory[] ChildDirectories;
        public DropBoxFile[] ChildFiles;
    }
}