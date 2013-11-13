using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    internal class PathHelper
    {
        private String _Path;

        static private char _Delimiter = '/';

        public PathHelper(String path)
        {
            _Path = path;
        }

        public Boolean IsPathRooted()
        {
            if (_Path.Length == 0 || _Path[0] != _Delimiter)
                return false;
            else
                return true;
        }

        public String[] GetPathElements()
        {            
            String WorkingPath;

            // remove heading /
            if (IsPathRooted())
                WorkingPath = _Path.Remove(0, 1);
            else
                WorkingPath = _Path;

            if (WorkingPath.Length == 0)
                return new String[0];
            else
                return WorkingPath.Split(_Delimiter);
        }
        
        public String GetDirectoryName()
        {
            int idx = _Path.LastIndexOf(_Delimiter);
            if (idx == 0)
                return "";
            else
                return _Path.Substring(0, idx);
        }

        public String GetFileName()
        {
            return Path.GetFileName(_Path);
        }
    }
}
