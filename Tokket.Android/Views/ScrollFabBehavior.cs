using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.View;
using Google.Android.Material.FloatingActionButton;
using Java.Lang;
using Tokket.Android;

namespace HideMyFab
{

	[Register("HideMyFab.ScrollAwareFABBehavior")]
	public class ScrollAwareFABBehavior : CoordinatorLayout.Behavior
	{
        FloatingActionButton floatingActionButtonChild;
        public ScrollAwareFABBehavior(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}
        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes, int type)
        {
            return axes == ViewCompat.ScrollAxisVertical || base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, axes, type);
        }
        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed, int type)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed, type);

            floatingActionButtonChild = child.JavaCast<FloatingActionButton>();

            if (dyConsumed > 0 && floatingActionButtonChild.Visibility == ViewStates.Visible)
            {
                Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaledown);
                floatingActionButtonChild.StartAnimation(myAnim);
                floatingActionButtonChild.Visibility = ViewStates.Invisible;
            }
            else if (dyConsumed < 0 && floatingActionButtonChild.Visibility != ViewStates.Visible)
            {
                floatingActionButtonChild.Show();
                Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaleup);
                floatingActionButtonChild.StartAnimation(myAnim);
            }
        }
    }

}