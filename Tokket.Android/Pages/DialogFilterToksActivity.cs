using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Helpers;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "DialogFilterToksActivity", Theme = "@style/Theme.Transparent", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class DialogFilterToksActivity : BaseActivity
    {
        bool isPublicFeed = true, currentIsPublicFeed;  int filterByType = 0, currentFilterByType; string filterByItems = "", currentFilterByItems;
        bool isChanged = false;
        private int REQUEST_CLASS_FILTER = 1001;
        bool setDesceding = false;
        string setReferenceIdSort = "";
        internal static DialogFilterToksActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.dialog_filter_toks);

            Instance = this;

            currentIsPublicFeed = Intent.GetBooleanExtra("isPublicFeed", false);
            currentFilterByType = Intent.GetIntExtra("filterBy", 0);
            currentFilterByItems = Intent.GetStringExtra("filterItems");

            //Set default values
            isPublicFeed = currentIsPublicFeed;
            filterByType = currentFilterByType;
            filterByItems = currentFilterByItems;

            btnGlobalToks.Checked = isPublicFeed;
            btnFeaturedToks.Checked = !isPublicFeed;

            switch (currentFilterByType)
            {
                case (int)FilterBy.Class:
                    btnClass.Checked = true;
                    break;
                case (int)FilterBy.Category:
                    btnCategory.Checked = true;
                    break;
                case (int)FilterBy.Type:
                    btnType.Checked = true;
                    break;
                default:
                    btnFilterByAll.Checked = true;
                    break;
            }
            
            btnGlobalToks.Click += btnFilterClick;
            btnFeaturedToks.Click += btnFilterClick;

            btnFilterByAll.Click += btnFilterClick;
            btnClass.Click += btnFilterClick;
            btnCategory.Click += btnFilterClick;
            btnType.Click += btnFilterClick;


            //Sort for dialog use only, do not use setting
            //ReferenceID
            btnRefAscending.Click += btnRefAscendingClick;
            btnRefDescending.Click += btnRefDescendingClick;

            //Title Id
            btnTitleAscending.Click += btnTitleAscendingClick;
            btnTitleDescending.Click += btnTitleDescendingClick;

            btnClose.Click += delegate
            {
                Finish();
            };

#if (_CLASSTOKS)
            btnGlobalToks.Text = "Public";
            btnFeaturedToks.Text = "My Class Toks";
            setColorBtn();
#endif

            btnApplyFilter.Click += delegate
            {
                Intent = new Intent();
                Intent.PutExtra("isPublicFeed", isPublicFeed);
                Intent.PutExtra("filterBy", currentFilterByType);
                Intent.PutExtra("filterByList", currentFilterByItems);
                Intent.PutExtra("sortReferenceID", setReferenceIdSort);
                Intent.PutExtra("sortTitle", setDesceding);
                SetResult(Result.Ok, Intent);
                this.Finish();
            };
        }

        private void btnTitleDescendingClick(object sender, EventArgs e)
        {
            setDesceding = false;
            isFilterChanged(true);
        }

        private void btnTitleAscendingClick(object sender, EventArgs e)
        {
            setDesceding = true;
            isFilterChanged(true);
        }

        private void btnRefDescendingClick(object sender, EventArgs e)
        {
            setReferenceIdSort = "reference_id";
            isFilterChanged(true);
        }

        private void btnRefAscendingClick(object sender, EventArgs e)
        {
            setReferenceIdSort = "";
            isFilterChanged(true);
        }
        private Boolean isFilterChanged(bool changed = false)
        {
            isChanged = changed;
            if (currentIsPublicFeed != isPublicFeed)
            {
                isChanged = true;
            }
            else if (filterByType != currentFilterByType)
            {
                isChanged = true;
                isPublicFeed = true;
            }
            else if (filterByType == currentFilterByType && filterByItems != (currentFilterByItems + ""))
            {
                isChanged = true;
                isPublicFeed = true;
            }

            if (isChanged)
            {
                btnApplyFilter.Enabled = true;
            }
            else
            {
                btnApplyFilter.Enabled = false;
            }

            return isChanged;
        }

        private void btnFilterClick(object sender, EventArgs e)
        {
            View v = (sender as View);

            switch (v.Id)
            {
                case Resource.Id.btnGlobalToks:
                    if (!isPublicFeed)
                    {
                        isPublicFeed = true;
                    }
                    isFilterChanged();
                    break;
                case Resource.Id.btnFeaturedToks:
                    if (isPublicFeed)
                    {
                        isPublicFeed = false;
                    }
                    isFilterChanged();
                    break;
                case Resource.Id.btnFilterByAll:
                    currentFilterByType = 0;
                    currentFilterByItems = "";
                    isFilterChanged();
                    break;
                case Resource.Id.btnClass:
                case Resource.Id.btnCategory:
                case Resource.Id.btnType:
                    Intent nextActivity = new Intent(this, typeof(ClassFilterbyActivity));
                    nextActivity.PutExtra("filterByItems", filterByItems);
                    nextActivity.PutExtra("filterby", v.ContentDescription);
                    nextActivity.PutExtra("caller", "");
                    StartActivityForResult(nextActivity, REQUEST_CLASS_FILTER);
                    break;
                default:
                    break;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == REQUEST_CLASS_FILTER) && (resultCode == Result.Ok) && (data != null))
            {
                currentFilterByType = data.GetIntExtra("filterby", 0);
                currentFilterByItems = data.GetStringExtra("filterByList");
                isFilterChanged();
            }
        }

        private void setColorBtn()
        {
            btnGlobalToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_orange);
            btnFeaturedToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_orange);
        }
        public ImageButton btnClose => FindViewById<ImageButton>(Resource.Id.btnClose);
        public RadioButton btnGlobalToks => FindViewById<RadioButton>(Resource.Id.btnGlobalToks);
        public RadioButton btnFeaturedToks => FindViewById<RadioButton>(Resource.Id.btnFeaturedToks);
        public RadioButton btnFilterByAll => FindViewById<RadioButton>(Resource.Id.btnFilterByAll);
        public RadioButton btnClass => FindViewById<RadioButton>(Resource.Id.btnClass);
        public RadioButton btnCategory => FindViewById<RadioButton>(Resource.Id.btnCategory);
        public RadioButton btnType => FindViewById<RadioButton>(Resource.Id.btnType);
        public Button btnApplyFilter => FindViewById<Button>(Resource.Id.btnApplyFilter);

        public RadioButton btnRefAscending => FindViewById<RadioButton>(Resource.Id.btnRefAscending);
        public RadioButton btnRefDescending => FindViewById<RadioButton>(Resource.Id.btnRefDescending);
        public RadioButton btnTitleAscending => FindViewById<RadioButton>(Resource.Id.btnTitleAscending);
        public RadioButton btnTitleDescending => FindViewById<RadioButton>(Resource.Id.btnTitleDescending);
    }
}