using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Tokket.Android.Helpers
{
    public class BetterLinkMovementMethod : LinkMovementMethod
    {
        #region Fields

        private static BetterLinkMovementMethod singleInstance;
        private const int LINKIFY_NONE = -2;
        private const int MAX_LINK_LENGTH = 30;

        private IOnLinkClickListener onLinkClickListener;
        private IOnLinkLongClickListener onLinkLongClickListener;
        private RectF touchedLineBounds = new RectF();
        private ClickableSpan clickableSpanUnderTouchOnActionDown;
        private int activeTextViewHashcode;
        private LongPressTimer ongoingLongPressTimer;
        private bool wasLongPressRegistered;

        #endregion

        #region Interfaces

        public interface IOnLinkClickListener
        {
            bool OnClick(TextView textView, string url);
        }

        public interface IOnLinkLongClickListener
        {
            bool OnLongClick(TextView textView, string url);
        }

        #endregion

        #region Constructor

        protected BetterLinkMovementMethod()
        {
        }

        #endregion

        #region Private

        private void CleanupOnTouchUp(TextView textView)
        {
            wasLongPressRegistered = false;
            clickableSpanUnderTouchOnActionDown = null;
            RemoveLongPressCallback(textView);
        }

        private static void AddLinks(MatchOptions linkifyMask, BetterLinkMovementMethod movementMethod, TextView textView, bool removeUnderline = true)
        {
            textView.MovementMethod = movementMethod;

            SpannableStringBuilder ssb = new SpannableStringBuilder(textView.Text);
            global::Android.Text.Util.Linkify.AddLinks(ssb, linkifyMask);

            URLSpan[] spans = ssb.GetSpans(0, ssb.Length(), Class.FromType(typeof(URLSpan))).Cast<URLSpan>().ToArray();

            foreach (var span in spans)
            {
                int start = ssb.GetSpanStart(span);
                int end = ssb.GetSpanEnd(span);
                var flags = ssb.GetSpanFlags(span);

                string linkText = ssb.SubSequence(start, end);

                if (linkText.Length > MAX_LINK_LENGTH)
                {
                    //1 - Remove the https:// or http:// prefix
                    if (linkText.ToString().ToLower().StartsWith("https://"))
                        linkText = linkText.Substring("https://".Length, linkText.Length - "https://".Length);
                    else if (linkText.ToString().ToLower().StartsWith("http://"))
                        linkText = linkText.Substring("http://".Length, linkText.Length - "http://".Length);

                    // 2 - Remove the www. prefix
                    else if (linkText.ToString().ToLower().StartsWith("www."))
                        linkText = linkText.Substring("www.".Length, linkText.Length - "www.".Length);

                    // 3 - Truncate if still longer than MAX_LINK_LENGTH
                    if (linkText.Length > MAX_LINK_LENGTH && linkifyMask.HasFlag(MatchOptions.WebUrls))
                        linkText = linkText.Substring(0, MAX_LINK_LENGTH) + "...";
                }

                // 4 - Replace the text preserving the spans
                ssb.Replace(start, end, linkText);
                ssb.RemoveSpan(span);

                if (removeUnderline)
                {
                    URLSpanNoUnderline s = new URLSpanNoUnderline(span.URL);
                    ssb.SetSpan(s, start, start + linkText.Length, flags);
                }
                else
                    ssb.SetSpan(span, start, start + linkText.Length, flags);
            }

            textView.SetText(ssb, TextView.BufferType.Spannable);
        }

        private static void RecursivelyAddLinks(MatchOptions linkifyMask, ViewGroup viewGroup, BetterLinkMovementMethod movementMethod)
        {
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                View child = viewGroup.GetChildAt(i);

                if (child is ViewGroup)
                {
                    // Recursively find child TextViews.
                    RecursivelyAddLinks(linkifyMask, ((ViewGroup)child), movementMethod);

                }
                else if (child is TextView textView)
                {
                    AddLinks(linkifyMask, movementMethod, textView);
                }
            }
        }

        private void StartTimerForRegisteringLongClick(TextView textView, LongPressTimer.IOnTimerReachedListener longClickListener)
        {
            ongoingLongPressTimer = new LongPressTimer();
            ongoingLongPressTimer.SetOnTimerReachedListener(longClickListener);
            textView.PostDelayed(ongoingLongPressTimer, ViewConfiguration.LongPressTimeout);
        }

        #endregion

        #region Protected

        protected ClickableSpan FindClickableSpanUnderTouch(TextView textView, ISpannable text, MotionEvent e)
        {
            // So we need to find the location in text where touch was made, regardless of whether the TextView
            // has scrollable text. That is, not the entire text is currently visible.
            int touchX = (int)e.GetX();
            int touchY = (int)e.GetY();

            // Ignore padding.
            touchX -= textView.TotalPaddingLeft;
            touchY -= textView.TotalPaddingTop;

            // Account for scrollable text.
            touchX += textView.ScrollX;
            touchY += textView.ScrollY;

            Layout layout = textView.Layout;
            int touchedLine = layout.GetLineForVertical(touchY);
            int touchOffset = layout.GetOffsetForHorizontal(touchedLine, touchX);

            touchedLineBounds.Left = layout.GetLineLeft(touchedLine);
            touchedLineBounds.Top = layout.GetLineTop(touchedLine);
            touchedLineBounds.Right = layout.GetLineWidth(touchedLine) + touchedLineBounds.Left;
            touchedLineBounds.Bottom = layout.GetLineBottom(touchedLine);

            if (touchedLineBounds.Contains(touchX, touchY))
            {
                // Find a ClickableSpan that lies under the touched area.
                Java.Lang.Object[] spans = text.GetSpans(touchOffset, touchOffset, Class.FromType(typeof(ClickableSpan)));
                foreach (Java.Lang.Object span in spans)
                {
                    if (span is ClickableSpan)
                    {
                        return (ClickableSpan)span;
                    }
                }
                // No ClickableSpan found under the touched location.
                return null;

            }
            else
            {
                // Touch lies outside the line's horizontal bounds where no spans should exist.
                return null;
            }
        }

        //Remove the long-press detection timer.
        protected void RemoveLongPressCallback(TextView textView)
        {
            if (ongoingLongPressTimer != null)
            {
                textView.RemoveCallbacks(ongoingLongPressTimer);
                ongoingLongPressTimer = null;
            }
        }

        protected void DispatchUrlClick(TextView textView, ClickableSpan clickableSpan)
        {
            ClickableSpanWithText clickableSpanWithText = ClickableSpanWithText.OfSpan(textView, clickableSpan);
            bool handled = onLinkClickListener != null && onLinkClickListener.OnClick(textView, clickableSpanWithText.Text);

            if (!handled)
            {
                // Let Android handle this click.
                clickableSpanWithText.Span.OnClick(textView);
            }
        }

        protected void DispatchUrlLongClick(TextView textView, ClickableSpan clickableSpan)
        {
            ClickableSpanWithText clickableSpanWithText = ClickableSpanWithText.OfSpan(textView, clickableSpan);
            bool handled = onLinkLongClickListener != null && onLinkLongClickListener.OnLongClick(textView, clickableSpanWithText.Text);

            if (!handled)
            {
                // Let Android handle this long click as a short-click.
                clickableSpanWithText.Span.OnClick(textView);
            }
        }

        #endregion

        #region Public

        //Get a static instance of BetterLinkMovementMethod. Do note that registering a click listener on the returned
        //instance is not supported because it will potentially be shared on multiple TextViews.
        public static BetterLinkMovementMethod GetInstance()
        {
            if (singleInstance == null)
            {
                singleInstance = new BetterLinkMovementMethod();
            }
            return singleInstance;
        }

        public static BetterLinkMovementMethod NewInstance()
        {
            return new BetterLinkMovementMethod();
        }

        public static BetterLinkMovementMethod Linkify(MatchOptions linkifyMask, List<TextView> textViews)
        {
            BetterLinkMovementMethod movementMethod = NewInstance();
            foreach (var textView in textViews)
            {
                AddLinks(linkifyMask, movementMethod, textView);
            }
            return movementMethod;
        }

        public static BetterLinkMovementMethod Linkify(MatchOptions linkifyMask, ViewGroup viewGroup)
        {
            BetterLinkMovementMethod movementMethod = NewInstance();
            RecursivelyAddLinks(linkifyMask, viewGroup, movementMethod);
            return movementMethod;
        }

        public static BetterLinkMovementMethod Linkify(MatchOptions linkifyMask, Activity activity)
        {
            // Find the layout passed to setContentView().
            ViewGroup activityLayout = ((ViewGroup)((ViewGroup)activity.FindViewById(Window.IdAndroidContent)).GetChildAt(0));

            BetterLinkMovementMethod movementMethod = NewInstance();
            RecursivelyAddLinks(linkifyMask, activityLayout, movementMethod);

            return movementMethod;
        }

        //Set a listener that will get called whenever any link is clicked on the TextView.
        public BetterLinkMovementMethod SetOnLinkClickListener(IOnLinkClickListener clickListener)
        {
            if (this == singleInstance)
            {
                throw new UnsupportedOperationException("Setting a click listener on the instance returned by getInstance() is not supported to avoid memory " +
                    "leaks. Please use newInstance() or any of the linkify() methods instead.");
            }

            this.onLinkClickListener = clickListener;
            return this;
        }

        //Set a listener that will get called whenever any link is clicked on the TextView.
        public BetterLinkMovementMethod SetOnLinkLongClickListener(IOnLinkLongClickListener longClickListener)
        {
            if (this == singleInstance)
            {
                throw new UnsupportedOperationException("Setting a long-click listener on the instance returned by getInstance() is not supported to avoid " +
                    "memory leaks. Please use newInstance() or any of the linkify() methods instead.");
            }

            this.onLinkLongClickListener = longClickListener;
            return this;
        }

        public override bool OnTouchEvent(TextView textView, ISpannable text, MotionEvent e)
        {
            if (activeTextViewHashcode != textView.GetHashCode())
            {
                // Bug workaround: TextView stops calling onTouchEvent() once any URL is highlighted.
                // A hacky solution is to reset any "autoLink" property set in XML. But we also want
                // to do this once per TextView.
                activeTextViewHashcode = textView.GetHashCode();
                textView.AutoLinkMask = 0;
            }

            ClickableSpan clickableSpanUnderTouch = FindClickableSpanUnderTouch(textView, text, e);
            if (e.Action == MotionEventActions.Down)
            {
                clickableSpanUnderTouchOnActionDown = clickableSpanUnderTouch;
            }
            bool touchStartedOverAClickableSpan = clickableSpanUnderTouchOnActionDown != null;

            switch (e.Action)
            {
                case MotionEventActions.Down:

                    if (touchStartedOverAClickableSpan && onLinkLongClickListener != null)
                    {
                        LongPressTimer.IOnTimerReachedListener longClickListener = new CustomTimerReachedListener(() =>
                        {
                            wasLongPressRegistered = true;
                            textView.PerformHapticFeedback(FeedbackConstants.LongPress);
                            DispatchUrlLongClick(textView, clickableSpanUnderTouch);
                        });


                        StartTimerForRegisteringLongClick(textView, longClickListener);
                    }
                    return touchStartedOverAClickableSpan;
                case MotionEventActions.Up:
                    // Register a click only if the touch started and ended on the same URL.
                    if (!wasLongPressRegistered && touchStartedOverAClickableSpan && clickableSpanUnderTouch == clickableSpanUnderTouchOnActionDown)
                    {
                        DispatchUrlClick(textView, clickableSpanUnderTouch);
                    }
                    CleanupOnTouchUp(textView);

                    // Consume this event even if we could not find any spans to avoid letting Android handle this event.
                    // Android's TextView implementation has a bug where links get clicked even when there is no more text
                    // next to the link and the touch lies outside its bounds in the same direction.
                    return touchStartedOverAClickableSpan;

                case MotionEventActions.Cancel:
                    CleanupOnTouchUp(textView);
                    return false;

                case MotionEventActions.Move:
                    // Stop listening for a long-press as soon as the user wanders off to unknown lands.
                    if (clickableSpanUnderTouch != clickableSpanUnderTouchOnActionDown)
                    {
                        RemoveLongPressCallback(textView);
                    }

                    return touchStartedOverAClickableSpan;

                default:
                    return false;
            }
        }

        #endregion

        #region Support classes

        private class LongPressTimer : Java.Lang.Object, IRunnable
        {
            private IOnTimerReachedListener onTimerReachedListener;

            public LongPressTimer()
            {
            }

            public interface IOnTimerReachedListener
            {
                void OnTimerReached();
            }

            public void Run()
            {
                onTimerReachedListener.OnTimerReached();
            }

            public void SetOnTimerReachedListener(IOnTimerReachedListener listener)
            {
                onTimerReachedListener = listener;
            }
        }

        private class CustomTimerReachedListener : LongPressTimer.IOnTimerReachedListener
        {
            public Action Action { get; set; }

            public CustomTimerReachedListener(Action action)
            {
                Action = action;
            }

            public void OnTimerReached()
            {
                Action();
            }
        }


        // A wrapper to support all {@link ClickableSpan}s that may or may not provide URLs.
        private class ClickableSpanWithText
        {
            public ClickableSpan Span { get; set; }
            public string Text { get; set; }

            public ClickableSpanWithText(ClickableSpan span, string text)
            {
                Span = span;
                Text = text;
            }

            public static ClickableSpanWithText OfSpan(TextView textView, ClickableSpan span)
            {
                SpannedString s = new SpannedString(textView.Text);
                string text;
                if (span is URLSpan)
                {
                    text = ((URLSpan)span).URL;
                }
                else
                {
                    int start = s.GetSpanStart(span);
                    int end = s.GetSpanEnd(span);
                    text = s.SubSequence(start, end).ToString();
                }
                return new ClickableSpanWithText(span, text);
            }
        }

        #endregion
    }
    public class URLSpanNoUnderline : URLSpan
    {
        public URLSpanNoUnderline(string url) : base(url)
        {
        }

        public override void UpdateDrawState(TextPaint ds)
        {
            base.UpdateDrawState(ds);
            ds.UnderlineText = false;
        }
    }
}