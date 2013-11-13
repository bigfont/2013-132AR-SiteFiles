using System;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    internal class PathHelper
    {
        private readonly String _path;

        public const char Delimiter = '/';

        public PathHelper(String path)
        {
            _path = path;
        }

        public Boolean IsPathRooted()
        {
        	return _path.Length != 0 && _path[0] == Delimiter;
        }

    	public String[] GetPathElements()
        {            
            String workingPath;

            // remove heading and trailing /
            workingPath = IsPathRooted() ? _path.Remove(0, 1) : _path;
            
            workingPath = workingPath.TrimEnd(Delimiter);

            return workingPath.Length == 0 ? new String[0] : workingPath.Split(Delimiter);
        }
        
        public String GetDirectoryName()
        {
        	int idx = _path.LastIndexOf(Delimiter);
        	return idx == 0 ? "" : _path.Substring(0, idx);
        }

    	public String GetFileName()
        {
            return Path.GetFileName(_path);
        }
    }
}
