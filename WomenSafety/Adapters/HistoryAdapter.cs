using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using WomenSafety.Model;

namespace WomenSafety.Adapters
{
    public class HistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryAdapterClickEventArgs> ItemLongClick; 
        public event EventHandler<HistoryAdapterClickEventArgs> falseAlarmEventClick;
        public event EventHandler<HistoryAdapterClickEventArgs> DeleteEventClick;
        List<HistoryModel> items;

        public HistoryAdapter(List<HistoryModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.HistoryRow;
            itemView = LayoutInflater.From(parent.Context).
                  Inflate(id, parent, false);

            var vh = new HistoryAdapterViewHolder(itemView, OnClick, OnLongClick, OnfalseAlarmEventClick, OnDeleteEventClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryAdapterViewHolder;
            holder.HistoryDate.Text = items[position].HistoryDate;
            holder.HistoryLat.Text = items[position].HistoryLat;
            holder.HistoryLon.Text = items[position].HistoryLon;
            holder.HistoryCombined.Text = items[position].HistoryCombinedLatLon;

        }

        public override int ItemCount => items.Count;

        void OnClick(HistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args); 
        void OnLongClick(HistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnfalseAlarmEventClick(HistoryAdapterClickEventArgs args) => falseAlarmEventClick?.Invoke(this, args);
        void OnDeleteEventClick(HistoryAdapterClickEventArgs args) => DeleteEventClick?.Invoke(this, args);

    }

    public class HistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView HistoryCombined { get; set; }
        public TextView HistoryDate { get; set; }
        public TextView HistoryLat { get; set; }
        public TextView HistoryLon { get; set; }
        public ImageView DeleteAlarm { get; set; }
        public Button FalseAlarm { get; set; }

        public HistoryAdapterViewHolder(View itemView, Action<HistoryAdapterClickEventArgs> clickListener,
                            Action<HistoryAdapterClickEventArgs> longClickListener,
                            Action<HistoryAdapterClickEventArgs> falseAlarmEventClickListener,
                            Action<HistoryAdapterClickEventArgs> DeleteAlarmClickListener) : base(itemView)
        {
            HistoryDate = itemView.FindViewById<TextView>(Resource.Id.HistoryDate);
            HistoryLat = itemView.FindViewById<TextView>(Resource.Id.HistoryLat);
            HistoryLon = itemView.FindViewById<TextView>(Resource.Id.HistoryLon);
            FalseAlarm= itemView.FindViewById<Button>(Resource.Id.falseAlarmEvent);
            DeleteAlarm= itemView.FindViewById<ImageView>(Resource.Id.DeleteAlarm);
            HistoryCombined = itemView.FindViewById<TextView>(Resource.Id.HistoryCombined);

            DeleteAlarm.Click += (sender, e) => DeleteAlarmClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            FalseAlarm.Click += (sender, e) => falseAlarmEventClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}