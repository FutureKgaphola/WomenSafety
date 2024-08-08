using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BackgroundTask;
using Google.Android.Material.BottomNavigation;
using Javax.Crypto;
using System;
using System.Collections.Generic;
using WomenSafety.Controller;
using WomenSafety.Controller.Activities;
using WomenSafety.Fragments;
using Xamarin.Essentials;

namespace WomenSafety
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private AndroidX.Fragment.App.Fragment Currentfragment;
        private HomeFragment homeFragment;
        private HistoryFragment History_Fragment;
        TrackingService trackingService= new TrackingService();
        Intent serviceIntent;
        const string User_PREF_KEY = "UserKeyPref";

        private Stack<AndroidX.Fragment.App.Fragment> mstackFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            serviceIntent = new Intent(Application.Context, trackingService.Class);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            homeFragment = new HomeFragment();
            History_Fragment = new HistoryFragment();

            mstackFragment = new Stack<AndroidX.Fragment.App.Fragment>();

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Add(Resource.Id.fragmentContainer, History_Fragment, "Fragment2");
            trans.Hide(History_Fragment);
            trans.Add(Resource.Id.fragmentContainer, homeFragment, "Fragment1");
            trans.Commit();
            Currentfragment = homeFragment;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                getLocationPermissions();
            }
        }

        public async void getLocationPermissions()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            var trans = SupportFragmentManager.BeginTransaction();
            trans.Hide(Currentfragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();
            mstackFragment.Push(Currentfragment);
            Currentfragment = fragment;
        }

        public void RequestSignOut()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Sign Out");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("You are about to Sign Out");
            builder.SetPositiveButton("Sign Out & Remeber me", delegate
            {
                Finish();

            });
            builder.SetNeutralButton("Sign Out & forget me", delegate
            {

                ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString(User_PREF_KEY, "No User Signed in");
                editor.Apply();
                this.StopService(serviceIntent);
                Intent intent = new Intent(this, typeof(Login));
                StartActivity(intent);
                Finish();
                builder.Dispose();

            });

            builder.Show();
        }

        [System.Obsolete]
        public override void OnBackPressed()
        {
            RequestSignOut();
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    ShowFragment(homeFragment);
                    return true;
                case Resource.Id.navigation_dashboard:
                    ShowFragment(History_Fragment);
                    return true;
                case Resource.Id.navigation_notifications:
                    RequestSignOut();
                    return true;
            }
            return false;
        }
    }
}

