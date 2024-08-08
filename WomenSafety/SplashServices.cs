using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using Javax.Crypto;
using System;
using System.Threading.Tasks;
using WomenSafety.Controller;
using WomenSafety.Controller.Activities;
using Xamarin.Essentials;
using static Android.Service.Voice.VoiceInteractionSession;

namespace WomenSafety
{
    [Activity(Label = "WomenSafety", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashServices : Activity
    {
        bool isGooglePlayServicesInstalled;
        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;
        public static readonly string TAG = "XamarinDevice";
        private AlertDialog.Builder dialogB;
        private AlertDialog dialogf;
        private int month = System.DateTime.Now.Month;
        private int day = System.DateTime.Now.Day;
        private int year = System.DateTime.Now.Year;
        View view;

        const string User_PREF_KEY = "UserKeyPref";
        ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();
            
            
            if (isGooglePlayServicesInstalled==true)
            {
            //  keyGenerator = new UserKeyGenerator();
                var config = new Gr.Net.MaroulisLib.EasySplashScreen(this)
                .WithFullScreen()
                .WithTargetActivity(prefs.GetString(User_PREF_KEY, "No User Signed in") == "No User Signed in" ? Java.Lang.Class.FromType(typeof(Login)) : Java.Lang.Class.FromType(typeof(MainActivity)))
                .WithSplashTimeOut(4000)
                .WithLogo(Resource.Mipmap.safe)
                .WithFooterText("Women Safety")
                .WithBackgroundColor(Color.ParseColor("#FFFFFF"));
                view = config.Create();
                SetContentView(view);
                RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            }
            else
            {
                try
                {
                    var config = new Gr.Net.MaroulisLib.EasySplashScreen(this)
                            .WithFullScreen()
                            .WithSplashTimeOut(4000)
                            .WithLogo(Resource.Mipmap.safe)
                            .WithFooterText("Women Safety")
                            .WithBackgroundColor(Color.ParseColor("#FFFFFF"));
                    view = config.Create();
                    SetContentView(view);
                    RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
                    errorDialog();
                    Task.Delay(5000);
                    Finish();
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "There is a problem with Google Play Services on this device", ToastLength.Long).Show();
                    Finish();
                }
            }
        }

        private void errorDialog()
        {
            dialogB = new Android.App.AlertDialog.Builder(this);
            LayoutInflater inflater = LayoutInflater.From(this);
            View dialogView = inflater.Inflate(Resource.Layout.serviceError, null);
            dialogB.SetView(dialogView);
            dialogB.SetCancelable(false);
            dialogf = dialogB.Create();
            dialogf.Show();
        }
        bool TestIfGooglePlayServicesIsInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(TAG, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(TAG, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                //report error
                Toast.MakeText(this, "There is a problem with Google Play Services on this device", ToastLength.Long).Show();

            }

            return false;
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
            {
                isGooglePlayServicesInstalled = true;
            }
            else
            {
                Log.Warn(TAG, $"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
            }
        }
    }
}