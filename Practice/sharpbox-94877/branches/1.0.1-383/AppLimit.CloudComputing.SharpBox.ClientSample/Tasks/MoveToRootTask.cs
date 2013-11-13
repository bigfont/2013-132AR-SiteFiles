using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.Common.Objects;
using AppLimit.Common.Toolkit.Controller.TaskController;
using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.ClientSample.Tasks
{
    public class MoveToRootTask : CommonTask
    {
        public MoveToRootTask()
            : base("MOVE_ITEM_SHARPBOX", CommonTaskType.Unknown, "Move to Root", "Moves the selected item into root", null)
        { }

        public override bool Execute(object TargetObject, object parameter, out string ResultObjectTag)
        {
            ResultObjectTag = null;

            var cloudStorage = parameter as CloudStorage;
            if (cloudStorage == null)
                return false;

            if (!cloudStorage.IsOpened)
                return false;

            var fs = TargetObject as CloudFileShim;
            if (fs == null)
                return false;

            return cloudStorage.MoveFileSystemEntry(fs.cloudFSEntry, cloudStorage.GetRoot() );            
        }
    }
}
