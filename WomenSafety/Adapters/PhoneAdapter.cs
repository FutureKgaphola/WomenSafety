using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using WomenSafety.Model;

namespace WomenSafety.Adapters
{
    public class PhoneAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PhoneAdapterClickEventArgs> ItemClick;
        public event EventHandler<PhoneAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<PhoneAdapterClickEventArgs> NameClick;
        
        List<PhoneModel> items;

        public PhoneAdapter(List<PhoneModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.phoneRow;
            itemView = LayoutInflater.From(parent.Context).
                  Inflate(id, parent, false);

            var vh = new UsersAdapterViewHolder(itemView, OnClick, OnLongClick, OnPhoneClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as UsersAdapterViewHolder;
            holder.Name.Text = items[position].Name;
            holder.phone.Text = items[position].phone; 
        }
        public override int ItemCount => items.Count;

        void OnClick(PhoneAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PhoneAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnPhoneClick(PhoneAdapterClickEventArgs args) => NameClick?.Invoke(this, args);
    }

    public class UsersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public TextView phone { get; set; }
        public UsersAdapterViewHolder(View itemView, Action<PhoneAdapterClickEventArgs> clickListener,
                            Action<PhoneAdapterClickEventArgs> longClickListener,
                            Action<PhoneAdapterClickEventArgs> phoneClickListener) : base(itemView)
        {
            phone = itemView.FindViewById<TextView>(Resource.Id.phone);
            Name = itemView.FindViewById<Button>(Resource.Id.Name);

            Name.Click += (sender, e) => phoneClickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
    public class PhoneAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}