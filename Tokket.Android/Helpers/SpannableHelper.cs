using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Net;
using Tokket.Shared.Helpers;
using Tokket.Core;

namespace Tokket.Android.Helpers
{
    public class SpannableHelper
    {
        //private static readonly List<Tokmoji> ListTokMoji;
        public static List<Tokmoji> ListTokMoji;

        static SpannableHelper()
        {
            ListTokMoji = new List<Tokmoji>();

            //TokMoji = new List<PurchaseModel>();
            //var resultTokMoji = PurchasesHelper.GetProducts().Where(x => x.ProductType == ProductType.Tokmoji).ToList();
            //resultTokMoji = resultTokMoji.OrderBy(x => x.Name).ToList();
            //foreach (var stickers in resultTokMoji)
            //{
            //    TokMoji[0].Id = ":" + stickers.Id + ":";
            //    TokMoji.Add(stickers);
            //}
        }
        public static ISpannable AddStickersSpannable(Context context, ISpannable spannable)
        {
            foreach (var entry in ListTokMoji)
            {
                var smiley = ":" + entry.Id + ":";
                var smileyImage = entry.Image;
                var indices = spannable.ToString().IndexesOf(smiley);
                foreach (var index in indices)
                {
                    var set = true;
                    foreach (ImageSpan span in spannable.GetSpans(index, index + smiley.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                    {
                        if (spannable.GetSpanStart(span) >= index && spannable.GetSpanEnd(span) <= index + smiley.Length)
                            spannable.RemoveSpan(span);
                        else
                        {
                            set = false;
                            break;
                        }
                    }
                    if (set)
                    {
                        System.Net.WebRequest request = System.Net.WebRequest.Create(smileyImage);
                        //System.Net.WebResponse response = request.GetResponse();
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            System.IO.Stream responseStream = response.GetResponseStream();
                            Bitmap bitmap = BitmapFactory.DecodeStream(responseStream);
                            Bitmap scaledBitmap = scaleDown(bitmap, 500, true);
                            spannable.SetSpan(new ImageSpan(context, scaledBitmap), index, index + smiley.Length, SpanTypes.ExclusiveExclusive);
                        }
                    }
                }
            }
            return spannable;
        }

        public static bool AddStickersBool(Context context, ISpannable spannable)
        {
            var hasChanges = false;
            foreach (var entry in ListTokMoji)
            {
                var smiley = entry.Id;
                var smileyImage = entry.Image;
                var indices = spannable.ToString().IndexesOf(smiley);
                foreach (var index in indices)
                {
                    var set = true;
                    foreach (ImageSpan span in spannable.GetSpans(index, index + smiley.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                    {
                        if (spannable.GetSpanStart(span) >= index && spannable.GetSpanEnd(span) <= index + smiley.Length)
                            spannable.RemoveSpan(span);
                        else
                        {
                            set = false;
                            break;
                        }
                    }
                    if (set)
                    {
                        System.Net.WebRequest request = System.Net.WebRequest.Create(smileyImage);
                        //System.Net.WebResponse response = request.GetResponse();
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            System.IO.Stream responseStream = response.GetResponseStream();
                            Bitmap bitmap = BitmapFactory.DecodeStream(responseStream);
                            spannable.SetSpan(new ImageSpan(context, bitmap), index, index + smiley.Length, SpanTypes.ExclusiveExclusive);
                        }
                    }
                }
            }
            return hasChanges;
        }
        public static ISpannable GetSmiledText(Context context, ICharSequence text)
        {
            var spannable = SpannableFactory.Instance.NewSpannable(text);
            AddStickersBool(context, spannable);
            return spannable;
        }


        public static void AddSmiley(string textSmiley, string smileyResource)
        {
            var tokmojisItem = new Tokmoji();
            tokmojisItem.Id = textSmiley;
            tokmojisItem.Image = smileyResource;

            ListTokMoji.Add(tokmojisItem);
        }
        public static Bitmap scaleDown(Bitmap realImage, float maxImageSize, bool filter)
        {
            float ratio = Java.Lang.Math.Min(
                    (float)maxImageSize / realImage.Width,
                    (float)maxImageSize / realImage.Height);
            int width = Java.Lang.Math.Round((float)ratio * realImage.Width);
            int height = Java.Lang.Math.Round((float)ratio * realImage.Height);

            Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width,
                    height, filter);
            return newBitmap;
        }

    }

    internal class ImageDownloadInfo
    {
    }

    //Taken from http://stackoverflow.com/a/767788/368379
    public static class StringExtensions
    {
        public static IEnumerable<int> IndexesOf(this string haystack, string needle)
        {
            var lastIndex = 0;
            while (true)
            {
                var index = haystack.IndexOf(needle, lastIndex);
                if (index == -1)
                {
                    yield break;
                }
                yield return index;
                lastIndex = index + needle.Length;
            }
        }
    }
}