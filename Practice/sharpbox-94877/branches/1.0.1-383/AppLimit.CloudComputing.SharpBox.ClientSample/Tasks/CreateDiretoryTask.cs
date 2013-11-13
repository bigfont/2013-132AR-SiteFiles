using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AppLimit.Common.Objects;
using AppLimit.Common.Toolkit.Controller.TaskController;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.ClientSample;

namespace AppLimit.CloudComputing.SharpBox.ClientSample.Tasks
{
    public class CreateDiretoryTask : CommonTask
    {
        public CreateDiretoryTask()
            : base("CREATE_DIR_SHARPBOX", CommonTaskType.CreationTask, "Create Directory", "Creates a directory in the cloud storage", null)
        {}

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

            EnterFileNameDialog dlg = new EnterFileNameDialog("Enter Folder-Name");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(dlg.FileName, fs.cloudFSEntry as ICloudDirectoryEntry);
                if (newFolder == null)
                    return false;

                ResultObjectTag = newFolder.Name;

                return true;
            }
            else
                return false;                        
        }
    }
}
