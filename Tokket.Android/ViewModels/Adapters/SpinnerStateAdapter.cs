using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;

namespace Tokket.Android.Adapters
{
    public class SpinnerStateAdapter : ArrayAdapter<string>
    {

        Context context;
        List<string> contentArray;
        List<string> imageArray;
        public SpinnerStateAdapter(Context context, int resource, List<string> objects, List<string> imgArray) : base(context, Resource.Layout.signup_page_state_row, Resource.Id.spinnerTextView, objects)
        {
            this.context = context;
            this.contentArray = objects;
            this.imageArray = imgArray;
        }
        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            return getCustomView(position, convertView, parent);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return getCustomView(position, convertView, parent);
        }
        public View getCustomView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View row = inflater.Inflate(Resource.Layout.signup_page_state_row, parent, false);

            TextView textView = (TextView)row.FindViewById(Resource.Id.spinnerTextView);
            textView.Text = contentArray[position];

            ImageView imageView = (ImageView)row.FindViewById(Resource.Id.spinnerImages);
            Glide.With(row).Load(imageArray[position]).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(imageView);
            return row;
        }
    }
}