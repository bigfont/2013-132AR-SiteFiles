using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using AppLimit.CloudComputing.SharpBox;

namespace DropBoxBrowser.iPhone
{
	public class SharpBoxTableViewDataSource : UITableViewDataSource
	{
		protected ICloudDirectoryEntry _root;
		
		public SharpBoxTableViewDataSource (ICloudDirectoryEntry root)
		{
			_root = root;
		}
		
		#region implemented abstract members of MonoTouch.UIKit.UITableViewDataSource
		public override int RowsInSection (UITableView tableview, int section)
		{
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			if ( _root == null )
				return 0;
			else				
				return _root.Count;
		}
		
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			
			//---- declare vars
			string cellIdentifier = "SimpleCellTemplate";
			
			//---- try to grab a cell object from the internal queue
			var cell = tableView.DequeueReusableCell (cellIdentifier);

			//---- if there wasn't any available, just create a new one
			if (cell == null)
			{
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			}
			
			//---- set the cell properties
			cell.TextLabel.Text = GetChildName(indexPath.Row);
			
			if ( IsFile(indexPath.Row) ) 
				cell.Accessory = UITableViewCellAccessory.None;    
			else
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
		
			//---- return the cell
			return cell;	
		}
		
		public SharpBoxTableViewDataSource GetChildSource(int idx)
		{
		
			// get the child
			ICloudDirectoryEntry child = _root.GetChild(idx) as ICloudDirectoryEntry;
			if ( child == null )
				return null;
						
			// refresh the childs
			child.GetEnumerator();
			
			return new SharpBoxTableViewDataSource(child);
		}
		
		public String GetChildName(int idx)
		{
			//---- set the cell properties
			ICloudFileSystemEntry entry = _root.GetChild(idx);
			
			if ( entry != null )
			{
				return entry.Name;
			}
			else
			{
				return "n/a";
			}
		}						
		
		public Boolean IsFile(int idx)
		{
			//---- set the cell properties
			ICloudFileSystemEntry entry = _root.GetChild(idx);
			if ( entry == null )
				return true;
			
			if (entry is ICloudDirectoryEntry)
				return false;
			
			return true;			
		}
		#endregion		
	}
}

