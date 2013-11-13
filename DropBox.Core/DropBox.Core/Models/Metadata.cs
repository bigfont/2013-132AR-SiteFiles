using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspDropBox.Core.Models
{
    public class Metadata
    {
        public string Size;
        public string Rev;
        public string Thumb_Exists;
        public string Bytes;
        public string Modified;
        public string Client_Mtime;
        public string Path;
        public bool Is_Dir;
        public string Icon;
        public string Root;
        public Metadata[] Contents;
        public string Mime_Type;
        public string Revision;
    }
}
