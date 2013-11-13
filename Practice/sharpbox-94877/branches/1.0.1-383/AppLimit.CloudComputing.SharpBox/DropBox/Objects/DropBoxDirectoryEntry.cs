using System;
using System.Collections.Generic;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox.Logic;
using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
    internal class DropBoxDirectoryEntry : DropBoxFileSystemEntry, ICloudDirectoryEntry
    {
        private Dictionary<String, ICloudFileSystemEntry> _subDirectories;
		private List<ICloudFileSystemEntry> _subDirectoriesByIndex;
		
        public DropBoxDirectoryEntry(DropBoxApplication associatedApplication, DropBoxSession associatedSession, String name)
            : base(associatedApplication, associatedSession, name, null )
        {}

        public DropBoxDirectoryEntry(DropBoxApplication associatedApplication, DropBoxSession associatedSession, ICloudDirectoryEntry parent, String jsonText)
            : base(associatedApplication, associatedSession,parent, jsonText)
        {}

        public IEnumerator<ICloudFileSystemEntry> GetEnumerator()
        {
            CreateSubItemsLazy();

            return _subDirectories.Values.GetEnumerator();
        }

		public int Count 
		{
			get 
			{				
				CreateSubItemsLazy();
				
				return _subDirectories.Count;		
			}
		}
		
        private Boolean CreateSubItemsLazy()
        {
            // release old references
            if (_subDirectories != null)
                _subDirectories = null;
            
			if (_subDirectoriesByIndex != null)
                _subDirectoriesByIndex = null;
            
            // create item list
            _subDirectories = new Dictionary<string, ICloudFileSystemEntry>();
			_subDirectoriesByIndex = new List<ICloudFileSystemEntry>();
			
            // rebuild my self
            return AssociatedApplication.FillUpChilds(AssociatedSession, this);				
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override Boolean BuildEntry(JsonHelper json)
        {
            // build the base entry
            if ( false == base.BuildEntry(json) )
				return false;
            
            // now take the content 
			List<String> content = json.GetListProperty("contents");
			
			if ( content.Count == 0)
				return false;
			
			_subDirectories = new Dictionary<string, ICloudFileSystemEntry>();
			_subDirectoriesByIndex = new List<ICloudFileSystemEntry>();
			
			foreach(String jsonContent in content)
			{
				// parse the item
				JsonHelper jc = new JsonHelper();
				if (!jc.ParseJsonMessage(jsonContent))
					continue;
				
				// check if we have a directory
				Boolean isDir = jc.GetBooleanProperty("is_dir");
                
				ICloudFileSystemEntry fentry = null;

                if (isDir)
                {
                    fentry = new DropBoxDirectoryEntry(AssociatedApplication, AssociatedSession, this, jsonContent);
                }
                else
                {
                    fentry = new DropBoxFileSystemEntry(AssociatedApplication, AssociatedSession, this, jsonContent);                    
                }

                _subDirectories.Add(fentry.Name, fentry);
				_subDirectoriesByIndex.Add(fentry);
			}
					
			// go ahead
			return true;
        }
        
        public ICloudFileSystemEntry GetChild(string name)
        {
            // create the subitems if needed
            if( false == CreateSubItemsLazy())
				return null;

            // query subitems
            ICloudFileSystemEntry pe = null;
            _subDirectories.TryGetValue(name, out pe);

            // return
            return pe;
        }       
		
		public ICloudFileSystemEntry GetChild(int idx)
		{
			if ( idx < _subDirectoriesByIndex.Count )
				return _subDirectoriesByIndex[idx];
			else
				return null;	
		}
    }
}
