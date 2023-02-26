using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Tokket.Android
{
    [Register("Tokket.Android.SquareImageView")]
    public class SquareImageView : ImageView
    {
        Context mContext;
        public SquareImageView(Context context) : base(context)
        {
            Init(context, null);
        }
        public SquareImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs);
        }
        private void Init(Context ctx, IAttributeSet attrs)
        {
            mContext = ctx;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(MeasuredWidth, MeasuredWidth);
        }
    }
}