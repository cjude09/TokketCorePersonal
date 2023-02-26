using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Tokket.Core;
using SharedService = Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Android.Text;
using AndroidX.RecyclerView.Widget;
using Android.Content.PM;

namespace Tokket.Android
{
    [Activity(Label = "", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SubAccountActivity : BaseActivity
    {
        internal static SubAccountActivity Instance { get; private set; }
        GridLayoutManager mLayoutManager;
        List<TokketSubaccount> SubAccntList;
        List<string> Colors = new List<string>() {
               "#4472C7", "#732FA0", "#05adf4", "#73AD46",
               "#E23DB5", "#BE0400", "#195B28", "#E88030",
               "#873B09", "#FFC100"
               };
        List<string> ColorItems = new List<string>();
        int lastrows = 0, lastcolor = 0;
        public string currentuser = "";
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.subaccount_page);
            Instance = this;

            SubAccntList = new List<TokketSubaccount>();
            mLayoutManager = new GridLayoutManager(this, 2);
            RecyclerSubAccounts.SetLayoutManager(mLayoutManager);

            if (Settings.GetTokketUser().AccountType.ToLower() == "group")
            {
                TextAccountName.Text = Settings.GetTokketUser().DisplayName;
                TextGroupAccountType.Text = Settings.GetTokketUser().GroupAccountType.Substring(0, 1).ToUpper() + Settings.GetTokketUser().GroupAccountType.Substring(1);

                await GetSubAccounts();
            }

            BtnSelectSubAccnt.Click += async delegate
            {
                if (BtnSelectSubAccnt.ContentDescription == "selected")
                {
                    if (Settings.GetTokketSubaccount().IsSubaccountKeyDisabled.Value){
                        BtnSelectSubAccnt.ContentDescription = "";
                        //var _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
                        //if(_navigationService == null)
                        //{

                        //}
                        //else
                        //{
                        //    _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
                        //}
                     
                            BtnSelectSubAccnt.ContentDescription = "";
                            Intent nextActivity = new Intent(this, typeof(MainActivity));
                            StartActivity(nextActivity);
                            this.Finish();
                       
                   
                        //Intent nextActivity = new Intent(this, typeof(MainActivity));
                        //StartActivity(nextActivity);
                        //this.Finish();
                    }
                    else
                    {
                        
                        if (await SharedService.AccountService.Instance.LoginSubaccountAsync(Settings.GetTokketSubaccount().UserId, Settings.GetTokketSubaccount().Id, inputKeyCode.Text))
                        {
                            BtnSelectSubAccnt.ContentDescription = "";
                            Intent nextActivity = new Intent(this, typeof(MainActivity));
                            StartActivity(nextActivity);
                            this.Finish();
                        }
                        else
                        {
                            showAlertDialog("Wrong subaccount key. Please try again...");
                        }
                    }  
                }
                else
                {
                    showAlertDialog("Please select a sub account.");
                }
            };

            closeButton.Click += delegate
            {
                if (BtnSelectSubAccnt.ContentDescription != "selected")
                {
                    if (SubAccntList.Count > 0)
                        showAlertDialog("Please select a sub account.");
                    else
                        this.Finish();
                }
                else
                {
                    this.Finish();
                }
            };

            if (RecyclerSubAccounts != null)
            {
                RecyclerSubAccounts.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerSubAccounts.ContentDescription))
                    {
                        //Load more stuff here
                        await GetSubAccounts();
                    };
                };


                RecyclerSubAccounts.AddOnScrollListener(onScrollListener);

                RecyclerSubAccounts.SetLayoutManager(mLayoutManager);
            }
        }

        private void showAlertDialog(string message = "")
        {
            var dialognetwork = new AlertDialog.Builder(this);
            var alertnetwork = dialognetwork.Create();
            alertnetwork.SetTitle("");
            alertnetwork.SetMessage(message);
            alertnetwork.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            alertnetwork.Show();
            alertnetwork.SetCanceledOnTouchOutside(false);
        }

        public void SelectedSubAccnt(TokketSubaccount subAccnt)
        {
            TokketUser tokketUser = new TokketUser();
            tokketUser = Settings.GetTokketUser();
            tokketUser.SubaccountId = subAccnt.Id;
            tokketUser.SubaccountName = subAccnt.SubaccountName;
            tokketUser.SubaccountOwner = subAccnt.IsSubaccountOwner;
            tokketUser.SubaccountPhoto = subAccnt.SubaccountPhoto;


            bool keyDisabled = subAccnt.IsSubaccountKeyDisabled.Value;
            if (!keyDisabled)
            {
                textKeyCode.Visibility = ViewStates.Visible;
                inputKeyCode.Visibility = ViewStates.Visible;
            }
            else
            {
                textKeyCode.Visibility = ViewStates.Gone;
                inputKeyCode.Visibility = ViewStates.Gone;
            }


            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
            Settings.TokketSubAccount = JsonConvert.SerializeObject(subAccnt);
        }
        private async Task GetSubAccounts()
        {
            linearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
            var result = await SharedService.AccountService.Instance.GetSubaccountsAsync(Settings.GetUserModel().UserId, RecyclerSubAccounts.ContentDescription);
            RecyclerSubAccounts.ContentDescription = result.ContinuationToken;
            SubAccntList.AddRange(result.Results.ToList());

            for (int i = lastrows; i < SubAccntList.Count; i++)
            {
                if (string.IsNullOrEmpty(SubAccntList[i].SubaccountPhoto))
                {
                    int ndx = lastcolor % Colors.Count;
                    lastcolor += 1;
                    ColorItems.Add(Colors[ndx]);
                }
                else
                {
                    ColorItems.Add("#FFFFFF");//White
                }
            }

            lastrows += SubAccntList.Count;
            var subAccntAdapter = new SubAccountAdapter(SubAccntList, ColorItems);
            RecyclerSubAccounts.SetAdapter(subAccntAdapter);

            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            linearProgress.Visibility = ViewStates.Invisible;
        }

        public TextView TextAccountName => FindViewById<TextView>(Resource.Id.TextAccountName);
        public TextView TextGroupAccountType => FindViewById<TextView>(Resource.Id.TextGroupAccountType);
        public RecyclerView RecyclerSubAccounts => FindViewById<RecyclerView>(Resource.Id.RecyclerSubAccounts);
        public LinearLayout LinearMainGroupAccount => FindViewById<LinearLayout>(Resource.Id.LinearMainGroupAccount);
        public Button BtnSelectSubAccnt => FindViewById<Button>(Resource.Id.BtnSelectSubAccnt);
        public TextView textKeyCode => FindViewById<TextView>(Resource.Id.textKeyCode);
        public EditText inputKeyCode => FindViewById<EditText>(Resource.Id.inputKeyCode);
        public Button closeButton => FindViewById<Button>(Resource.Id.closeButton);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideText);

    }
}