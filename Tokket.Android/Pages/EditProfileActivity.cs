using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Android.Helpers;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Tokket.Core.Tools;
using ServiceAccount = Tokket.Shared.Services;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Edit Profile", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Edit Profile", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class EditProfileActivity : BaseActivity
    {
        TokketUser tokketUser; 
        List<CountryModel> countryModels; List<Shared.Models.StateModel> stateModel;
        string tokketUserString;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_edit_profile);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            tokketUserString = Intent.GetStringExtra("tokketUser");

            if (!string.IsNullOrEmpty(tokketUserString))
            {
                tokketUser = JsonConvert.DeserializeObject<TokketUser>(tokketUserString);
            }
            else
            {
                tokketUser = Settings.GetTokketUser();
            }

            loadCountries();

            txtBio.Text = Intent.GetStringExtra("bio");
            txtWeb.Text = Intent.GetStringExtra("web");
            txtDisplayname.Text = Intent.GetStringExtra("displayname");

            bool result = false;
            btnBio.Click += async(o, e) =>
            {
                showLoading();
                result = await ServiceAccount.AccountService.Instance.UpdateUserBioAsync(txtBio.Text);
                hideLoading();
                if (result)
                {
                    tokketUser.Bio = txtBio.Text;
                    reloadTokketUser();
                    showDialog("Bio updated");
                }
            };

            btnWeb.Click += async (o, e) =>
            {
                if (URLUtil.IsValidUrl(txtWeb.Text))
                {
                    inputLayoutWebsite.Error = null;

                    showLoading();
                    result = await ServiceAccount.AccountService.Instance.UpdateUserWebsiteAsync(txtWeb.Text);
                    hideLoading();
                    if (result)
                    {
                        tokketUser.Website = txtWeb.Text;
                        reloadTokketUser();
                        showDialog("Web updated");
                    }
                }
                else
                {
                    inputLayoutWebsite.Error = "Invalid URL";
                }
            };

            btnUpdateDisplayName.Click += async (o, e) =>
            {
                showLoading();
                result = await ServiceAccount.AccountService.Instance.UpdateUserDisplayNameAsync(txtDisplayname.Text);
                hideLoading();
                if (result)
                {
                    tokketUser.DisplayName = txtDisplayname.Text;
                    reloadTokketUser();
                    showDialog("Display name updated");
                }
            };

            btnUpdateCountryState.Click += async (o, e) =>
            {
                var resultCountry = countryModels.FirstOrDefault(c => c.Name == txtCountry.Text);
                if (resultCountry == null) return;

                var countryPosition = countryModels.IndexOf(resultCountry);  
                var selectedCountry = countryModels[countryPosition].Id;
                string state = null;
                if (!string.IsNullOrEmpty(txtState.Text))
                {
                    var stateResult = stateModel.FirstOrDefault(c => c.Name == txtState.Text);
                    if (stateResult == null) return;

                    var statePosition = stateModel.IndexOf(stateResult);
                    var selectedState = stateModel[statePosition].Id;
                    tokketUser.State = selectedState;
                    state = selectedState;
                }

                showLoading();
                var resultModel = await ServiceAccount.AccountService.Instance.UpdateUserCountryStateAsync(selectedCountry, state);
                
                if (resultModel.ResultEnum == Shared.Helpers.Result.Success)
                {
                    tokketUser.Country = selectedCountry;
                    reloadTokketUser();

                    try
                    {
                        ProfileFragment.Instance.loadStateCountry(tokketUser);
                        ProfileUserActivity.Instance.loadStateCountry(tokketUser);
                    }
                    catch (Exception)
                    {
                    }

                    loadToks();

                    hideLoading();

                    showDialog("Country/State updated.");
                }
                else
                {
                    hideLoading();
                }
            };
        }
        private void reloadTokketUser()
        {
            if (!string.IsNullOrEmpty(tokketUserString))
            {

            }
            else
            {
                Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
            }
        }

        private void loadToks()
        {
#if (_CLASSTOKS)
            if (ProfileFragment.Instance.classtokDataAdapter != null)  //if classtokadapter used
            {
                var itemIndex = ProfileFragment.Instance.classtokDataAdapter.items.FindLastIndex(x => x.UserId == tokketUser.Id);
                if (itemIndex != -1)
                {
                    ProfileFragment.Instance.ClassTokList[itemIndex].UserState = tokketUser.State;
                }

                ProfileFragment.Instance.SetClassRecyclerAdapter(ProfileFragment.Instance.ClassTokList);
            }

            if (ClassToksFragment.Instance.ClassTokDataAdapter != null) //if classtokadapter used
            {
                var itemIndex = ClassToksFragment.Instance.ClassTokDataAdapter.items.FindLastIndex(x => x.UserId == tokketUser.Id);
                if (itemIndex != -1)
                {
                    ClassToksFragment.Instance.ClassTokDataAdapter.items[itemIndex].UserState = tokketUser.State;
                }
                ClassToksFragment.Instance.setDefaultAdapter();
            }
            

            if (!string.IsNullOrEmpty(tokketUserString))
            {
                if (ProfileUserActivity.Instance.classtokDataAdapter != null)  //if classtokadapter used
                {
                    var itemIndex = ProfileUserActivity.Instance.classtokDataAdapter.items.FindLastIndex(x => x.UserId == tokketUser.Id);
                    if (itemIndex != -1)
                    {
                        ProfileUserActivity.Instance.ClassTokList[itemIndex].UserState = tokketUser.State;
                    }
                    ProfileUserActivity.Instance.SetClassRecyclerAdapter();
                }
            }
#endif
        }
        private void showLoading()
        {
            linearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
        }
        private void hideLoading()
        {
            linearProgress.Visibility = ViewStates.Gone;
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
        }

        private void showDialog(string message = "")
        {
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle("");
            alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDialog.SetMessage(message);
            alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
            {
            });
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    setResult();
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void OnBackPressed()
        {
            setResult();
            base.OnBackPressed();
        }

        private void setResult()
        {
            Intent = new Intent();
            Intent.PutExtra("bio", txtBio.Text);
            Intent.PutExtra("web", txtWeb.Text);
            Intent.PutExtra("displayname", txtDisplayname.Text);
            SetResult(AppResult.Ok, Intent);
        }

        public void loadCountries()
        {
            txtCountry.FreezesText = false;
            int position = 0;
            countryModels = CountryHelper.GetCountries();
            var countriesList = new List<string>();
            for (int i = 0; i < countryModels.Count; i++)
            {
                if (countryModels[i].Name.ToLower() == tokketUser.Country.ToLower())
                {
                    position = i;
                }
                countriesList.Add(countryModels[i].Name);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, countriesList);
            adapter.SetNotifyOnChange(true);
            txtCountry.Adapter = adapter;

            if (countriesList.Count > 0)
            {
                txtCountry.SetText(countriesList[position], false);
                txtCountry.ItemClick += txtCountry_ItemClick;

                if (txtCountry.Text.ToLower() == "united states")
                {
                    inputLayoutState.Visibility = ViewStates.Visible;
                    loadState("us");
                }
            }
        }

        private void txtCountry_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (txtCountry.Text.ToLower() == "united states")
            {
                //linearState.Visibility = ViewStates.Visible;
                inputLayoutState.Visibility = ViewStates.Visible;
                loadState("us");
            }
            else
            {
                inputLayoutState.Visibility = ViewStates.Gone;
                txtState.Adapter = null;
            }
        }
        public void loadState(string countryId)
        {
            txtState.FreezesText = false;
            int position = 0;
            stateModel = CountryHelper.GetCountryStates(countryId);
            var statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            for (int i = 0; i < stateModel.Count; i++)
            {
                if (!string.IsNullOrEmpty(tokketUser.State))
                {
                    if (stateModel[i].Id.ToLower() == tokketUser.State.ToLower())
                    {
                        position = i;
                    }
                }

                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            SpinnerStateAdapter adapter = new SpinnerStateAdapter(this, Resource.Layout.signup_page_state_row, statelist, imageStateList);
            adapter.SetNotifyOnChange(true);
            txtState.Adapter = adapter;

            if (statelist.Count > 0)
            {
                txtState.SetText(statelist[position], false);
            }
            /*var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            txtState.Adapter = adapter;*/
        }

        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public EditText txtBio => FindViewById<EditText>(Resource.Id.txtBio);
        public Button btnBio => FindViewById<Button>(Resource.Id.btnBio);
        public EditText txtWeb => FindViewById<EditText>(Resource.Id.txtWeb);
        public Button btnWeb => FindViewById<Button>(Resource.Id.btnWeb);
        public EditText txtDisplayname => FindViewById<EditText>(Resource.Id.txtDisplayname);
        public Button btnUpdateDisplayName => FindViewById<Button>(Resource.Id.btnUpdateDisplayName);
        public AutoCompleteTextView txtCountry => FindViewById<AutoCompleteTextView>(Resource.Id.txtCountry);
        public AutoCompleteTextView txtState => FindViewById<AutoCompleteTextView>(Resource.Id.txtState);
        public Button btnUpdateCountryState => FindViewById<Button>(Resource.Id.btnUpdateCountryState);
        public TextInputLayout inputLayoutState => FindViewById<TextInputLayout>(Resource.Id.inputLayoutState);
        public TextInputLayout inputLayoutWebsite => FindViewById<TextInputLayout>(Resource.Id.inputLayoutWebsite);
    }
}