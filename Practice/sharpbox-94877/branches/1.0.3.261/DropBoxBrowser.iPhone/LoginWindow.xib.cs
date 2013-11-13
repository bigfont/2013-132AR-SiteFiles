
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace DropBoxBrowser.iPhone
{
	public partial class LoginWindow : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public LoginWindow (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public LoginWindow (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public LoginWindow () : base("LoginWindow", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
	}
}

