using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Result = Android.App.Result;
using Tokket.Android.ViewModels;
using Tokket.Shared.Models;
using Android.Views;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Color Selection", Theme = "@style/Theme.AppCompat.Light.Dialog.NoTitle")]
    public class ColorSelectionActivity : BaseActivity
    {
        List<string> Colors32 = new List<string>() {
        "#e57373","#f06292","#ba68c8","#9575cd",
        "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
        "#7986cb", "#64b5f6", "#4fc3f7", "#4dd0e1",
        "#303F9F", "#1976D2", "#0288D1", "#0097A7",
        "#4db6ac", "#81c784", "#aed581", "#dce775",
        "#00796B", "#388E3C", "#689F38", "#AFB42B",
        "#fff176", "#ffd54f", "#ffb74d", "#ff8a65",
        "#FBC02D", "#FFA000", "#F57C00", "#E64A19" };
        private ObservableRecyclerAdapter<ColorViewModel, CachingViewHolder> adapterColors;

        ObservableCollection<ColorViewModel> colorsCollection;
        string colorHex = "", keyvalue = "", className = "";
        DefaultColor defaultColor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_color_selection);

            defaultColor = new DefaultColor();
            className = Intent.GetStringExtra("className");
            colorHex = Intent.GetStringExtra("color");
            keyvalue = Intent.GetStringExtra("keyvalue");

            colorsCollection = new ObservableCollection<ColorViewModel>();
            foreach (var col in Colors32)
            {
                ColorViewModel color = new ColorViewModel();
                color.color = col;
                colorsCollection.Add(color);
            }

            recyclerColors.SetLayoutManager(new GridLayoutManager(this, 4));
            adapterColors = colorsCollection.GetRecyclerAdapter(BindColorsViewHolder, Resource.Layout.color_selection_row);
            recyclerColors.SetAdapter(adapterColors);

            btnClose.Click += delegate
            {
                this.Finish();
            };

            btnSelect.Click += async(s, e) =>
            {
                if (btnSelect.ContentDescription.Trim() != "")
                {
                    Intent intent = new Intent();
                    intent.PutExtra("color", btnSelect.ContentDescription);
                    SetResult(Result.Ok, intent);
                    this.Finish();
                }
            };

            btnRemovecolor.Click += delegate
            {
                Intent intent = new Intent();
                intent.PutExtra("color", "#FFFFFF");
                SetResult(Result.Ok, intent);
                this.Finish();
            };

            chkSetDefault.CheckedChange += async(s, e) =>
            {
                if (string.IsNullOrEmpty(btnSelect.ContentDescription))
                {
                    defaultColor = new DefaultColor();
                    btnSelect.ContentDescription = defaultColor.ColorHex;
                }

                if (chkSetDefault.Checked)
                {
                    txtProgressText.Text = "Checking default color for " + className + "...";
                    await SetDefaultColor();
                }
            };
            btnRemovecolor.Visibility = ViewStates.Gone;
        }
        private async Task SetDefaultColor()
        {
            btnSelect.Enabled = false;
            string colorHex = btnSelect.ContentDescription;
            string userId = Settings.GetUserModel().UserId;

            linearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

            defaultColor = await AccessoriesService.Instance.DefaultColorAsync(colorHex: colorHex, userId: userId, keyvalue, method: "post");
            if (defaultColor == null)
            {
                defaultColor = await AccessoriesService.Instance.DefaultColorAsync(colorHex: colorHex, userId: userId, keyvalue, method: "post");
            }
            else if (defaultColor.ColorHex != colorHex)
            {
                defaultColor = await AccessoriesService.Instance.DefaultColorAsync(colorHex: colorHex, userId: userId, keyvalue, method: "post");
            }

            if (defaultColor == null)
                btnSelect.ContentDescription = colorHex;
            else
                btnSelect.ContentDescription = defaultColor.ColorHex;


            btnSelect.Enabled = true;

            linearProgress.Visibility = ViewStates.Gone;
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
        }

        private void BindColorsViewHolder(CachingViewHolder holder, ColorViewModel color, int position)
        {
            var linearView = holder.FindCachedViewById<LinearLayout>(Resource.Id.linearView);
            var txtColor = holder.FindCachedViewById<TextView>(Resource.Id.txtColor);
            txtColor.SetBackgroundColor(Color.ParseColor(color.color));
            linearView.SetBackgroundColor(Color.ParseColor(color.color));

            txtColor.Click += delegate
            {
                foreach(var col in colorsCollection)
                {
                    col.isSelected = false;
                };

                color.isSelected = !color.isSelected;

                defaultColor = new DefaultColor();
                btnSelect.ContentDescription = defaultColor.ColorHex;
                btnRemovecolor.Visibility = ViewStates.Visible;
                recyclerColors.SetAdapter(adapterColors);
            };

            if (color.isSelected)
            {
                btnSelect.ContentDescription = color.color;
                linearView.SetBackgroundColor(Color.White);
            }
            if (colorHex == color.color) {
                btnSelect.ContentDescription = color.color;
                linearView.SetBackgroundColor(Color.White);
                btnRemovecolor.Visibility = ViewStates.Visible;
            }
        }
        public RecyclerView recyclerColors => FindViewById<RecyclerView>(Resource.Id.recyclerColors);
        public Button btnClose => FindViewById<Button>(Resource.Id.btnClose);
        public Button btnSelect => FindViewById<Button>(Resource.Id.btnSelect);
        public Button btnRemovecolor => FindViewById<Button>(Resource.Id.btnRemoveColor);
        public CheckBox chkSetDefault => FindViewById<CheckBox>(Resource.Id.chkSetDefault);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView txtProgressText => FindViewById<TextView>(Resource.Id.txtProgressText);
    }
}