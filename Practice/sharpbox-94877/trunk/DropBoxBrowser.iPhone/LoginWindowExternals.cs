using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace DropBoxBrowser.iPhone
{
	public delegate void MoveNextView(Object sender, EventArgs e);
	
	public partial class LoginWindow
	{
		private ICloudStorageAccessToken _cloudCreds;
		private ICloudStorageConfiguration _cloudConfig;
		private CloudStorage _storage;
		
		public event MoveNextView moveNextView;
		
		public override void ViewDidLoad()
		{
			// add the login button handler
			LoginButton.TouchDown += HandleLoginButtonTouchDown;
		}
		
		void HandleLoginButtonTouchDown (object sender, EventArgs e)
		{						
			// build the standard dropbox config
			_cloudCreds = DropBoxStorageProviderTools.LoginWithMobileAPI(
				this.UserNameLabel.Text, this.PasswordLabel.Text, 
			"1kqqci5fkiw5yd9", "qloes0saw19kpq6");
			
			_cloudConfig = DropBoxConfiguration.GetStandardConfiguration();
			
			// create storage
			_storage = new CloudStorage();
									
			// open the storage and load root items
			_storage.Open(_cloudConfig,_cloudCreds);
			
			if ( moveNextView != null )
				moveNextView(_storage.GetRoot(), new EventArgs());								
		}		
	}
}

