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
using AndroidX.ViewPager.Widget;

namespace Tokket.Android.Helpers
{
    public class FadeTransformerViewPager : Java.Lang.Object, ViewPager.IPageTransformer
    {
        private const float MaxAngle = 30F;
        public void TransformPage(View view, float position)
        {
            if (position < -1 || position > 1)
            {
                view.Alpha = 0; // The view is offscreen.
            }
            else
            {
                view.Alpha = 1;

                view.PivotY = view.Height / 2; // The Y Pivot is halfway down the view.

                // The X pivots need to be on adjacent sides.
                if (position < 0)
                {
                    view.PivotX = view.Width;
                }
                else
                {
                    view.PivotX = 0;
                }

                view.RotationY = MaxAngle * position; // Rotate the view.
            }
        }
    }
}