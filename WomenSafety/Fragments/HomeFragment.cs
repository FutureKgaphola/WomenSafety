using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BackgroundTask;
using Google.Android.Material.Button;
using Google.Android.Material.Snackbar;
using System;
using Android.Locations;
using Xamarin.Essentials;
using System.Threading;
using System.Threading.Tasks;

namespace WomenSafety.Fragments
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment
    {
        TrackingService trackingService = new TrackingService();
        View view;
        private MaterialButton EnableBtn, DisableBtn;
        private const string CHANNEL_ID = "tracking_service_channel";
        Intent serviceIntent;
        private Android.App.AlertDialog.Builder dialogBuilder;
        private Android.App.AlertDialog dialog;
        string message = "Are you sure you want to disable you safety feature. all backgruod safety features will be discarted";
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.HomeFragment, container, false);
            CreateNotificationChannel();
            serviceIntent = new Intent(Application.Context, trackingService.Class);
            
            EnableBtn = view.FindViewById<MaterialButton>(Resource.Id.EnableBtn);
            DisableBtn = view.FindViewById<MaterialButton>(Resource.Id.DisableBtn);

            EnableBtn.Click += EnableBtn_Click;
            DisableBtn.Click += DisableBtn_Click;
            return view;
        }

        private void DisableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ConfirmatmationMsg(message);
            }
            catch (Exception disableError)
            {

                Toast.MakeText(Application.Context, disableError.Message, ToastLength.Long).Show();
            }
           
        }

        private void EnableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    getLocationPermissions();
                }
                else
                {
                    bool isLocationEnabled = IsLocationServiceEnabled();
                    if (isLocationEnabled)
                    {
                        Context.StartService(serviceIntent);
                        Snackbar.Make(view, "🛰️Enabling Safety feature", Snackbar.LengthLong)
                                .Show();
                        Toast.MakeText(Application.Context, "Allow 10 - 11 seconds to fully enable service",ToastLength.Long).Show();
                    }
                    else
                    {
                        ShowLoacationOpener();
                    }
                    
                }
            }
            catch (Exception enableError)
            {

                Toast.MakeText(Application.Context, enableError.Message, ToastLength.Long).Show();
            }
        }

        /*private void EnablingServiceDialog()
        {
            dialogBuilder = new Android.App.AlertDialog.Builder(Activity);
            dialog = null;
            LayoutInflater inflater = LayoutInflater.From(Activity);
            View dialogView = inflater.Inflate(Resource.Layout.LoadingDialog, null);
            dialogBuilder.SetView(dialogView);
            dialogBuilder.SetCancelable(false);
            dialog = dialogBuilder.Create();
            dialog.Show();
            
        }*/

        private void ShowLoacationOpener()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("Location Modification");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("Please turn on your mobile device location before using this service.");
            builder.SetPositiveButton("🛰️Yes, enable", delegate
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                StartActivity(intent);
                builder.Dispose();

            });
            builder.SetNeutralButton("❌no, cancel", delegate
            {
                builder.Dispose();

            });

            builder.Show();
        }

        private bool IsLocationServiceEnabled()
        {
            Activity activity = Activity;
            LocationManager locationManager = (LocationManager)activity.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider) ||
                   locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
        }
        public async void getLocationPermissions()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        private void ConfirmatmationMsg(string message)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("Feature Modification");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage(message);
            builder.SetPositiveButton("Yes, disable", delegate
            {
                Context.StopService(serviceIntent);
                Snackbar.Make(view, "🛰️Disabling Safety feature", Snackbar.LengthLong)
                               .Show();
                builder.Dispose();

            });
            builder.SetNeutralButton("no, cancel", delegate
            {

                builder.Dispose();

            });

            builder.Show();
        }

        private void CreateNotificationChannel()
        {
            Activity activity = Activity;
            // Check if the device is running Android Oreo or higher
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                // Define the name and description of the notification channel
                string channelName = "Tracking Service Channel";
                string channelDescription = "Channel for the Tracking Service";

                // Define the importance level of the notification channel
                NotificationImportance importance = NotificationImportance.Default;

                // Create the notification channel
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, channelName, importance);
                channel.Description = channelDescription;

                // Get the system's notification manager
                NotificationManager notificationManager = (NotificationManager)activity.GetSystemService(Context.NotificationService);

                // Register the notification channel with the system
                notificationManager.CreateNotificationChannel(channel);
            }
        }

    }
}