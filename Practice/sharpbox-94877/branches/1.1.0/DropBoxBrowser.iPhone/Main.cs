
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace DropBoxBrowser.iPhone
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{			
		private UITableViewController _mainTableViewController;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{					
			// If you have defined a view, add it here:			
			window.AddSubview(mainNavController.View);
			
			// attach event
			LoginWindow lw = mainNavController.TopViewController as LoginWindow;
			if ( lw == null )
				return false;
			
			lw.moveNextView += HandleLwmoveNextView;
			
			// show all
			window.MakeKeyAndVisible ();
			
			// return
			return true;
		}

		void HandleLwmoveNextView (object sender, EventArgs e)
		{
			ICloudDirectoryEntry root = sender as ICloudDirectoryEntry;
			if ( root != null )
			{
				_mainTableViewController = new UITableViewController();
				
				UITableView view = _mainTableViewController.View as UITableView;
				
				view.DataSource = new SharpBoxTableViewDataSource(root);
				view.Delegate = new SharpBoxTableViewDelegate(mainNavController);	
				
				mainNavController.NavigationBarHidden = false;
				mainNavController.PushViewController(_mainTableViewController, true);				
			}
		}		
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

