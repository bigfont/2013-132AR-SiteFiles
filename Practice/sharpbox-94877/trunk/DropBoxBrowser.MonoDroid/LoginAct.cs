using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Util;

namespace DropBoxBrowser.MonoDroid
{
    [Activity(Label = "Login")]
    public class LoginAct : Activity
    {
        Button LoginBtnLogin = null;
        EditText txtUser = null;
        EditText txtPwd = null;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Verbose("SharpBox", "Create LoginAct activity");

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.login);

            LoginBtnLogin = FindViewById<Button>(Resource.Id.LoginBtnLogin);
            txtUser = FindViewById<EditText>(Resource.Id.LoginEditUser);
            txtPwd = FindViewById<EditText>(Resource.Id.LoginEditPwd);

            txtUser.TextChanged += new EventHandler<global::Android.Text.TextChangedEventArgs>(Credendials_TextChanged);
            txtPwd.TextChanged += new EventHandler<global::Android.Text.TextChangedEventArgs>(Credendials_TextChanged);

            LoginBtnLogin.Click += new EventHandler(LoginBtnLogin_Click);
        }

        void Credendials_TextChanged(object sender, EventArgs e)
        {
            Log.Verbose("SharpBox", "User: " + txtUser.Text.ToString());

            LoginBtnLogin.Enabled = (txtUser.Text.ToString().Length > 0) && (txtPwd.Text.ToString().Length > 0);
        }

        void LoginBtnLogin_Click(object sender, EventArgs e)
        {
            // create bundle
            Bundle bdl = new Bundle();
            bdl.PutString("user", txtUser.Text.ToString());
            bdl.PutString("password", txtPwd.Text.ToString());

            // create intent to return to parent
            Intent retIntent = new Intent();
            retIntent.PutExtra("credentials", bdl);
            
            // set activity result
            Log.Verbose("SharpBox", "Return OK");
            SetResult(Result.Ok, retIntent);
            // Close activity
            Finish();
        }
    }
}