using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcEconomics.Models
{
    public class DropBoxEntity
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                while(value.IndexOf("/") == 0)
                { 
                    value = value.Remove(0, 1);
                }
                name = value;
            }
        }

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