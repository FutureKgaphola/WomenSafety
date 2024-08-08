using Android;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using Newtonsoft.Json;
using Org.Apache.Http.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WomenSafety.Controller;
using WomenSafety.DatabaseConnection;
using Xamarin.Essentials;
using static Android.Gms.Common.Apis.Api;
using Encoding = System.Text.Encoding;

namespace BackgroundTask
{
    [Service(Enabled = true)]
    public class TrackingService : Service, ISensorEventListener, IValueEventListener
    {
        private SensorManager _sensorManager;
        private CancellationTokenSource _cancellationTokenSource;
        private CurrentLocation currentLocation;
        private static readonly HttpClient client = new HttpClient();

        // Notification channel ID
        private const string CHANNEL_ID = "tracking_service_channel";


        const string User_PREF_KEY = "UserKeyPref";
        ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);

        const string LOCATION_PREF_KEY = "cached_location", LOCATION_LON = "cached_lon", LOCATION_LAT = "cached_lat";

        ISharedPreferences combined = Application.Context.GetSharedPreferences("MyPrefs", FileCreationMode.Private);
        ISharedPreferences prefsLat = Application.Context.GetSharedPreferences("MyLat", FileCreationMode.Private);
        ISharedPreferences prefsLon = Application.Context.GetSharedPreferences("MyLon", FileCreationMode.Private);
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        private void RetriveMyProfile(string v)
        {

            DatabaseReference dref = firebase_Connection.GetDatabase().GetReference("WomenSafetyUsers").Child(v.Trim());
            dref.AddValueEventListener(this);
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _sensorManager = (SensorManager)GetSystemService(SensorService);


            // Register accelerometer sensor for shake detection
            Sensor accelerometer = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            _sensorManager.RegisterListener(this, accelerometer, SensorDelay.Ui);

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async ()=>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    RetriveMyProfile(prefs.GetString(User_PREF_KEY, "No User Signed in"));
                    currentLocation = new CurrentLocation();
                    await currentLocation.GetCurrentLocation();

                    

                    // Sleep for a specified interval
                    Thread.Sleep(10000); // Example: Track every 9 seconds
                }
            }, _cancellationTokenSource.Token);

            // Start service as a foreground service
            StartForeground(1, new Notification.Builder(this, CHANNEL_ID)
                .SetContentTitle("Tracking Service")
                .SetContentText("Tracking your device in the background")
                
                .Build());

            return StartCommandResult.Sticky;
        }

        
        public override void OnDestroy()
        {
            // Unregister accelerometer sensor
            _sensorManager?.UnregisterListener(this);

            // Stop the periodic task and cancel any ongoing operations
            _cancellationTokenSource?.Cancel();

            base.OnDestroy();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void VibrateDevice()
        {
            try
            {
                // Use default vibration length
                Vibration.Vibrate();

                // Or use specified time
                var duration = TimeSpan.FromSeconds(1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Console.WriteLine(ex.Message);
            }
        }
        void cancelVibration()
        {
            try
            {
                Vibration.Cancel();
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Console.WriteLine(ex.Message);
            }
        }


        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                // Calculate acceleration magnitude
                double acceleration = Math.Sqrt(e.Values[0] * e.Values[0] +
                                                 e.Values[1] * e.Values[1] +
                                                 e.Values[2] * e.Values[2]);

                // Threshold for shake detection (adjust as needed)
                double shakeThreshold = 40;

                // If the device is shaken, send a push notification
                if (acceleration > shakeThreshold)
                {
                    // Send push notification
                    SendPushNotification();
                }
            }
        }
        

        private int month = System.DateTime.Now.Month;
        private int day = System.DateTime.Now.Day;
        private int year = System.DateTime.Now.Year;
        private void SendPushNotification()
        {
            string tot = combined.GetString(LOCATION_PREF_KEY, "No location cached.");
            string _lat= "No latitude cached", _lng= "No longtitude cached";
            _lat = tot.Substring(0, tot.IndexOf(','));
            tot= tot.Remove(0, tot.IndexOf(',')+1);
            _lng = tot;
            System.Diagnostics.Debug.WriteLine("This is a log Shake Detected");
            VibrateDevice();
            HashMap data = new HashMap();
            data.Put("HistoryCombinedLatLon", combined.GetString(LOCATION_PREF_KEY, "No location cached."));
            data.Put("HistoryLat", _lat != "No latitude cached" && _lat != "" ? _lat : "No latitude cached");
            data.Put("HistoryLon", _lng!= "No longtitude cached" && _lng != "" ? _lng : "No longtitude cached");
            data.Put("createdPeriod", day.ToString()+'/'+ month.ToString()+'/'+year.ToString()+" "+ DateTime.Now.ToString("h:mm tt"));
            DatabaseReference databaseRef = firebase_Connection.GetDatabase().GetReference("History").Child(prefs.GetString(User_PREF_KEY, "No User Signed in"));
            databaseRef.Push().SetValue(data);
            // _cancellationTokenSource?.Cancel();
            // System.Diagnostics.Debug.WriteLine("This is a log Stop listening");
            string googleMapsUrl = "https://www.google.com/maps?q=" + combined.GetString(LOCATION_PREF_KEY, "No location cached.");
            Console.WriteLine(googleMapsUrl);
            SendSmsAlert(googleMapsUrl);
        }
        private string currentUser = "";
        private async void SendSmsAlert(string googleMapsUrl)
        {

            var values = new Dictionary<string, string>
            {
                { "googleUrl", googleMapsUrl },
                { "victimName",  currentUser}
            };
            //HttpContent content = new FormUrlEncodedContent(values);
            Console.WriteLine(googleMapsUrl);
            Console.WriteLine(currentUser);
            // Convert Dictionary to JSON string
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(values);
            // Send POST request
            HttpResponseMessage response = await client.PostAsync("https://womensafetyapi.onrender.com/sendsms", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
            //var response = await client.PostAsync("https://womensafetyapi.onrender.com/sendsms", content);

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString.ToString());
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }
        
        public void OnDataChange(DataSnapshot snapshot)
        {
            currentUser = snapshot.Child("Name").Value.ToString();
        }
    }
}
