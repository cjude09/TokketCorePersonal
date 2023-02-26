using Android.App;
using Android.Content;
using Android.Content.PM;
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
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Xamarin.Essentials;

namespace Tokket.Android
{
    [Activity(Label = "AddEditAbbreviation",Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddEditAbbreviation : BaseActivity
    {
        public bool isAdd = true;
        Tokket.Shared.Models.TokModel TokInfo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addedit_alphatoks);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar_addedittok);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
            SetSupportActionBar(tokback_toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SetAddEdit();
            loadCountries();
            Progress.Visibility = ViewStates.Gone;
            // Create your application here
        }

        void SetAddEdit()
        {
            isAdd = JsonConvert.DeserializeObject<string>(Intent.GetStringExtra("isAdd")) == "true" ? true : false;
            if (!isAdd) {
                TokInfo = JsonConvert.DeserializeObject<Tokket.Shared.Models.TokModel>(Intent.GetStringExtra("abbreviationInfo"));
                Category.Text = TokInfo.Category;
                Primary.Text = TokInfo.PrimaryFieldText;
                Secondary.Text = TokInfo.SecondaryFieldText;
               
            }
            if (isAdd)
            {
                AddEditButton.Text = "+ Add Tok";
                AddEditButton.Click += OnClickAddTok;


            }
            else {
                AddEditButton.Text = "Edit Tok";
                AddEditButton.Click += OnClickEditTok;
            }
        }

        private async void OnClickEditTok(object sender, EventArgs e)
        {
            var button = sender as Button;
            button.Clickable = false;
            var tokd = TokInfo;
            tokd.PrimaryFieldText = Primary.Text;
            tokd.SecondaryFieldText = Secondary.Text;
            tokd.Category = Category.Text;
            tokd.CategoryId = $"{tokd.UserId}-{tokd.PrimaryFieldText}";
            tokd.UserPhoto = Settings.GetTokketUser().UserPhoto;
            var country = CountrySpinner.GetItemAtPosition(CountrySpinner.FirstVisiblePosition).ToString();
            tokd.UserCountry = CountryHelper.GetCountryAbbreviation(country);
            if (StateLinear.Visibility == ViewStates.Visible)
            {
                var estate = StateSpinner.GetItemAtPosition(StateSpinner.FirstVisiblePosition).ToString();
                var s = estate == null ? string.Empty : estate;
                var state = CountryHelper.GetStateAbbreviation(s);
                tokd.UserState = state;
            }
            var add = await TokService.Instance.UpdateTokAsync(tokd);
            Progress.Visibility = ViewStates.Visible;
            if (add.ResultEnum == Tokket.Shared.Helpers.Result.Success)
            {
                Finish();
            }
            else
            {
                var dialogDelete = new AlertDialog.Builder(this);
                var alertD = dialogDelete.Create();
                alertD.SetTitle("");
                alertD.SetIcon(Resource.Drawable.alert_icon_blue);
                alertD.SetMessage("Something went wrong!");
                alertD.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                {

                });
                alertD.Show();
                alertD.SetCanceledOnTouchOutside(false);
                button.Clickable = true;
            }
            Progress.Visibility = ViewStates.Gone;
        }

        private async  void OnClickAddTok(object sender, EventArgs e)
        {

            var button = sender as Button;
            button.Clickable = false;
            var tokd = new Tokket.Shared.Models.TokModel();
            tokd.UserDisplayName = Settings.GetTokketUser().DisplayName;
            tokd.UserId = Settings.GetTokketUser().Id;
            tokd.PrimaryFieldText = Primary.Text;
            tokd.SecondaryFieldText = Secondary.Text;
            tokd.TokGroup = "Tokket Games";
            tokd.Category = Category.Text;
            tokd.TokType = "Alpha Guess";
            tokd.TokTypeId = "toktype-tokketgames-alphaguess";
            tokd.CategoryId = $"{tokd.UserId}-{tokd.PrimaryFieldText}";
            tokd.UserPhoto = Settings.GetTokketUser().UserPhoto;
            var country = CountrySpinner.GetItemAtPosition(CountrySpinner.FirstVisiblePosition).ToString();
            tokd.UserCountry = country;
            if (StateLinear.Visibility == ViewStates.Visible) {
                var estate = StateSpinner.GetItemAtPosition(StateSpinner.FirstVisiblePosition).ToString();
                var s = estate == null ? string.Empty : estate;
                var state = CountryHelper.GetStateAbbreviation(s);
                tokd.UserState = state;
            }
            tokd.CreatedTime = DateTime.Now;
        
            
         
                var add = await AlphaTokService.Instance.CreateTokAsync(tokd);
                Progress.Visibility = ViewStates.Visible;
                if (add.ResultEnum == Tokket.Shared.Helpers.Result.Success)
                {
                    
                    AlphaToksMain.Instance.AlphaTokDataAdapter.AddItem(tokd);
                    Finish();
                }
                else
                {
                    var dialogDelete = new AlertDialog.Builder(this);
                    var alertD = dialogDelete.Create();
                    alertD.SetTitle("");
                    alertD.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertD.SetMessage("Something went wrong!");
                    alertD.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                    {

                    });
                    alertD.Show();
                    alertD.SetCanceledOnTouchOutside(false);
                    button.Clickable = true;
            }
           
                Progress.Visibility = ViewStates.Gone;
        
           
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


        public void loadCountries()
        {
            CountrySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(txtCountry_ItemSelected);
            List<CountryModel> countryModels = CountryHelper.GetCountries();
            List<string> countriesList = new List<string>();
            countriesList.Add(string.Empty);
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
            List<Shared.Models.StateModel> stateModel = CountryHelper.GetCountryStates(countryId);
            List<string> statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            statelist.Add(string.Empty);
            for (int i = 0; i < stateModel.Count(); i++)
            {
                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            SpinnerStateAdapter adapter = new SpinnerStateAdapter(this, Resource.Layout.signup_page_state_row, statelist, imageStateList);
            StateSpinner.Adapter = adapter;

            //var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //txtState.Adapter = adapter;
        }

        private void spnState_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {

        }

        public Spinner CountrySpinner => FindViewById<Spinner>(Resource.Id.spn_country);

        public Spinner StateSpinner => FindViewById<Spinner>(Resource.Id.spn_state);

        public Button AddEditButton => FindViewById<Button>(Resource.Id.btn_addedit);

        public EditText Category => FindViewById<EditText>(Resource.Id.txtCategory);

        public EditText Primary => FindViewById<EditText>(Resource.Id.txtAbbr);

        public EditText Secondary => FindViewById<EditText>(Resource.Id.txtMeaning);
        public LinearLayout StateLinear => FindViewById<LinearLayout>(Resource.Id.linearState);

        public LinearLayout Progress => FindViewById<LinearLayout>(Resource.Id.linearProgress_toks);
    }
}