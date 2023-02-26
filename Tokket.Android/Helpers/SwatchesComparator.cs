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
using AndroidX.Palette.Graphics;
using Java.Lang;
using Java.Util;

namespace Tokket.Android.Helpers
{
    public class SwatchesComparator : Java.Lang.Object, IComparator
    {

        public int Compare(Java.Lang.Object o1, Java.Lang.Object o2)
        {
            Palette.Swatch swatch1 = (Palette.Swatch)o1;
            Palette.Swatch swatch2 = (Palette.Swatch)o2;


            return swatch2.Population - swatch1.Population;
        }
    }
}