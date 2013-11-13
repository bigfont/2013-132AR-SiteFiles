using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace DropBoxBrowser.MonoDroid
{
    [Activity(Label = "DropBoxBrowser", MainLauncher = true)]
    public class DropBoxBrowserAct : ListActivity
    {
        // CloudStorage        
        CloudStorage myStorage = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                Log.Verbose("SharpBox", "display Login activity");
                // Open login activity
                Intent intentLogin = new Intent();
                intentLogin.SetClass(this, typeof(LoginAct));
                StartActivityForResult(intentLogin, (int)RequestCode.REQUESTCODE_LOGIN);
            }
            catch (Exception exx)
            {
                Log.Error("SharpBox", "Error: " + exx.ToString());
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Log.Verbose("SharpBox", "Activity has ended");
            Log.Verbose("SharpBox", "ResultCode: " + resultCode.ToString());

            if (resultCode == Result.Ok)
            {
                // switch from request code
                Log.Verbose("SharpBox", "requestCode: " + requestCode.ToString());
                if (requestCode == (int)RequestCode.REQUESTCODE_LOGIN)
                {
                    Log.Verbose("SharpBox", "LoginAct return");
                    // result from login activity
                    try
                    {
                        // retrieve data from intent
                        Bundle credBdl = data.GetBundleExtra("credentials");
                        if (credBdl != null)
                        {
                            // create the token by user and password
                            Log.Verbose("SharpBox", "Create a usertoken");
                            ICloudStorageAccessToken token = DropBoxStorageProviderTools.LoginWithMobileAPI(
                                credBdl.GetString("user"), credBdl.GetString("password"),
                                "54y04klf7ovhw3q", "20c5pvfg182t65c");

                            // Create storage object
                            Log.Verbose("SharpBox", "Make CloudStorage");
                            myStorage = new CloudStorage();

                            // Open storage
                            Log.Verbose("SharpBox", "Open storage");
                            myStorage.Open(DropBoxConfiguration.GetStandardConfiguration(), token);

                            Log.Verbose("SharpBox", "I'm logged");

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
                            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.main, dirs);
                        }
                        else
                        {
                            Log.Verbose("SharpBox", "Nothing returned");
                        }
                    }
                    catch (Exception exx)
                    {
                        Log.Error("SharpBox", "Error: " + exx.ToString());
                    }
                    finally
                    {
                        if (myStorage.IsOpened)
                        {
                            Log.Verbose("SharpBox", "Close storage");
                            myStorage.Close();
                        }
                    }
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}