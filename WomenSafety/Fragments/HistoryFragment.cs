using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Javax.Crypto;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WomenSafety.Adapters;
using WomenSafety.Controller;
using WomenSafety.DatabaseConnection;
using WomenSafety.Model;
using Xamarin.Essentials;
using static Android.Gms.Common.Apis.Api;

namespace WomenSafety.Fragments
{
    public class HistoryFragment : AndroidX.Fragment.App.Fragment, IValueEventListener
    {
        HistoryAdapter hAdapter;
        private RecyclerView Recycle_History;
        private List<HistoryModel> hmodel = new List<HistoryModel>();
        private HistoryEvent hData = new HistoryEvent();

        PhoneAdapter pAdapter;
        private RecyclerView RecyclerPhones;
        private List<PhoneModel> pmodel = new List<PhoneModel>();
        private PhoneEvent pData = new PhoneEvent();

        private Android.App.AlertDialog.Builder dialogBuilder;
        private Android.App.AlertDialog dialog;
        View view;
        private TextInputEditText Updphone;
        private TextInputEditText UName;
        private Button Updatebtn;

        private static readonly HttpClient client = new HttpClient();

        const string User_PREF_KEY = "UserKeyPref";
        ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);

        private TextView womanName, womanEmail;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.HistoryFragment, container, false);
            Recycle_History = view.FindViewById<RecyclerView>(Resource.Id.Recycle_History);
            RecyclerPhones = view.FindViewById<RecyclerView>(Resource.Id.RecyclerPhones);
            womanName= view.FindViewById<TextView>(Resource.Id.womanName);
            womanName.Click += WomanName_Click;
            womanEmail= view.FindViewById<TextView>(Resource.Id.womanEmail);
            Retrieve_HistoryData();
            Retrieve_PhoneData();
           

            RetriveMyProfile(prefs.GetString(User_PREF_KEY, "No User Signed in"));
            return view;
        }

        private void WomanName_Click(object sender, EventArgs e)
        {
            UpdateMyDetails_PopUp();
        }

        private void RetriveMyProfile(string v)
        {
            
            DatabaseReference dref = firebase_Connection.GetDatabase().GetReference("WomenSafetyUsers").Child(v.Trim());
            dref.AddValueEventListener(this);
        }

        public void Retrieve_PhoneData()
        {
            pData.Retrieve_Users(prefs.GetString(User_PREF_KEY, "No User Signed in"));
            pData.RetrivePhoneNumbers += PData_RetrivePhoneNumbers;
        }

        private void PData_RetrivePhoneNumbers(object sender, PhoneEvent.RetrivedPhoneEventHandeler e)
        {
            pmodel = e.UserList;
            setPhoneNumbersRecycler();
        }

        public void setPhoneNumbersRecycler()
        {
            LinearLayoutManager linMan = new LinearLayoutManager(Application.Context, LinearLayoutManager.Horizontal, false);
            pAdapter = new PhoneAdapter(pmodel);
            RecyclerPhones.SetLayoutManager(linMan);
            RecyclerPhones.SetAdapter(pAdapter);
            pAdapter.NameClick += PAdapter_NameClick;
        }

        private void PAdapter_NameClick(object sender, PhoneAdapterClickEventArgs e)
        {
            ShowOptions(pmodel[e.Position]);
        }

        private void ShowOptions(PhoneModel pObjct)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("Options");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("What would you like to do⚙️\nPhone call\nUpdate Details");
            builder.SetPositiveButton("📱Call", delegate
            {
                PlacePhoneCall(pObjct.phone.Trim());
                builder.Dispose();

            });
            builder.SetNeutralButton("🖋️Update", delegate
            {
                Update_PopUp(pObjct);
                builder.Dispose();

            });

            builder.Show();
        }

        private TextInputEditText MyName;
        private Button updatePersonal;
        string updateId="";
        private void Update_PopUp(PhoneModel contacts)
        {
            dialogBuilder = new Android.App.AlertDialog.Builder(Activity);
            dialog = null;
            LayoutInflater inflater = LayoutInflater.From(Activity);
            View dialogView = inflater.Inflate(Resource.Layout.UpdatePhone, null);
            Updphone = dialogView.FindViewById<TextInputEditText>(Resource.Id.updphone);
            UName = dialogView.FindViewById<TextInputEditText>(Resource.Id.name);
            Updatebtn = dialogView.FindViewById<Button>(Resource.Id.updatebtn);
            updateId = contacts.userId;
            Updphone.Text = contacts.phone;
            UName.Text = contacts.Name;
            Updatebtn.Click += Updatebtn_Click;
            dialogBuilder.SetView(dialogView);
            dialogBuilder.SetCancelable(true);
            dialog = dialogBuilder.Create();
            dialog.Show();
        }
        private void UpdateMyDetails_PopUp()
        {
            dialogBuilder = new Android.App.AlertDialog.Builder(Activity);
            dialog = null;
            LayoutInflater inflater = LayoutInflater.From(Activity);
            View dialogView = inflater.Inflate(Resource.Layout.UpDateMyDetails, null);
            MyName = dialogView.FindViewById<TextInputEditText>(Resource.Id.MyName);
            updatePersonal = dialogView.FindViewById<Button>(Resource.Id.updatePersonal);
            MyName.Text = womanName.Text;
            updatePersonal.Click += UpdatePersonal_Click;
            dialogBuilder.SetView(dialogView);
            dialogBuilder.SetCancelable(true);
            dialog = dialogBuilder.Create();
            dialog.Show();
        }

        private void UpdatePersonal_Click(object sender, EventArgs e)
        {
            if (!NameChecker())
            {
                firebase_Connection.GetDatabase().GetReference("WomenSafetyUsers").Child(prefs.GetString(User_PREF_KEY, "No User Signed in")).Child("Name").SetValue(MyName.Text.Trim());

                Snackbar.Make(view, "Updating your Name", Snackbar.LengthLong)
                               .Show();
                dialog.Dismiss();
            }
        }

        private bool NameChecker()
        {
            bool founderror = false;
            if (string.IsNullOrEmpty(MyName.Text.Trim()))
            {
                founderror = true;
                MyName.Error = "Cannot be empty";
            }
            return founderror;
        }

        private void Updatebtn_Click(object sender, EventArgs e)
        {
            if (!ErrorChecker())
            {
                firebase_Connection.GetDatabase().GetReference("Phones").Child(prefs.GetString(User_PREF_KEY, "No User Signed in")).Child(updateId).Child("phone").SetValue(Updphone.Text.Trim());
                firebase_Connection.GetDatabase().GetReference("Phones").Child(prefs.GetString(User_PREF_KEY, "No User Signed in")).Child(updateId).Child("Name").SetValue(UName.Text.Trim());
                Snackbar.Make(view, "Updating contacts", Snackbar.LengthLong)
                               .Show();
                dialog.Dismiss();
            }

        }

        private bool ErrorChecker()
        {
            bool founderror = false;
            if (string.IsNullOrEmpty(Updphone.Text.Trim()) || Updphone.Text.Trim().Length<10)
            {
                founderror = true;
                Updphone.Error = "10 digits required";
            }
            if (string.IsNullOrEmpty(UName.Text.Trim()))
            {
                founderror = true;
                UName.Error = "Cannot be empty";
            }
            return founderror;
        }

        public void PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
                Toast.MakeText(Application.Context, anEx.Message,ToastLength.Long).Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        public void Retrieve_HistoryData()
        {
            const string User_PREF_KEY = "UserKeyPref";

            ISharedPreferences prefs = Application.Context.GetSharedPreferences("userKey", FileCreationMode.Private);
            
            Console.WriteLine(prefs.GetString(User_PREF_KEY, "No User Signed in"));
            hData.Retrieve_Shake_History(prefs.GetString(User_PREF_KEY, "No User Signed in"));
            hData.RetriveHistory += HData_RetriveHistory;
     
        }

        private void HData_RetriveHistory(object sender, HistoryEvent.RetrivedHistoryEventHandeler e)
        {
            hmodel = e.HistoryList;
            setHistoryRecycler();
        }
        private void setHistoryRecycler()
        {
            LinearLayoutManager linMan = new LinearLayoutManager(Application.Context, LinearLayoutManager.Vertical, false);
            hAdapter = new HistoryAdapter(hmodel);
            Recycle_History.SetLayoutManager(linMan);
            Recycle_History.SetAdapter(hAdapter);
            hAdapter.DeleteEventClick += HAdapter_DeleteEventClick;
            hAdapter.falseAlarmEventClick += HAdapter_falseAlarmEventClick;
        }
        private string currentUser = "";
        private void HAdapter_falseAlarmEventClick(object sender, HistoryAdapterClickEventArgs e)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("False Alarm");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("You are about to report back to your associates that this event was accidental(false alarm)");
            builder.SetPositiveButton("🗑️Yes, false alarm", delegate
            {
                SendFalseAlarm(currentUser, hmodel[e.Position].HistoryDate);
                firebase_Connection.GetDatabase().GetReference("History").Child(prefs.GetString(User_PREF_KEY, "No User Signed in")).Child(hmodel[e.Position].HistoryId.Trim()).RemoveValue();
                Snackbar.Make(view, "🗑️Removing item", Snackbar.LengthLong)
                               .Show();
                hmodel.Remove(hmodel[e.Position]);
                hAdapter.NotifyDataSetChanged();
                builder.Dispose();
            });
            builder.SetNeutralButton("❌Cancel", delegate
            {
                builder.Dispose();
            });
            builder.Show();

            
        }
        private async void SendFalseAlarm(string currentUser,string HistoryDayTime)
        {
            var values = new Dictionary<string, string>
            {
                { "victimName",  currentUser},
                { "date_Time",  HistoryDayTime}
            };
            //var content = new FormUrlEncodedContent(values);

            //var response = await client.PostAsync("https://womensafetyapi.onrender.com/falsesendsms", content);

            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(values);
            // Send POST request
            HttpResponseMessage response = await client.PostAsync("https://womensafetyapi.onrender.com/falsesendsms", new StringContent(jsonContent, Encoding.UTF8, "application/json"));


            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString.ToString());
        }
        private void HAdapter_DeleteEventClick(object sender, HistoryAdapterClickEventArgs e)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("Deleting");
            builder.SetIcon(Resource.Mipmap.safe);
            builder.SetMessage("You are about to delete this record. 🚮\nRecord(s) cannot be recovered after they/it has been deleted.");
            builder.SetPositiveButton("🗑️Yes, delete", delegate
            {
                firebase_Connection.GetDatabase().GetReference("History").Child(prefs.GetString(User_PREF_KEY, "No User Signed in")).Child(hmodel[e.Position].HistoryId.Trim()).RemoveValue();
                Snackbar.Make(view, "🗑️Removing item", Snackbar.LengthLong)
                               .Show();
                hmodel.Remove(hmodel[e.Position]);
                hAdapter.NotifyDataSetChanged();
                builder.Dispose();
            });
            builder.SetNeutralButton("❌Cancel", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                womanName.Text = snapshot.Child("Name").Value.ToString();
                womanEmail.Text = snapshot.Child("Email").Value.ToString();
                currentUser = snapshot.Child("Name").Value.ToString();
            }
        }
    }
}