using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "AddOpportunityActivity")]
    public class AddOpportunityActivity : BaseActivity
    {
        internal static AddOpportunityActivity Instance { get; private set; }
        string country = "";
        OpportunityTok OpportunityTok = new OpportunityTok();
        List<string> countriesList = new List<string>();
        bool isEdit = false;
        string[] OpportunityTypes = new string[] { "Choose Type...", "Job", "Internship", "Scholarship" };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addopportunity_page);

            Instance = this;

            CancelButton.Click += (obj, _event) => { 
                Finish(); 
            };

            isEdit = !string.IsNullOrEmpty(Intent.GetStringExtra("isEdit"));
            if (isEdit) {
                OpportunityTok = JsonConvert.DeserializeObject<OpportunityTok>(Intent.GetStringExtra("opportunityModel"));
                SaveOpportunityButton.Text = "Update Opportunity";
            }
             
            var adapter1 = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, OpportunityTypes);
            adapter1.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            OpportunityTypeSpinner.Adapter = adapter1;
            loadCountries();
            DatePicker.Click += OnClick_DueDate;
            SaveOpportunityButton.Click += OnClick_AddOpportunity;
            InitData();
            // Create your application here
        }

        private async void OnClick_AddOpportunity(object sender, EventArgs e)
        {
            linearLayoutProgress.Visibility = ViewStates.Visible;
       
            OpportunityTok.OpportunityType = OpportunityTypeSpinner.GetItemAtPosition(OpportunityTypeSpinner.SelectedItemPosition).ToString().ToLower();
            OpportunityTok.PrimaryFieldText = TitleView.Text;
            OpportunityTok.SecondaryFieldText = CompanyView.Text;
            OpportunityTok.ApplicationDeadline = DatePicker.Text;
            if (string.IsNullOrEmpty(AmountView.Text))
                OpportunityTok.Amount = 0;
            else
                OpportunityTok.Amount = double.Parse(AmountView.Text);
            OpportunityTok.Description = DescriptionView.Text;
            OpportunityTok.Email = ContactInfoView.Text;
            OpportunityTok.HomeAddress = AddressView.Text;
            OpportunityTok.Description = DescriptionView.Text;
            OpportunityTok.Requirements = RequirementsView.Text;
            OpportunityTok.AboutCompany = AboutCompanyView.Text;
            OpportunityTok.PhoneNumber = PhoneNumberView.Text;
            OpportunityTok.ItemCountry = country;

            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageDisplay.ContentDescription))
                {
                    OpportunityTok.Image = "data:image/jpeg;base64," + ImageDisplay.ContentDescription;
                }
            }
            var result = new ResultModel() { };
            if (!isEdit)
            {
                result = await TokService.Instance.CreateOpportunityAsync(OpportunityTok);
            }
            else
            {
                result = await TokService.Instance.UpdateOpportunityAsync(OpportunityTok);
            }
            
            if (result.ResultEnum == Shared.Helpers.Result.Success)
            {
                alertMessage("", "Opportunity successfully saved!", 0, (d, fv) => {
                    //   OpportunityTok = JsonConvert.DeserializeObject<OpportunityTok>(result.ResultObject.ToString());
                    if (!isEdit) {
                        var resultData = JsonConvert.SerializeObject(result.ResultObject);
                        Intent intent = new Intent();
                        intent.PutExtra("OppData", resultData);
                        SetResult(Result.Ok, intent);
                    }
                    else
                    {
                        if (result.ResultObject != null)
                        {
                            OpportunityTok = JsonConvert.DeserializeObject<OpportunityTok>(result.ResultObject.ToString());
                        }

                        if (OpportunityTok.Image.Contains("data:image/jpeg;base64,"))
                        {
                            //Set image to empty because this will cause to OOM or error if setResult is called.
                            //result.ResultObject should return the updated tok to avoid any issue
                            OpportunityTok.Image = "";
                        }

                        OpportunityListActivity.Instance.UpdateOpportunityItem(OpportunityTok);

                        Intent intent = new Intent();
                        intent.PutExtra("updatedOpportunity", JsonConvert.SerializeObject(OpportunityTok));
                        SetResult(Result.Ok, intent);
                    }
                    Finish();
                });
            }
            else
            {
                alertMessage("", "Opportunity failed saved!", 0);
            }
            linearLayoutProgress.Visibility = ViewStates.Gone;
        }

        private void OnClick_DueDate(object sender, EventArgs e)
        {
            DatePickerDialog dialog = new DatePickerDialog(this);
            dialog.Show();
            dialog.DateSet += Event_DateSet;
        }

        private void Event_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            DatePicker.Text = e.Date.ToShortDateString();
        }

        private void InitData() {
          

            var user = Settings.GetTokketUser();
            if (!isEdit)
            {
                OpportunityTok = new OpportunityTok();
                OpportunityTok.UserPhoto = user.UserPhoto;
                OpportunityTok.UserId = user.Id;
                OpportunityTok.UserDisplayName = user.DisplayName;
                OpportunityTok.UserCountry = user.Country;
                OpportunityTok.UserState = user.State;
                OpportunityTok.Category = "Opportunity";
                OpportunityTok.CategoryId = "category-" + OpportunityTok.Category.ToIdFormat();
                OpportunityTok.TokGroup = "";
                OpportunityTok.TokType = "";
                OpportunityTok.TokTypeId = "";
            }
            else {
                var index = OpportunityTypes.ToList().IndexOf(OpportunityTok.OpportunityType.FirstLetterToUpperCase());
                OpportunityTypeSpinner.SetSelection(index);
                TitleView.Text = OpportunityTok.PrimaryFieldText;
                CompanyView.Text = OpportunityTok.SecondaryFieldText;

                int pos = countriesList.FindIndex(c => c.ToLower() == OpportunityTok.ItemCountry.ToLower());
                CountrySpinner.SetSelection(pos);

                AmountView.Text = OpportunityTok.Amount.ToString();
                DatePicker.Text = OpportunityTok.ApplicationDeadline;
                DescriptionView.Text = OpportunityTok.Description;
                RequirementsView.Text = OpportunityTok.Requirements;
                ContactInfoView.Text = OpportunityTok.Email;
                AddressView.Text = OpportunityTok.HomeAddress;
                PhoneNumberView.Text = OpportunityTok.PhoneNumber;
                AboutCompanyView.Text = OpportunityTok.AboutCompany;
            }

        }
        public void loadCountries()
        {
            CountrySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(txtCountry_ItemSelected);
            List<CountryModel> countryModels = CountryHelper.GetCountries();
            countriesList = new List<string>();
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
                //linearState.Visibility = ViewStates.Visible;
                StateLinear.Visibility = ViewStates.Visible;
                loadState("us");
            }
            else
            {
                StateLinear.Visibility = ViewStates.Gone;
                StateSpinner.Adapter = null;
            }

            country = spinner.GetItemAtPosition(e.Position).ToString();
        }
        public void loadState(string countryId)
        {
            List<Shared.Models.StateModel> stateModel = CountryHelper.GetCountryStates(countryId);
            List<string> statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            for (int i = 0; i < stateModel.Count(); i++)
            {
                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            SpinnerStateAdapter adapter = new SpinnerStateAdapter(this, Resource.Layout.signup_page_state_row, statelist, imageStateList);
            StateSpinner.Adapter = adapter;

            /*var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            txtState.Adapter = adapter;*/
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddOpportunityActivity);
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            ImageDisplay.SetImageBitmap(null);
            //Yearbook.Image = null;
            ImageDisplay.ContentDescription = "";
            BrowseImgButton.Visibility = ViewStates.Visible;
            RemoveImgButton.Visibility = ViewStates.Gone;
        }

        public void displayImageBrowse()
        {
            //Main Image
            ImageDisplay.SetImageBitmap(null);
            if (Settings.BrowsedImgTag == -1)
            {
                //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
                ImageDisplay.ContentDescription = Settings.ImageBrowseCrop;
                byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                BrowseImgButton.Visibility = ViewStates.Gone;
                RemoveImgButton.Visibility = ViewStates.Visible;
            }


            Settings.ImageBrowseCrop = null;
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

        private void alertMessage(string title, string message, int icon, EventHandler<DialogClickEventArgs> Okhandler = null)
        {
            if (Okhandler == null)
            {
                Okhandler = (d, fv) => { };
            }
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(icon);
            alertDialog.SetMessage(message);
            alertDialog.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), Okhandler);
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.AddOpportunityActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                Settings.ActivityInt = (int)ActivityType.AddOpportunityActivity;
                this.StartActivityForResult(nextActivity, (int)ActivityType.AddOpportunityActivity);

                if (Settings.BrowsedImgTag != -1)
                {
                    int vtag = Settings.BrowsedImgTag;


                }
            }
        }
        #region view properties
        Spinner OpportunityTypeSpinner => FindViewById<Spinner>(Resource.Id.spn_opportunityType);

        TextView TitleView => FindViewById<TextView>(Resource.Id.txt_title);

        TextView CompanyView => FindViewById<TextView>(Resource.Id.txt_company);

        TextView CancelButton => FindViewById<TextView>(Resource.Id.btnCancel);

        Spinner CountrySpinner => FindViewById<Spinner>(Resource.Id.spn_country);

        LinearLayout StateLinear => FindViewById<LinearLayout>(Resource.Id.linearState);

        Spinner StateSpinner => FindViewById<Spinner>(Resource.Id.spn_state);
        TextView AmountView => FindViewById<TextView>(Resource.Id.txt_amount);

        TextView DatePicker => FindViewById<TextView>(Resource.Id.datepicker);

        TextView DescriptionView => FindViewById<TextView>(Resource.Id.txt_description);
        TextView RequirementsView => FindViewById<TextView>(Resource.Id.txt_requirements);
        TextView ContactInfoView => FindViewById<TextView>(Resource.Id.txt_email);

        TextView AddressView => FindViewById<TextView>(Resource.Id.txt_address);

        TextView PhoneNumberView => FindViewById<TextView>(Resource.Id.txt_phonenumber);

        TextView AboutCompanyView => FindViewById<TextView>(Resource.Id.txt_about);
        Button SaveOpportunityButton => FindViewById<Button>(Resource.Id.btnAddOpportunity);
        ImageView ImageDisplay => FindViewById<ImageView>(Resource.Id.img_browse);

        Button BrowseImgButton => FindViewById<Button>(Resource.Id.btnBrowseImage);
        Button RemoveImgButton => FindViewById<Button>(Resource.Id.btnRemoveImage);
        LinearLayout linearLayoutProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_ClassGroup);
        #endregion
    }
}