using System;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxService
    {
        public readonly DropBoxConfiguration Configuration;
        
        public DropBoxService()
        {
            Configuration = new DropBoxConfiguration();
        }

        public DropBoxApplication GetApplication(String consumerKey, String consumerSecret)
        {
            return new DropBoxApplication(this, consumerKey, consumerSecret);            
        }
    }
}
