using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using WomenSafety.DatabaseConnection;

namespace WomenSafety.Controller.Activities
{
    [Activity(Label = "forgotpassword", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class forgotpassword : Activity, IOnFailureListener, IOnSuccessListener
    {
        private Button proccedrset;
        ProgressBar progBar;
        TextInputEditText user_name;
        private FirebaseAuth auth;

        private TextView goback;
        public void OnFailure(Java.Lang.Exception e)
        {
            progBar.Visibility = ViewStates.Gone;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("warning");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("Something went wrong: " + e.Message.ToString());
            builder.SetNeutralButton("OK", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }



        public void OnSuccess(Java.Lang.Object result)
        {
            progBar.Visibility = ViewStates.Gone;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Notification");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("Successfully sent to your email " + user_name.Text.ToLower().Trim() + " . do go and reset your password there.\n Check your email in the email spam section if it is not in the normal inbox.");
            builder.SetNeutralButton("OK", delegate
            {
                builder.Dispose();
                Finish();
            });
            builder.Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Resetpassword);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            proccedrset = (Button)FindViewById(Resource.Id.proccedrset);
            proccedrset.Click += Procced_Click;
            user_name = FindViewById<TextInputEditText>(Resource.Id.user_name);
            progBar = (ProgressBar)FindViewById(Resource.Id.progBar);
            goback = FindViewById<TextView>(Resource.Id.goback);
            goback.Click += Goback_Click;
        }


        private void Goback_Click(object sender, EventArgs e)
        {
            Finish();
        }
        private void Procced_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(user_name.Text.Trim()) && user_name.Text.Trim().Contains('@'))
            {
                progBar.Visibility = ViewStates.Visible;
                
                auth = new firebase_Connection().GetFirebaseAuth();
                auth.SendPasswordResetEmail(user_name.Text.ToLower().Trim())
                               .AddOnFailureListener(this)
                               .AddOnSuccessListener(this);
            }
            else
            {
                user_name.Error = "provide correct email";
            }
        }
    }
}