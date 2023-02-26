using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Lang;
using Xamarin.Essentials;
using SharedHelpers = Tokket.Shared.Helpers;

namespace Tokket.Android.Adapters
{
    public class AddTokOptionalAdapter : BaseAdapter<string>, ITextWatcher
    {
        string[] items;
        long[] limits;
        Activity context;
        int positionx = 0;
        View view; public ViewGroup Optionalparent;
        public AddTokOptionalAdapter(Activity context, string[] items, long[] limit) : base()
        {
            this.context = context;
            this.items = items;
            this.limits = limit;
        }

        public override long GetItemId(int position)
        {
            positionx = position;
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            view = convertView; // re-use an existing view, if one is available
            Optionalparent = parent;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.addtok_optionalfields, null);
            }
            view.FindViewById<TextView>(Resource.Id.lblAddTokOptional).Text = items[position] + " (" + limits[position] + " characters max)";
            view.FindViewById<TextInputLayout>(Resource.Id.inputlayoutOptionalFields).CounterMaxLength = (int)limits[position];
            view.FindViewById<TextView>(Resource.Id.txtAddTokOptional).SetFilters(new IInputFilter[] { new InputFilterLengthFilter((int)limits[position]) });

            return view;
        }
        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
        }

        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            //optionalValue[positionx] = (string)holder.txtOptionalFieldValue.Text;
        }

        public override int Count
        {
            get { return items.Length; }
        }

        public override string this[int position]
        {
            get { return items[position]; }
        }
    }

    class AddTokOptionalAdapterViewHolder : Java.Lang.Object
    {
    }
}