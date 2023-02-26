using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Core;
using Tokket.Shared.Extensions;
using Newtonsoft.Json;
using Tokket.Android.ViewModels;
using Tokket.Shared.Models;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Adapters
{
    public class MyToksAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        public event EventHandler<int> ItemClick;
#if (_TOKKEPEDIA)
        public List<TokModel> items;
#endif
#if (_CLASSTOKS)
        public List<ClassTokModel> items;
#endif
        Context context;
        int cnttoksselected = 0;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => items.Count();
        View itemView;
#region Constructor
        public MyToksAdapter(Context cntxt, List<TokModel> _items, List<ClassTokModel> _itemClass)
        {
            context = cntxt;
#if (_TOKKEPEDIA)
        items = _items;
#endif
#if (_CLASSTOKS)
            items = _itemClass;
#endif
        }
        #endregion
        
        public void UpdateItems(List<TokModel> _items, List<ClassTokModel> _itemClass, int position)
        {
#if (_TOKKEPEDIA)
       items.AddRange(_items);
       NotifyItemRangeChanged(position, _items.Count);
#endif
#if (_CLASSTOKS)
            items.AddRange(_itemClass);
            NotifyItemRangeChanged(position, _itemClass.Count);
#endif
        }

        #region Override Events/Methods/Delegates
        MySetsViewHolder vh;
        public MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        AssetManager assetManager = Application.Context.Assets;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as MySetsViewHolder;

            //Set LinearColor
            int ndx = position % Colors.Count;
            if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            vh.linearMySetsColor.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));

            //Hide since we are not displaying it in the UI
            vh.txtSetsReferenceId.Visibility = ViewStates.Gone;
            vh.SetUserImage.Visibility = ViewStates.Gone;
            vh.SetUserName.Visibility = ViewStates.Gone;

            MySetsVm.lblMySetPopUp = vh.lblMySetPopUp;
            vh.txtSetsTokUpper.Text = items[position].PrimaryFieldText;

            string type = "Playable";
            if (items[position].IsMegaTok == true || items[position].TokGroup.ToLower() == "mega" || items[position].TokGroup.ToLower() == "pic" || items[position].TokGroup.ToLower() == "list")
            {
                type = "Non-playable";
            }

            vh.txtSetsTokBottom.Text = type + " - " + items[position].TokType; // + " • " + (Convert.ToDateTime(items[position].DateCreated)).ToString("MM/dd/yyyy");
          
            vh.lblMySetPopUp.Tag = position;
            vh.ItemView.Tag = position;
            vh.ItemView.SetOnTouchListener(this);
        }

        public void OnItemRowClick(object sender, int position)
        {
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.mytoksets_row, parent, false);

            vh = new MySetsViewHolder(itemView, OnClick);
            return vh;
        }
        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }
#endregion

#region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                string[] typeArray = MySetsVm.TokTypeID.Split("-");
                string type = typeArray[1].ToLower();

                int position = (int)v.Tag;

                bool isPlayable = true;
                if (items[position].IsMegaTok == true || items[position].TokGroup.ToLower() == "mega" || items[position].TokGroup.ToLower() == "pic" || items[position].TokGroup.ToLower() == "list")
                {
                    isPlayable = false;
                }

                if (string.IsNullOrEmpty(v.ContentDescription))
                {
                    if (isPlayable)
                    {
                        v.ContentDescription = "selected";
                        v.SetBackgroundColor(Color.LightBlue);
                        cnttoksselected += 1;
                        MySetsVm.TokIdsList.Add(items[position].Id);
                        MySetsVm.TokPKsList.Add(items[position].PartitionKey);
                        MySetsVm.ClassToksSelectedList.Add(items[position]);
                    }
                    else
                    {
                        //Show alert message that only same type will be added
                        showAlertMessage("Sets can only have toks of a single type");
                    }
                }
                else
                {
                    if (isPlayable)
                    {
                        v.ContentDescription = "";
                        v.SetBackgroundColor(Color.Transparent);
                        cnttoksselected -= 1;
                        MySetsVm.TokIdsList.Remove(items[position].Id);
                        MySetsVm.TokPKsList.Remove(items[position].PartitionKey);
                        MySetsVm.ClassToksSelectedList.Remove(items[position]);
                    }
                    else
                    {
                        //Show alert message that only same type will be removed
                        showAlertMessage("Sets with single type can be removed.");
                    }
                }

#if (_TOKKEPEDIA)
                if (cnttoksselected == 1)
                {
                    MySetsActivity.Instance.txtTotalToksSelected.Text = cnttoksselected + " Tok Selected";
                }
                else if (cnttoksselected > 1)
                {
                    MySetsActivity.Instance.txtTotalToksSelected.Text = cnttoksselected + " Toks Selected";
                }
                else
                {
                    MySetsActivity.Instance.txtTotalToksSelected.Text = "";
                }
#endif
#if (_CLASSTOKS)
                try {
                    if (cnttoksselected == 1)
                    {
                        MyClassSetsActivity.Instance.txtTotalToksSelected.Text = cnttoksselected + " Tok Selected";
                    }
                    else if (cnttoksselected > 1)
                    {
                        MyClassSetsActivity.Instance.txtTotalToksSelected.Text = cnttoksselected + " Toks Selected";
                    }
                    else
                    {
                        MyClassSetsActivity.Instance.txtTotalToksSelected.Text = "";
                    }
                } catch (Exception ex) { }
             
#endif
            }
            return true;
        }

        private void showAlertMessage(string message)
        {
            var builder = new AlertDialog.Builder(context)
                            .SetMessage(message)
                            .SetPositiveButton("OK", (_, args) =>
                            {

                            })
                            .SetCancelable(false)
                            .Show();
        }
        #endregion
    }
}