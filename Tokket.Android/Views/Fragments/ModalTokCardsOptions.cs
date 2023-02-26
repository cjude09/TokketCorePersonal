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
using static Tokket.Android.TokCardsMiniGameActivity;

namespace Tokket.Android.Fragments
{
    public class ModalTokCardsOptions : AndroidX.AppCompat.App.AppCompatDialogFragment
    {
        TokCardsMiniGameActivity tokCardsMiniGameActivity;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            tokCardsMiniGameActivity = Activity as TokCardsMiniGameActivity;
            var v = inflater.Inflate(Resource.Layout.modal_tokcards_option, container, false);
            var switchTokCardsPlayStarred = v.FindViewById<Switch>(Resource.Id.switchTokCardsPlayStarred);
            var switchTokCardsShowImages = v.FindViewById<Switch>(Resource.Id.switchTokCardsShowImages);
            var btnTokCardsRefresh = v.FindViewById<Button>(Resource.Id.btnTokCardsRefresh);
            var btnTokCardsOptionFlip = v.FindViewById<Button>(Resource.Id.btnTokCardsOptionFlip);

            Typeface font = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            btnTokCardsRefresh.SetTypeface(font, TypefaceStyle.Bold);
            btnTokCardsOptionFlip.SetTypeface(font, TypefaceStyle.Bold);

            Bundle mArgs = Arguments;
            String isPlayFavorite = mArgs.GetString("isPlayFavorite");
            string isImageVisible = mArgs.GetString("isImageVisible");

            if (isPlayFavorite.ToLower() == "true")
            {
                switchTokCardsPlayStarred.Checked = true;
            }
            else
            {
                switchTokCardsPlayStarred.Checked = false;
            }

            if (isImageVisible.ToLower() == "true")
            {
                switchTokCardsShowImages.Checked = true;
            }
            else
            {
                switchTokCardsShowImages.Checked = false;
            }

            btnTokCardsRefresh.Click -= OnRefreshClick;
            btnTokCardsRefresh.Click += OnRefreshClick;

            btnTokCardsOptionFlip.Click += (object sender, EventArgs e) =>
            {
                tokCardsMiniGameActivity.OnFlipCard(sender,e);
            };

            switchTokCardsPlayStarred.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokCardsMiniGameActivity.removeFragmentinFL();
                tokCardsMiniGameActivity.isPlayFavorite = switchTokCardsPlayStarred.Checked;
                tokCardsMiniGameActivity.TriggerFavorite();
            };

            switchTokCardsShowImages.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokCardsMiniGameActivity.isImageVisible = switchTokCardsShowImages.Checked;
                tokCardsMiniGameActivity.loadFrontCard();
            };
            return v;
        }
        public void OnRefreshClick(object sender, EventArgs e)
        {
            tokCardsMiniGameActivity.isFavorite = Enumerable.Repeat(false, tokCardsMiniGameActivity.TokLists.Count).ToList();
            tokCardsMiniGameActivity.cnt = 0;
            tokCardsMiniGameActivity.cardProgress.Progress = tokCardsMiniGameActivity.cnt + 1;
            tokCardsMiniGameActivity.cardProgressText.Text = tokCardsMiniGameActivity.cardProgress.Progress + "/" + tokCardsMiniGameActivity.cardProgress.Max;
            tokCardsMiniGameActivity.loadFrontCard();

            tokCardsMiniGameActivity.btnPrevious.Enabled = false;
            if (tokCardsMiniGameActivity.cardProgress.Max > 1)
            {
                tokCardsMiniGameActivity.btnNext.Enabled = true;
            }
        }
    }
}