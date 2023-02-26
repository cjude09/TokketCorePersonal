using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Palette.Graphics;
using Java.Util;

namespace Tokket.Android.Helpers
{
    public class GetDominantColor
    {
        public static Color GetDominantColorImg(Bitmap bitmap, float factor)
        {

            List<Palette.Swatch> swatchesTemp = Palette.From(bitmap).Generate().Swatches.ToList();
            List<Palette.Swatch> swatches = new List<Palette.Swatch>(swatchesTemp);

            var Comparator = new SwatchesComparator();

            Collections.Sort(swatches, Comparator);

            //return swatches.Count > 0 ? swatches[0].Rgb : Color.Black;
            return swatches.Count > 0 ? ManipulateColor.manipulateColor(swatches[0].Rgb, factor) : Color.Black;
        }
        public int Compare(Palette.Swatch swatch1, Palette.Swatch swatch2)
        {
            return swatch2.Population - swatch1.Population;
        }
    }
}