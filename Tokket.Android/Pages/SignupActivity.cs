using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using SharedService = Tokket.Shared.Services;
using Tokket.Core.Tools;
using Android.Icu.Util;
using Xamarin.Essentials;
using System.Drawing;
using Android.Graphics;
using Android.Webkit;
using Android.Content.PM;
using Tokket.Android.Adapters;
using Tokket.Shared.Models.Purchase;
using Tokket.Core;
using Newtonsoft.Json;
using SharedAccount = Tokket.Shared.Services;
using Android.Text;
using Result = Android.App.Result;
using Android.Util;
using Google.Android.Material.TextField;
using Tokket.Android.Helpers;
using AndroidX.AppCompat.App;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Sign Up", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Sign Up", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class SignupActivity : BaseActivity
    {
        internal static SignupActivity Instance { get; private set; }
        AutoCompleteTextView txtBday; AutoCompleteTextView txtCountry; AutoCompleteTextView txtState; TextView txtTermsofService;
        TextInputEditText txtDisplayName; TextInputEditText txtPassword; TextInputEditText txtConfirmPassword;
        Button btnSignup; TextInputLayout inputLayoutBirthday; TextInputEditText txtEmail;
        Intent intent;
        bool errorOnInput = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.signup_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            Settings.ActivityInt = (int)ActivityType.SignUpActivity;

            btnSignup = FindViewById<Button>(Resource.Id.btnSignupIndividual);
            txtDisplayName = FindViewById<TextInputEditText>(Resource.Id.txtSignupDisplayName);
            txtEmail = FindViewById<TextInputEditText>(Resource.Id.txtSignupEmail);
            txtPassword = FindViewById<TextInputEditText>(Resource.Id.txtSignupPassword);
            txtConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.txtSignupConfirmPassword);
            txtCountry = FindViewById<AutoCompleteTextView>(Resource.Id.txtSignupCountry);
            txtState = FindViewById<AutoCompleteTextView>(Resource.Id.txtSignupState);
            txtBday = FindViewById<AutoCompleteTextView>(Resource.Id.txtSignupBday);
            inputLayoutBirthday = FindViewById<TextInputLayout>(Resource.Id.inputLayoutBirthday);
            var chkIagree = FindViewById<CheckBox>(Resource.Id.chkSignupTermsofService);
            txtTermsofService = FindViewById<TextView>(Resource.Id.lblSignupTermsofService);

            var ArrAccntType = new string[] {"Group", "Individual"};
            var AadapterAccntType = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, ArrAccntType);
            SpinAccounType.Adapter = null;
            SpinAccounType.Adapter = AadapterAccntType;

            SpinAccounType.ItemClick += SpinAccounType_ItemClick;

            SpinAccounType.SetText("Individual", false); //Select individual as default
            SetAccountType();

            var ArrGroupType = new string[] {"Family", "Organization" };
            var AadapterGroupType = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, ArrGroupType);
            SpinGroupType.Adapter = null;
            SpinGroupType.Adapter = AadapterGroupType;
            SpinGroupType.ItemClick += SpinGroupType_ItemClick;

            btnSignup.Enabled = false;
            btnSignup.SetBackgroundResource(Resource.Drawable.myButtonDisabled);
            txtBday.Click += DateSelect_OnClick;
            txtDisplayName.TextChanged += onEditTextChange;
            txtPassword.TextChanged += onEditTextChange;
            txtConfirmPassword.TextChanged += onEditTextChange;
            txtBday.TextChanged += onEditTextChange;
            txtDisplayName.FocusChange += onFocusChangeDisplayName;
            txtEmail.FocusChange += onFocusChangeEmailCheck;

            loadCountries();

            txtEmail.TextChanged += onEditTextChange;
            //Signup Button is clicked
            btnSignup.Click += async (object sender, EventArgs e) =>
            {
                var dialog = new AppCompatDialogService();

                if (chkIagree.Checked && !errorOnInput)
                {
                    var email = txtEmail.Text;
                    var password = txtPassword.Text;
                    var displayname = txtDisplayName.Text;
                    var country = txtCountry.Text;
                    var bday  = DateTime.ParseExact(txtBday.Text, "dd/MM/yyyy", null);
                    string imgPhoto = ImgUserPhoto.ContentDescription;
                    if (!URLUtil.IsValidUrl(ImgUserPhoto.ContentDescription))
                    {
                        imgPhoto = "data:image/jpeg;base64," + ImgUserPhoto.ContentDescription;
                    }

                    if (SpinAccounType.Text.Contains("...")) {
                        MainThread.BeginInvokeOnMainThread(async () => {
                            await
                            dialog.ShowError(
                                "Please select an account type!",
                                "Warning",
                                "OK",
                                null);
                        });
                        return;
                    }
                    this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                    LinearProgress.Visibility = ViewStates.Visible;

                    var accounttype  = SpinAccounType.Text;
                    if (accounttype.ToLower() == "group")
                    {
                        if (string.IsNullOrEmpty(SpinGroupType.Text)) {
                            MainThread.BeginInvokeOnMainThread(async () => {
                                await
                                dialog.ShowError(
                                    "Please select a group type!",
                                    "Warning",
                                    "OK",
                                    null);
                            });

                            return;
                        }
                        var key = string.IsNullOrEmpty(SubaccountKey.Text) ? string.Empty : SubaccountKey.Text ;
                        var tokketUser = new TokketUser() { Email = email, PasswordHash = password, DisplayName = displayname, UserPhoto = imgPhoto, BirthDate = bday.ToString(), Country = country
                        , GroupAccountType = SpinAccounType.ContentDescription
                        ,AccountType = SpinAccounType.ContentDescription
                        ,SubaccountKey = key
                        ,SubaccountOwner = true
                        ,SubaccountName = EditOwnerName.Text};
                        var groupPurchase = PurchasesTool.GetProduct("groupaccount_tokket");
                        var result = await SharedService.PurchaseService.Instance.BillingStart(groupPurchase.Id,groupPurchase,groupAccount: tokketUser);
                        if (result) {
                            LinearProgress.Visibility = ViewStates.Gone;
                            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                            //if (!tokketUser.EmailVerified)
                            //{
                            //    if (string.IsNullOrEmpty(tokketUser.IdToken))
                            //    {
                            //        var Credentials = new LoginModel { Username = email, Password = password };
                            //        var loginResult = await SharedAccount.AccountService.Instance.Login(Credentials);
                            //        if (loginResult.ResultEnum == Shared.Helpers.Result.Success && loginResult != null)
                            //        {
                            //            var resultObject = loginResult.ResultObject.ToString();
                            //            var userAccount = JsonConvert.DeserializeObject<AuthorizationTokenModel>(resultObject);
                            //            tokketUser = await SharedAccount.AccountService.Instance.GetUserAsync(userAccount.UserId);
                            //            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                            //            Settings.UserAccount = JsonConvert.SerializeObject(userAccount);

                            //            userAccount.UserPhoto = tokketUser.UserPhoto;
                            //            Settings.UserCoins = tokketUser.Coins.Value;

                            //            var tokens = Settings.GetUserModel();
                            //            await SecureStorage.SetAsync("idtoken", tokens.IdToken);
                            //            await SecureStorage.SetAsync("refreshtoken", tokens.RefreshToken);
                            //            await SecureStorage.SetAsync("userid", tokens.UserId);
                            //            await SecureStorage.SetAsync("accounttype", tokketUser.AccountType);

                            //            tokketUser.IdToken = tokens.IdToken;
                            //            if (!tokketUser.EmailVerified)
                            //            {
                            //                VerifyEmailAccount(tokketUser, password);
                            //            }
                            //        }
                            //        else
                            //        {
                            //            ShowLottieMessageDialog(this, "Unknown email address", true, header: "Failed!");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        VerifyEmailAccount(tokketUser, password);
                            //    }
                            //}

                            List<string> data = new List<string>() { email, password };
                            var converData = JsonConvert.SerializeObject(data);
                            intent = new Intent();
                            intent.PutExtra("SignUpData", converData);
                            this.SetResult(Result.Ok, intent);
                            SetResult(Result.Ok, intent);

                            Finish();
                        }                 
                    }
                    else {
                        var resultModel = await SharedService.AccountService.Instance.SignUpAsync(email,
                                   password, displayname, country, bday, imgPhoto, SpinAccounType.ContentDescription, SpinGroupType.ContentDescription, EditOwnerName.Text);
                        LinearProgress.Visibility = ViewStates.Gone;
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        List<string> data = new List<string>() { email, password };
                        var converData = JsonConvert.SerializeObject(data);
                        intent = new Intent();
                        intent.PutExtra("SignUpData", converData);
                        this.SetResult(Result.Ok, intent);
                        SetResult(Result.Ok, intent);

                        Finish();
                        //var objBuilder = new AlertDialog.Builder(this);
                        //objBuilder.SetTitle("");
                        //objBuilder.SetMessage(resultModel.ResultMessage);
                        //objBuilder.SetCancelable(false);

                        //AlertDialog objDialog = objBuilder.Create();
                        //objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                        //{
                        //    if (resultModel.ResultEnum == Shared.Helpers.Result.Success)
                        //    {
                        //        this.Finish();
                        //    }
                        //});
                        //objDialog.Show();
                    }
               
                }
                else
                {
                    var objBuilder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                    objBuilder.SetTitle("");
                    if(!chkIagree.Checked)
                         objBuilder.SetMessage("Please accept Terms and Conditions to continue.");
                    else if(errorOnInput)
                        objBuilder.SetMessage("The data you entered is wrong");

                    objBuilder.SetCancelable(false);

                    AndroidX.AppCompat.App.AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) => { });
                    objDialog.Show();
                }
            };
            //Terms of Service is clicked
            txtTermsofService.Click += async (object sender, EventArgs e) =>
            {
                await Browser.OpenAsync("https://tokket.com/terms", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = System.Drawing.Color.AliceBlue,
                    PreferredControlColor = System.Drawing.Color.Violet
                });
            };
        }
        
        private void SpinGroupType_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            SpinGroupType.ContentDescription = SpinGroupType.Text;
            switch (SpinGroupType.ContentDescription.ToLower())
            {
                case "family":
                    inputLayoutGroupTypeHeader.Hint = "Account Owner's First Name";
                    inputLayoutDisplayName.Hint = "Family's Last Name or Display Name ('Family' will be appended)";

                    break;
                case "organization":
                    inputLayoutGroupTypeHeader.Hint = "Account Owner's Name";
                    inputLayoutDisplayName.Hint = "Organization Name";
                    break;

            }
        }
        private void SpinAccounType_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            SetAccountType();
        }

        private void SetAccountType()
        {
            SpinAccounType.ContentDescription = SpinAccounType.Text;
            var paytext = FindViewById<TextView>(Resource.Id.payrequireText);
            var acceptbutton = FindViewById<Button>(Resource.Id.payrequireAcceptBtn);
            var declinebutton = FindViewById<Button>(Resource.Id.payrequireDeclineBtn);
            var birthday = FindViewById<TextInputLayout>(Resource.Id.inputLayoutBirthday);
            var country = FindViewById<TextInputLayout>(Resource.Id.inputLayoutCountry);
            var state = FindViewById<TextInputLayout>(Resource.Id.inputLayoutState);
            var email = FindViewById<TextInputLayout>(Resource.Id.inputLayoutEmail);
            var image = FindViewById<LinearLayout>(Resource.Id.image_linear);
            var terms = FindViewById<LinearLayout>(Resource.Id.terms_linear);
            var signbutton = FindViewById<Button>(Resource.Id.btnSignupIndividual);
            image.Visibility = ViewStates.Gone;
            switch (SpinAccounType.ContentDescription.ToLower())
            {
                case "group":
                    btnSignup.Text = "Register Group ($2.99 USD)";
                    inputLayoutUpGroupType.Visibility = ViewStates.Visible;
                    LinearGroupName.Visibility = ViewStates.Visible;
                    inputLayoutDisplayName.Hint = "Family's Last Name or Display Name ('Family' will be appended)";
                    paytext.Visibility = ViewStates.Visible;
                    acceptbutton.Visibility = ViewStates.Visible;
                    declinebutton.Visibility = ViewStates.Visible;
                    inputLayoutDisplayName.Visibility = ViewStates.Gone;
                    birthday.Visibility = ViewStates.Gone;
                    country.Visibility = ViewStates.Gone;
                    state.Visibility = ViewStates.Gone;
                    email.Visibility = ViewStates.Gone;
                    inputLayoutPassword.Visibility = ViewStates.Gone;
                    inputLayoutConfirmPassword.Visibility = ViewStates.Gone;
                    image.Visibility = ViewStates.Gone;
                    terms.Visibility = ViewStates.Gone;
                    signbutton.Visibility = ViewStates.Gone;
                    break;
                case "individual":
                    EditOwnerName.Text = "";
                    btnSignup.Text = "Register";
                    inputLayoutUpGroupType.Visibility = ViewStates.Gone;
                    LinearGroupName.Visibility = ViewStates.Gone;
                    inputLayoutDisplayName.Hint = "Display Name";

                    paytext.Visibility = ViewStates.Gone;
                    acceptbutton.Visibility = ViewStates.Gone;
                    declinebutton.Visibility = ViewStates.Gone;
                    inputLayoutSubKey.Visibility = ViewStates.Gone;
                    inputLayoutDisplayName.Visibility = ViewStates.Visible;
                    birthday.Visibility = ViewStates.Visible;
                    country.Visibility = ViewStates.Visible;
                    state.Visibility = ViewStates.Visible;
                    email.Visibility = ViewStates.Visible;
                    inputLayoutPassword.Visibility = ViewStates.Visible;
                    inputLayoutConfirmPassword.Visibility = ViewStates.Visible;
                    image.Visibility = ViewStates.Visible;
                    terms.Visibility = ViewStates.Visible;
                    signbutton.Visibility = ViewStates.Visible;

                    break;
            }
        }
        private void VerifyEmailAccount(TokketUser tokketUser, string password)
        {
            ShowLottieMessageDialog(this, GetString(Resource.String.emailverify), false, "Resend Email", async (s, e) =>
            {
                showBlueLoading(this);
                var check = await SharedAccount.AccountService.Instance.SendEmailVerificationAsync(tokketUser.Email, tokketUser.IdToken);
                hideBlueLoading(this);

                if (check)
                {
                    ShowLottieMessageDialog(this, $"Email Verification sent to {tokketUser.Email}", true, header: "Email Verification");

                    List<string> data = new List<string>() { tokketUser.Email, password };
                    var converData = JsonConvert.SerializeObject(data);
                    intent = new Intent();
                    intent.PutExtra("SignUpData", converData);
                    this.SetResult(Result.Ok, intent);
                    SetResult(Result.Ok, intent);

                    Finish();
                }
                else
                {
                    ShowLottieMessageDialog(this, "Failed to resend verification, try again!", false, handlerOKText: "Retry", (s, e) =>
                    {
                        VerifyEmailAccount(tokketUser, password);
                    }, header: "Email Verification");
                }
            }, header: "Email Verification");
        }
        private async void onEditTextChange(object sender , TextChangedEventArgs e)
        {
            var returnvalue = true;
            if (string.IsNullOrEmpty(txtDisplayName.Text.Trim()))
            {
                txtDisplayName.Error = "Display name must have a value.";
                returnvalue = false;
            }

            if (txtPassword.Text.Trim().Length < 7)
            {
                inputLayoutPassword.Error = "Password must be greater than 6 characters.";
                returnvalue = false;
            }
            else
            {
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    inputLayoutPassword.Error = "Passwords do not match.";
                    returnvalue = false;
                }
                else
                {
                    inputLayoutPassword.Error = null;
                }
            }

            if (!Patterns.EmailAddress.Matcher(txtEmail.Text).Matches())
            {
                inputLayoutEmail.Error = "Invalid Email Address";
                //txtEmail.SetBackgroundResource(Resource.Drawable.errortedittext);
                returnvalue = false;
            }
            else {
                inputLayoutEmail.Error = string.Empty;
                //txtEmail.SetBackgroundResource(Resource.Drawable.rounded_edittext_white_bg);
            }
           

            var today = DateTime.Today;
            var age = 0;
            if (!string.IsNullOrEmpty(txtBday.Text))
            {
                var date = DateTime.ParseExact(txtBday.Text, "dd/MM/yyyy", null);
                age = today.Year - date.Year;
            };
            //if (DateTime.Parse(txtBday.Text).Date > today.AddYears(-age)) age--;
            if (age < 13)
            {
                inputLayoutBirthday.Error = "You must be 13yrs old or above to register.";
                returnvalue = false;
            }
            else
            {
                inputLayoutBirthday.Error = string.Empty;
            }
            
            btnSignup.Enabled = returnvalue;
            if (btnSignup.Enabled == true)
            {
                btnSignup.SetBackgroundResource(Resource.Drawable.myButton);
            }
            else
            {
                btnSignup.SetBackgroundResource(Resource.Drawable.myButtonDisabled);
            }
        }
        public void loadCountries()
        {
            List<CountryModel> countryModels = CountryHelper.GetCountries();
            List<string> countriesList = new List<string>();
            for (int i = 0; i < countryModels.Count(); i++)
            {
                countriesList.Add(countryModels[i].Name);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, countriesList);
            txtCountry.Adapter = adapter;
            txtCountry.ItemClick += txtCountry_ItemClick;
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
            List<Shared.Models.StateModel> stateModel = CountryHelper.GetCountryStates(countryId);
            List<string> statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            for (int i = 0; i < stateModel.Count(); i++)
            {
                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            SpinnerStateAdapter adapter = new SpinnerStateAdapter(this, Resource.Layout.signup_page_state_row, statelist, imageStateList);
            txtState.Adapter = adapter;

            /*var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            txtState.Adapter = adapter;*/
        }
        void DateSelect_OnClick(object sender, EventArgs eventArgs)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                txtBday.Text = time.ToString("dd/MM/yyyy");
            });
            frag.Show(SupportFragmentManager, DatePickerFragment.TAG);
        }

        public async void onFocusChangeDisplayName(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
             
            }
            else
            {
                try {
                    var accounttype = SpinAccounType.Text;
                    var displayname = txtDisplayName.Text;
                    var result = await SharedService.AccountService.Instance.GetUsers(displayname, accounttype);
                    var check = result.Results.ToList();
                    foreach(var item in check) {
                        Console.WriteLine("Name: "+item.DisplayName);
                    }
                    var finding = check.Where(s => s.DisplayName == displayname).FirstOrDefault();
                    if (finding != null)
                    {

                        txtDisplayName.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                        txtDisplayName.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(Resource.Drawable.closered_24px), null);
                        errorOnInput = true;
                        // txtDisplayName.Error = "Display name not available.";
                    }
                    else
                    {
                        txtDisplayName.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                        txtDisplayName.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(Resource.Drawable.checkgreen_24px), null);
                        errorOnInput = false;
                    }
                } catch (Exception ex) { }
             
            }
        }

        public async void onFocusChangeEmailCheck(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {

            }
            else
            {
                try
                {
                  
                    var account = await SharedService.AccountService.Instance.GetUserByEmailAsync(txtEmail.Text);
                    if (account != null)
                    {
                        txtEmail.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                        txtEmail.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(Resource.Drawable.closered_24px), null);
                        //txtEmail.SetBackgroundResource(Resource.Drawable.errortedittext);
                        errorOnInput = true;
                        //txtEmail.Error = "Email already in use.";
             
                    }
                    else
                    {
                        txtEmail.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                        txtEmail.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(Resource.Drawable.checkgreen_24px), null);
                        //txtEmail.SetBackgroundResource(Resource.Drawable.rounded_edittext_white_bg);
                        errorOnInput = false;
                    }
                }
                catch (Exception ex) { }

            }
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.SignUpActivity);
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            ImgUserPhoto.SetImageBitmap(null);

            BtnBrowseImg.Visibility = ViewStates.Visible;
            BtnRemoveImg.Visibility = ViewStates.Gone;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.SignUpActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }

        }
        public void displayImageBrowse()
        {
            //Main Image
            ImgUserPhoto.SetImageBitmap(null);
            if (Settings.BrowsedImgTag == -1)
            {
                //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
                ImgUserPhoto.ContentDescription = Settings.ImageBrowseCrop;
                byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                ImgUserPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                BtnBrowseImg.Visibility = ViewStates.Gone;
                BtnRemoveImg.Visibility = ViewStates.Visible;
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

        [Java.Interop.Export("OnClickCancel")]
        public void OnClickCancelk(View v)
        {
            Finish();
        }
        [Java.Interop.Export("OnClickPayAccept")]
        public  void OnClickPayAccept(View v) {
            var owner = FindViewById<TextInputEditText>(Resource.Id.EditSignupOwnerName);
            var error = FindViewById<TextView>(Resource.Id.TextErrorInput);
            

            if (!string.IsNullOrEmpty(owner.Text))
            {
                var birthday = FindViewById<TextInputLayout>(Resource.Id.inputLayoutBirthday);
                var country = FindViewById<TextInputLayout>(Resource.Id.inputLayoutCountry);
                var state = FindViewById<TextInputLayout>(Resource.Id.inputLayoutState);
                var email = FindViewById<TextInputLayout>(Resource.Id.inputLayoutEmail);
                var paytext = FindViewById<TextView>(Resource.Id.payrequireText);
                var acceptbutton = FindViewById<Button>(Resource.Id.payrequireAcceptBtn);
                var declinebutton = FindViewById<Button>(Resource.Id.payrequireDeclineBtn) ;
                var emailcheck = FindViewById<TextView>(Resource.Id.emailcheckText);
                var emailBA = FindViewById<Button>(Resource.Id.emailAcceptBtn);
                var emailBD = FindViewById<Button>(Resource.Id.emailDeclineBtn);
                paytext.Visibility = ViewStates.Gone;
                acceptbutton.Visibility = ViewStates.Gone;
                declinebutton.Visibility = ViewStates.Gone;
                error.Visibility = ViewStates.Gone;
                inputLayoutDisplayName.Visibility = ViewStates.Visible;
                birthday.Visibility = ViewStates.Visible;
                country.Visibility = ViewStates.Visible;
                state.Visibility = ViewStates.Visible;
                email.Visibility = ViewStates.Visible;
                emailcheck.Visibility = ViewStates.Visible;
                emailBA.Visibility = ViewStates.Visible;
                emailBD.Visibility = ViewStates.Visible;
                inputLayoutSubKey.Visibility = ViewStates.Gone;
            }
            else {
                error.Visibility = ViewStates.Visible;
            }
         
        }
        
         public override void Finish()
        {
            if (intent != null)
                SetResult(Result.Ok, intent);
            base.Finish();
        }
        [Java.Interop.Export("OnClickEmailAccept")]
        public void OnClickEmailAccept(View v) {
            var emailcheck = FindViewById<TextView>(Resource.Id.emailcheckText);
            var emailBA = FindViewById<Button>(Resource.Id.emailAcceptBtn);
            var emailBD = FindViewById<Button>(Resource.Id.emailDeclineBtn);
            var image = FindViewById<LinearLayout>(Resource.Id.image_linear);
            var terms = FindViewById<LinearLayout>(Resource.Id.terms_linear);
            var signbutton = FindViewById<Button>(Resource.Id.btnSignupIndividual);
            emailcheck.Visibility = ViewStates.Gone;
            emailBA.Visibility = ViewStates.Gone;
            emailBD.Visibility = ViewStates.Gone;
            inputLayoutPassword.Visibility = ViewStates.Visible;
            inputLayoutConfirmPassword.Visibility = ViewStates.Visible; 
            image.Visibility = ViewStates.Gone;
            terms.Visibility = ViewStates.Visible;
            signbutton.Visibility = ViewStates.Visible;
            inputLayoutSubKey.Visibility = ViewStates.Visible;

        }
        public AutoCompleteTextView SpinAccounType => FindViewById<AutoCompleteTextView>(Resource.Id.SpinnerSignupAccountType);
        public AutoCompleteTextView SpinGroupType => FindViewById<AutoCompleteTextView>(Resource.Id.SpinnerSignupGroupType);
        public TextInputLayout inputLayoutGroupTypeHeader => FindViewById<TextInputLayout>(Resource.Id.inputLayoutGroupTypeHeader);
        public TextInputLayout inputLayoutEmail => FindViewById<TextInputLayout>(Resource.Id.inputLayoutEmail);
        public TextInputLayout inputLayoutPassword => FindViewById<TextInputLayout>(Resource.Id.inputLayoutPassword);
        public TextInputLayout inputLayoutConfirmPassword => FindViewById<TextInputLayout>(Resource.Id.inputLayoutConfirmPassword);
        public TextInputEditText EditOwnerName => FindViewById<TextInputEditText>(Resource.Id.EditSignupOwnerName);
        public TextInputLayout inputLayoutUpGroupType => FindViewById<TextInputLayout>(Resource.Id.inputLayoutUpGroupType); 
        public LinearLayout LinearGroupName => FindViewById<LinearLayout>(Resource.Id.LinearSignUpGroupOwnerName);
        public TextInputLayout inputLayoutDisplayName => FindViewById<TextInputLayout>(Resource.Id.inputLayoutDisplayName); 
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_Signup);
        public TextInputLayout inputLayoutState => FindViewById<TextInputLayout>(Resource.Id.inputLayoutState);
        public ImageView ImgUserPhoto => FindViewById<ImageView>(Resource.Id.ImgSignupPhoto);
        public Button BtnBrowseImg => FindViewById<Button>(Resource.Id.btnSignupBrowseImg);
        public Button BtnRemoveImg => FindViewById<Button>(Resource.Id.btnSignupRemoveImg);

        public TextInputLayout inputLayoutSubKey => FindViewById<TextInputLayout>(Resource.Id.inputLayoutSubKey);
        public TextInputEditText SubaccountKey => FindViewById<TextInputEditText>(Resource.Id.txtSubKey);
    }
}