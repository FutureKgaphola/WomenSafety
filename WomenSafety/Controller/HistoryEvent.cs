using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WomenSafety.DatabaseConnection;
using WomenSafety.Model;

namespace WomenSafety.Controller
{
    public class HistoryEvent : Java.Lang.Object, IValueEventListener
    {
        private List<HistoryModel> HistoryPos = new List<HistoryModel>();

        public event EventHandler<RetrivedHistoryEventHandeler> RetriveHistory;
        public class RetrivedHistoryEventHandeler : EventArgs
        {
            public List<HistoryModel> HistoryList { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            if (snapshot!=null)
            {
               
                    HistoryPos.Clear();
                    var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                    foreach (DataSnapshot data in child)
                    {
                    HistoryModel uM = new HistoryModel
                    {
                        HistoryId = data.Key,
                        HistoryDate = data.Child("createdPeriod").Value.ToString(),
                        HistoryCombinedLatLon = data.Child("HistoryCombinedLatLon").Value.ToString(),
                        HistoryLat = data.Child("HistoryLat").Value.ToString(),
                        HistoryLon = data.Child("HistoryLon").Value.ToString(),

                    };
                    HistoryPos.Add(uM);
                    
                }
                
                

            }
            
            RetriveHistory.Invoke(this, new RetrivedHistoryEventHandeler
            {
                HistoryList = HistoryPos
            });
        }

        public void Retrieve_Shake_History(string userId)
        {
            DatabaseReference dref = firebase_Connection.GetDatabase().GetReference("History").Child(userId.Trim());
            dref.AddValueEventListener(this);
        }

    }
}