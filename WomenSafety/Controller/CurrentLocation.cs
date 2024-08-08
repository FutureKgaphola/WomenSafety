
using Android.App;
using Android.Content;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace WomenSafety.Controller
{
    public class CurrentLocation
    {
        LaskKnowLocation laskKnowLocation;

        System.Threading.CancellationToken cts;
        const string LOCATION_PREF_KEY = "cached_location", LOCATION_LON= "cached_lon", LOCATION_LAT= "cached_lat";

        public async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(8));
                cts = new System.Threading.CancellationToken();
                var location = await Geolocation.GetLocationAsync(request, cts);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    // Cache the location in shared preferences
                    ISharedPreferences prefs = Application.Context.GetSharedPreferences("MyPrefs", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString(LOCATION_PREF_KEY, $"{(location.Latitude).ToString().Replace(',','.')},{(location.Longitude).ToString().Replace(',','.')}");
                    editor.Apply();

                    ISharedPreferences prefsLat = Application.Context.GetSharedPreferences("MyLat", FileCreationMode.Private);
                    ISharedPreferencesEditor editorlat = prefs.Edit();
                    editorlat.PutString(LOCATION_LAT, (location.Latitude).ToString().Replace(',', '.'));
                    editorlat.Apply();

                    ISharedPreferences prefsLon = Application.Context.GetSharedPreferences("MyLon", FileCreationMode.Private);
                    ISharedPreferencesEditor editorLon = prefs.Edit();
                    editorLon.PutString(LOCATION_LON, (location.Longitude).ToString().Replace(',', '.'));
                    editorLon.Apply();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Console.WriteLine(fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Console.WriteLine (fneEx.Message);
                //laskKnowLocation = new LaskKnowLocation();
                // await laskKnowLocation.getlastKnowLocationDeviceAsync();
                Console.WriteLine("Sharepreference: "+RetrieveCachedLocation());

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Console.WriteLine($"{pEx.Message}");    
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine(ex.Message);
            }
        }

        private string RetrieveCachedLocation()
        {
            // Retrieve the cached location from shared preferences
            ISharedPreferences prefs = Application.Context.GetSharedPreferences("MyPrefs", FileCreationMode.Private);
            return prefs.GetString(LOCATION_PREF_KEY, "No location cached.");
        }

    }
}