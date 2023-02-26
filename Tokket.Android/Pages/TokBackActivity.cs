using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Widget;
using Java.Util.Regex;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Tokket.Android
{
    [Activity(Label = "", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokBackActivity : BaseActivity, View.IOnTouchListener, View.IOnLongClickListener
    {
        TokModel tokModel; Spinner spinner_tokbackNumBlocks;
        LinearLayout linear_tokbackNumberBlocks, linear_tokbackDetail;
        private bool Showingback; TextView tokgroup; string stringblocks = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.tokback_page);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.tokback_toolbar);

            SetSupportActionBar(tokback_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            tokModel = JsonConvert.DeserializeObject<TokModel>(Intent.GetStringExtra("tokModel"));

            linear_tokbackNumberBlocks = FindViewById<LinearLayout>(Resource.Id.linear_tokbackNumberBlocks);
            linear_tokbackDetail = FindViewById<LinearLayout>(Resource.Id.linear_tokbackDetail);
            var tokcategory = FindViewById<TextView>(Resource.Id.lblTokBackTokCategory);
            tokgroup = FindViewById<TextView>(Resource.Id.lblTokBackTokGroup);
            var toktype = FindViewById<TextView>(Resource.Id.lblTokBackTokType);
            var lblTokBackPrimaryField = FindViewById<TextView>(Resource.Id.lblTokBackPrimaryField);
            spinner_tokbackNumBlocks = FindViewById<Spinner>(Resource.Id.spinner_tokblocks);

            nestedScrollParent.SetOnTouchListener(this);

#if (_CLASSTOKS)
            txtTokGroupHeader.Text = "Type";
            txtTokTypeHeader.Text = "Class Name";
#endif

            tokcategory.Text = tokModel.Category;
            tokgroup.Text = tokModel.TokGroup;
            toktype.Text = tokModel.TokType;

            if (tokgroup.Text.ToLower() == "quote")
            {
                lblTokBackPrimaryField.Text = tokModel.SecondaryFieldText;
                stringblocks = tokModel.PrimaryFieldText;
            }
            else
            {
                lblTokBackPrimaryField.Text = tokModel.PrimaryFieldText;
                stringblocks = tokModel.SecondaryFieldText;
            }
            

            if (tokModel.IsDetailBased)
            {
                linear_tokbackNumberBlocks.Visibility = ViewStates.Gone;

                linear_tokbackDetail.RemoveAllViews();

                for (int i = 0; i < tokModel.Details.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tokModel.Details[i]))
                    {
                        View view = LayoutInflater.Inflate(Resource.Layout.tokback_detailview, null);
                        
                        TextView lblTokBackCardBack = view.FindViewById<TextView>(Resource.Id.lblTokBackCardBack);
                        lblTokBackCardBack.SetOnLongClickListener(this);

                        lblTokBackCardBack.Click -= lblTokBackCardBackOnClick;
                        lblTokBackCardBack.Click += lblTokBackCardBackOnClick;

                        lblTokBackCardBack.MovementMethod = new ScrollingMovementMethod();
                        lblTokBackCardBack.Text = tokModel.Details[i];
                        view.Tag = i;
                        linear_tokbackDetail.AddView(view);

                        //Show the back for the first row
                        if (i == 0)
                        {
                            OnTapViewFlipperTokBack(view);
                        }
                    }
                }

            }
            else
            {
                spinner_tokbackNumBlocks.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(txtTokBackNumberBlock_ItemSelected);
                string[] arrayNumBlocks = new string[] { "3", "4", "5", "6", "7" };
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, arrayNumBlocks);
                adapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
                spinner_tokbackNumBlocks.Adapter = adapter;
            }
        }
        private void lblTokBackCardBackOnClick(object sender, EventArgs e)
        {
            OnTapViewFlipperTokBack((((sender as View).Parent as View).Parent as View).Parent as View); //until we get the main parent which is this view viewFlipper_tokback
        }
        private void txtTokBackNumberBlock_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            int selectedvalue = Convert.ToInt32(spinner.GetItemAtPosition(e.Position).ToString());

            if (!string.IsNullOrEmpty(stringblocks))
            {
                List<string> groupList = justifyStringLimit(selectedvalue);
                if (groupList.Count == selectedvalue)
                {
                    linear_tokbackDetail.RemoveAllViews();

                    TextView lblTokBackCardBack;
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        View view = LayoutInflater.Inflate(Resource.Layout.tokback_detailview, null);
                        lblTokBackCardBack = view.FindViewById<TextView>(Resource.Id.lblTokBackCardBack);
                        lblTokBackCardBack.SetOnLongClickListener(this);

                        lblTokBackCardBack.Click -= lblTokBackCardBackOnClick;
                        lblTokBackCardBack.Click += lblTokBackCardBackOnClick;

                        lblTokBackCardBack.MovementMethod = new ScrollingMovementMethod();
                        lblTokBackCardBack.Text = groupList[i];

                        view.Tag = i;
                        linear_tokbackDetail.AddView(view);

                        //Show the back for the first row
                        if (i == 0)
                        {
                            OnTapViewFlipperTokBack(view);
                        }
                    }
                }
                else
                {
                    showError("Not compatible with " + spinner.GetItemAtPosition(e.Position) + " blocks!");
                }
            }
        }

        private void showError(string message)
        {
            var objBuilder = new AlertDialog.Builder(this);
            objBuilder.SetTitle("");
            objBuilder.SetIcon(Resource.Drawable.tokback_icon);
            objBuilder.SetMessage(message);
            objBuilder.SetCancelable(false);

            AlertDialog objDialog = objBuilder.Create();
            objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) => { });
            objDialog.Show();
        }

        [Java.Interop.Export("OnTapViewFlipperTokBack")]
        public void OnTapViewFlipperTokBack(View v)
        {
            var flipper = v.FindViewById<ViewFlipper>(Resource.Id.viewFlipper_tokback);
            if (Showingback)
            { //Front
                // Use custom animations
                flipper.SetOutAnimation(this, Resource.Animation.card_flip_bottom_out);
                flipper.SetInAnimation(this, Resource.Animation.card_flip_bottom_in);
                flipper.ShowPrevious();
                Showingback = false;
            }
            else
            { //Back
                // Use custom animations
                flipper.SetOutAnimation(this, Resource.Animation.card_flip_top_out);
                flipper.SetInAnimation(this, Resource.Animation.card_flip_top_in);
                flipper.ShowNext();
            }
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

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.nestedScrollParent:
                    FindViewById<TextView>(Resource.Id.lblTokBackCardBack).Parent.RequestDisallowInterceptTouchEvent(false);
                    break;
                case Resource.Id.lblTokBackCardBack:
                    //FindViewById<TextView>(Resource.Id.lblTokBackCardBack).Parent.RequestDisallowInterceptTouchEvent(true);
                    break;
                default:
                    break;
            }
            return false;
        }

        public bool OnLongClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.nestedScrollParent:
                    FindViewById<TextView>(Resource.Id.lblTokBackCardBack).Parent.RequestDisallowInterceptTouchEvent(false);
                    break;
                case Resource.Id.lblTokBackCardBack:
                    FindViewById<TextView>(Resource.Id.lblTokBackCardBack).Parent.RequestDisallowInterceptTouchEvent(true);
                    break;
                default:
                    break;
            }
            return true;
        }

        private List<string> justifyStringLimit(int SelectedBlockCount)
        {
            List<string> ProcessedItems = new List<string>();

            var SplitText = stringblocks.Trim().Split(' ', '\n');
            if (SplitText.Length > 2)
            {
                int BlockCountMax = SelectedBlockCount;
                if (SplitText.Length >= BlockCountMax)
                {
                    int quotient = SplitText.Length / BlockCountMax;
                    int extraCount = SplitText.Length % BlockCountMax;

                    int z = 0;
                    for (int i = 0; i < BlockCountMax; i++)
                    {
                        var pText = string.Empty;

                        if (ProcessedItems.Count == BlockCountMax - 1) //for the last row
                        {
                            for (int x = z; x < SplitText.Length; x++)
                            {
                                pText = pText + SplitText[x] + " ";
                            }
                        }
                        else
                        {
                            for (int x = 0; x < quotient; x++)
                            {
                                pText = pText + SplitText[x + z] + " ";
                            }
                        }

                        z += quotient;

                        if (!string.IsNullOrEmpty(pText))
                        {
                            ProcessedItems.Add(pText);
                        }
                    }
                }
                else
                {
                    // Process all items if number of block is greater than the number of words
                    foreach (var sText in SplitText)
                    {
                        if (!string.IsNullOrEmpty(sText))
                        {
                            ProcessedItems.Add(sText);
                        }
                    }
                }
            }
            else
            {
                foreach (var sText in SplitText)
                {
                    if (!string.IsNullOrEmpty(sText))
                    {
                        ProcessedItems.Add(sText);
                    }
                }
            }

            return ProcessedItems;
        }

        public NestedScrollView nestedScrollParent => FindViewById<NestedScrollView>(Resource.Id.nestedScrollParent);
        public TextView txtTokGroupHeader => FindViewById<TextView>(Resource.Id.txtTokGroupHeader);
        public TextView txtTokTypeHeader => FindViewById<TextView>(Resource.Id.txtTokTypeHeader);
    }
}