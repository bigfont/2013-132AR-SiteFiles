using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;

namespace DropBoxBrowser.iPhone
{
	public class SharpBoxTableViewDelegate : UITableViewDelegate
	{
		protected UINavigationController _naviController;		
		
		public SharpBoxTableViewDelegate (UINavigationController naviController)
		{
			_naviController = naviController;			
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// get the current datasource
			SharpBoxTableViewDataSource ds = tableView.DataSource as SharpBoxTableViewDataSource;
			if ( ds == null )
				return;
			
			// check if we have a file
			if ( ds.IsFile(indexPath.Row ) )
			    return;
			    
			// show the navigation bar
			_naviController.NavigationBarHidden = false;
			
			// build a new table view controller
			UITableViewController tc = new UITableViewController();
			tc.Title = ds.GetChildName(indexPath.Row);
			
			// adjust tje datasource and delegate of the new view
			UITableView tv = tc.View as UITableView;
			tv.DataSource = ds.GetChildSource(indexPath.Row);
			tv.Delegate = new SharpBoxTableViewDelegate(_naviController);
			
			// push the new controller
			_naviController.PushViewController(tc, true);
		}			                                                                      	
	}
}

