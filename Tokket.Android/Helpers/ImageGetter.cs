using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Tokket.Android.Helpers
{
    public class ImageGetter : Java.Lang.Object, Html.IImageGetter
    {
        [Obsolete]
        public Drawable GetDrawable(string source)
        {
            Drawable drawable;
            Bitmap bitMap;
            BitmapFactory.Options bitMapOption;
            try
            {
                bitMapOption = new BitmapFactory.Options();
                bitMapOption.InJustDecodeBounds = false;
                bitMapOption.InPreferredConfig = Bitmap.Config.Argb4444;
                bitMapOption.InPurgeable = true;
                bitMapOption.InInputShareable = true;
                var url = new Java.Net.URL(source);

                bitMap = BitmapFactory.DecodeStream(url.OpenStream(), null, bitMapOption);


                //using (var webClient = new WebClient())
                //{
                //    var imageBytes = webClient.DownloadData(source);
                //    if (imageBytes != null && imageBytes.Length > 0)
                //    {
                //        bitMap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                //    }
                //}

                drawable = new BitmapDrawable(bitMap);
            }
            catch (Exception ex)
            {
                return null;
            }

            drawable.SetBounds(0, 0, bitMapOption.OutWidth, bitMapOption.OutHeight);
            return drawable;
        }
    }
}