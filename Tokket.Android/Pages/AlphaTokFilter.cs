using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Core.Tools;

namespace Tokket.Android
{
    [Activity(Label = "AlphaTokFilter")]
    public class AlphaTokFilter : BaseActivity
    {
        TokQueryValues Values = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.alphatok_filter);
            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.alphatoksfilter_toolbar);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
            SetSupportActionBar(tokback_toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            CategoryText.TextChanged += OnCategoryTextChanged;
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, new string[] { "All Abbreviations","My Abbreviations" });
            adapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            ContentSpinner.Adapter = adapter;
            loadCountries();
            try
            {
                Values = JsonConvert.DeserializeObject<TokQueryValues>(Intent.GetStringExtra("TokQuery"));
            }
            catch (Exception ex)
            {
                Values = null;
            }

            FilterButton.Click += OnClickFilter;
            ClearCountry.Click += OnClearItem;
            ClearState.Click += OnClearItem;
            ClearCategory.Click += OnClearItem;

            StateLinear.Visibility = ViewStates.Visible;
            StateSpinner.Visibility = ViewStates.Visible;
            // Create your application here
        }

        private void OnClearItem(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Id) {
                case Resource.Id.btn_catClear: CategoryText.Text = string.Empty; ClearCategory.Visibility = ViewStates.Gone; break;
                case Resource.Id.btn_countryClear: CountrySpinner.SetSelection(0); ClearCountry.Visibility = ViewStates.Gone; break;
                case Resource.Id.btn_stateClear: StateSpinner.SetSelection(0); ClearState.Visibility = ViewStates.Gone; break;
            }
        }

        private void OnClickFilter(object sender, EventArgs e)
        {
            if (Values == null)
                Values = new TokQueryValues() { serviceid = "alphaguess", itemsbase = "toks" };

            var selectedCountry = CountrySpinner.GetItemAtPosition(CountrySpinner.FirstVisiblePosition).ToString();
            Values.country = selectedCountry == "Country" ? string.Empty: selectedCountry  ;
            if (StateLinear.Visibility == ViewStates.Visible) {
                var state = string.IsNullOrEmpty(StateSpinner.GetItemAtPosition(StateSpinner.FirstVisiblePosition).ToString()) ? string.Empty : StateSpinner.GetItemAtPosition(StateSpinner.FirstVisiblePosition).ToString();
              //  Values. = state;
            }

            Values.category = CategoryText.Text;
            var id = ContentSpinner.GetItemAtPosition(ContentSpinner.FirstVisiblePosition).ToString() == "My Abbreviations" ? Settings.GetTokketUser().Id:string.Empty;
            Values.userid = id;

            var intent = new Intent(this, typeof(AlphaToksMain));
            intent.PutExtra("TokQuery", JsonConvert.SerializeObject(Values));

            StartActivity(intent);
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
        
                    Finish();
                    break;
            }
          return  base.OnOptionsItemSelected(item);
        }

        private void OnCategoryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CategoryText.Text.Length > 0)
            {
               
                ClearCategory.Visibility = ViewStates.Visible;
            }
            else {
             
                ClearCategory.Visibility = ViewStates.Gone;
            }
        }

        public void loadCountries()
        {
            CountrySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(txtCountry_ItemSelected);
            List<CountryModel> countryModels = CountryHelper.GetCountries();
            List<string> countriesList = new List<string>();
            countriesList.Add("Country");
            for (int i = 0; i < countryModels.Count(); i++)
            {
                countriesList.Add(countryModels[i].Name);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, countriesList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            CountrySpinner.Adapter = adapter;
        }
        private void txtCountry_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
           
            Spinner spinner = (Spinner)sender;
            if (e.Position == 0)
                ClearCountry.Visibility = ViewStates.Gone;
            else
                ClearCountry.Visibility = ViewStates.Visible;

            if (spinner.GetItemAtPosition(e.Position).ToString().ToLower() == "united states")
            {
               

                StateLinear.Visibility = ViewStates.Visible;
                loadState("us");
            }
            else
            {

                StateLinear.Visibility = ViewStates.Gone;
                StateSpinner.Adapter = null;
            }
        }
        public void loadState(string countryId)
        {
            StateSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spnState_ItemSelected);
            List<Tokket.Shared.Models.StateModel> stateModel = CountryHelper.GetCountryStates(countryId);
            List<string> statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            statelist.Add("State");
            for (int i = 0; i < stateModel.Count(); i++)
            {
                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, statelist);
            StateSpinner.Adapter = adapter;

            //var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //txtState.Adapter = adapter;
        }

        private void spnState_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 0)
                ClearState.Visibility = ViewStates.Gone;
            else
                ClearState.Visibility = ViewStates.Visible;
        }


        public EditText CategoryText => FindViewById<EditText>(Resource.Id.txt_category);
        public Spinner ContentSpinner => FindViewById<Spinner>(Resource.Id.spn_content);
        public Spinner CountrySpinner => FindViewById<Spinner>(Resource.Id.spn_country);
       public Spinner StateSpinner => FindViewById<Spinner>(Resource.Id.spn_state);
        public Button ClearCategory => FindViewById<Button>(Resource.Id.btn_catClear);
        public Button ClearContent => FindViewById<Button>(Resource.Id.btn_conClear);
        public Button ClearCountry => FindViewById<Button>(Resource.Id.btn_countryClear);
        public Button ClearState => FindViewById<Button>(Resource.Id.btn_stateClear);

        public Button FilterButton => FindViewById<Button>(Resource.Id.btn_applyFilter);

       public LinearLayout StateLinear => FindViewById<LinearLayout>(Resource.Id.linear_state);
    }
}