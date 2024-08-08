using Android.App;
using Android.Widget;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using WomenSafety.DatabaseConnection;
using WomenSafety.Model;
namespace WomenSafety.Controller
{
    public class PhoneEvent: Java.Lang.Object, IValueEventListener
    {
        private List<PhoneModel> UserPos = new List<PhoneModel>();

        public event EventHandler<RetrivedPhoneEventHandeler> RetrivePhoneNumbers;
        public class RetrivedPhoneEventHandeler : EventArgs
        {
            public List<PhoneModel> UserList { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                try
                {
                    UserPos.Clear();
                    var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                    foreach (DataSnapshot data in child)
                    {
                        PhoneModel uM = new PhoneModel
                        {
                            userId = data.Key,
                            phone = data.Child("phone").Value.ToString(),
                            Name = data.Child("Name").Value.ToString(),
 
                        };
                        UserPos.Add(uM);
                    }
                }
                catch (Exception getFailure)
                {
                    Toast.MakeText(Application.Context, "Error: couldn't fetch data as expected: " + getFailure.Message, ToastLength.Long).Show();
                }

            }
            RetrivePhoneNumbers.Invoke(this, new RetrivedPhoneEventHandeler
            {
                UserList = UserPos
            });
        }

        public void Retrieve_Users(string userId)
        {
            DatabaseReference dref = firebase_Connection.GetDatabase().GetReference("Phones").Child(userId);
            dref.AddValueEventListener(this);
        }
    }
}