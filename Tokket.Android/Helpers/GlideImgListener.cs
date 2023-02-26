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
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Palette.Graphics;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Java.Lang;

namespace Tokket.Android.Helpers
{
    public class GlideImgListener : Java.Lang.Object, IRequestListener
    {
        public int mColorPalette { get; set; }
        public Activity ParentActivity { get; set; }
        public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
        {
            ParentActivity.StartPostponedEnterTransition();
            return false;
        } 

        public bool OnResourceReady(Java.Lang.Object resource, Java.Lang.Object model, ITarget target, DataSource dataSource, bool isFirstResource)
        {
            ParentActivity.StartPostponedEnterTransition();
            if (resource != null)
            {
                try
                {
                    Bitmap bmResource = (resource as BitmapDrawable).Bitmap;
                    var p = Palette.From(bmResource).Generate();
                    // Use generated instance
                    mColorPalette = p.GetMutedColor(ContextCompat.GetColor(ParentActivity, Resource.Color.primary_dark));
                }
                catch (System.Exception ex) { }
            }

            return false;
        }
    }
}