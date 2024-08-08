using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.TextField;
using WomenSafety.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WomenSafety.Controller;

namespace WomenSafety.Controller.Activities
{
    [Activity(Label = "Login")]
    public class Login : Activity, IValueEventListener, IOnFailureListener, IOnSuccessListener
    {
        private TextView goregister, reset;
        TextInputEditText username, password;
        private FirebaseAuth auth;
        Button procced;
        ProgressBar progBar;
        const string User_PREF_KEY = "UserKeyPref";
      //UserKeyGenerator keyGenerator;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);

            ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);
            if (prefs.GetString(User_PREF_KEY, "No User Signed in") != "No User Signed in")
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            username = FindViewById<TextInputEditText>(Resource.Id.username);
            password = FindViewById<TextInputEditText>(Resource.Id.edtpassword);
            procced = FindViewById<Button>(Resource.Id.procced);
            procced.Click += Procced_Click;
            reset = FindViewById<TextView>(Resource.Id.reset);
            reset.Click += Reset_Click;
            goregister = FindViewById<TextView>(Resource.Id.goregister);
            goregister.Click += Goregister_Click;
            // Create your application here
        }
        public void OnCancelled(DatabaseError error)
        {

        }
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                // Cache user key
                ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString(User_PREF_KEY, id);
                editor.Apply();

                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
        }
        
        private void Goregister_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(CreateAccount));
            StartActivity(i);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(forgotpassword));
            StartActivity(i);
        }


        private void Procced_Click(object sender, EventArgs e)
        {
            if (validfound() == true)
            {
                progBar.Visibility = ViewStates.Visible;
                auth = new firebase_Connection().GetFirebaseAuth();
                auth.SignInWithEmailAndPassword(username.Text.Trim(), password.Text)
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
            }
        }
        private bool validfound()
        {
            bool Result = true;

            if (string.IsNullOrEmpty(username.Text.Trim()))
            {
                Result = false;
                username.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(password.Text.Trim()))
            {
                Result = false;
                password.Error = "invalid input";
            }

            return Result;
        }
        string id="";
        public void OnSuccess(Java.Lang.Object result)
        {
            id = auth.CurrentUser.Uid;
            firebase_Connection.GetDatabase().GetReference("WomenSafetyUsers").Child(id.Trim())
              .AddValueEventListener(this);
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progBar.Visibility = ViewStates.Gone;
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("Something went wrong: " + e.Message.ToString());
            builder.SetNeutralButton("OK", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}