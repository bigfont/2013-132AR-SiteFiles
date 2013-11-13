using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox;

namespace DropBoxBrowser.iPhone
{
	public delegate void MoveNextView(Object sender, EventArgs e);
	
	public partial class LoginWindow
	{
		private ICloudeStorageCredentials _cloudCreds;
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
			DropBoxCredentials cloudCreds = new DropBoxCredentials();
			cloudCreds.ConsumerKey = "1kqqci5fkiw5yd9";
			cloudCreds.ConsumerSecret = "qloes0saw19kpq6";
			cloudCreds.UserName = this.UserNameLabel.Text;
			cloudCreds.Password = this.PasswordLabel.Text;
			_cloudCreds = cloudCreds;
			
			_cloudConfig = DropBoxConfiguration.GetStandardConfiguration();
			
			// create storage
			_storage = new CloudStorage();
									
			// open the storage and load root items
			if ( !_storage.Open(_cloudConfig,_cloudCreds))
				return;
			
			if ( moveNextView != null )
				moveNextView(_storage.GetRoot(), new EventArgs());								
		}		
	}
}

