using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using AlertDialog = Android.App.AlertDialog;
using HelperResult = Tokket.Shared.Helpers.Result;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "")]
    public class AddStickerDialogActivity : BaseActivity
    {
        internal static AddStickerDialogActivity Instance { get; private set; }
        GridLayoutManager mLayoutManager; RecyclerView recyclerView;
        StickerAdapter stickerAdapter; List<Tokket.Core.Sticker> stickerResult;
        public View customView; TokModel tokModel; string stickerid, stickerimage;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //SetTheme(Resource.Style.AppTheme_Dialog); // can either use R.style.AppTheme_Dialog or R.style.AppTheme as deined in styles.xml
            //SetContentView(Resource.Layout.modal_addsticker);

            customView = LayoutInflater.Inflate(Resource.Layout.modal_addsticker, null);
            Instance = this;


            string model = Intent.GetStringExtra("tokModel");
            tokModel = JsonConvert.DeserializeObject<TokModel>(model);

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetView(customView);

            builder.Create();
            builder.Show();

            mLayoutManager = new GridLayoutManager(Application.Context, 2);
            recyclerView = customView.FindViewById<RecyclerView>(Resource.Id.recycler_modalsticker);
            recyclerView.SetLayoutManager(mLayoutManager);

            InitializeData();

            CloseBtn.Click += (object sender, EventArgs e) =>
            {
                Finish();
            };

            BuySelectBtn.Click += async(object sender, EventArgs e) =>
            {
                if (!string.IsNullOrEmpty(stickerid))
                {
                    ProgressText.Text = "Loading...";
                    LinearProgress.Visibility = ViewStates.Visible;
                    this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                    bool success = false;
                    var res = await StickerService.Instance.PurchaseStickerAsync(stickerid, tokModel.Id);
                   
                    //Temporary add this outside of success to display image
                    tokModel.Sticker = stickerid;
                    tokModel.StickerImage = stickerimage;

                    if (res.ResultEnum == HelperResult.Success)
                    {
                        Settings.UserCoins -= 7; //10 = Cost of Sticker;
                        res.ResultEnum = HelperResult.Success;
                        res.ResultMessage = "Sticker has been purchased and attached succesfully!";
                        success = true;
                    }
                    else {
                        if (res.ResultMessage.Contains("not enough coins"))
                        {
                            res.ResultMessage = "Not Enough Coins";
                        }
                        else {
                            res.ResultMessage = "Could not add tile sticker.";
                        }

                        success = false;
                        
                    }

                    LinearProgress.Visibility = ViewStates.Gone;
                    this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                    var dialogSelect = new AlertDialog.Builder(this);
                    var alertSelect = dialogSelect.Create();
                    alertSelect.SetTitle("");
                    alertSelect.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertSelect.SetMessage(res.ResultMessage);
                    alertSelect.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                    {
                        if (success)
                        {
                        Intent = new Intent();
                            Intent.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
                            SetResult(Result.Ok, Intent);
                            Finish();
                        }
                    });
                    alertSelect.Show();
                    alertSelect.SetCanceledOnTouchOutside(false);
                }
            };
        }
        private void InitializeData()
        {
            stickerid = "";
            recyclerView.SetAdapter(null);
            var result = GetStickers();
            stickerAdapter = new StickerAdapter(result);
            stickerAdapter.ItemClick += OnGridBackgroundClick;
            recyclerView.SetAdapter(stickerAdapter);
        }
        private List<Tokket.Core.Sticker> GetStickers()
        {
            stickerResult = new List<Tokket.Core.Sticker>();
            var stickersList = StickerHelper.Stickers;

            stickerResult = stickersList.OrderByDescending(x => x.Name).ToList() ?? new List<Tokket.Core.Sticker>();

            return stickerResult;
        }
        public void OnGridBackgroundClick(object sender, int position)
        {
            stickerid = stickerResult[position].Id;
            stickerimage = stickerResult[position].Image;
        }
        public ImageView CloseBtn => customView.FindViewById<ImageView>(Resource.Id.modalAddSticker_btnClose);
        public Button BuySelectBtn => customView.FindViewById<Button>(Resource.Id.modalAddSticker_BuySelect);
        public RelativeLayout LinearProgress => customView.FindViewById<RelativeLayout>(Resource.Id.linear_addstickerprogress);
        public ProgressBar ProgressBar => customView.FindViewById<ProgressBar>(Resource.Id.progressbarAddSticker);
        public TextView ProgressText => customView.FindViewById<TextView>(Resource.Id.progressBarTextAddSticker);
    }
}