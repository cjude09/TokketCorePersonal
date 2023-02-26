using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ServiceAccount = Tokket.Shared.Services;

namespace Tokket.Android.Helpers
{
    public static class ImageConverter
    {
        public static String BitmapToBase64(Bitmap mybitmap)
        {
            System.IO.MemoryStream byteArrayOutputStream = new System.IO.MemoryStream();
            mybitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
            byte[] byteArray = byteArrayOutputStream.ToArray();
            return Base64.EncodeToString(byteArray, default);
        }
    }
}