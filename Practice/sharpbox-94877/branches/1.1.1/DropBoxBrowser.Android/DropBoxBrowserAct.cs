using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace DropBoxBrowser.Android
{
    [Activity(Label = "DropBoxBrowser", MainLauncher = true)]
    public class DropBoxBrowserAct : ListActivity
    {
        // CloudStorage
        DropBoxCredentials myCreds = null;
        CloudStorage myStorage = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                Log.V("SharpBox", "display Login activity");
                // Open login activity
                Intent intentLogin = new Intent();
                intentLogin.SetClass(this, typeof(LoginAct));
                StartActivityForResult(intentLogin, (int)RequestCode.REQUESTCODE_LOGIN);
            }
            catch (Exception exx)
            {
                Log.E("SharpBox", "Error: " + exx.ToString());
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Log.V("SharpBox", "Activity has ended");
            Log.V("SharpBox", "ResultCode: " + resultCode.ToString());

            if (resultCode == Result.Ok)
            {
                // switch from request code
                Log.V("SharpBox", "requestCode: " + requestCode.ToString());
                if (requestCode == (int)RequestCode.REQUESTCODE_LOGIN)
                {
                    Log.V("SharpBox", "LoginAct return");
                    // result from login activity
                    try
                    {
                        // retrieve data from intent
                        Bundle credBdl = data.GetBundleExtra("credentials");
                        if (credBdl != null)
                        {
                            Log.V("SharpBox", "Bundle data are present");
                            myCreds = new DropBoxCredentials();
                            myCreds.UserName = credBdl.GetString("user");
                            myCreds.Password = credBdl.GetString("password");
                            myCreds.ConsumerKey = "lnqnb8t03npy91q";
                            myCreds.ConsumerSecret = "so9kmxcd5annbno";

                            // Create storage object
                            Log.V("SharpBox", "Make CloudStorage");
                            myStorage = new CloudStorage();

                            // Open storage
                            Log.V("SharpBox", "Open storage");
                            myStorage.Open(DropBoxConfiguration.GetStandardConfiguration(), myCreds);

                            Log.V("SharpBox", "I'm logged");

                            List<string> dirs = new List<string>();
                            ICloudDirectoryEntry rootE = myStorage.GetRoot();
                            if (rootE != null)
                            {
                                foreach (ICloudFileSystemEntry fsentry in rootE)
                                {
                                    dirs.Add(fsentry.Name);
                                    /*if (fsentry is ICloudDirectoryEntry)
                                    {
                                        Log.V("SharpBox", "Found Directory: " + fsentry.Name);
                                    }
                                    else
                                    {
                                        Log.V("SharpBox", "File Directory: " + fsentry.Name);
                                    }*/
                                }
                            }
                            ListAdapter = new ArrayAdapter<string>(this, Resource.layout.main, dirs);
                        }
                        else
                        {
                            Log.V("SharpBox", "Nothing returned");
                        }
                    }
                    catch (Exception exx)
                    {
                        Log.E("SharpBox", "Error: " + exx.ToString());
                    }
                    finally
                    {
                        if (myStorage.IsOpened)
                        {
                            Log.V("SharpBox", "Close storage");
                            myStorage.Close();
                        }
                    }
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}