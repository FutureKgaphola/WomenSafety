using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.TextField;
using Java.Util;
using Org.Apache.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WomenSafety.DatabaseConnection;

namespace WomenSafety.Controller.Activities
{
    [Activity(Label = "CreateAccount")]
    public class CreateAccount : Activity, IOnSuccessListener, IOnFailureListener
    {
        FirebaseAuth auth;
        private ProgressBar progBar;
        private TextInputEditText username, edtpassword, MyName,
            frstFriendPhone, frstFriendName, secondFriendName,
            secondFriendPhone, ThirdFriendPhone, ThirdFriendName;
        private Button BtnSignup;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateAccount);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);

            username =FindViewById<TextInputEditText>(Resource.Id.username);
            edtpassword = FindViewById<TextInputEditText>(Resource.Id.edtpassword);
            MyName = FindViewById<TextInputEditText>(Resource.Id.MyName);

            frstFriendPhone = FindViewById<TextInputEditText>(Resource.Id.frstFriendPhone);
            frstFriendName = FindViewById<TextInputEditText>(Resource.Id.frstFriendName);
            secondFriendName = FindViewById<TextInputEditText>(Resource.Id.secondFriendName);
            secondFriendPhone = FindViewById<TextInputEditText>(Resource.Id.secondFriendPhone);
            ThirdFriendName = FindViewById<TextInputEditText>(Resource.Id.ThirdFriendName);
            ThirdFriendPhone = FindViewById<TextInputEditText>(Resource.Id.ThirdFriendPhone);

            BtnSignup = FindViewById<Button>(Resource.Id.procced);
            BtnSignup.Click += BtnSignup_Click;
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            if (!PersonalDetailsChecker() && !FriendChecker())
            {
                progBar.Visibility = ViewStates.Visible;
                auth = new firebase_Connection().GetFirebaseAuth();
                auth.CreateUserWithEmailAndPassword(username.Text.Trim(),edtpassword.Text)
                    .AddOnFailureListener(this)
                    .AddOnSuccessListener(this);

            }
        }

        private bool PersonalDetailsChecker()
        {
            bool found = false;
            if (string.IsNullOrEmpty(username.Text.Trim()) || !username.Text.Trim().Contains('@') || !username.Text.Trim().Contains('.'))
            {
                found = true;
                username.Error = "Requires an email";
            }
            if (string.IsNullOrEmpty(edtpassword.Text.Trim()) || edtpassword.Text.Trim().Length<6)
            {
                found = true;
                edtpassword.Error = "Requires 6-more characters";
            }
            if (string.IsNullOrEmpty(MyName.Text.Trim()))
            {
                found = true;
                MyName.Error = "Requires (1-16 characters)";
            }
            return found;
        }

        private bool FriendChecker()
        {
            bool found=false;
            //1st
            if (string.IsNullOrEmpty(frstFriendName.Text.Trim()))
            {
                found = true;
                frstFriendName.Error = "Requires (1-16 characters)";
            }
            if (string.IsNullOrEmpty(frstFriendPhone.Text.Trim()) || frstFriendPhone.Text.Trim().Length!=10)
            {
                found = true;
                frstFriendPhone.Error = "Requires (10 characters)";
            }

            //2nd
            if (string.IsNullOrEmpty(secondFriendName.Text.Trim()))
            {
                found = true;
                secondFriendName.Error = "Requires (1-16 characters)";
            }
            if (string.IsNullOrEmpty(secondFriendPhone.Text.Trim()) || secondFriendPhone.Text.Trim().Length != 10)
            {
                found = true;
                secondFriendPhone.Error = "Requires (10 characters)";
            }

            //3rd
            if (string.IsNullOrEmpty(ThirdFriendName.Text.Trim()))
            {
                found = true;
                ThirdFriendName.Error = "Requires (1-16 characters)";
            }
            if (string.IsNullOrEmpty(ThirdFriendPhone.Text.Trim()) || ThirdFriendPhone.Text.Trim().Length != 10)
            {
                found = true;
                ThirdFriendPhone.Error = "Requires (10 characters)";
            }


            return found;
        }


        private void AddPersonalToDb()
        {
            try
            {
                HashMap data = new HashMap();
                data.Put("Email", username.Text.Trim().ToLower());
                data.Put("Name", MyName.Text.Trim());
                DatabaseReference databaseRef = firebase_Connection.GetDatabase().GetReference("WomenSafetyUsers").Child(auth.CurrentUser?.Uid);
                databaseRef.SetValue(data);
            }
            catch (Exception er)
            {
                progBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, er.Message,ToastLength.Long).Show();
            }
        }
        private void AddFriendsToDb()
        {
            try
            {
                HashMap data1 = new HashMap();
                data1.Put("Name", frstFriendName.Text.Trim().ToLower());
                data1.Put("phone", frstFriendPhone.Text.Trim());
                DatabaseReference databaseRef1 = firebase_Connection.GetDatabase().GetReference("Phones").Child(auth.CurrentUser?.Uid);
                databaseRef1.Push().SetValue(data1);

                HashMap data2 = new HashMap();
                data2.Put("Name", secondFriendName.Text.Trim().ToLower());
                data2.Put("phone", secondFriendPhone.Text.Trim());
                DatabaseReference databaseRef2 = firebase_Connection.GetDatabase().GetReference("Phones").Child(auth.CurrentUser?.Uid);
                databaseRef2.Push().SetValue(data2);

                HashMap data3 = new HashMap();
                data3.Put("Name", ThirdFriendName.Text.Trim().ToLower());
                data3.Put("phone", ThirdFriendPhone.Text.Trim());
                DatabaseReference databaseRef3 = firebase_Connection.GetDatabase().GetReference("Phones").Child(auth.CurrentUser?.Uid);
                databaseRef3.Push().SetValue(data3);
            }
            catch (Exception er)
            {
                progBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, er.Message, ToastLength.Long).Show();
            }
        }

        private void CleanUp()
        {
            username.Text = "";
            MyName.Text = "";
            edtpassword.Text = "";
            frstFriendName.Text = "";
            frstFriendPhone.Text = "";
            secondFriendName.Text = "";
            secondFriendPhone.Text = "";
            ThirdFriendName.Text = "";
            ThirdFriendPhone.Text = "";
            progBar.Visibility = ViewStates.Gone;

        }
        public void OnSuccess(Java.Lang.Object result)
        {
            AddPersonalToDb();
            AddFriendsToDb(); 
            CleanUp();
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("completed");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("succsessfully registered");
            builder.SetPositiveButton("ok", delegate
            {
                builder.Dispose();
                Finish();

            });

            builder.Show();
        }

        public void OnFailure(Java.Lang.Exception e)
        {

            progBar.Visibility = ViewStates.Gone;

            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("Something went wrong: " + e.Message);
            builder.SetPositiveButton("ok", delegate
            {
                builder.Dispose();
            });

            builder.Show();

        }
    }
}