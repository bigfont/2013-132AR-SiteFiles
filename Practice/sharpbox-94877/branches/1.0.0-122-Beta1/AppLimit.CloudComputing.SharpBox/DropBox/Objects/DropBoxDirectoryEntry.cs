﻿using System;
using System.Collections.Generic;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox.Logic;

using Newtonsoft.Json.Linq;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
    internal class DropBoxDirectoryEntry : DropBoxFileSystemEntry, ICloudDirectoryEntry
    {
        private Dictionary<String, ICloudFileSystemEntry> _subDirectories;

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

        private void CreateSubItemsLazy()
        {
            // release old references
            if (_subDirectories != null)
                _subDirectories = null;
            
            // create item list
            _subDirectories = new Dictionary<string, ICloudFileSystemEntry>();

            // rebuild my self
            AssociatedApplication.FillUpChilds(AssociatedSession, this);                            
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override void BuildEntry(JObject json)
        {
            // build the base entry
            base.BuildEntry(json);
            
            // now take the content 
            var childs = json.SelectToken("contents") as JArray;
            if (childs == null || childs.Count <= 0) return;
            _subDirectories = new Dictionary<string, ICloudFileSystemEntry>();

            foreach (var item in childs)
            {
                var isDir = item.SelectToken("is_dir");
                if ( isDir == null )
                    continue;

                var trueValue = new JValue(true);
                ICloudFileSystemEntry fentry = null;

                if (trueValue.Equals(isDir))
                {
                    fentry = new DropBoxDirectoryEntry(AssociatedApplication, AssociatedSession, this, item.ToString());
                }
                else
                {
                    fentry = new DropBoxFileSystemEntry(AssociatedApplication, AssociatedSession, this, item.ToString());                    
                }

                _subDirectories.Add(fentry.Name, fentry);
            }
        }
        
        public ICloudFileSystemEntry GetChild(string name)
        {
            // create the subitems if needed
            CreateSubItemsLazy();

            // query subitems
            ICloudFileSystemEntry pe = null;
            _subDirectories.TryGetValue(name, out pe);

            // return
            return pe;
        }               
    }
}