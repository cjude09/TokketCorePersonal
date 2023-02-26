using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tokket.Android.Helpers;
using Tokket.Android.ViewHolders;
using Tokket.Android.ViewModels;
using static Android.App.ActionBar;

namespace Tokket.Android
{
    [Activity(Label = "Color Picker", Theme = "@style/Theme.Transparent", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class ColorPickerDialog : BaseActivity
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

        private ObservableCollection<ColorViewModel> ColorsCollection;
        private ObservableRecyclerAdapter<ColorViewModel, CachingViewHolder> adapterColors;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.dialog_color_picker);
            this.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);

            ColorsCollection = new ObservableCollection<ColorViewModel>();
            foreach (var item in Colors32)
            {
                var model = new ColorViewModel();
                model.color = item;
                ColorsCollection.Add(model);
            }

            var mLayoutManager = new GridLayoutManager(this, 4);
            recyclerColors.SetLayoutManager(mLayoutManager);

            adapterColors = ColorsCollection.GetRecyclerAdapter(BindColorsViewHolder, Resource.Layout.color_row);
            recyclerColors.SetAdapter(adapterColors);

            btnClose.Click += delegate
            {
                this.Finish();
            };
        }

        private void BindColorsViewHolder(CachingViewHolder holder, ColorViewModel model, int position)
        {
            var viewColor = holder.FindCachedViewById<View>(Resource.Id.viewColor);

            viewColor.SetBackgroundColor(Color.ParseColor(model.color));

            viewColor.Tag = position;
            viewColor.Click -= ViewItemClick;
            viewColor.Click += ViewItemClick;
        }

        private void ViewItemClick(object sender, EventArgs e)
        {
            int ndx = 0;
            try { ndx = (int)(sender as View).Tag; } catch { ndx = int.Parse((string)(sender as View).Tag); }

            Intent intent = new Intent();
            intent.PutExtra("color", Colors32[ndx]);
            SetResult(Result.Ok, intent);
            Finish();
        }

        public Button btnClose => FindViewById<Button>(Resource.Id.btnClose);
        public RecyclerView recyclerColors => FindViewById<RecyclerView>(Resource.Id.recyclerColors);
    }
}