using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services;
using Tokket.Core;
using Android.Text;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "ProfileGenericTitleBuyActivity", Theme ="@style/CustomAppThemeBlue")]
    public class ProfileGenericTitleBuyActivity : BaseActivity
    {
        List<TokketTitle> TokketTitles;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.profilegenerictitlesfragment_page);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.profilegeneric_toolbar);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
        //    SetSupportActionBar(tokback_toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            InitializeTitles();

            CategorySpinner.ItemSelected += OnCategoryItemSelected;
            ValueSpinner.ItemSelected += OnValueItemSelected;
            SelectedText.SetRawInputType(InputTypes.Null);

            BuyButton.Click += OnBuyClicked;
            // Create your application here
        }

        private void OnValueItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = sender as Spinner;
            var item = spinner.GetItemAtPosition(e.Position).ToString();
            SelectedText.Text = item;
        }

        private void OnCategoryItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = sender as Spinner;
            var item = spinner.GetItemAtPosition(e.Position).ToString();
            SortTitleValyeByCategory(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                  
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        async void InitializeTitles() {
            var titles = await Tokket.Shared.Services.AccountService.Instance.GetGenericTitlesAsync("");
            TokketTitles = titles.Results.ToList();
            var categoryTitles = new List<string>();

            foreach (var title in TokketTitles) {
                if (!categoryTitles.Contains(title.Category)) {
                    categoryTitles.Add(title.Category);                
                }   
            }

            ArrayAdapter<string> categoryAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, categoryTitles.ToArray());
            categoryAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            CategorySpinner.Adapter = categoryAdapter;
            SortTitleValyeByCategory(categoryTitles[0]);
        }

        void SortTitleValyeByCategory(string category) {
            List<string> newList = new List<string>();
            foreach (var title in TokketTitles) {
                if (title.Category == category) {
                    newList.Add(title.TitleDisplay);
                }
            }
            ArrayAdapter<string> valueAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, newList.ToArray());
            valueAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            ValueSpinner.Adapter = valueAdapter;

         
        }
        private async void OnBuyClicked(object sender, EventArgs e)
        {
            LinearProgress.Visibility = ViewStates.Visible;
            var item = PurchasesTool.GetProduct("title_generic_tokket");
            item.Name = SelectedText.Text;
            var billing = await PurchaseService.Instance.BillingStart(item.Id, item,titleId:SelectedText.Text);
            if (billing)
            {
                LinearProgress.Visibility = ViewStates.Gone;
                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                var objBuilder = new AlertDialog.Builder(this);
                objBuilder.SetTitle("");
                objBuilder.SetMessage("Title Purchased Complete!");
                objBuilder.SetCancelable(false);

                AlertDialog objDialog = objBuilder.Create();
                objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                {
                    SetResult(AppResult.Ok);
                    this.Finish();

                });
                objDialog.Show();
            }
            else {
                LinearProgress.Visibility = ViewStates.Gone;
                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                var objBuilder = new AlertDialog.Builder(this);
                objBuilder.SetTitle("");
                objBuilder.SetMessage("Title Purchase Failed");
                objBuilder.SetCancelable(false);

                AlertDialog objDialog = objBuilder.Create();
                objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                {

                    this.Finish();

                });
                objDialog.Show();
            }
        }
        public Spinner CategorySpinner => FindViewById<Spinner>(Resource.Id.CatgorySpinner);
        public Spinner ValueSpinner => FindViewById<Spinner>(Resource.Id.ValueSpinner);

        public EditText SelectedText => FindViewById<EditText>(Resource.Id.SelectedTitle);

        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);

        public Button BuyButton => FindViewById<Button>(Resource.Id.BuyButtonGeneric);
    }
}