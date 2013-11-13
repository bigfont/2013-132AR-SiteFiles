using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.Common.Objects;
using AppLimit.Common.Toolkit.Controller.TaskController;
using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.ClientSample.Tasks
{
    public class RemoveItemTask : CommonTask
    {
        public RemoveItemTask()
            : base("REMOVE_ITEM_SHARPBOX", CommonTaskType.RemoveTask, "Remove Item", "Removes a file or directory", null)
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

            return cloudStorage.DeleteFileSystemEntry( fs.cloudFSEntry );
        }
    }
}
